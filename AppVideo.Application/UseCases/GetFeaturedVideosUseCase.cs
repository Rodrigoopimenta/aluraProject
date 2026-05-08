using AppVideo.Application.DTOs;
using AppVideo.Domain.Repositories;

namespace AppVideo.Application.UseCases;

public sealed class GetFeaturedVideosUseCase
{
    private readonly IVideoRepository _videoRepository;

    public GetFeaturedVideosUseCase(IVideoRepository videoRepository)
    {
        _videoRepository = videoRepository;
    }

    public async Task<IReadOnlyList<VideoDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var videos = await _videoRepository.GetFeaturedAsync(cancellationToken);

        return videos
            .Select(video => new VideoDto(video.Id, video.Title, video.Category, video.Duration))
            .ToList();
    }
}