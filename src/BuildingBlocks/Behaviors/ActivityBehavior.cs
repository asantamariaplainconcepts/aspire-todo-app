using System.Diagnostics;
using BuildingBlocks.Common;
using BuildingBlocks.Diagnostics;

namespace BuildingBlocks.Behaviors;

public class ActivityBehavior<TRequest, TResponse>()
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        using var activity = Instrumentation.Source.StartActivity(request.GetType().GetRequestName()!);
        {
            try
            {
                var response = await next();
                
                if (response is Result result)
                {
                    activity?.SetStatus(result.IsSuccess == false ? ActivityStatusCode.Error : ActivityStatusCode.Ok);
                }
                else
                {
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
                
                return response;
            }
            catch (Exception ex)
            {
                activity?.SetStatus(ActivityStatusCode.Error);
                activity?.AddException(ex);
                throw;
            }
          
        }
    }
}