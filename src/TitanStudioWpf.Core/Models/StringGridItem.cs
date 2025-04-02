using System.ComponentModel;

namespace TitanStudioWpf.Core.Models;

public class StringGridItem : INotifyPropertyChanged
{
    private int _index;
    private string _id;
    private string _offset;
    private string _string = string.Empty;

    public int Index
    {
        get => _index;
        set
        {
            if (_index != value)
            {
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }
    }

    public string ID
    {
        get => _id;
        set
        {
            if (_id != value)
            {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
    }

    public string Offset
    {
        get => _offset;
        set
        {
            if (_offset != value)
            {
                _offset = value;
                OnPropertyChanged(nameof(Offset));
            }
        }
    }

    public string String
    {
        get => _string;
        set
        {
            if (_string != value)
            {
                _string = value;
                OnPropertyChanged(nameof(String));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
