using AppVideo.Domain.Entities;

namespace AppVideo.Domain.Repositories;

public interface IVideoRepository
{
    Task<IReadOnlyList<Video>> GetFeaturedAsync(CancellationToken cancellationToken = default);
}