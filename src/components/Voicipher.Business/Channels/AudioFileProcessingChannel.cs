using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<Guid, IList<RecognitionFile>> _cache = new();
        private readonly ConcurrentDictionary<Guid, int> _progressCache = new();
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
                    _logger.Warning($"[{recognitionFile.UserId}] Recognition file {JsonConvert.SerializeObject(recognitionFile)} was not added to cache");
                }

                _progressCache.TryAdd(recognitionFile.AudioFileId, 0);

                if (_channel.Writer.TryWrite(recognitionFile))
                {
                    _logger.Information($"[{recognitionFile.UserId}] Recognition file {JsonConvert.SerializeObject(recognitionFile)} was written to the channel");

                    return true;
                }
            }

            return false;
        }

        public bool IsProcessingForUser(Guid userId)
        {
            lock (_logger)
            {
                return _cache.ContainsKey(userId) && _cache[userId].Any();
            }
        }

        public void FinishProcessing(RecognitionFile recognitionFile)
        {
            _progressCache.TryRemove(recognitionFile.AudioFileId, out _);

            lock (_lockObject)
            {
                if (_cache.ContainsKey(recognitionFile.UserId))
                {
                    var countBefore = _cache[recognitionFile.UserId].Count;
                    var cacheItem = _cache[recognitionFile.UserId].SingleOrDefault(x => x.AudioFileId == recognitionFile.AudioFileId);

                    if (cacheItem != null)
                    {
                        _cache[recognitionFile.UserId].Remove(cacheItem);

                        var countAfter = _cache[recognitionFile.UserId].Count;
                        _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was removed from cache. Cache ({countBefore}) -> ({countAfter})");
                    }
                    else
                    {
                        var countAfter = _cache[recognitionFile.UserId].Count;
                        _logger.Information($"Recognition file {JsonConvert.SerializeObject(recognitionFile)} was not found in the cache. Cache ({countBefore}) -> ({countAfter})");
                    }
                }
            }
        }

        public void UpdateProgress(Guid audioFileId, int progress)
        {
            _progressCache.TryGetValue(audioFileId, out var oldValue);
            _progressCache.TryUpdate(audioFileId, progress, oldValue);
        }

        public int? GetProgress(Guid audioFile)
        {
            if (_progressCache.TryGetValue(audioFile, out int progress))
            {
                return progress;
            }

            return null;
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
