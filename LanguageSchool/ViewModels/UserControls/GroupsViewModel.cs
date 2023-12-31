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
using LanguageSchool.Views.Dialogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.UserControls;

public class GroupsViewModel : ViewModelBase, IDisposable
{
    private readonly Window? _parentWindow;
    
    private List<Group> _itemsFromDatabase;

    private List<Group> _itemsFilter;

    private readonly string _sql = "select " + 
                                   "`group`.id as `id`, " + 
                                   "`group`.teacher_id as `teacher_id`, " + 
                                   "concat(teacher.name, ' ', teacher.surname) as `teacher_name`, " +
                                   "`group`.course_id as `course_id`, " +
                                   "`course`.name as `course_name`, " +
                                   "`group`.name as `name` " +
                                   "from `group` " +
                                   "join `teacher` " +
                                   "on `group`.teacher_id = `teacher`.id " +
                                   "join `course` " +
                                   "on `group`.course_id = `course`.id";
    
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

    public Group CurrentItem { get; set; }

    private ObservableCollection<Group> _itemsOnDataGrid;
    
    public ObservableCollection<Group> ItemsOnDataGrid
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
    
    public GroupsViewModel()
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
        var view = new GroupInfoCard(InfoCardEnum.Add);
        var vm = new GroupInfoCardViewModel(GetAndUpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new GroupInfoCard(InfoCardEnum.Edit);
        var vm = new GroupInfoCardViewModel(GetAndUpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    public void OpenCardItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new GroupInfoCard(InfoCardEnum.Info);
        var vm = new GroupInfoCardViewModel(CurrentItem);
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
            it.TeacherName.Contains(SearchQuery) ||
            it.CourseName.Contains(SearchQuery) ||
            it.Name.Contains(SearchQuery)));
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Group>();

        using Database db = new Database();
        
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new Group()
            {
                Id = reader.GetInt32("id"),
                TeacherId = reader.GetInt32("teacher_id"),
                CourseId = reader.GetInt32("course_id"),
                TeacherName = reader.GetString("teacher_name"),
                CourseName = reader.GetString("course_name"),
                Name = reader.GetString("name"),
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

        ItemsOnDataGrid = new ObservableCollection<Group>(_itemsFilter.Skip((CurrentPage - 1) * _countItems).Take(_countItems));
    }
    
    public void Dispose() { }
}