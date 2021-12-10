using System.Linq;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Relativity.Services.Objects.DataContracts;
using ObjectTypeRef = Relativity.Services.Objects.DataContracts.ObjectTypeRef;
using QueryRequest = Relativity.Services.Objects.DataContracts.QueryRequest;
using Sort = Relativity.Services.Objects.DataContracts.Sort;
using SortEnum = Relativity.Services.Objects.DataContracts.SortEnum;

namespace Reductech.EDR.Connectors.Relativity.Steps;

/// <summary>
/// Query for Workspaces, Documents, RDOs and System Types
/// </summary>
public sealed class RelativitySendQuery : RelativityApiRequest<(int workspaceId, QueryRequest
    request, int
    indexOfFirst, int lengthOfResults), IObjectManager1, QueryResult, Array<Entity>>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<RelativitySendQuery, Array<Entity>>();

    /// <inheritdoc />
    public override Result<Array<Entity>, IErrorBuilder> ConvertOutput(QueryResult serviceOutput)
    {
        var entities = new List<Entity>(serviceOutput.Objects.Count);

        foreach (var relativityObject in serviceOutput.Objects)
        {
            var entity = Entity.Create(GetEntityValues(relativityObject));
            entities.Add(entity);
        }

        return entities.ToSCLArray();

        static IEnumerable<(EntityPropertyKey key, object? value)> GetEntityValues(
            RelativityObject relativityObject)
        {
            yield return (new EntityPropertyKey(nameof(RelativityObject.Name)),
                          relativityObject.Name);

            yield return (new EntityPropertyKey(nameof(RelativityObject.ArtifactID)),
                          relativityObject.ArtifactID);

            if (relativityObject.ParentObject is not null)
                yield return (new EntityPropertyKey(nameof(relativityObject.ParentObject)),
                              relativityObject.ParentObject.ArtifactID);

            foreach (var f in relativityObject.FieldValues)
            {
                yield return (new EntityPropertyKey(f.Field.Name), f.Value);
            }
        }
    }

    /// <inheritdoc />
    public override Task<QueryResult> SendRequest(
        IStateMonad stateMonad,
        IObjectManager1 service,
        (int workspaceId, QueryRequest request, int indexOfFirst, int lengthOfResults)
            requestObject,
        CancellationToken cancellationToken)
    {
        var progress = new ProgressReportProgress(stateMonad, this);
        var (workspaceId, request, indexOfFirst, lengthOfResults) = requestObject;

        return service.QueryAsync(
            workspaceId,
            request,
            indexOfFirst,
            lengthOfResults,
            cancellationToken,
            progress
        ); // TODO these
    }

    /// <inheritdoc />
    public override async
        Task<Result<(int workspaceId, QueryRequest request, int indexOfFirst, int lengthOfResults),
            IError>>
        TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
    {
        var data = await stateMonad.RunStepsAsync(
            Workspace.WrapArtifact(Relativity.ArtifactType.Case, stateMonad, this),
            Condition.WrapStringStream(),
            Fields.WrapNullable(
                StepMaps.OneOf(StepMaps.Array<int>(), StepMaps.Array(StepMaps.String()))
            ),
            Start,
            Length,
            ArtifactType.WrapArtifactId(this),
            SortArtifactId.WrapNullable(),
            SortDirection,
            cancellation
        );

        if (data.IsFailure)
            return data
                .ConvertFailure<(int workspaceId, QueryRequest request, int indexOfFirst, int
                    lengthOfResults)>();

        var (workspaceArtifactId, condition, fields, start, length, artifactTypeId,
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
            ObjectType      = new ObjectTypeRef { ArtifactTypeID = (int)artifactTypeId },
            Condition       = condition,
            IncludeIDWindow = false,
            RelationalField =
                null, //name of relational field to expand query results to related objects
            SampleParameters        = null,
            SearchProviderCondition = null,  //see documentation on building search providers
            Sorts                   = sorts, //an array of Fields with sorting order
            //QueryHint = "waitfor:5" //TODO this
        };

        if (fields.HasValue)
        {
            queryRequest.Fields =
                fields.Value.Match(
                    l => l.Select(x => new FieldRef() { ArtifactID = x }),
                    l => l.Select(x => new FieldRef() { Name       = x })
                );
        }
        else
        {
            queryRequest.Fields = new List<FieldRef>(0);
        }

        return (workspaceArtifactId, queryRequest, start, length);
    }

    /// <summary>
    /// The Workspace to query.
    /// You can provide either the Artifact Id or the name
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

    /// <summary>
    /// The query condition
    /// See Relativity's developer documentation for more details
    /// </summary>
    [StepProperty(2)]
    [Required]
    [Example("('Email From' IN ['Test0@Test.com','Test1@Test.com'])")]
    public IStep<StringStream> Condition { get; set; } = null!;

    /// <summary>
    /// The fields to return.
    /// The ArtifactId field is always returned.
    /// You can provide either the ArtifactId or the name
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("Just ArtifactId")]
    public IStep<OneOf<Array<int>, Array<StringStream>>>? Fields { get; set; } = null!;

    /// <summary>
    /// 1-based index of first document in query results to retrieve
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("0")]
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

    /// <summary>
    /// The Artifact type to query
    /// </summary>
    [StepProperty]
    [DefaultValueExplanation("Document (10)")]
    public IStep<OneOf<ArtifactType, int>> ArtifactType { get; set; } =
        new OneOfStep<ArtifactType, int>(
            new EnumConstant<ArtifactType>(Relativity.ArtifactType.Document)
        );
}
