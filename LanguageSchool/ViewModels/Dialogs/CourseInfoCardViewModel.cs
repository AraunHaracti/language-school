using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class CourseInfoCardViewModel : ViewModelBase
{
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

    private int _languageLevelsNameIndex = 0;
    
    public int LanguageLevelsNameIndex
    {
        get => _languageLevelsNameIndex;
        set
        {
            _languageLevelsNameIndex = value;
            this.RaisePropertyChanged("LanguageLevelsNameIndex");
        } 
    }

    private List<string> _languagesName = new();
    
    public List<string> LanguagesName => _languagesName;

    private List<string> _languageLevelsName = new();

    public List<string> LanguageLevelsName => _languageLevelsName;
    
    private List<Language> _languages = new();

    private List<LanguageLevel> _languageLevels = new();
    
    private Course _item;
    
    public Course Item => _item;
    
    public CourseInfoCardViewModel()
    {
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
            MySqlDataReader reader = db.GetData("select * from language_level");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new LanguageLevel();

                PropertyInfo[] propertyInfos = typeof(LanguageLevel).GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    propertyInfos[i].SetValue(currentItem, reader.GetValue(i));
                }

                _languageLevels.Add(currentItem);
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
        _languageLevelsName = new List<string>();
        var language = _languages.Where(it => it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0];
        
        foreach (var item in _languageLevels.Where(it => it.LanguageId == language.Id))
        {
            LanguageLevelsName.Add(item.Name);
        }
        
        this.RaisePropertyChanged("LanguagesName");
        this.RaisePropertyChanged("ProficiencyLevelsName");
    }
    
    private void AddClient()
    {
        string sql = $"insert into course (name, info, price, language_level_id) values (" +
                     $"'{Item.Name}', " +
                     $"'{Item.Info}', " +
                     $"'{Item.Price.ToString().Replace(",", ".")}', " +
                     $"{_languageLevels.
                         Where(it => 
                             it.LanguageId == _languages.
                                 Where(it => 
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.Name == LanguageLevelsName[LanguageLevelsNameIndex]).ToList()[0].Id})";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void EditClient()
    {
        string sql = $"update course set " +
                     $"name = '{Item.Name}', " +
                     $"info = '{Item.Info}', " +
                     $"price = '{Item.Price.ToString().Replace(",", ".")}', " +
                     $"language_level_id = {_languageLevels.
                         Where(it =>
                             it.LanguageId == _languages.
                                 Where(it =>
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it =>
                             it.Name == LanguageLevelsName[LanguageLevelsNameIndex]).ToList()[0].Id} " +
                     $"where id = {Item.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}