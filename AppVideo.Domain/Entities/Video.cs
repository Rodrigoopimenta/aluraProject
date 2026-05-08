namespace AppVideo.Domain.Entities;

public sealed class Video
{
    public Video(Guid id, string title, string category, TimeSpan duration)
    {
        if (id == Guid.Empty) throw new ArgumentException("Video id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Title is required.", nameof(title));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("Category is required.", nameof(category));
        if (duration <= TimeSpan.Zero) throw new ArgumentException("Duration must be greater than zero.", nameof(duration));

        Id = id;
        Title = title;
        Category = category;
        Duration = duration;
    }

    public Guid Id { get; }
    public string Title { get; }
    public string Category { get; }
    public TimeSpan Duration { get; }
}