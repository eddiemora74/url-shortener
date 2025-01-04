using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;
using url_shortener.core.Contracts;
using url_shortener.core.Primitives;

namespace url_shortener.api.Features.ShortenedUrl;

public static class ListShortenedUrls
{
    public class Query : IRequest<Result<List<GetShortenedUrlResponse>>>
    {
        public static Query Create() => new Query();
    }

    internal sealed class Handler(AppDbContext dbContext)
        : IRequestHandler<Query, Result<List<GetShortenedUrlResponse>>>
    {
        public async Task<Result<List<GetShortenedUrlResponse>>> Handle(Query request,
            CancellationToken cancellationToken)
        {
            try
            {
                var results = await dbContext.ShortenedUrls
                    .OrderByDescending(x => x.UpdatedAt)
                    .ToListAsync(cancellationToken);
                return Result.Success(results.Select(GetShortenedUrlResponse.Create).ToList());
            }
            catch (Exception e)
            {
                return Result.Failure<List<GetShortenedUrlResponse>>(
                    new Error("ListShortenedUrls.Exception", e.Message));
            }
        }
    }
}

public class ListShortenedUrlsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/shorten-list", async (ISender sender) =>
        {
            var query = ListShortenedUrls.Query.Create();
            var result = await sender.Send(query);
            if (result.IsSuccess) return Results.Ok(result.Value);
            return Results.BadRequest(result.Error);
        });
    }
}