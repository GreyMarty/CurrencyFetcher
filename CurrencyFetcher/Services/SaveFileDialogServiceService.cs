using Microsoft.Win32;

namespace CurrencyFetcher.Services;

public interface ISaveFileDialogService : IFileDialogService
{
}

public class SaveFileDialogServiceService : FileDialogServiceBase, ISaveFileDialogService
{
    protected override FileDialog CreateDialog()
    {
        return new SaveFileDialog();
    }
}