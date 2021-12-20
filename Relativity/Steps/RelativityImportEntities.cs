using System.Linq;
using System.Text;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.Connectors.Relativity.Errors;
using Reductech.Sequence.Core.Entities.Schema;
using ReductechEntityImport;

namespace Reductech.Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Import Entities into Relativity
/// </summary>
public sealed class RelativityImportEntities : CompoundStep<Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityImportEntities, Unit>();

    /// <inheritdoc />
    protected override async Task<Result<Unit, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var settings = stateMonad.Settings.TryGetRelativitySettings();

        if (settings.IsFailure)
            return settings.MapError(x => x.WithLocation(this)).ConvertFailure<Unit>();

        var data = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
            Entities,
            Schema.WrapStep(StepMaps.ConvertToSchema(Schema)),
            ControlNumberField.WrapStringStream(),
            FilePathField.WrapStringStream(),
            FolderPathField.WrapStringStream(),
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Unit>();

        var (workspaceId, entities, schema, controlNumberField, filePathField, folderPathField) =
            data.Value;

        var clientPath = GetClientPath(settings.Value);

        var startResult = stateMonad.ExternalContext.ExternalProcessRunner.StartExternalProcess(
                clientPath,
                new List<string>(),
                new Dictionary<string, string>(),
                Encoding.Default,
                stateMonad,
                this
            )
            .MapError(x => x.WithLocation(this));

        if (startResult.IsFailure)
            return startResult.ConvertFailure<Unit>();

        using var processReference = startResult.Value;

        var loggingTask = processReference.OutputChannel.ReadAllAsync(cancellationToken)
            .ForEachAsync(x => stateMonad.Logger.LogInformation(x.line), cancellationToken);

        Channel channel = new("127.0.0.1:30051", ChannelCredentials.Insecure);

        var client = new Reductech_Entity_Import.Reductech_Entity_ImportClient(channel);

        var importRequest = new StartImportCommand()
        {
            WorkspaceArtifactId = workspaceId,
            RelativityPassword  = settings.Value.RelativityPassword,
            RelativityUsername  = settings.Value.RelativityUsername,
            RelativityWebAPIUrl = Path.Combine(settings.Value.Url, "relativitywebapi"),
            ControlNumberField  = controlNumberField,
            FilePathField       = filePathField,
            FolderPathField     = folderPathField
        };

        var schemaNode = SchemaNode.Create(schema);

        if (schemaNode is not EntityNode entityNode)
            return Result.Failure<Unit, IError>(
                ErrorCode_Relativity.Unsuccessful
                    .ToErrorBuilder("Schema does not represent an entity")
                    .WithLocation(this)
            );

        foreach (var (key, (node, required)) in entityNode.EntityPropertiesData.Nodes)
        {
            var dt = GetDataType(node);

            if (dt.IsFailure)
                return dt.ConvertFailure<Unit>().MapError(x => x.WithLocation(this));

            var dataField = new StartImportCommand.Types.DataField()
            {
                Name = key, DataType = dt.Value
            };

            importRequest.DataFields.Add(dataField);
        }

        var startImportResult = await client.StartImportAsync(
            importRequest,
            cancellationToken: cancellationToken
        );

        if (!startImportResult.Success)
            return Result.Failure<Unit, IError>(
                ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(startImportResult.Message)
                    .WithLocation(this)
            );

        using var call = client.ImportData(new CallOptions());

        async ValueTask<Result<Unit, IError>> WriteEntity(Entity e, CancellationToken _1)
        {
            var importObject = Create(e);

            if (importObject.IsFailure)
                return importObject.ConvertFailure<Unit>().MapError(x => x.WithLocation(this));

            await call.RequestStream.WriteAsync(importObject.Value);
            return Unit.Default;

            Result<ImportObject, IErrorBuilder> Create(Entity entity)
            {
                var io = new ImportObject();

                var validateResult = schema.Validate(entity.ToJsonElement());

                if (!validateResult.IsValid)
                    return Result.Failure<ImportObject, IErrorBuilder>(
                        ErrorBuilderList.Combine(
                            validateResult.GetErrorMessages().Select(
                                x => ErrorCode.SchemaViolation.ToErrorBuilder(
                                    x.message,
                                    x.location
                                )
                            )));

                foreach (var (key, (node, required)) in entityNode.EntityPropertiesData.Nodes)
                {
                    var value = entity.TryGetValue(key);

                    ImportObject.Types.FieldValue fieldValue;

                    if (value.HasValue)
                    {
                        var fieldValueResult = GetFieldValue(value.GetValueOrThrow());

                        if (fieldValueResult.IsFailure)
                            return fieldValueResult.ConvertFailure<ImportObject>();

                        fieldValue = fieldValueResult.Value;
                    }
                    else
                    {
                        fieldValue = new ImportObject.Types.FieldValue();
                    }

                    if (fieldValue.TestOneofCase
                     == ImportObject.Types.FieldValue.TestOneofOneofCase.None
                     && required)
                    {
                        return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(
                            $"Property '{key}' is required."
                        );
                    }

                    io.Values.Add(fieldValue);
                }

                return io;
            }
        }

        var writeResult = await entities.ForEach(WriteEntity, cancellationToken);

        if (writeResult.IsFailure)
            return writeResult;

        await call.RequestStream.CompleteAsync();

        var response = await call.ResponseAsync;

        if (!response.Success)
            return Result.Failure<Unit, IError>(
                ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(response.Message)
                    .WithLocation(this)
            );

        processReference.Dispose();
        await loggingTask;

        return Unit.Default;
    }

    private static string GetClientPath(RelativitySettings relativitySettings)
    {
        if (!string.IsNullOrWhiteSpace(relativitySettings.ImportClientPath))
            return relativitySettings.ImportClientPath;

        var assembly = typeof(RelativityImportEntities).Assembly;

        const string importClientName = "EntityImportClient.exe";

        var clientPath = Path.Combine(
            Path.GetDirectoryName(assembly.Location) ?? string.Empty,
            "Sequence.EntityImportClient",
            importClientName
        );

        return clientPath;
    }

    /// <summary>
    /// The Workspace to import into.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    [StepProperty(2)][Required] public IStep<Array<Entity>> Entities { get; set; } = null!;

    [StepProperty(3)][Required] public IStep<Entity> Schema { get; set; } = null!;

    [StepProperty(4)]
    [DefaultValueExplanation("Control Number")]
    public IStep<StringStream> ControlNumberField { get; set; } =
        new SCLConstant<StringStream>("Control Number");

    [StepProperty(5)]
    [DefaultValueExplanation("No file path")]
    public IStep<StringStream> FilePathField { get; set; } = new SCLConstant<StringStream>("");

    [StepProperty(6)]
    [DefaultValueExplanation("No folder path")]
    public IStep<StringStream> FolderPathField { get; set; } = new SCLConstant<StringStream>("");

    private static Result<ImportObject.Types.FieldValue, IErrorBuilder> GetFieldValue(
        ISCLObject entityValue)
    {
        switch (entityValue)
        {
            case SCLBool boolean:
                return
                    new ImportObject.Types.FieldValue { BoolValue = boolean.Value };
            case SCLDateTime dateTime:
                return
                    new ImportObject.Types.FieldValue { DateValue = dateTime.Serialize(SerializeOptions.Primitive) };
            case SCLDouble d:
                return
                    new ImportObject.Types.FieldValue { DoubleValue = d.Value };
            case ISCLEnum enumerationValue:
                return
                    new ImportObject.Types.FieldValue
                    {
                        StringValue = enumerationValue.EnumValue
                    };
            case SCLInt integer:
                return
                    new ImportObject.Types.FieldValue { IntValue = integer.Value };
            case StringStream s:
                return
                    new ImportObject.Types.FieldValue { StringValue = s.GetString() };
            default:
                return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(
                    $"Cannot import {entityValue}"
                );
        }
    }

    private static Result<StartImportCommand.Types.DataField.Types.DataType, IErrorBuilder>
        GetDataType(SchemaNode schemaNode)
    {
        switch (schemaNode)
        {
            case BooleanNode: return StartImportCommand.Types.DataField.Types.DataType.Bool;
            case IntegerNode: return StartImportCommand.Types.DataField.Types.DataType.Int;
            case NumberNode:  return StartImportCommand.Types.DataField.Types.DataType.Double;
            case StringNode stringNode:
            {
                if(stringNode.Format is DateTimeStringFormat)
                    return StartImportCommand.Types.DataField.Types.DataType.Date;

                return StartImportCommand.Types.DataField.Types.DataType.String;
            }
            default:
                return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(
                    $"Cannot import an entity of type {schemaNode.SchemaValueType}"
                );
        }
    }
}
