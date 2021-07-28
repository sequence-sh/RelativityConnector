using System;
using Flurl.Http;

namespace Reductech.EDR.Connectors.Relativity
{

public interface IFlurlClientFactory : IDisposable
{
    IFlurlClient FlurlClient { get; }
}

}
