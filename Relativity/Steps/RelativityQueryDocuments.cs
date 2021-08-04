using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Internal.Logging;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.Util;
using Relativity.Environment.V1.Shared.Models;
using Relativity.Services.Objects;
using Relativity.Services.Objects.DataContracts;
using Entity = Reductech.EDR.Core.Entity;
using ObjectTypeRef = Relativity.Services.Objects.DataContracts.ObjectTypeRef;
using QueryRequest = Relativity.Services.Objects.DataContracts.QueryRequest;
using Sort = Relativity.Services.Objects.DataContracts.Sort;
using SortEnum = Relativity.Services.Objects.DataContracts.SortEnum;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Query for Workspaces, Documents, RDOs and System Types
/// </summary>
public sealed class RelativityQueryDocuments : RelativityApiRequest<(int workspaceId, QueryRequest
    request, int
    indexOfFirst, int lengthOfResults), IObjectManager, QueryResult, Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativityQueryDocuments, Array<Entity>>();

    /// <inheritdoc />
    public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(QueryResult serviceOutput)
    {
        return TryConvertToEntityArray(serviceOutput.Objects);
    }

    /// <inheritdoc />
    public override Task<QueryResult> SendRequest(
        IStateMonad stateMonad,
        IObjectManager service,
        (int workspaceId, QueryRequest request, int indexOfFirst, int lengthOfResults)
            requestObject,
        CancellationToken cancellationToken)
    {
        var progress = new ProgressReportProgress(stateMonad, this);
        var (workspaceId, request, indexOfFirst, lengthOfResults) = requestObject;
        return service.QueryAsync(workspaceId, request, indexOfFirst, lengthOfResults
        , cancellationToken, progress );// TODO these
    }

    /// <inheritdoc />
    public override async
        Task<Result<(int workspaceId, QueryRequest request, int indexOfFirst, int lengthOfResults),
            IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var artifactIdsStep = FieldArtifactIds ?? ArrayNew<int>.CreateArray(new List<IStep<int>>());

        var data = await stateMonad.RunStepsAsync(
            WorkspaceArtifactId,
            Condition.WrapStringStream(),
            artifactIdsStep.WrapArray(),
            Start,
            Length,
            ArtifactTypeId,
            SortArtifactId.WrapNullable(),
            SortDirection,
            cancellation
        );

        if (data.IsFailure)
            return data
                .ConvertFailure<(int workspaceId, QueryRequest request, int indexOfFirst, int
                    lengthOfResults)>();

        var (workspaceArtifactId, condition, artifactIds, start, length, artifactTypeId,
            sortArtifactId,
            sortDirection) = data.Value;

        var sorts = new List<Sort>();

        if (sortArtifactId.HasValue)
        {
            sorts.Add(
                new Sort()
                {
                    Direction       = sortDirection,
                    FieldIdentifier = new FieldRef() { ArtifactID = sortArtifactId.Value }
                }
            );
        }

        var queryRequest = new QueryRequest
        {
            ObjectType      = new ObjectTypeRef { ArtifactTypeID = artifactTypeId },
            Condition       = condition,
            Fields          = artifactIds.Select(x => new FieldRef() { ArtifactID = x }).ToList(),
            IncludeIDWindow = false,
            RelationalField =
                null, //name of relational field to expand query results to related objects
            SampleParameters        = null,
            SearchProviderCondition = null,  //see documentation on building search providers
            Sorts                   = sorts, //an array of Fields with sorting order
            //QueryHint = "waitfor:5" //TODO this
        };

        return (workspaceArtifactId, queryRequest, start, length);
    }

    [StepProperty(1)][Required] public IStep<int> WorkspaceArtifactId { get; set; } = null!;

    /// <summary>
    /// The query condition
    /// See Relativity's developer documentation for more details
    /// </summary>
    [StepProperty(2)]
    [Required]
    [Example("('Email From' IN ['Test0@Test.com','Test1@Test.com'])")]
    public IStep<StringStream> Condition { get; set; } = null!;

    /// <summary>
    /// Artifact Ids of fields to return. The ArtifactId field is always returned
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("Just ArtifactId")]
    public IStep<Array<int>>? FieldArtifactIds { get; set; }

    /// <summary>
    /// 1-based index of first document in query results to retrieve
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("1")]
    public IStep<int> Start { get; set; } = new IntConstant(0);

    /// <summary>
    /// Max number of results to return in this query call.
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("100")]
    public IStep<int> Length { get; set; } = new IntConstant(100);

    /// <summary>
    /// ArtifactId of the field to sort by
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("Do not sort")]
    public IStep<int>? SortArtifactId { get; set; } = null!;

    /// <summary>
    /// Direction to sort by
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation(nameof(SortEnum.Ascending))]
    public IStep<SortEnum> SortDirection { get; set; } =
        new EnumConstant<SortEnum>(SortEnum.Ascending);

    [StepProperty]
    [DefaultValueExplanation("10 - Document")]
    public IStep<int> ArtifactTypeId { get; set; } = new IntConstant(10);
}

}
