using System;

namespace Voicipher.Domain.Interfaces.Services
{
    public interface IAudioService
    {
        TimeSpan? GetTotalTime(string filePath);
    }
}
