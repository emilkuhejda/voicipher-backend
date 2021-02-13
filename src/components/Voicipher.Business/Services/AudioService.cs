using System;
using NAudio.Wave;
using Voicipher.Domain.Interfaces.Services;

namespace Voicipher.Business.Services
{
    public class AudioService : IAudioService
    {
        public TimeSpan? GetTotalTime(string filePath)
        {
            try
            {
                using (var reader = new MediaFoundationReader(filePath))
                {
                    return reader.TotalTime;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
