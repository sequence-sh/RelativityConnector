using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Newtonsoft.Json;
using OneOf;
using Reductech.EDR.Connectors.Relativity.ManagerInterfaces;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;
using Relativity.Services.Interfaces.Field.Models;
using Relativity.Services.Interfaces.Shared.Models;

namespace Reductech.EDR.Connectors.Relativity.Steps
{

/// <summary>
/// Creates a new Fixed Length Field and returns the ArtifactId of that field
/// </summary>
public sealed class RelativityCreateField : RelativityApiRequest<(FixedLengthFieldRequest1 fieldRequest, int workspaceId),
    IFieldManager1,
    int, int>
{
    /// <inheritdoc />
    public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateField, int>();

    /// <inheritdoc />
    public override Result<int, IErrorBuilder> ConvertOutput(int serviceOutput)
    {
        return serviceOutput;
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
    public IStep<OneOf<int, StringStream>> Workspace { get; set; } = null!;

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
    //public IStep<OneOf<ArtifactType, int>> ObjectType { get; set; } =
    //    new OneOfStep<ArtifactType, int>(
    //        new EnumConstant<ArtifactType>(ArtifactType.Document)
    //    );

    /// <summary>
    /// The length of the field
    /// </summary>
    [StepProperty(3)]
    [DefaultValueExplanation("100")]
    public IStep<int> Length { get; set; } = new IntConstant(100);

    /// <inheritdoc />
    public override async Task<Result<(FixedLengthFieldRequest1 fieldRequest, int workspaceId), IError>> TryCreateRequest(IStateMonad stateMonad, CancellationToken cancellation)
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
            IsLinked = false,
            FilterType = FilterType.None,
            ObjectType = new ObjectTypeIdentifier()
            {
                //ArtifactID = 
                Guids = new List<Guid>(){new Guid("15c36703-74ea-4ff8-9dfb-ad30ece7530d")},
                ArtifactID = 1035231,
                ArtifactTypeID = 10,
                Name = "Document"
            }

        };

        return (request, workspaceId);
    }
}

}
