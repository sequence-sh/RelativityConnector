﻿using System.Linq;
using Sequence.Connectors.Relativity.Errors;
using Sequence.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Objects.DataContracts;

namespace Sequence.Connectors.Relativity.Steps;

/// <summary>
/// Create a relativity dynamic objects from entities.
/// Returns an array of the new artifact ids
/// </summary>
public class RelativityCreateDynamicObjects : RelativityApiRequest<(int workspaceId,
    MassCreateRequest createRequest
    ), IObjectManager1, MassCreateResult, Array<SCLInt>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityCreateDynamicObjects, Array<SCLInt>>();

    /// <inheritdoc />
    public override Result<Array<SCLInt>, IErrorBuilder> ConvertOutput(MassCreateResult serviceOutput)
    {
        if (!serviceOutput.Success)
            return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(serviceOutput.Message);

        var array = serviceOutput.Objects.Select(x => x.ArtifactID.ConvertToSCLObject()).ToSCLArray();
        return array;
    }

    /// <inheritdoc />
    public override Task<MassCreateResult> SendRequest(
        IStateMonad stateMonad,
        IObjectManager1 service,
        (int workspaceId, MassCreateRequest createRequest) requestObject,
        CancellationToken cancellationToken)
    {
        return service.CreateAsync(
            requestObject.workspaceId,
            requestObject.createRequest,
            cancellationToken
        );
    }

    /// <inheritdoc />
    public override async ValueTask<Result<(int workspaceId, MassCreateRequest createRequest), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var stepsResult = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            Entities.WrapArray(),
            ArtifactType.WrapArtifactId(this),
            ParentArtifactId.WrapNullable(),
            cancellation
        );

        if (stepsResult.IsFailure)
            return stepsResult.ConvertFailure<(int workspaceId, MassCreateRequest createRequest)>();

        var (workspaceId, entities, artifactType, parentArtifactId) = stepsResult.Value;

        var request = ToCreateRequest(entities, artifactType, parentArtifactId);

        if (request.IsFailure)
            return request.MapError(x => x.WithLocation(this))
                .ConvertFailure<(int workspaceId, MassCreateRequest createRequest)>();

        return (workspaceId, request.Value);
    }

    /// <summary>
    /// Convert an entity to a Create Request
    /// </summary>
    public static Result<MassCreateRequest, IErrorBuilder> ToCreateRequest(
        IReadOnlyList<Entity> entities,
        ArtifactType artifactType,
        Maybe<SCLInt> parentArtifactId)
    {
        var createRequest = new MassCreateRequest
        {
            ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int) artifactType }
        };

        if (parentArtifactId.HasValue)
            createRequest.ParentObject =
                new RelativityObjectRef() { ArtifactID = parentArtifactId.Value };

        var fieldsNames = entities.SelectMany(x => x.Headers).Select(x=>x.Inner)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var fieldRefs = new List<FieldRef>(fieldsNames.Count);

        foreach (var fieldName in fieldsNames)
        {
            fieldRefs.Add(new FieldRef() { Name = fieldName });
        }

        createRequest.Fields = fieldRefs;

        var valueListList = new List<IReadOnlyList<object?>>();

        foreach (var entity in entities)
        {
            var valueList = new List<object?>(fieldRefs.Count);

            foreach (var fieldRef in fieldRefs)
            {
                var v = entity.TryGetValue(fieldRef.Name);

                if (v.HasValue)
                    valueList.Add(v.Value.ToCSharpObject()); //TODO maybe do some conversion here
                else
                    valueList.Add(null);
            }

            valueListList.Add(valueList);
        }

        createRequest.ValueLists = valueListList;

        return createRequest;
    }

    /// <summary>
    /// The Workspace where you want to create the objects.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<SCLOneOf<SCLInt, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The entities to import
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<Array<Entity>> Entities { get; set; } = null!;

    /// <summary>
    /// The type of the object to create
    /// </summary>
    [StepProperty(3)]
    [Required]
    public IStep<SCLOneOf<SCLEnum<ArtifactType>, SCLInt>> ArtifactType { get; set; } = null!;

    /// <summary>
    /// The artifact Id of the parent object
    /// </summary>
    [StepProperty(4)]
    [DefaultValueExplanation("The Workspace Root")]
    public IStep<SCLInt>? ParentArtifactId { get; set; } = null;
}
