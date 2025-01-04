using System.Runtime.CompilerServices;
using MediatR;
using url_shortener.api.Database;
using url_shortener.core.Entities;
using url_shortener.core.Primitives;

namespace url_shortener.api.Features.ShortenedUrl;

public static class AccessShortenedUrl
{
    public class Command : IRequest<Result>
    {
        public Guid Id { get; }
        public string IpAddress { get; set; }

        private Command(Guid id, string ipAddress)
        {
            Id = id;
            IpAddress = ipAddress;
        }

        public static Command Create(Guid id, string ipAddress) => new Command(id, ipAddress);
    }

    internal sealed class Handler(AppDbContext context)
        : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                var visit = new ShortenedUrlVisit
                {
                    Id = Guid.NewGuid(),
                    IpAddress = request.IpAddress,
                    DateAccessed = DateTime.UtcNow,
                    ShortenedUrlId = request.Id
                };
                
                await context.ShortenedUrlVisits.AddAsync(visit, cancellationToken);
                await context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }
            catch (Exception e)
            {
                return Result.Failure(
                    new Error("TrackVisit.Exception", e.Message));
            }
        }
    }
}