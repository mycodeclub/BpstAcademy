namespace BpstEdu.Application.Abstractions;

/// <summary>Current time. Always UTC; convert to India time (IST) only for display.</summary>
public interface IClock
{
    DateTimeOffset UtcNow { get; }
}
