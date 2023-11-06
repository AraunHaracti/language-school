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
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.UserControls;

public class CoursesViewModel : ViewModelBase, IDisposable
{
 private readonly Window? _parentWindow;
    
    private List<Course> _itemsFromDatabase;

    private List<Course> _itemsFilter;
    
    private readonly string _sql = "select " + 
                                   "`course`.id as `id`, " +
                                   "`course`.name as `name`, " +
                                   "`course`.info as `info`, " +
                                   "`course`.price as `price`, " +
                                   "`course`.language_level_id as `language_level_id`, " +
                                   "`language_level`.name as `language_level_name`, " +
                                   "`language_level`.language_id as `language_id`, " +
                                   "`language`.name as `language_name` " +
                                   "from `course` " +
                                   "join `language_level` " +
                                   "on `course`.language_level_id = `language_level`.id " +
                                   "join `language` " +
                                   "on `language_level`.language_id = `language`.id";
    
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

    public Course CurrentItem { get; set; }

    private ObservableCollection<Course> _itemsOnDataGrid;
    
    public ObservableCollection<Course> ItemsOnDataGrid
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

    public CoursesViewModel()
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
        var view = new CourseInfoCard();
        var vm = new CourseInfoCardViewModel(GetAndUpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new CourseInfoCard();
        var vm = new CourseInfoCardViewModel(GetAndUpdateItems, CurrentItem);
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
            it.LanguageName.Contains(SearchQuery) ||
            it.LanguageLevelName.Contains(SearchQuery) ||
            it.Info!.Contains(SearchQuery) ||
            it.Price.ToString()!.Contains(SearchQuery)));
    }

    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Course>();

        using Database db = new Database();
        
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new Course()
            {
                Id = reader.GetInt32("id"),
                Name = reader.GetString("name"),
                Info = reader.GetString("info"),
                Price = reader.GetDouble("price"),
                LanguageLevelId = reader.GetInt32("language_level_id"),
                LanguageLevelName = reader.GetString("language_level_name"),
                LanguageId = reader.GetInt32("language_id"),
                LanguageName = reader.GetString("language_name")
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

        ItemsOnDataGrid = new ObservableCollection<Course>(_itemsFilter.Skip((CurrentPage - 1) * _countItems).Take(_countItems));
    }
    
    public void Dispose() { }
}