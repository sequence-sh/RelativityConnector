using Reductech.EDR.Core;

namespace Reductech.Connectors.Relativity
{
    public interface IRelativitySettings : ISettings
    {
        /// <summary>
        /// The path to the relativity desktop client
        /// </summary>
        string DesktopClientPath { get; }

        /// <summary>
        /// The Relativity User Name (usually an email address)
        /// </summary>
        string RelativityUsername { get; }

        /// <summary>
        /// The relativity password
        /// </summary>
        string RelativityPassword { get; } //TODO replace with a different form of authentication


    }
}