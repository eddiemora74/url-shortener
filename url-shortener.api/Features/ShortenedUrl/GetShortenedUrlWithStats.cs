using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;
using url_shortener.core.Contracts;
using url_shortener.core.Primitives;

namespace url_shortener.api.Features.ShortenedUrl;

public static class GetShortenedUrlWithStats
{
    public class Query : IRequest<Result<GetShortenedUrlStatsResponse>>
    {
        public string ShortCode { get; }
        private Query(string shortCode) => ShortCode = shortCode;
        public static Query Create(string shortCode) => new Query(shortCode);
    }

    internal sealed class Handler(AppDbContext dbContext) : IRequestHandler<Query, Result<GetShortenedUrlStatsResponse>>
    {
        public async Task<Result<GetShortenedUrlStatsResponse>> Handle(Query request, CancellationToken cancellationToken)
        {
            try
            {
                var url = await dbContext.ShortenedUrls
                    .Where(u => u.ShortCode.Equals(request.ShortCode))
                    .Include(u => u.Visits)
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (url == null) return Result.Failure<GetShortenedUrlStatsResponse>(
                    new Error("GetShortenedUrlWithStats.NotFound", "Shortened url doesn't exist"));
                
                return Result.Success(GetShortenedUrlStatsResponse.Create(url));
            }
            catch (Exception e)
            {
                return Result.Failure<GetShortenedUrlStatsResponse>(
                    new Error("GetShortenedUrlWithStats.Exception", e.Message));
            }
        }
    }
}

public class GetShortenedUrlStatsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/shorten/{shortCode}/stats", async (string shortCode, ISender sender) =>
        {
            var query = GetShortenedUrlWithStats.Query.Create(shortCode);
            var result = await sender.Send(query);
            if (result.IsSuccess) return Results.Ok(result.Value);
            return result.Error.Code switch
            {
                "GetShortenedUrlWithStats.NotFound" => Results.NotFound(result.Error),
                _ => Results.BadRequest(result.Error)
            };
        });
    }
}