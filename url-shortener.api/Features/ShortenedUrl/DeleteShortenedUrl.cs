using Carter;
using MediatR;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;
using url_shortener.core.Primitives;

namespace url_shortener.api.Features.ShortenedUrl;

public static class DeleteShortenedUrl
{
    public class Query : IRequest<Result>
    {
        public string ShortCode { get; }
        private Query(string shortCode) => ShortCode = shortCode;
        public static Query Create(string shortCode) => new(shortCode);
    }

    internal sealed class Handler(AppDbContext context) : IRequestHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var urlEntry = await context.ShortenedUrls
                .FirstOrDefaultAsync(x => x.ShortCode.Equals(request.ShortCode), cancellationToken);

            if (urlEntry == null)
            {
                return Result.Failure(
                    new Error("DeleteShortenedUrl.NotFound", "Shortened url does not exist."));
            }

            try
            {
                context.ShortenedUrls.Remove(urlEntry);
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(
                    new Error("DeleteShortenedUrl.Exception", e.Message));
            }
        }
    }
}

public class DeleteShortenedUrlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/shorten/{shortCode}", async (string shortCode, ISender sender) =>
        {
            var query = DeleteShortenedUrl.Query.Create(shortCode);
            var response = await sender.Send(query);
            if (response.IsSuccess) return Results.NoContent();
            return response.Error.Code switch
            {
                "DeleteShortenedUrl.NotFound" => Results.NotFound(response.Error),
                _ => Results.BadRequest(response.Error)
            };
        });
    }
}