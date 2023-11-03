using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class CourseInfoCardViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private int _languagesNameIndex = 0;
    
    public int LanguagesNameIndex
    {
        get => _languagesNameIndex;
        set
        {
            _languagesNameIndex = value;
            this.RaisePropertyChanged("LanguagesNameIndex");
        }
    }

    private int _proficiencyLevelsNameIndex = 0;
    
    public int ProficiencyLevelsNameIndex
    {
        get => _proficiencyLevelsNameIndex;
        set
        {
            _proficiencyLevelsNameIndex = value;
            this.RaisePropertyChanged("ProficiencyLevelsNameIndex");
        } 
    }

    private List<string> _languagesName = new();
    
    public List<string> LanguagesName => _languagesName;

    private List<string> _proficiencyLevelsName = new();

    public List<string> ProficiencyLevelsName => _proficiencyLevelsName;
    
    private List<Language> _languages = new();

    private List<ProficiencyLevel> _proficiencyLevels = new();
    
    private Course _item;
    
    public Course Item => _item;
    
    public CourseInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from language");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Language();

                PropertyInfo[] propertyInfos = typeof(Language).GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    propertyInfos[i].SetValue(currentItem, reader.GetValue(i));
                }

                _languages.Add(currentItem);
            }
        }
        
        foreach (var item in _languages)
        {
            LanguagesName.Add(item.Name);
        }
        
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from proficiency_level");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new ProficiencyLevel();

                PropertyInfo[] propertyInfos = typeof(ProficiencyLevel).GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    propertyInfos[i].SetValue(currentItem, reader.GetValue(i));
                }

                _proficiencyLevels.Add(currentItem);
            }
        }
    }
    
    public CourseInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new Course();

        _isEdit = false;
    }
    
    public CourseInfoCardViewModel(Action action, Course course) : this()
    {
        _action = action;
        _item = course;

        _isEdit = true;
    }
    
    public bool ActionCourseLanguage()
    {
        if (Item.Name == null || Item.Name == "")
            return false;
        
        if (_isEdit)
        {
            EditClient();
        }
        else
        {
            AddClient();
        }
        
        _action.Invoke();

        return true;
    }

    public void LanguagesComboBoxChanged()
    {
        _proficiencyLevelsName = new List<string>();
        var language = _languages.Where(it => it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0];
        
        foreach (var item in _proficiencyLevels.Where(it => it.LanguageId == language.Id))
        {
            ProficiencyLevelsName.Add(item.Name);
        }
        
        this.RaisePropertyChanged("LanguagesName");
        this.RaisePropertyChanged("ProficiencyLevelsName");
    }
    
    private void AddClient()
    {
        string sql = $"insert into curse (name, info, price, proficiency_level_id) values (" +
                     $"'{Item.Name}', " +
                     $"'{Item.Info}', " +
                     $"'{Item.Price.ToString().Replace(",", ".")}', " +
                     $"{_proficiencyLevels.
                         Where(it => 
                             it.LanguageId == _languages.
                                 Where(it => 
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.Name == ProficiencyLevelsName[ProficiencyLevelsNameIndex]).ToList()[0].Id})";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void EditClient()
    {
        string sql = $"update curse set " +
                     $"name = '{Item.Name}', " +
                     $"info = '{Item.Info}', " +
                     $"price = '{Item.Price.ToString().Replace(",", ".")}', " +
                     $"proficiency_level_id = {_proficiencyLevels.
                         Where(it =>
                             it.LanguageId == _languages.
                                 Where(it =>
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it =>
                             it.Name == ProficiencyLevelsName[ProficiencyLevelsNameIndex]).ToList()[0].Id} " +
                     $"where id = {Item.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}