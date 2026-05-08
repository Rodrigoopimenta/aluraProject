using AppVideo.Domain.Entities;
using AppVideo.Domain.Repositories;

namespace AppVideo.Infrastructure.Persistence;

public sealed class InMemoryVideoRepository : IVideoRepository
{
    private static readonly IReadOnlyList<Video> Videos =
    [
        new Video(Guid.Parse("f0a65a35-c362-4fb9-8fd3-08debe7c4d11"), "Fundamentos de MAUI", "Mobile", TimeSpan.FromMinutes(15)),
        new Video(Guid.Parse("ce5e34fc-2a56-4f79-a253-c7ea1843d2e7"), "Introducao ao DDD", "Arquitetura", TimeSpan.FromMinutes(21)),
        new Video(Guid.Parse("1a6f33bb-c6d2-4d0d-a28a-1f8a159fa8f6"), "Clean Architecture na pratica", "Arquitetura", TimeSpan.FromMinutes(18))
    ];

    public Task<IReadOnlyList<Video>> GetFeaturedAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(Videos);
    }
}