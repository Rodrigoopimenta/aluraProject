using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppVideo.Application.UseCases;

namespace AppVideo.ViewModels;

public sealed class MainPageViewModel : INotifyPropertyChanged
{
    private readonly GetFeaturedVideosUseCase _getFeaturedVideosUseCase;
    private bool _isBusy;

    public MainPageViewModel(GetFeaturedVideosUseCase getFeaturedVideosUseCase)
    {
        _getFeaturedVideosUseCase = getFeaturedVideosUseCase;
    }

    public ObservableCollection<VideoItemViewModel> FeaturedVideos { get; } = [];

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (IsBusy || FeaturedVideos.Count > 0) return;

        IsBusy = true;
        try
        {
            var videos = await _getFeaturedVideosUseCase.ExecuteAsync(cancellationToken);
            FeaturedVideos.Clear();

            foreach (var video in videos)
            {
                FeaturedVideos.Add(new VideoItemViewModel(video.Title, video.Category, video.Duration));
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public sealed record VideoItemViewModel(string Title, string Category, TimeSpan Duration)
{
    public string DurationText => $"{(int)Duration.TotalMinutes} min";
}