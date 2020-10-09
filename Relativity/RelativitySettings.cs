using System;
using System.Text;
using CSharpFunctionalExtensions;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Util;

namespace Reductech.Connectors.Relativity
{
    public sealed class RelativitySettings : IRelativitySettings
    {
        public string RelativityUsername { get;set; }

        public string RelativityPassword { get;set; }

        public string Url { get; set; }

        /// <inheritdoc />
        public string DesktopClientPath { get; set; }

        public string AuthParameters => GenerateBasicAuthorizationParameter(RelativityUsername, RelativityPassword);





        static string GenerateBasicAuthorizationParameter(string username, string password)
        {
            var unencodedUsernameAndPassword = $"{username}:{password}";
            var unencodedBytes = Encoding.ASCII.GetBytes(unencodedUsernameAndPassword);
            var base64UsernameAndPassword = Convert.ToBase64String(unencodedBytes);

            return $"Basic {base64UsernameAndPassword}";
        }

        /// <inheritdoc />
        public Result<Unit, IRunErrors> CheckRequirement(string processName, Requirement requirement) //TODO add relativity version
            => Unit.Default;
    }
}