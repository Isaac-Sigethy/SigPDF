using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PdfEditor.Core.Logging;

public sealed class JsonLineLoggerProvider : ILoggerProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _filePath;
    private readonly object _writeLock = new();
    private StreamWriter? _writer;
    private bool _disposed;

    public JsonLineLoggerProvider(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        Directory.CreateDirectory(directory);
        _filePath = Path.Combine(directory, $"pdfeditor-{DateTime.UtcNow:yyyyMMdd}.log");
    }

    public string FilePath => _filePath;

    public ILogger CreateLogger(string categoryName) => new JsonLineLogger(categoryName, this);

    internal void Write(IReadOnlyDictionary<string, object?> payload)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        var line = JsonSerializer.Serialize(payload, JsonOptions);
        lock (_writeLock)
        {
            _writer ??= new StreamWriter(new FileStream(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                AutoFlush = true
            };
            _writer.WriteLine(line);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        lock (_writeLock)
        {
            _writer?.Dispose();
            _writer = null;
            _disposed = true;
        }
    }

    private sealed class JsonLineLogger : ILogger
    {
        private readonly string _category;
        private readonly JsonLineLoggerProvider _provider;

        public JsonLineLogger(string category, JsonLineLoggerProvider provider)
        {
            _category = category;
            _provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var message = formatter(state, exception);
            var payload = new Dictionary<string, object?>
            {
                ["ts"] = DateTimeOffset.UtcNow,
                ["level"] = logLevel.ToString(),
                ["category"] = _category,
                ["eventId"] = eventId.Id,
                ["event"] = eventId.Name
            };

            foreach (var (key, value) in ReadStructuredState(state))
            {
                if (key == "{OriginalFormat}")
                {
                    continue;
                }

                message = LogSanitizer.RedactMessage(message, key, value);
                payload[key] = LogSanitizer.SanitizeValue(key, value);
            }

            payload["message"] = message;

            if (exception is not null)
            {
                payload["exceptionType"] = exception.GetType().FullName;
            }

            _provider.Write(payload);
        }

        private static IEnumerable<(string Key, object? Value)> ReadStructuredState<TState>(TState state)
        {
            if (state is not System.Collections.IEnumerable raw)
            {
                yield break;
            }

            foreach (var item in raw)
            {
                if (item is KeyValuePair<string, object> pair)
                {
                    yield return (pair.Key, pair.Value);
                }
            }
        }
    }

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
