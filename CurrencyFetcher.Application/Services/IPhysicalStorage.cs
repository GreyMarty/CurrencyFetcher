using CurrencyFetcher.Application.Util;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CurrencyFetcher.Application.Services;

public interface IPhysicalStorage<T>
{
    public Task SaveAsync(T value, string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
    public Task<T> LoadAsync(string path, IProgress<SimpleProgress>? progress = null, CancellationToken cancellationToken = default);
}
