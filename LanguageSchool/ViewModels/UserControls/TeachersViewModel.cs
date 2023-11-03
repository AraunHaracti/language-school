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

public class TeachersViewModel : ViewModelBase, IDisposable
{
    private readonly Window _parentWindow;
    
    private List<Teacher> _itemsFromDatabase;

    private List<Teacher> _itemsFilter;
    
    public int CurrentPage { get; set; } = 1;

    public int TotalPages
    {
        get
        {
            int page = (int)Math.Ceiling(_itemsFilter.Count / (double)10);
            return page == 0 ? 1 : page;
        }
    }

    public Teacher CurrentItem { get; set; }
    
    private string _sql = $"select * from teacher";
    
    private ObservableCollection<Teacher> _itemsOnDataGrid;
    
    public ObservableCollection<Teacher> ItemsOnDataGrid
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
    
    public TeachersViewModel()
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
            PropertyInfo[] propertyInfos = typeof(Teacher).GetProperties();
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
    
    public void AddTeacherButton()
    {
        var view = new TeacherInfoCard(InfoCardEnum.Add);
        var vm = new TeacherInfoCardViewModel(UpdateItems);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditTeacherButton()
    {
        if (CurrentItem == null)
            return;
        var view = new TeacherInfoCard(InfoCardEnum.Edit);
        var vm = new TeacherInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void OpenCardTeacherButton()
    {
        if (CurrentItem == null)
            return;
        var view = new TeacherInfoCard(InfoCardEnum.Info);
        var vm = new TeacherInfoCardViewModel(UpdateItems, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<Teacher>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Teacher()
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name"),
                    Surname = reader.GetString("Surname"),
                    Birthday = reader.GetDateTime("Birthday"),
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

        ItemsOnDataGrid = new ObservableCollection<Teacher>(_itemsFilter.Skip((CurrentPage - 1) * 10).Take(10));
    }
    
    public void Dispose()
    {
    }
}