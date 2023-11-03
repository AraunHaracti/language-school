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
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class GroupInfoCardViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private List<Client> _itemsFromDatabase = new();
    
    private List<Client> _itemsFilter = new();
    
    public int CurrentPage { get; set; } = 1;

    public int TotalPages
    {
        get
        {
            int page = (int)Math.Ceiling(_itemsFilter.Count / (double)10);
            return page == 0 ? 1 : page;
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
    
    private Group _item;
    
    private ObservableCollection<Client> _groupClients = new();

    private List<Teacher> _teachers = new ();
    private List<Course> _courses = new ();
    
    private List<string> _teachersName = new ();
    private List<string> _coursesName = new ();
    
    public List<string> TeachersName => _teachersName;
    public List<string> CoursesName => _coursesName;
    
    private int _teacherNameIndex = 0;
    
    public int TeacherNameIndex
    {
        get => _teacherNameIndex;
        set
        {
            _teacherNameIndex = value;
            this.RaisePropertyChanged("TeacherNameIndex");
        }
    }

    private int _courseNameIndex = 0;
    
    public int CourseNameIndex
    {
        get => _courseNameIndex;
        set
        {
            _courseNameIndex = value;
            this.RaisePropertyChanged("CourseNameIndex");
        } 
    }
    
    public Client CurrentItem { get; set; }
    
    public Group Item => _item;
    
    public ObservableCollection<Client> GroupClients
    {
        get => _groupClients;
        set
        {
            _groupClients = value;
            this.RaisePropertyChanged("GroupClients");
        }
    }
    
    public GroupInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from teacher");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Teacher();

                PropertyInfo[] propertyInfos = typeof(Teacher).GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    propertyInfos[i].SetValue(currentItem, reader.GetValue(i));
                }

                _teachers.Add(currentItem);
            }
        }
        
        foreach (var item in _teachers)
        {
            TeachersName.Add(item.Name);
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from curse");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Course()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name")
                };

                _courses.Add(currentItem);
            }
        }
        
        foreach (var item in _courses)
        {
            CoursesName.Add(item.Name);
        }
        
        UpdateItems();
        
        PropertyChanged += OnSearchQueryChanged;
    }
    
    public GroupInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new Group();

        _isEdit = false;
    }

    public GroupInfoCardViewModel(Action action, Group group) : this()
    {
        _action = action;
        _item = group;

        _isEdit = true;
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
            PropertyInfo[] propertyInfos = typeof(Client).GetProperties();
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
    
    private void GetDataFromDatabase()
    {
        if (Item?.Id == null)
            return;

        string sql = "select " +
                     "client.* " +
                     "from client " +
                     "join client_in_group " +
                     "on client.id = client_in_group.client_id " +
                     $"where group_id = {Item.Id}";
        
        _itemsFromDatabase = new List<Client>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Client()
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
    
    public bool ActionGroup()
    {
        if (Item.Name == null || Item.Name == "")
            return false;
        
        if (_isEdit)
        {
            EditGroup();
        }
        else
        {
            AddGroup();
        }
        
        _action.Invoke();

        return true;
    }
    
    private void AddGroup()
    {
        string sql = $"insert into `group` (name, teacher, curse_id) values (" +
                     $"'{Item.Name}', " +
                     $"{_teachers.Where(it => it.Name == TeachersName[TeacherNameIndex]).ToList()[0].Id}, " +
                     $"{_courses.Where(it => it.Name == CoursesName[CourseNameIndex]).ToList()[0].Id})";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }

    public void EditGroup()
    {
        string sql = $"update group set " +
                     $"name = '{Item.Name}', " +
                     $"teacher = {Item.TeacherId}, " +
                     $"curse_id = {Item.CourseId} " +
                     $"where id = {Item.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void AddClientButton()
    {
        
    }

    public void DeleteClientButton()
    {
        
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

        GroupClients = new ObservableCollection<Client>(_itemsFilter.Skip((CurrentPage - 1) * 10).Take(10));
    }
}