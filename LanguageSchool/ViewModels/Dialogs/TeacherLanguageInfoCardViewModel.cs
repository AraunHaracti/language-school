using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class TeacherLanguageInfoCardViewModel : ViewModelBase
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

    private Teacher _person;
    
    private TeacherLanguage _personLanguage;
    
    public TeacherLanguage PersonLanguage => _personLanguage;
    
    public TeacherLanguageInfoCardViewModel()
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
    
    public TeacherLanguageInfoCardViewModel(Action action, Teacher teacher) : this()
    {
        _action = action;
        _person = teacher;
        _personLanguage = new TeacherLanguage();

        _isEdit = false;
    }
    
    public TeacherLanguageInfoCardViewModel(Action action, Teacher teacher, TeacherLanguage teacherLanguage) : this()
    {
        _action = action;
        _person = teacher;
        _personLanguage = teacherLanguage;

        _isEdit = true;
    }

    public void ActionTeacherLanguage()
    {
        if (_isEdit)
        {
            EditTeacherLanguage();
        }
        else
        {
            AddTeacherLanguage();
        }
        
        _action.Invoke();
    }

    private void AddTeacherLanguage()
    {
        string sql = $"insert into teacher_language (teacher, proficiency_level_id) values (" +
                     $"{_person.Id}, " +
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

    public void EditTeacherLanguage()
    {
        string sql = $"update teacher_language set " +
                     $"proficiency_level_id = {_proficiencyLevels.
                         Where(it =>
                             it.LanguageId == _languages.
                                 Where(it =>
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it =>
                             it.Name == ProficiencyLevelsName[ProficiencyLevelsNameIndex]).ToList()[0].Id} " +
                     $"where id = {PersonLanguage.Id}";
                     
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}