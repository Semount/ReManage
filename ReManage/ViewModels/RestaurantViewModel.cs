using Newtonsoft.Json;
using ReManage.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

public class RestaurantViewModel : INotifyPropertyChanged
{
    private const string SaveFilePath = "tables_layout.json";

    public ObservableCollection<TableModel> Tables { get; set; }

    public ICommand AddTableCommand { get; private set; }
    public ICommand RemoveTableCommand { get; private set; }
    public ICommand SaveLayoutCommand { get; private set; }
    public ICommand LoadLayoutCommand { get; private set; }

    public RestaurantViewModel()
    {
        Tables = new ObservableCollection<TableModel>();
        AddTableCommand = new RelayCommand(_ => AddTable());
        RemoveTableCommand = new RelayCommand<TableModel>(RemoveTable);
        SaveLayoutCommand = new RelayCommand(_ => SaveLayout());
        LoadLayoutCommand = new RelayCommand(_ => LoadLayout());

        LoadLayout();
    }

    private void AddTable()
    {
        var newTableNumber = GetNextTableNumber();
        var newTable = new TableModel { X = 50, Y = 50, Number = newTableNumber };
        newTable.RemoveCommand = RemoveTableCommand;
        Tables.Add(newTable);
    }

    private void RemoveTable(TableModel table)
    {
        if (Tables.Contains(table))
        {
            Tables.Remove(table);
        }
    }

    private void SaveLayout()
    {
        try
        {
            var settings = new JsonSerializerSettings { ContractResolver = new ShouldSerializeContractResolver() };
            var json = JsonConvert.SerializeObject(Tables, Formatting.Indented, settings);
            File.WriteAllText(SaveFilePath, json);
        }
        catch (IOException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при сохранении макета: {ex.Message}");
        }
    }

    private void LoadLayout()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                var json = File.ReadAllText(SaveFilePath);
                var tables = JsonConvert.DeserializeObject<ObservableCollection<TableModel>>(json);
                if (tables != null)
                {
                    Tables.Clear();
                    foreach (var table in tables)
                    {
                        table.RemoveCommand = RemoveTableCommand;
                        Tables.Add(table);
                    }
                }
            }
        }
        catch (IOException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке макета: {ex.Message}");
        }
        catch (JsonException ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при разборе JSON: {ex.Message}");
        }
    }

    private int GetNextTableNumber()
    {
        if (!Tables.Any())
            return 1;

        var existingNumbers = Tables.Select(t => t.Number).OrderBy(x => x).ToList();

        int expectedNumber = 1;
        foreach (var num in existingNumbers)
        {
            if (num != expectedNumber)
                break;
            expectedNumber++;
        }

        return expectedNumber;
    }

    public bool CheckForCollision(TableModel table, double newX, double newY)
    {
        var tableCircle = new System.Windows.Rect(newX, newY, table.Diameter, table.Diameter);
        foreach (var otherTable in Tables)
        {
            if (otherTable == table) continue;
            var otherCircle = new System.Windows.Rect(otherTable.X, otherTable.Y, otherTable.Diameter, otherTable.Diameter);
            if (tableCircle.IntersectsWith(otherCircle))
            {
                return true;
            }
        }
        return false;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}