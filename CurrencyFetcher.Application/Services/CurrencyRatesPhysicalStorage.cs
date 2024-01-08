using CurrencyFetcher.Application.Models;
using CurrencyFetcher.Application.Services.Optimization;
using CurrencyFetcher.Application.Util.Helpers;
using CurrencyFetcher.Application.Util;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace CurrencyFetcher.Application.Services;

public class CurrencyRatesPhysicalStorage : IPhysicalStorage<IReadOnlyList<CurrencyRate>>
{
    private readonly IStringPool? _stringPool;

    public CurrencyRatesPhysicalStorage(IStringPool? stringPool = null)
    {
        _stringPool = stringPool;
    }

    public async Task SaveAsync(IReadOnlyList<CurrencyRate> rates, string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        var progressValue = new SimpleProgress(0, 0);
        progress?.Report(progressValue);

        var directory = Path.GetDirectoryName(path);

        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = File.OpenWrite(path);

        try
        {
            await JsonSerializer.SerializeAsync(stream, rates, cancellationToken: cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        stream.Close();

        progressValue.Finished = true;
        progress?.Report(progressValue);
    }

    public async Task<IReadOnlyList<CurrencyRate>?> LoadAsync(string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default)
    {
        var progressValue = new SimpleProgress(0, 0);
        progress?.Report(progressValue);

        using var stream = File.OpenRead(path);
        var result = await CurrencyHelper.DeserializeCurrenciesAsync(stream, _stringPool);

        stream.Close();

        progressValue.Finished = true;
        progress?.Report(progressValue);

        return result;
    }
}
