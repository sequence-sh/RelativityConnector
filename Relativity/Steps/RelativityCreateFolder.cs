//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using CSharpFunctionalExtensions;
//using Reductech.EDR.Core;
//using Reductech.EDR.Core.Internal;
//using Reductech.EDR.Core.Internal.Errors;
//using Reductech.EDR.Core.Util;

//namespace Reductech.EDR.Connectors.Relativity.Steps
//{
//    public class RelativityCreateFolder : CompoundStep<Unit>
//    {
//        /// <inheritdoc />
//        protected override async Task<Result<Unit, IError>> Run(IStateMonad stateMonad, CancellationToken cancellationToken)
//        {
//            var workSpaceRequest = await CreateWorkspaceRequest(stateMonad, cancellationToken);
//            if (workSpaceRequest.IsFailure) return workSpaceRequest.ConvertFailure<Entity>();

//            var settingsResult = stateMonad.Settings.TryGetRelativitySettings();
//            if (settingsResult.IsFailure)
//                return settingsResult.MapError(x => x.WithLocation(this)).ConvertFailure<Entity>();

//            var flurlClientResult = stateMonad.GetFlurlClientFactory().Map(x => x.FlurlClient);
//            if (flurlClientResult.IsFailure)
//                return flurlClientResult.MapError(x => x.WithLocation(this)).ConvertFailure<Entity>();
//        }

//        /// <inheritdoc />
//        public override IStepFactory StepFactory => new SimpleStepFactory<RelativityCreateFolder, Unit>();
//    }
//}
