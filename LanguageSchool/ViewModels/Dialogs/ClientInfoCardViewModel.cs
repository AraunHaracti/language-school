using System;
using System.Collections.Generic;
using System.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;


namespace LanguageSchool.ViewModels.Dialogs;

public class ClientInfoCardViewModel
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private List<ClientLanguage> _itemsFromDatabase;
    
    private string _sql = $"select " +
                          $"client_language.id as id, " +
                          $"client_language.client_id as client_id, " +
                          $"client_language.proficiency_level_id as proficiency_level_id, " +
                          $"proficiency_level.name as proficiency_level_name, " +
                          $"proficiency_level.language_id as language_id, " +
                          $"language.name as language_name, " +
                          $"client_language.last_experience as last_experience, " +
                          $"client_language.needs as needs " +
                          $"from client_language " +
                          $"join proficiency_level " +
                          $"on client_language.proficiency_level_id = proficiency_level.id " +
                          $"join language " +
                          $"on proficiency_level.language_id = language.id";

    
    private Client _person;

    private List<ClientLanguage> _personLanguage = new();
    
    public ClientLanguage CurrentItem { get; set; }
    
    public Client Person => _person;
    
    public List<ClientLanguage> PersonLanguage => _personLanguage;

    public ClientInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
    }
    
    public void UpdateItems()
    {
        GetDataFromDatabase();
    }
    
    private void GetDataFromDatabase()
    {
        _itemsFromDatabase = new List<ClientLanguage>();

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(_sql);
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new ClientLanguage()
                {
                    Id = reader.GetInt32("Id"),
                    ClientId = reader.GetInt32("ClientId"),
                    ProficiencyLevelId = reader.GetInt32("ProficiencyLevelId"), 
                    LanguageId = reader.GetInt32("LanguageId"),
                    ProficiencyLevelName = reader.GetString("ProficiencyLevelName"),
                    LanguageName = reader.GetString("LanguageName"),
                };
                currentItem.LastExperience = (reader.IsDBNull("LastExperience") ? null : reader.GetString("LastExperience"));
                currentItem.Needs = (reader.IsDBNull("Needs") ? null : reader.GetString("Needs"));

                _itemsFromDatabase.Add(currentItem);
            }
        }
    }
    
    public ClientInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _person = new Client();

        _isEdit = false;
    }
    
    public ClientInfoCardViewModel(Action action, Client client) : this()
    {
        _action = action;
        _person = client;

        _isEdit = true;
        
        UpdateItems();
    }

    public bool ActionClient()
    {
        if (Person.Name == null || Person.Name == "" || Person.Surname == null || Person.Surname == "")
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

    private void AddClient()
    {
        string sql = $"insert into client (name, surname, birthday, phone, email) values (" +
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

    public void EditClient()
    {
        string sql = $"update client set " +
                     $"name = '{Person.Name}', " +
                     $"surname = '{Person.Surname}', " +
                     $"birthday = '{Person.Birthday.ToString("yyyy-MM-dd")}', " +
                     $"phone = '{Person.Phone}', " +
                     $"email = '{Person.Email}' " +
                     $"where id = {Person.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }

    public void AddLanguageButton()
    {
        var view = new ClientLanguageInfoCard();
        var vm = new ClientLanguageInfoCardViewModel(UpdateItems, Person);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditLanguageButton()
    {
        var view = new ClientLanguageInfoCard();
        var vm = new ClientLanguageInfoCardViewModel(UpdateItems, Person, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void DeleteLanguageButton()
    {
        
    }
}