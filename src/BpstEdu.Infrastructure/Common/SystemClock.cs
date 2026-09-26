using BpstEdu.Application.Abstractions;

namespace BpstEdu.Infrastructure.Common;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
