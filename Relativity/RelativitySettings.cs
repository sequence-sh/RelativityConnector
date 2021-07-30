using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Reductech.EDR.Core;

namespace Reductech.EDR.Connectors.Relativity
{
    [DataContract]
    public sealed class RelativitySettings : IEntityConvertible
    {
        [DataMember] public string RelativityUsername { get; set; } = null!;

        [DataMember] public string RelativityPassword { get; set; } = null!;

        [DataMember] public string Url { get; set; } = null!;

        [DataMember] public string DesktopClientPath { get; set; } = null!;

        /// <summary>
        /// The version of the API
        /// </summary>
        [DataMember]
        public int APIVersionNumber { get; set; } = 1;

        public string AuthParameters => GenerateBasicAuthorizationParameter(RelativityUsername, RelativityPassword);

        static string GenerateBasicAuthorizationParameter(string username, string password)
        {
            var unencodedUsernameAndPassword = $"{username}:{password}";
            var unencodedBytes = Encoding.ASCII.GetBytes(unencodedUsernameAndPassword);
            var base64UsernameAndPassword = Convert.ToBase64String(unencodedBytes);

            return $"Basic {base64UsernameAndPassword}";
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>()
            {
                { nameof(RelativityUsername), RelativityUsername },
                { nameof(RelativityPassword), RelativityPassword },
                { nameof(Url), Url },
                { nameof(DesktopClientPath), DesktopClientPath },
                { nameof(APIVersionNumber), APIVersionNumber },
            };
        }
    }
}