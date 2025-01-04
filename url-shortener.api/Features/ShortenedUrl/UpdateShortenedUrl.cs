using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using url_shortener.api.Database;
using url_shortener.core.Contracts;
using url_shortener.core.Primitives;
using url_shortener.core.Utilities;

namespace url_shortener.api.Features.ShortenedUrl;

public static class UpdateShortenedUrl
{
    public class Command : IRequest<Result<GetShortenedUrlResponse>>
    {
        public string ShortCode { get; }
        public string Url { get; }

        private Command(string shortCode, string url)
        {
            ShortCode = shortCode;
            Url = url;
        }
        
        public static Command Create(string shortCode, string url) => new(shortCode, url);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ShortCode).NotEmpty().WithMessage("Short code is required");
            RuleFor(x => x.Url).NotEmpty().WithMessage("Url is required")
                .Must(x => x.IsValidAbsoluteUrl()).WithMessage("Url is invalid");
        }
    }
    
    internal sealed class Handler(AppDbContext context, IValidator<Command> validator)
        : IRequestHandler<Command, Result<GetShortenedUrlResponse>>
    {
        public async Task<Result<GetShortenedUrlResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result.Failure<GetShortenedUrlResponse>(
                    new Error("UpdateShortenedUrl.ValidationError", validationResult.ToString()));
            }
            
            var urlEntry = await context.ShortenedUrls
                .FirstOrDefaultAsync(x => x.ShortCode.Equals(request.ShortCode), cancellationToken);

            if (urlEntry == null)
            {
                return Result.Failure<GetShortenedUrlResponse>(
                    new Error("UpdateShortenedUrl.NotFound", $"Short code {request.ShortCode} not found."));
            }
            
            urlEntry.Url = new Uri(request.Url);
            urlEntry.UpdatedAt = DateTime.UtcNow;
            try
            {
                await context.ShortenedUrls
                    .Where(x => x.ShortCode.Equals(request.ShortCode))
                    .ExecuteUpdateAsync(c =>
                        c.SetProperty(p => p.UpdatedAt, urlEntry.UpdatedAt)
                            .SetProperty(p => p.Url, urlEntry.Url), cancellationToken);
                return Result.Success(GetShortenedUrlResponse.Create(urlEntry));
            }
            catch (Exception e)
            {
                return Result.Failure<GetShortenedUrlResponse>(
                    new Error("UpdatedShortenedUrl.Exception", e.Message));
            }
        }
    }
}

public class UpdatedShortenedUrlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/shorten/{shortCode}",
            async (string shortCode, UpdateShortenedUrlRequest request, ISender sender) =>
            {
                var command = UpdateShortenedUrl.Command.Create(shortCode, request.Url);
                var response = await sender.Send(command);
                if (response.IsSuccess) return Results.Ok(response.Value);
                return response.Error.Code switch
                {
                    "UpdateShortenedUrl.NotFound" => Results.NotFound(response.Error),
                    _ => Results.BadRequest(response.Error)
                };
            });
    }
}