using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Internal.Errors;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Relativity
{
    public static class StepHelpers //TODO move to Core
    {

        private class StringStreamWrapper : IStep<string>
        {
            public IStep<StringStream> _step;

            public StringStreamWrapper(IStep<StringStream> step)
            {
                _step = step;
            }

            /// <inheritdoc />
            public async Task<Result<string, IError>> Run(IStateMonad stateMonad, CancellationToken cancellationToken)
            {
                return await _step.Run(stateMonad, cancellationToken).Map(x => x.GetStringAsync());
            }

            /// <inheritdoc />
            public async Task<Result<T, IError>> Run<T>(IStateMonad stateMonad, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Result<Unit, IError> Verify(StepFactoryStore stepFactoryStore)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public string Serialize()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Maybe<EntityValue> TryConvertToEntityValue()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public string Name => throw new NotImplementedException();

            /// <inheritdoc />
            public bool ShouldBracketWhenSerialized => throw new NotImplementedException();

            /// <inheritdoc />
            public TextLocation? TextLocation
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Type OutputType => throw new NotImplementedException();

            /// <inheritdoc />
            public IEnumerable<Requirement> RuntimeRequirements => throw new NotImplementedException();
        }


        private class NullableStepWrapper<T> : IStep<T?> //TODO change to a different interface
        where T : struct
        {
            private IStep<T>? _step;

            public NullableStepWrapper(IStep<T>? step)
            {
                _step = step;
            }

            /// <inheritdoc />
            public async Task<Result<T?, IError>> Run(IStateMonad stateMonad, CancellationToken cancellationToken)
            {
                if(_step is null)
                    return Result.Success<T?, IError>((T?) null);

                var r = await _step.Run(stateMonad, cancellationToken);
                return r.Value;
            }


            /// <inheritdoc />
            public async Task<Result<T1, IError>> Run<T1>(IStateMonad stateMonad, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Result<Unit, IError> Verify(StepFactoryStore stepFactoryStore)
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public string Serialize()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Maybe<EntityValue> TryConvertToEntityValue()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public string Name => throw new NotImplementedException();

            /// <inheritdoc />
            public bool ShouldBracketWhenSerialized => throw new NotImplementedException();

            /// <inheritdoc />
            public TextLocation? TextLocation
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Type OutputType => throw new NotImplementedException();

            /// <inheritdoc />
            public IEnumerable<Requirement> RuntimeRequirements => throw new NotImplementedException();
        }

        public static IStep<string> WrapStringStream(this IStep<StringStream> step)
        {
            return new StringStreamWrapper(step);
        }

        public static IStep<T?> WrapNullable<T>(this IStep<T>? step) where T : struct
        {
            return new NullableStepWrapper<T>(step);
        }

        public static async Task<Result<(T1, T2), IError>> RunSteps<T1, T2>(IStep<T1> s1, IStep<T2> s2,
            StateMonad stateMonad, CancellationToken cancellationToken)
        {
            var r1 = await s1.Run(stateMonad, cancellationToken);
            if (r1.IsFailure) return r1.ConvertFailure<(T1, T2)>();

            var r2 = await s2.Run(stateMonad, cancellationToken);
            if (r2.IsFailure) return r2.ConvertFailure<(T1, T2)>();

            return (r1.Value, r2.Value);
        }

        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3), IError>> RunSteps<T1, T2, T3>(IStep<T1> s1, IStep<T2> s2,
            IStep<T3> s3, StateMonad stateMonad, CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3)>();
            var r3 = await s3.Run(stateMonad, cancellationToken);
            if (r3.IsFailure) return r3.ConvertFailure<(T1, T2, T3)>();
            var result = (p.Value.Item1, p.Value.Item2, r3.Value);
            return result;
        }


        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3, T4), IError>> RunSteps<T1, T2, T3, T4>(IStep<T1> s1, IStep<T2> s2,
            IStep<T3> s3, IStep<T4> s4, StateMonad stateMonad, CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, s3, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3, T4)>();
            var r4 = await s4.Run(stateMonad, cancellationToken);
            if (r4.IsFailure) return r4.ConvertFailure<(T1, T2, T3, T4)>();
            var result = (p.Value.Item1, p.Value.Item2, p.Value.Item3, r4.Value);
            return result;
        }


        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3, T4, T5), IError>> RunSteps<T1, T2, T3, T4, T5>(IStep<T1> s1,
            IStep<T2> s2, IStep<T3> s3, IStep<T4> s4, IStep<T5> s5, StateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, s3, s4, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3, T4, T5)>();
            var r5 = await s5.Run(stateMonad, cancellationToken);
            if (r5.IsFailure) return r5.ConvertFailure<(T1, T2, T3, T4, T5)>();
            var result = (p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4, r5.Value);
            return result;
        }


        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3, T4, T5, T6), IError>> RunSteps<T1, T2, T3, T4, T5, T6>(
            IStep<T1> s1, IStep<T2> s2, IStep<T3> s3, IStep<T4> s4, IStep<T5> s5, IStep<T6> s6, StateMonad stateMonad,
            CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, s3, s4, s5, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3, T4, T5, T6)>();
            var r6 = await s6.Run(stateMonad, cancellationToken);
            if (r6.IsFailure) return r6.ConvertFailure<(T1, T2, T3, T4, T5, T6)>();
            var result = (p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4, p.Value.Item5, r6.Value);
            return result;
        }


        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3, T4, T5, T6, T7), IError>> RunSteps<T1, T2, T3, T4, T5, T6, T7>(
            IStep<T1> s1, IStep<T2> s2, IStep<T3> s3, IStep<T4> s4, IStep<T5> s5, IStep<T6> s6, IStep<T7> s7,
            StateMonad stateMonad, CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, s3, s4, s5, s6, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3, T4, T5, T6, T7)>();
            var r7 = await s7.Run(stateMonad, cancellationToken);
            if (r7.IsFailure) return r7.ConvertFailure<(T1, T2, T3, T4, T5, T6, T7)>();
            var result = (p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4, p.Value.Item5, p.Value.Item6,
                r7.Value);
            return result;
        }


        [GeneratedCode("CreateStepHelpers", "1")]
        public static async Task<Result<(T1, T2, T3, T4, T5, T6, T7, T8), IError>>
            RunSteps<T1, T2, T3, T4, T5, T6, T7, T8>(IStep<T1> s1, IStep<T2> s2, IStep<T3> s3, IStep<T4> s4,
                IStep<T5> s5, IStep<T6> s6, IStep<T7> s7, IStep<T8> s8, StateMonad stateMonad,
                CancellationToken cancellationToken)
        {
            var p = await RunSteps(s1, s2, s3, s4, s5, s6, s7, stateMonad, cancellationToken);
            if (p.IsFailure) return p.ConvertFailure<(T1, T2, T3, T4, T5, T6, T7, T8)>();
            var r8 = await s8.Run(stateMonad, cancellationToken);
            if (r8.IsFailure) return r8.ConvertFailure<(T1, T2, T3, T4, T5, T6, T7, T8)>();
            var result = (p.Value.Item1, p.Value.Item2, p.Value.Item3, p.Value.Item4, p.Value.Item5, p.Value.Item6,
                p.Value.Item7, r8.Value);
            return result;
        }
    }
}