using BpstAcademy.Application.Abstractions;

namespace BpstAcademy.Infrastructure.Common;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
