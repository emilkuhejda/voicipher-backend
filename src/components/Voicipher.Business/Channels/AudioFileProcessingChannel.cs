using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using Voicipher.Domain.Interfaces.Channels;
using Voicipher.Domain.Models;

namespace Voicipher.Business.Channels
{
    public class AudioFileProcessingChannel : IAudioFileProcessingChannel
    {
        private readonly ConcurrentDictionary<Guid, IList<RecognitionFile>> _cache = new ConcurrentDictionary<Guid, IList<RecognitionFile>>();
        private readonly Channel<RecognitionFile> _channel;
        private readonly ILogger _logger;

        private readonly object _lockObject = new object();

        public AudioFileProcessingChannel(ILogger logger)
        {
            _channel = Channel.CreateUnbounded<RecognitionFile>();
            _logger = logger.ForContext<AudioFileProcessingChannel>();
        }

        public IAsyncEnumerable<RecognitionFile> ReadAllAsync(CancellationToken cancellationToken = default)
        {
            return _channel.Reader.ReadAllAsync(cancellationToken);
        }

        public async Task<bool> AddFileAsync(RecognitionFile recognitionFile, CancellationToken cancellationToken = default)
        {
            while (await _channel.Writer.WaitToWriteAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                if (!TryAddToCache(recognitionFile))
                {
                    _logger.Warning("Recognition file {JsonConvert.SerializeObject(recognitionFile)} was not added to cache");
                }

                if (_channel.Writer.TryWrite(recognitionFile))
                {
                    _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was written to the channel");

                    return true;
                }
            }

            return false;
        }

        public void FinishProcessing(RecognitionFile recognitionFile)
        {
            lock (_lockObject)
            {
                if (_cache.ContainsKey(recognitionFile.UserId))
                {
                    _cache[recognitionFile.UserId].Remove(recognitionFile);

                    _logger.Information("Recognition file {JsonConvert.SerializeObject(recognitionFile)} was removed from cache");
                }
            }
        }

        private bool TryAddToCache(RecognitionFile recognitionFile)
        {
            lock (_lockObject)
            {
                if (_cache.ContainsKey(recognitionFile.UserId))
                {
                    _cache[recognitionFile.UserId].Add(recognitionFile);

                    return true;
                }

                return _cache.TryAdd(recognitionFile.UserId, new List<RecognitionFile> { recognitionFile });
            }
        }
    }
}
