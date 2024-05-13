using System;
using System.ComponentModel;
using System.Windows.Input;

public class TableModel : INotifyPropertyChanged
{
    private double _x;
    private double _y;
    private int _width = 150;
    private int _height = 150;
    public const int GridSize = 75;

    private int _number;
    public int Number
    {
        get => _number;
        set
        {
            _number = value;
            OnPropertyChanged(nameof(Number));
        }
    }

    public double X
    {
        get { return _x; }
        set
        {
            _x = SnapToGrid(value);
            OnPropertyChanged(nameof(X));
        }
    }

    public double Y
    {
        get { return _y; }
        set
        {
            _y = SnapToGrid(value);
            OnPropertyChanged(nameof(Y));
        }
    }

    public double SnapToGrid(double coordinate)
    {
        return Math.Round(coordinate / GridSize) * GridSize;
    }

    public int Width
    {
        get { return _width; }
        set
        {
            _width = value;
            OnPropertyChanged(nameof(Width));
        }
    }

    public int Height
    {
        get { return _height; }
        set
        {
            _height = value;
            OnPropertyChanged(nameof(Height));
        }
    }

    private ICommand _removeCommand;
    public ICommand RemoveCommand
    {
        get => _removeCommand;
        set
        {
            _removeCommand = value;
            OnPropertyChanged(nameof(RemoveCommand));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public TableModel Clone()
    {
        return new TableModel
        {
            X = this.X,
            Y = this.Y,
            Width = this.Width,
            Height = this.Height,
            Number = this.Number,
            RemoveCommand = this.RemoveCommand
        };
    }
}