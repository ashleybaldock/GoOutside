using System;

namespace GoOutside.Configuration
{
    public interface IConfiguration
    {
        TimeSpan PomoDuration { get; }
        TimeSpan PomoBreakDuration { get; }
    }
}