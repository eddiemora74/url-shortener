using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;
using url_shortener.core.Contracts;
using url_shortener.core.Primitives;

namespace url_shortener.api.Features.ShortenedUrl;

public static class GetShortenedUrl
{
    public class Query : IRequest<Result<GetShortenedUrlResponse>>
    {
        public string ShortCode { get; }
        private Query(string shortCode) => ShortCode = shortCode;
        public static Query Create(string shortCode) => new(shortCode);
    }
    
    internal sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, Result<GetShortenedUrlResponse>> 
    {
        public async Task<Result<GetShortenedUrlResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var url = await dbContext.ShortenedUrls
                    .FirstOrDefaultAsync(u => u.ShortCode.Equals(request.ShortCode), cancellationToken);
                
                if (url == null) return Result.Failure<GetShortenedUrlResponse>(
                    new Error("GetShortenedUrl.NotFound", "Shortened url does not exist."));

                return Result.Success(GetShortenedUrlResponse.Create(url));
            }
            catch (Exception e)
            {
                return Result.Failure<GetShortenedUrlResponse>(
                    new Error("GetShortenedUrl.Exception", e.Message));
            }
        }
    }
}

public class GetShortenedUrlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/shorten/{shortCode}", async (string shortCode, ISender sender) =>
        {
            var query = GetShortenedUrl.Query.Create(shortCode);
            var result = await sender.Send(query);
            if (result.IsSuccess) return Results.Ok(result.Value);
            return result.Error.Code switch
            {
                "GetShortenedUrl.NotFound" => Results.NotFound(result.Error),
                _ => Results.BadRequest(result.Error)
            };
        });
    }
}