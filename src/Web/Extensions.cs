using Microsoft.AspNetCore.SignalR.Client;
namespace BlazorApp;

public static  class HubConnectionExtensions
{
    public static IHubConnectionBuilder WithUrl(this IHubConnectionBuilder builder, string url,
        IHttpMessageHandlerFactory clientFactory)
    {
        return builder.WithUrl(url,
            options => { options.HttpMessageHandlerFactory = _ => clientFactory.CreateHandler(); });
    }
}