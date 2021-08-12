using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using OneOf;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Create a relativity dynamic objects from entities.
/// Returns an array of the new artifact ids
/// </summary>
public class RelativityCreateDynamicObjects : RelativityApiRequest<(int workspaceId,
    MassCreateRequest createRequest
    ), IObjectManager1, MassCreateResult, Array<int>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory =>
        new SimpleStepFactory<RelativityCreateDynamicObjects, Array<int>>();

    /// <inheritdoc />
    public override Result<Array<int>, IErrorBuilder> ConvertOutput(MassCreateResult serviceOutput)
    {
        if (!serviceOutput.Success)
            return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(serviceOutput.Message);

        var array = serviceOutput.Objects.Select(x => x.ArtifactID).ToList().ToSCLArray();
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
    public override async Task<Result<(int workspaceId, MassCreateRequest createRequest), IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var stepsResult = await stateMonad.RunStepsAsync(
            Workspace.WrapWorkspace(stateMonad, this),
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
        Maybe<int> parentArtifactId)
    {
        var createRequest = new MassCreateRequest
        {
            ObjectType = new ObjectTypeRef() { ArtifactTypeID = (int) artifactType }
        };

        if (parentArtifactId.HasValue)
            createRequest.ParentObject =
                new RelativityObjectRef() { ArtifactID = parentArtifactId.Value };

        var fieldsNames = entities.SelectMany(x => x.Dictionary.Keys)
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
                    valueList.Add(v.Value.ObjectValue); //TODO maybe do some conversion here
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
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

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
    public IStep<OneOf<ArtifactType, int>> ArtifactType { get; set; } = null!;

    /// <summary>
    /// The artifact Id of the parent object
    /// </summary>
    [StepProperty(4)]
    [DefaultValueExplanation("The Workspace Root")]
    public IStep<int>? ParentArtifactId { get; set; } = null;
}

}
