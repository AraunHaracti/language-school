using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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

public class CoursesViewModel : ViewModelBase, IDisposable
{
 private readonly Window _parentWindow;
    
    private List<Course> _itemsFromDatabase;

    private List<Course> _itemsFilter;
    
    public int CurrentPage { get; set; } = 1;

    public int TotalPages
    {
        get
        {
            int page = (int)Math.Ceiling(_itemsFilter.Count / (double)10);
            return page == 0 ? 1 : page;
        }
    }

    public Course CurrentItem { get; set; }
    
    private string _sql = "select " +
                          "curse.id as id, " +
                          "curse.name as name, " +
                          "curse.info as info, " +
                          "curse.price as price, " +
                          "curse.proficiency_level_id as proficiency_level_id, " +
                          "proficiency_level.name as proficiency_level_name, " +
                          "proficiency_level.language_id as language_id, " +
                          "language.name as language_name " +
                          "from curse " +
                          "join proficiency_level " +
                          "on curse.proficiency_level_id = proficiency_level.id " +
                          "join language " +
                          "on proficiency_level.language_id = language.id";
    
    private ObservableCollection<Course> _itemsOnDataGrid;
    
    public ObservableCollection<Course> ItemsOnDataGrid
    {
        get => _itemsOnDataGrid;
        set
        {
            _itemsOnDataGrid = value;
            this.RaisePropertyChanged("ItemsOnDataGrid");
        }
    }

    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
            this.RaisePropertyChanged("SearchQuery");
        }
    }

    public CoursesViewModel()
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
        TakeItems(TakeItemsEnum.FirstItems);
    }

    private void Search()
    {
        if (SearchQuery == "")
        {
            _itemsFilter = new(_itemsFromDatabase);
            return;
        }

        _itemsFilter = new(_itemsFromDatabase.Where(it =>
        {
            PropertyInfo[] propertyInfos = typeof(Course).GetProperties();
            foreach (PropertyInfo f in propertyInfos)
            {
                if (f.GetValue(it) == null)
                    continue;
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
    
    public void AddCourseButton()
    {
        var view = new CourseInfoCard();
        var vm = new CourseInfoCardViewModel(UpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditCourseButton()
    {
        if (CurrentItem == null)
            return;
        var view = new CourseInfoCard();
        var vm = new CourseInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Course>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Course()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Info = reader.GetString("info"),
                    Price = reader.GetDouble("price"),
                    ProficiencyLevelId = reader.GetInt32("proficiency_level_id"),
                    ProficiencyLevelName = reader.GetString("proficiency_level_name"),
                    LanguageId = reader.GetInt32("language_id"),
                    LanguageName = reader.GetString("language_name")
                };

                _itemsFromDatabase.Add(currentItem);
            }
        }
    }
    
    public void TakeItems(TakeItemsEnum takeItems)
    {
        switch (takeItems)
        {
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
        
        this.RaisePropertyChanged("CurrentPage");
        this.RaisePropertyChanged("TotalPages");

        ItemsOnDataGrid = new ObservableCollection<Course>(_itemsFilter.Skip((CurrentPage - 1) * 10).Take(10));
    }
    
    public void Dispose()
    {
    }
}