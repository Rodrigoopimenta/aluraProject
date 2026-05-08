namespace AppVideo.Application.DTOs;

public sealed record VideoDto(Guid Id, string Title, string Category, TimeSpan Duration);