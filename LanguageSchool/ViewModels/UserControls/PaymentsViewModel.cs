using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.Dialogs;
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.UserControls;

public class PaymentsViewModel : ViewModelBase, IDisposable
{
    private readonly Window? _parentWindow;
    
    private List<Payment> _itemsFromDatabase;

    private List<Payment> _itemsFilter;
    
    private readonly string _sql = "select " +
                                   "`payment`.id as `id`, " +
                                   "`payment`.client_in_group_id as `client_in_group_id`, " +
                                   "`payment`.date as `date`, " +
                                   "`payment`.count as `count`, " +
                                   "`client_in_group`.group_id as `group_id`, " +
                                   "`client_in_group`.client_id as `client_id`, " +
                                   "`group`.name as `group_name`, " +
                                   "concat(client.name, ' ', client.surname) as `client_name` " + 
                                   "from `payment` " + 
                                   "join `client_in_group` " + 
                                   "on `payment`.client_in_group_id = `client_in_group`.id " + 
                                   "join `group` " + 
                                   "on `group`.id = `client_in_group`.group_id " +
                                   "join `client` " + 
                                   "on `client`.id = `client_in_group`.client_id";
    
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

    public Payment CurrentItem { get; set; }

    private ObservableCollection<Payment> _itemsOnDataGrid;
    
    public ObservableCollection<Payment> ItemsOnDataGrid
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
    
    public PaymentsViewModel()
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
        var view = new PaymentInfoCard(InfoCardEnum.Add);
        var vm = new PaymentInfoCardViewModel(GetAndUpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void OpenCardItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new PaymentInfoCard(InfoCardEnum.Info);
        var vm = new PaymentInfoCardViewModel(CurrentItem);
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
            it.GroupName.Contains(SearchQuery) ||
            it.ClientName.Contains(SearchQuery) ||
            it.Date.ToString().Contains(SearchQuery) ||
            it.Count.ToString(CultureInfo.InvariantCulture).Contains(SearchQuery)));
    }

    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Payment>();

        using Database db = new Database();
        
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new Payment()
            {
                Id = reader.GetInt32("id"),
                ClientInGroupId = reader.GetInt32("client_in_group_id"),
                GroupId = reader.GetInt32("group_id"),
                GroupName = reader.GetString("group_name"),
                ClientId = reader.GetInt32("client_id"),
                ClientName = reader.GetString("client_name"),
                Date = reader.GetDateTime("date"),
                Count = reader.GetDouble("count"),
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

        ItemsOnDataGrid = new ObservableCollection<Payment>(_itemsFilter.Skip((CurrentPage - 1) * _countItems).Take(_countItems));
    }
    
    public void Dispose() { }
}