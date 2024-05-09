﻿using ReManage.Core;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Newtonsoft.Json;

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

        // Загружаем расположение при инициализации
        LoadLayout();
    }

    private void AddTable()
    {
        Tables.Add(new TableModel { X = 50, Y = 50 });
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
            var json = JsonConvert.SerializeObject(Tables, Formatting.Indented);
            File.WriteAllText(SaveFilePath, json);
        }
        catch (IOException ex)
        {
            // Обработка ошибок ввода/вывода
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
                        Tables.Add(table);
                    }
                }
            }
        }
        catch (IOException ex)
        {
            // Обработка ошибок ввода/вывода
            System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке макета: {ex.Message}");
        }
        catch (JsonException ex)
        {
            // Обработка ошибок парсинга JSON
            System.Diagnostics.Debug.WriteLine($"Ошибка при разборе JSON: {ex.Message}");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}