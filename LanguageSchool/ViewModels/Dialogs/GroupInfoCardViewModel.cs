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
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class GroupInfoCardViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private List<ClientInGroup> _itemsFromDatabase = new();
    
    private List<ClientInGroup> _itemsFilter = new();

    private readonly string _sql = "select " +
                                   "client_in_group.id as id, " +
                                   "client_in_group.group_id as group_id, " +
                                   "client_in_group.client_id as client_id, " +
                                   "client.name as client_name, " +
                                   "client.surname as client_surname, " +
                                   "client.birthday as client_birthday " +
                                   "from client_in_group " +
                                   "join client " +
                                   "on client_in_group.client_id = client.id";
    
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
    
    private ObservableCollection<ClientInGroup> _groupClients = new();

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
    
    public ClientInGroup CurrentItem { get; set; }
    
    public Group Item => _item;
    
    public ObservableCollection<ClientInGroup> GroupClients
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
                var currentItem = new Teacher
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name"),
                    Surname = reader.GetString("surname"),
                    Birthday = reader.GetDateTime("birthday"),
                    Phone = reader.GetString("phone"),
                    Email = reader.GetString("email"),
                };

                _teachers.Add(currentItem);
            }
        }
        
        foreach (var item in _teachers)
        {
            TeachersName.Add(item.Name);
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from course");
            
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
    
    public GroupInfoCardViewModel(Group group) : this()
    {
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
        _itemsFromDatabase = new List<ClientInGroup>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new ClientInGroup()
                {
                    Id = reader.GetInt32("id"),
                    GroupId = reader.GetInt32("group_id"),
                    ClientId = reader.GetInt32("client_id"),
                    ClientName = reader.GetString("client_name"),
                    ClientSurname = reader.GetString("client_surname"),
                    ClientBirthday = reader.GetDateTime("client_birthday"),
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
        string sql = $"insert into `group` (name, teacher_id, course_id) values (" +
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
                     $"teacher_id = {Item.TeacherId}, " +
                     $"curse_id = {Item.CourseId} " +
                     $"where id = {Item.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void AddClientButton()
    {
        var view = new GroupClientInfoCard();
        var vm = new GroupClientInfoCardViewModel(UpdateItems, Item);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void DeleteClientButton()
    {
        if (CurrentItem == null)
            return;
        
        string sql = $"delete from client_in_group where client_in_group.id = {CurrentItem.Id}";

        using Database db = new Database();
        
        db.SetData(sql);

        UpdateItems();
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

        GroupClients = new ObservableCollection<ClientInGroup>(_itemsFilter.Skip((CurrentPage - 1) * 10).Take(10));
    }
}