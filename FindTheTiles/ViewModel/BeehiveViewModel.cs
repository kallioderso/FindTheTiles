using System.ComponentModel;
using System.Runtime.CompilerServices;
using FindTheTiles.Model;
using System.Windows.Input;

namespace FindTheTiles.ViewModel
{
    public class BeehiveViewModel : INotifyPropertyChanged
    {
        private ImageTileButton[,] _tiles = new ImageTileButton[7, 7];
        public ImageTileButton[,] _TileButtons
        {
            get => _tiles;
            set => _tiles = value;
        }

        public ICommand _tileClickCommand { get; }
        private Pattern _pattern;

        public BeehiveViewModel()
        {
            _tileClickCommand = new Command<ImageTileButton>(TileClicked);
        }

        public void GenerateTiles(Pattern pattern)
        {
            _pattern = pattern;
            for (int i = 0; i < 7; i++ )
                for (int j = 0; j < 7; j++)
                {
                    var Button = new ImageTileButton();
                    Button.Row = i;
                    Button.Column = j;
                    if (pattern.StartTiles.Contains((i, j)))
                        Button._state = 1;
                    else
                        Button._state = 0;
                    Button._bomb = false;
                    Button._neighbor = pattern.TilesNeighbors[i, j];
                    Button.Command = _tileClickCommand;
                    Button.CommandParameter = Button;
                    _tiles[i, j] = Button;
                }
        }

        private void TileClicked(ImageTileButton button)
        {
            if (_pattern.IsPattern[button.Row, button.Column])
                button._state = 2;
            else
                button._state = 3;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}