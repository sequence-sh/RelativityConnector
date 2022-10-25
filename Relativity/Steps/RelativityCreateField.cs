using CSharpFunctionalExtensions;
using Reductech.Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;
#pragma warning disable CS8618

namespace Reductech.Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Creates a new Fixed Length Field and returns the ArtifactId of that field
/// </summary>
public sealed class RelativityCreateField : RelativityApiRequest<(FixedLengthFieldRequest1 fieldRequest, int workspaceId),
    IFieldManager1,
    int, SCLInt>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateField, SCLInt>();

    /// <inheritdoc />
    public override Result<SCLInt, IErrorBuilder> ConvertOutput(int serviceOutput)
    {
        return serviceOutput.ConvertToSCLObject();
    }

    /// <inheritdoc />
    public override async Task<int> SendRequest(
        IStateMonad stateMonad,
        IFieldManager1 service,
        (FixedLengthFieldRequest1 fieldRequest, int workspaceId) requestObject,
        CancellationToken cancellationToken)
    {
        //var objectTypes = await service.GetAvailableObjectTypesAsync(requestObject.workspaceId);
            
        return await service.CreateFixedLengthFieldAsync(
            requestObject.workspaceId,
            requestObject.fieldRequest
        );
    }

    /// <summary>
    /// The Workspace where you want to create the folder.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The Name for the field
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<StringStream> FieldName { get; set; }

    //TODO reinstate this
    ///// <summary>
    ///// The Object type that the field will apply to
    ///// </summary>
    //[StepProperty(3)]
    //[DefaultValueExplanation("Document (10)")]
    //public IStep<SCLOneOf<SCLEnum<ArtifactType>, SCLInt>> ObjectType { get; set; } =
    //    new OneOfStep<SCLEnum<ArtifactType>, SCLInt>(
    //        new EnumConstant<ArtifactType>(ArtifactType.Document)
    //    );

    /// <summary>
    /// The length of the field
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("100")]
    public IStep<SCLInt> Length { get; set; } = new SCLConstant<SCLInt>(100.ConvertToSCLObject());

    /// <inheritdoc />
    public override async ValueTask<Result<(FixedLengthFieldRequest1 fieldRequest, int workspaceId), IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var r = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this) , FieldName.WrapStringStream(), Length, CancellationToken.None
        );

        if (r.IsFailure)
            return r.ConvertFailure<(FixedLengthFieldRequest1 fieldRequest, int workspaceId)>();

        

        var (workspaceId, fieldName, length) = r.Value;

        var request = new FixedLengthFieldRequest1()
        {
            Name       = fieldName,
            Length     = length,
            IsRequired = false,
            IsLinked   = false,
            FilterType = FilterType.None,
            ObjectType = new ObjectTypeIdentifier()
            {
                //ArtifactID = 
                Guids          = new List<Guid>(){new Guid("15c36703-74ea-4ff8-9dfb-ad30ece7530d")},
                ArtifactID     = 1035231,
                ArtifactTypeID = 10,
                Name           = "Document"
            }

        };

        return (request, workspaceId);
    }
}
