using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flurl.Http;

namespace Reductech.EDR.Connectors.Relativity.Managers
{



public abstract class ManagerBase : IDisposable
{
    protected ManagerBase(RelativitySettings relativitySettings, IFlurlClient flurlClient)
    {
        RelativitySettings = relativitySettings;
        FlurlClient        = flurlClient;
    }

    public RelativitySettings RelativitySettings { get; }

    public IFlurlClient FlurlClient { get; }

    /// <inheritdoc />
    public void Dispose() { }

    public Task PutAsync(string route, object data, CancellationToken cancellationToken)
    {
        var completeRoute = CreateCompleteRoute(route);

        return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
                .PutJsonAsync(data, cancellationToken)
            ;
    }

    //public Task<T> PutAsync<T>(string route, object data, CancellationToken cancellationToken)
    //{
    //    var completeRoute = CreateCompleteRoute(route);

    //    return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
    //            .PutJsonAsync(data, cancellationToken)
    //            .ReceiveJson<T>()
    //        ;
    //}

    public Task DeleteAsync(string route, CancellationToken cancellationToken)
    {
        var completeRoute = CreateCompleteRoute(route);

        return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
            .DeleteAsync(cancellationToken);
    }

    public Task<T> GetJsonAsync<T>(string route, CancellationToken cancellationToken)
    {
        var completeRoute = CreateCompleteRoute(route);

        return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
            .GetJsonAsync<T>(cancellationToken);
    }
    
    public Task<string> DownloadAsync(string route, CancellationToken cancellationToken)
    {
        var completeRoute = CreateCompleteRoute(route);

        return FlurlClient.SetupRelativityRequest(RelativitySettings, completeRoute)
            .GetStringAsync(cancellationToken);
    }

    public Task<T> PostJsonAsync<T>(string route, object thing, CancellationToken cancellation)
    {
        var completeRoute = CreateCompleteRoute(route);

        return PostJsonAsync<T>(completeRoute, thing, cancellation);
    }
    
    public Task<T> PostJsonAsync<T>(string[] completeRoute, object thing, CancellationToken cancellation)
    {

        return FlurlClient.SetupRelativityRequest(
                RelativitySettings,
                completeRoute
            )
            .PostJsonAsync(thing, cancellation)
            .ReceiveJson<T>();
    }

    public Task PostJsonAsync(string route, object thing, CancellationToken cancellation)
    {
        var completeRoute = CreateCompleteRoute(route);

        return FlurlClient.SetupRelativityRequest(
                RelativitySettings,
                completeRoute
            )
            .PostJsonAsync(thing, cancellation);
    }

    private string[] CreateCompleteRoute(string route)
    {
        var splitRoute = SplitRoute(route);

        var index = splitRoute.ToList().IndexOf("~");

        if (index >= 0)
        {
            splitRoute = splitRoute[(index + 1)..];
        }

        return Prefixes.Concat(splitRoute).ToArray();
    }

    protected abstract IEnumerable<string> Prefixes { get; }

    private static string[] SplitRoute(string s)
    {
        return s.Split("/");
    }
}

}
