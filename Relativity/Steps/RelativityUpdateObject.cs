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

public sealed class RelativityUpdateObject : RelativityApiRequest<(int workspaceId, UpdateRequest updateRequest, UpdateOptions updateOptions), IObjectManager1, UpdateResult, Unit>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityUpdateObject, Unit>();

    /// <inheritdoc />
    public override Result<Unit, IErrorBuilder> ConvertOutput(UpdateResult serviceOutput)
    {
        var errors = serviceOutput.EventHandlerStatuses
            .Where(x => !x.Success)
            .Select(x => ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(x.Message))
            .ToList();

        if (!errors.Any())
            return Unit.Default;

        var errorBuilderList = ErrorBuilderList.Combine(errors);
        return Result.Failure<Unit, IErrorBuilder>(errorBuilderList);
    }

    /// <inheritdoc />
    public override Task<UpdateResult> SendRequest(
        IStateMonad stateMonad,
        IObjectManager1 service,
        (int workspaceId, UpdateRequest updateRequest, UpdateOptions updateOptions) requestObject,
        CancellationToken cancellationToken)
    {
        return
            service.UpdateAsync(
                requestObject.workspaceId,
                requestObject.updateRequest,
                requestObject.updateOptions
            );
    }

    /// <inheritdoc />
    public override async
        Task<Result<(int workspaceId, UpdateRequest updateRequest, UpdateOptions updateOptions),
            IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var r = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(ArtifactType.Case, stateMonad, this),
            
            ObjectArtifactId,
            FieldValues,
            UpdateBehaviour,
            cancellation
        );

        if (r.IsFailure)
            return r
                .ConvertFailure<(int workspaceId, UpdateRequest updateRequest, UpdateOptions
                    updateOptions)>();

        var (workspaceId, objectArtifactId, fieldValues, updateBehaviour) = r.Value;

        return Result
            .Success<(int workspaceId, UpdateRequest updateRequest, UpdateOptions updateOptions),
                IError>(new(workspaceId, new UpdateRequest
            {
                Object      = new RelativityObjectRef{ArtifactID = objectArtifactId},
                FieldValues = GetFieldRefValuePairs(fieldValues)
            }, new UpdateOptions
            {
                UpdateBehavior = Convert(updateBehaviour) 
            }));

        static IEnumerable<FieldRefValuePair> GetFieldRefValuePairs(Entity entity)
        {
            foreach (var (key, value) in entity.Dictionary)
            {
                yield return new FieldRefValuePair()
                {
                    Field = new FieldRef() { Name = key }, Value = value.BestValue.ObjectValue
                };
            }
        }


        static FieldUpdateBehavior Convert(UpdateBehaviour updateBehaviour)
        {
            return updateBehaviour switch
            {
                Steps.UpdateBehaviour.Merge   => FieldUpdateBehavior.Merge,
                Steps.UpdateBehaviour.Remove  => FieldUpdateBehavior.Remove,
                Steps.UpdateBehaviour.Replace => FieldUpdateBehavior.Replace,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(updateBehaviour),
                    updateBehaviour,
                    null
                )
            };
        }
    }

    /// <summary>
    /// The Workspace containing the folder.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The artifact id of the object to update
    /// </summary>
    [StepProperty(2)]
    [Required]
    public IStep<int> ObjectArtifactId { get; set; } = null!;

    [StepProperty(3)]
    [Required]
    public IStep<Core.Entity> FieldValues { get; set; } = null!;

    /// <summary>
    /// indicates whether you want to replace or merge a choice or object. These options are available for only multiple choice and multiple object fields.
    /// </summary>
    [StepProperty(4)]
    [DefaultValueExplanation("Merge")]
    public IStep<UpdateBehaviour> UpdateBehaviour { get; set; } =
        new EnumConstant<UpdateBehaviour>(Steps.UpdateBehaviour.Merge);
}

}
