using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Relativity.Errors;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Interfaces.Field;
using Relativity.Services.Interfaces.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;

namespace Reductech.EDR.Connectors.Relativity.Steps
{
    /// <summary>
    /// Create a relativity dynamic objects from entities.
    /// Returns an array of the new artifact ids
    /// </summary>
    public class RelativityCreateDynamicObjects : RelativityApiRequest<(int workspaceId, MassCreateRequest createRequest
        ), IObjectManager, MassCreateResult, Array<int>>
    {
        /// <inheritdoc />
        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateDynamicObjects, Array<int>>();


        /// <inheritdoc />
        public override Result<Array<int>, IErrorBuilder> ConvertOutput(MassCreateResult serviceOutput)
        {
            if (!serviceOutput.Success)
                return ErrorCode_Relativity.Unsuccessful.ToErrorBuilder(serviceOutput.Message);

            var array= serviceOutput.Objects.Select(x => x.ArtifactID).ToList().ToSCLArray();
            return array;
        }

        /// <inheritdoc />
        public override Task<MassCreateResult> SendRequest(IStateMonad stateMonad, IObjectManager service,
            (int workspaceId, MassCreateRequest createRequest) requestObject, CancellationToken cancellationToken)
        {
            return service.CreateAsync(requestObject.workspaceId, requestObject.createRequest, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<Result<(int workspaceId, MassCreateRequest createRequest), IError>> TryCreateRequest(
            IStateMonad stateMonad, CancellationToken cancellation)
        {
            var stepsResult = await stateMonad.RunStepsAsync(WorkspaceArtifactId, Entities.WrapArray(), ArtifactTypeId,
                cancellation);

            if (stepsResult.IsFailure)
                return stepsResult.ConvertFailure<(int workspaceId, MassCreateRequest createRequest)>();

            var (workspaceId, entities, artifactTypeId) = stepsResult.Value;

            var fieldManager = TryGetService<IFieldManager>(stateMonad);
            if (fieldManager.IsFailure)
                return fieldManager.MapError(x => x.WithLocation(this))
                    .ConvertFailure<(int workspaceId, MassCreateRequest createRequest)>();

            var fields = await fieldManager.Value.GetAvailableObjectTypesAsync(workspaceId);

            var request = ToCreateRequest(entities, artifactTypeId, fields);

            if (request.IsFailure)
                return request.MapError(x => x.WithLocation(this))
                    .ConvertFailure<(int workspaceId, MassCreateRequest createRequest)>();

            return (workspaceId, request.Value);
        }

        /// <summary>
        /// Convert an entity to a Create Request
        /// </summary>
        public static Result<MassCreateRequest, IErrorBuilder> ToCreateRequest(IReadOnlyList<Entity> entities,
            int artifactTypeId, List<ObjectTypeIdentifier> objectTypeIdentifiers)
        {
            var fieldDictionary =
                objectTypeIdentifiers.ToDictionary(x => x.Name, x => x.ArtifactID, StringComparer.OrdinalIgnoreCase);


            var createRequest = new MassCreateRequest
            {
                ObjectType = new ObjectTypeRef() { ArtifactTypeID = artifactTypeId }
            };

            var fieldsNames = entities.SelectMany(x => x.Dictionary.Keys).Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            var fieldRefs = new List<FieldRef>(fieldsNames.Count);
            var missingFields = new List<string>();

            foreach (var fieldName in fieldsNames)
            {
                if (fieldDictionary.TryGetValue(fieldName, out var artifactId))
                {
                    fieldRefs.Add(new FieldRef() { ArtifactID = artifactId, Name = fieldName });
                }
                else
                {
                    missingFields.Add(fieldName);
                }
            }

            if (missingFields.Any())
            {
                var eb = ErrorBuilderList.Combine(
                    missingFields.Select(x => ErrorCode_Relativity.MissingField.ToErrorBuilder(x)));

                return Result.Failure<MassCreateRequest, IErrorBuilder>(eb);
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
                    else valueList.Add(null);
                }

                valueListList.Add(valueList);
            }

            createRequest.ValueLists = valueListList;

            return createRequest;
        }

        /// <summary>
        /// The artifact Id of the workspace to import into
        /// </summary>
        [StepProperty(1)]
        [Required]
        public IStep<int> WorkspaceArtifactId { get; set; } = null!;

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
        public IStep<int> ArtifactTypeId { get; set; } = null!;
    }
}