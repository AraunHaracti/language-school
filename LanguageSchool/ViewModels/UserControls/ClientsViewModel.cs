using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
    private readonly Window? _parentWindow;
    
    private List<Client> _itemsFromDatabase;

    private List<Client> _itemsFilter;
    
    private readonly string _sql = "select * " +
                                   "from client";
    
    private readonly int _countItems = 15;
    
    public int CurrentPage { get; set; } = 1;

    public int TotalPages
    {
        get
        {
            int page = (int)Math.Ceiling(_itemsFilter.Count / (double) _countItems);
            return page == 0 ? 1 : page;
        }
    }

    public Client CurrentItem { get; set; }

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
            this.RaisePropertyChanged();
        }
    }

    public ClientsViewModel()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
        
        GetAndUpdateItems();
        
        PropertyChanged += OnSearchQueryChanged;
    }
    
    
    public void AddItemButton()
    {
        var view = new ClientInfoCard(InfoCardEnum.Add);
        var vm = new ClientInfoCardViewModel(GetAndUpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new ClientInfoCard(InfoCardEnum.Edit);
        var vm = new ClientInfoCardViewModel(GetAndUpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void OpenCardItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new ClientInfoCard(InfoCardEnum.Info);
        var vm = new ClientInfoCardViewModel(CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    private void OnSearchQueryChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SearchQuery)) 
            return;

        UpdateItems();
    }

    private void GetAndUpdateItems()
    {
        GetDataFromDatabase();
        UpdateItems();
    }

    private void UpdateItems()
    {
        Search();
        this.RaisePropertyChanged(nameof(TotalPages));
        TakeItems(TakeItemsEnum.FirstItems);
        this.RaisePropertyChanged(nameof(CurrentPage));
    }

    private void Search()
    {
        if (SearchQuery == "")
        {
            _itemsFilter = new(_itemsFromDatabase);
            return;
        }

        _itemsFilter = new(_itemsFromDatabase.Where(it =>
            it.Name.Contains(SearchQuery) || 
            it.Surname.Contains(SearchQuery) ||
            it.Birthday.ToString().Contains(SearchQuery) ||
            it.Phone!.Contains(SearchQuery) ||
            it.Email!.Contains(SearchQuery)));
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Client>();

        using Database db = new Database();
        
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new Client()
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Surname = reader.GetString("surname"),
                Birthday = reader.GetDateTime("birthday"),
                Phone = reader.GetString("phone"),
                Email = reader.GetString("email"),
            };
            
            _itemsFromDatabase.Add(currentItem);
        }
    }
    

    public void TakeItems(TakeItemsEnum takeItems)
    {
        switch (takeItems)
        {
            default:
            case TakeItemsEnum.FirstItems:
                CurrentPage = 1;
                break;
            case TakeItemsEnum.LastItems:
                CurrentPage = TotalPages;
                break;
            case TakeItemsEnum.NextItems:
                if (CurrentPage < TotalPages)
                    CurrentPage += 1;
                break;
            case TakeItemsEnum.PreviousItems:
                if (CurrentPage > 1)
                    CurrentPage -= 1;
                break;
        }

        ItemsOnDataGrid = new ObservableCollection<Client>(_itemsFilter.Skip((CurrentPage - 1) * _countItems).Take(_countItems));
    }
    
    public void Dispose() { }
}