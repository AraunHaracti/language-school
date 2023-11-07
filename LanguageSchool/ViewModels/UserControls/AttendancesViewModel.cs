using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.Dialogs;
using LanguageSchool.Views.Dialogs;
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.UserControls;

public class AttendancesViewModel : ViewModelBase, IDisposable
{
    private readonly Window? _parentWindow;
    
    private List<AttendanceLog> _itemsFromDatabase;

    private List<AttendanceLog> _itemsFilter;

    private readonly string _sql = "select " +
                                   "attendance_log.id as id, " +
                                   "`value`.id as value_id, " +
                                   "`schedule`.id as schedule_id, " +
                                   "client_in_group.id as client_in_group_id, " +
                                   "`group`.name as group_name, " +
                                   "concat(client.name, ' ', client.surname) as client_name, " +
                                   "schedule.datetime as schedule_datetime, " +
                                   "`value`.name as value " +
                                   "from attendance_log " +
                                   "join value " +
                                   "on attendance_log.value_id = `value`.id " +
                                   "join schedule " +
                                   "on attendance_log.schedule_id = schedule.id " +
                                   "join client_in_group " +
                                   "on client_in_group.id = attendance_log.client_in_group_id " +
                                   "join `group` " +
                                   "on schedule.group_id = `group`.id and client_in_group.group_id = `group`.id " +
                                   "join client " +
                                   "on client_in_group.client_id = client.id";
    
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

    public AttendanceLog CurrentItem { get; set; }

    private ObservableCollection<AttendanceLog> _itemsOnDataGrid;
    
    public ObservableCollection<AttendanceLog> ItemsOnDataGrid
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

    public AttendancesViewModel()
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
        var view = new AttendanceInfoCard();
        var vm = new AttendanceInfoCardViewModel(GetAndUpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new AttendanceInfoCard();
        var vm = new AttendanceInfoCardViewModel(GetAndUpdateItems, CurrentItem);
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
            it.ScheduleDatetime.ToString().Contains(SearchQuery) ||
            it.Value!.Contains(SearchQuery)));
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<AttendanceLog>();

        using Database db = new Database();
        
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new AttendanceLog()
            {
                Id = reader.GetInt32("id"),
                ValueId = reader.GetInt32("value_id"),
                ScheduleId = reader.GetInt32("schedule_id"),
                ClientInGroupId = reader.GetInt32("client_in_group_id"),
                GroupName = reader.GetString("group_name"),
                ClientName = reader.GetString("client_name"),
                ScheduleDatetime = reader.GetDateTime("schedule_datetime"),
                Value = reader.GetString("value"),
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

        ItemsOnDataGrid = new ObservableCollection<AttendanceLog>(_itemsFilter.Skip((CurrentPage - 1) * _countItems).Take(_countItems));
    }
    
    public void Dispose() { }
}