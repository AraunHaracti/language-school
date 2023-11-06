using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.Views.Dialogs;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class TeacherInfoCardViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private List<TeacherLanguage> _itemsFromDatabase;
    
    private string _sql = $"select " +
                          $"`teacher_language`.id as `id`, " +
                          $"`teacher_language`.teacher_id as `teacher_id`, " +
                          $"`teacher_language`.language_level_id as `language_level_id`, " +
                          $"`language_level`.name as `language_level_name`, " +
                          $"`language_level`.language_id as `language_id`, " +
                          $"`language`.name as `language_name` " +
                          $"from `teacher_language` " +
                          $"join `language_level` " +
                          $"on `teacher_language`.language_level_id = `language_level`.id " +
                          $"join `language` " +
                          $"on `language_level`.language_id = `language`.id";
    
    private Teacher _person;

    private ObservableCollection<TeacherLanguage> _personLanguage;
    
    public TeacherLanguage CurrentItem { get; set; }
    
    public Teacher Person => _person;
    
    public ObservableCollection<TeacherLanguage> PersonLanguage
    {
        get => _personLanguage;
        set
        {
            _personLanguage = value;
            this.RaisePropertyChanged("PersonLanguage");
        }
    }

    public TeacherInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
    }
    
    public void UpdateItems()
    {
        GetDataFromDatabase();
        PersonLanguage = new ObservableCollection<TeacherLanguage>(_itemsFromDatabase.Where(it => it.TeacherId == _person.Id));
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<TeacherLanguage>();

        using Database db = new Database();
        MySqlDataReader reader = db.GetData(_sql);
            
        while (reader.Read() && reader.HasRows)
        {
            var currentItem = new TeacherLanguage()
            {
                Id = reader.GetInt32("id"),
                TeacherId = reader.GetInt32("teacher_id"),
                LanguageLevelId = reader.GetInt32("language_level_id"),
                LanguageId = reader.GetInt32("language_id"),
                LanguageLevelName = reader.GetString("language_level_name"),
                LanguageName = reader.GetString("language_name"),
            };

            _itemsFromDatabase.Add(currentItem);
        }
    }
    
    public TeacherInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _person = new Teacher();

        _isEdit = false;
    }
    
    public TeacherInfoCardViewModel(Action action, Teacher teacher) : this()
    {
        _action = action;
        _person = teacher;

        _isEdit = true;
        
        UpdateItems();
    }
    
    public TeacherInfoCardViewModel(Teacher teacher) : this()
    {
        _person = teacher;

        _isEdit = true;
        
        UpdateItems();
    }
    
    public bool ActionTeacher()
    {
        if (Person.Name == null || Person.Name == "" || Person.Surname == null || Person.Surname == "")
            return false;
        
        if (_isEdit)
        {
            EditTeacher();
        }
        else
        {
            AddTeacher();
        }
        
        _action.Invoke();

        return true;
    }
    
    private void AddTeacher()
    {
        string sql = $"insert into teacher (name, surname, birthday, phone, email) values (" +
                     $"'{Person.Name}', " +
                     $"'{Person.Surname}', " +
                     $"'{Person.Birthday.ToString("yyyy-MM-dd")}', " +
                     $"'{Person.Phone}', " +
                     $"'{Person.Email}')";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void EditTeacher()
    {
        string sql = $"update teacher set " +
                     $"name = '{Person.Name}', " +
                     $"surname = '{Person.Surname}', " +
                     $"birthday = '{Person.Birthday.ToString("yyyy-MM-dd")}' " +
                     $"where id = {Person.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void AddLanguageButton()
    {
        var view = new TeacherLanguageInfoCard();
        var vm = new TeacherLanguageInfoCardViewModel(UpdateItems, Person);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    public void EditLanguageButton()
    {
        if (CurrentItem == null)
            return;
        var view = new TeacherLanguageInfoCard();
        var vm = new TeacherLanguageInfoCardViewModel(UpdateItems, Person, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }
    
    public void DeleteLanguageButton()
    {
        if (CurrentItem == null)
            return;
        string sql = $"delete from teacher_language where teacher_language.id = {CurrentItem.Id}";

        using Database db = new Database();
        
        db.SetData(sql);

        UpdateItems();
    }
}