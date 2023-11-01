using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.Dialogs;
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

using Avalonia.Controls.ApplicationLifetimes;

namespace LanguageSchool.ViewModels.UserControls;

public class ClientsViewModel : ViewModelBase, IDisposable
{
    private readonly Window _parentWindow;
    
    private List<Client> _itemsFromDatabase;

    private List<Client> _itemsFilter;
    
    public Client CurrentItem { get; set; }
    
    private string _sql = $"select * from client";
    
    private ObservableCollection<Client> _itemsOnDataGrid;
    
    public ObservableCollection<Client> ItemsOnDataGrid
    {
        get => _itemsOnDataGrid;
        set
        {
            _itemsOnDataGrid = value;
            this.RaisePropertyChanged();
        }
    }

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
        }
    }


    public ClientsViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
        
        
        UpdateItems();
        
        PropertyChanged += OnSearchQueryChanged;
    }
    
    private void OnSearchQueryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SearchQuery)) return;
        Search();
    }

    private void Search()
    {
        if (SearchQuery == "")
        {
            _itemsFilter = new(_itemsFromDatabase);
        }

        _itemsFilter = new(_itemsFromDatabase.Where(it =>
        {
            PropertyInfo[] propertyInfos = typeof(Client).GetProperties();
            foreach (PropertyInfo f in propertyInfos)
            {
                if (f.GetValue(it).ToString().ToLower().Contains(SearchQuery.ToLower()))
                    return true;
            }

            return false;
        }));
    }
    
    public void UpdateItems()
    {
        GetDataFromDatabase();
        Search();
        TakeItems(TakeItemsEnum.FirstItems);
    }
    
    public void AddClientButton()
    {
        var view = new ClientInfoCard(InfoCardEnum.Add);
        var vm = new ClientInfoCardViewModel(UpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditClientButton()
    {
        var view = new ClientInfoCard(InfoCardEnum.Edit);
        var vm = new ClientInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void OpenCardClientButton()
    {
        var view = new ClientInfoCard(InfoCardEnum.Info);
        var vm = new ClientInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Client>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Client()
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Surname = reader.GetString("Surname"),
                    Birthday = reader.GetDateTime("Birthday"),
                };
                currentItem.Phone = (reader.IsDBNull("Phone") ? null : reader.GetString("Phone"));
                currentItem.Email = (reader.IsDBNull("Email") ? null : reader.GetString("Email"));

                _itemsFromDatabase.Add(currentItem);
            }
        }
    }

    private int _indexTake = 0;
    
    private int IndexTake
    {
        get => _indexTake;
        set
        {
            _indexTake = value;
            
            if (_indexTake > _itemsFilter.Count - 10)
            {
                _indexTake = _itemsFilter.Count - 10;
            }
            
            if (_indexTake < 0)
            {
                _indexTake = 0;
            } 
        }
    }
    
    public void TakeItems(TakeItemsEnum takeItems)
    {
        switch (takeItems)
        {
            case TakeItemsEnum.FirstItems:
                IndexTake = 0;
                break;
            case TakeItemsEnum.LastItems:
                IndexTake = _itemsFilter.Count - 10;
                break;
            case TakeItemsEnum.NextItems:
                IndexTake += 10;
                break;
            case TakeItemsEnum.PreviousItems:
                IndexTake -= 10;
                break;
        }

        ItemsOnDataGrid = new ObservableCollection<Client>(_itemsFilter.GetRange(IndexTake, _itemsFilter.Count > 10 ? 10 : _itemsFilter.Count));
    }
    
    public void Dispose()
    {
    }
}