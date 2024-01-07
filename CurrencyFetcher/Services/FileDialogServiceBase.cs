using Microsoft.Win32;

namespace CurrencyFetcher.Services;

public interface IFileDialogService
{
    public string? Path { get; }
    public bool? ShowDialog(FileDialogOptions? options = null);
}

public abstract class FileDialogServiceBase : IFileDialogService
{
    public string? Path { get; private set; }

    public bool? ShowDialog(FileDialogOptions? options = null)
    {
        var dialog = CreateDialog();
        dialog.Filter = options?.Filter ?? dialog.Filter;
        
        var result = dialog.ShowDialog();
        Path = dialog.FileName;

        return result;
    }

    protected abstract FileDialog CreateDialog();
}

public class FileDialogOptions
{
    public string? Filter { get; set; }
}