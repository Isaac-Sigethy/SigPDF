using PdfEditor.Core.Models;

namespace PdfEditor.Core.Hosting;

public interface ISettingsStore
{
    ApplicationSettings Load();
    void Save(ApplicationSettings settings);
}
