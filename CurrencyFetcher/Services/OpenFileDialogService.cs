using Microsoft.Win32;

namespace CurrencyFetcher.Services;

public interface IOpenFileDialogService : IFileDialogService
{
    
}

public class OpenFileDialogService : FileDialogServiceBase, IOpenFileDialogService
{
    protected override FileDialog CreateDialog() => new OpenFileDialog();
}