using Carter;
using FluentValidation;
using MediatR;
using url_shortener.api.Database;
using url_shortener.core.Contracts;
using url_shortener.core.Primitives;
using url_shortener.core.Utilities;

namespace url_shortener.api.Features.ShortenedUrl;

public static class CreateShortenedUrl
{
    public class Command : IRequest<Result<GetShortenedUrlResponse>>
    {
        public string Url { get; }
        private Command(string url) => Url = url;
        public static Command Create(string url) => new Command(url);
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
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
                    new Error("CreateShortenedUrl.ValidationError", validationResult.ToString()));
            }

            try
            {
                var dt = DateTime.UtcNow;
                var newUrl = new core.Entities.ShortenedUrl
                {
                    Id = Guid.NewGuid(),
                    Url = new Uri(request.Url),
                    CreatedAt = dt,
                    UpdatedAt = dt,
                    ShortCode = StringUtilities.GenerateUniqueCode()
                };
                await context.ShortenedUrls.AddAsync(newUrl, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success(GetShortenedUrlResponse.Create(newUrl));
            }
            catch (Exception e)
            {
                return Result.Failure<GetShortenedUrlResponse>(
                    new Error("CreateShortenedUrl.Exception", e.Message));
            }
        }
    }
}

public class CreateShortenedUrlEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/shorten", async (CreateShortenedUrlRequest request, ISender sender) =>
        {
            var command = CreateShortenedUrl.Command.Create(request.Url);
            var response = await sender.Send(command);
            if (!response.IsSuccess) return Results.BadRequest(response.Error);
            return Results.Ok(response.Value);
        });
    }
}