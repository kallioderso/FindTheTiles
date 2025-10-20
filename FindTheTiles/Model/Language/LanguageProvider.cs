using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FindTheTiles.Model;

public class LanguageProvider : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string this[string key] => LanguageManager.GetText(key);

    public void Update()
    {
        LanguageManager.Update();
        OnPropertyChanged("Item[]");
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}