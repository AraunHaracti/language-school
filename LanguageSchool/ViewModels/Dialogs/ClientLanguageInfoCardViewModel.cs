using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class ClientLanguageInfoCardViewModel : ViewModelBase
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

    private Client _client;
    
    private ClientLanguage _personLanguage;
    
    public ClientLanguage PersonLanguage => _personLanguage;
    
    public ClientLanguageInfoCardViewModel()
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

    public void LanguagesComboBoxChanged()
    {
        _languageLevelsName = new List<string>();
        var language = _languages.Where(it => it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0];
        
        foreach (var item in _languageLevels.Where(it => it.LanguageId == language.Id))
        {
            LanguageLevelsName.Add(item.Name);
        }
        
        this.RaisePropertyChanged("LanguagesName");
        this.RaisePropertyChanged("LanguageLevelsName");
    }
    
    public ClientLanguageInfoCardViewModel(Action action, Client client) : this()
    {
        _action = action;
        _client = client;
        _personLanguage = new ClientLanguage();

        _isEdit = false;
    }
    
    public ClientLanguageInfoCardViewModel(Action action, Client client, ClientLanguage clientLanguage) : this()
    {
        _action = action;
        _client = client;
        _personLanguage = clientLanguage;

        _isEdit = true;
    }

    public void ActionClientLanguage()
    {
        if (_isEdit)
        {
            EditClientLanguage();
        }
        else
        {
            AddClientLanguage();
        }
        
        _action.Invoke();
    }

    private void AddClientLanguage()
    {
        string sql = $"insert into client_language (client_id, language_level_id, last_experience, needs) values (" +
                     $"{_client.Id}, " +
                     $"{_languageLevels.
                         Where(it => 
                             it.LanguageId == _languages.
                                 Where(it => 
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.Name == LanguageLevelsName[LanguageLevelsNameIndex]).ToList()[0].Id}, " +
                     $"'{PersonLanguage.LastExperience}', " +
                     $"'{PersonLanguage.Needs}')";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }

    public void EditClientLanguage()
    {
        string sql = $"update client_language set " +
                     $"language_level_id = {_languageLevels.
                         Where(it =>
                             it.LanguageId == _languages.
                                 Where(it =>
                                     it.Name == LanguagesName[LanguagesNameIndex]).ToList()[0].Id).
                         Where(it =>
                             it.Name == LanguageLevelsName[LanguageLevelsNameIndex]).ToList()[0].Id}, " +
                     $"last_experience = {PersonLanguage.LastExperience}, " +
                     $"needs = {PersonLanguage.Needs}, " +
                     $"where id = {PersonLanguage.Id}";
                     
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}