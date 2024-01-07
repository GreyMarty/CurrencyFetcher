using Microsoft.Win32;

namespace CurrencyFetcher.Services;

public interface ISaveFileDialogService
{
    public string? Path { get; }
    public bool? ShowDialog(SaveFileDialogOptions? options = null);
}

public class SaveFileDialogService : ISaveFileDialogService
{
    public string? Path { get; private set; }

    public bool? ShowDialog(SaveFileDialogOptions? options = null)
    {
        var dialog = new SaveFileDialog();
        dialog.Filter = options?.Filter ?? dialog.Filter;
        
        var result = dialog.ShowDialog();
        Path = dialog.FileName;

        return result;
    }
}

public class SaveFileDialogOptions
{
    public string? Filter { get; set; }
}