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

public class GroupsViewModel : ViewModelBase, IDisposable
{
    private readonly Window _parentWindow;
    
    private List<Group> _itemsFromDatabase;

    private List<Group> _itemsFilter;

    public int CurrentPage { get; set; } = 1;

    public int TotalPages
    {
        get
        {
            int page = (int)Math.Ceiling(_itemsFilter.Count / (double)10);
            return page == 0 ? 1 : page;
        }
    }

    public Group CurrentItem { get; set; }
    
    private string _sql = $"select " +
                          $"`group`.id as id, " +
                          $"`group`.teacher as teacher_id, " +
                          $"concat(teacher.name, ' ', teacher.surname) as teacher_name, " +
                          $"`group`.curse_id as curse_id, " +
                          $"curse.name as curse_name, " +
                          $"`group`.name as name " +
                          $"from `group` " +
                          $"join teacher " +
                          $"on `group`.teacher = teacher.id " +
                          $"join curse " +
                          $"on `group`.curse_id = curse.id";
    
    private ObservableCollection<Group> _itemsOnDataGrid;
    
    public ObservableCollection<Group> ItemsOnDataGrid
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
    
    public GroupsViewModel()
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
            PropertyInfo[] propertyInfos = typeof(Group).GetProperties();
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
    
    public void AddGroupButton()
    {
        var view = new GroupInfoCard(InfoCardEnum.Add);
        var vm = new GroupInfoCardViewModel(UpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditGroupButton()
    {
        if (CurrentItem == null)
            return;
        var view = new GroupInfoCard(InfoCardEnum.Edit);
        var vm = new GroupInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    public void OpenCardGroupButton()
    {
        /*
        if (CurrentItem == null)
            return;
        var view = new GroupInfoCard(InfoCardEnum.Info);
        var vm = new GroupInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
        */
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Group>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Group()
                {
                    Id = reader.GetInt32("id"),
                    TeacherId = reader.GetInt32("teacher_id"),
                    CourseId = reader.GetInt32("curse_id"),
                    TeacherName = reader.GetString("teacher_name"),
                    CourseName = reader.GetString("curse_name"),
                    Name = reader.GetString("name"),
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

        ItemsOnDataGrid = new ObservableCollection<Group>(_itemsFilter.Skip((CurrentPage - 1) * 10).Take(10));
    }
    
    public void Dispose()
    {
    }
}