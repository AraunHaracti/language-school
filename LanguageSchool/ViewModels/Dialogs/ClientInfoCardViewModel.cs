using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using LanguageSchool.Views.Dilogs;
using MySql.Data.MySqlClient;
using ReactiveUI;


namespace LanguageSchool.ViewModels.Dialogs;

public class ClientInfoCardViewModel : ViewModelBase
{
    private readonly Window _parentWindow;
    private bool _isEdit;
    
    private Action _action;
    
    private List<ClientLanguage> _itemsFromDatabase;
    
    private readonly string _sql = "select " +
                                   "`client_language`.id as `id`, " +
                                   "`client_language`.client_id as `client_id`, " +
                                   "`client_language`.language_level_id as `language_level_id`, " +
                                   "`language_level`.name as `language_level_name`, " +
                                   "`language_level`.language_id as `language_id`, " +
                                   "`language`.name as `language_name`, " +
                                   "`client_language`.last_experience as `last_experience`, " +
                                   "`client_language`.needs as `needs` " +
                                   "from `client_language` " +
                                   "join `language_level` " +
                                   "on `client_language`.language_level_id = `language_level`.id " +
                                   "join `language` " +
                                   "on `language_level`.language_id = `language`.id";

    
    private Client _person;

    private ObservableCollection<ClientLanguage> _personLanguage;
    
    public ClientLanguage CurrentItem { get; set; }
    
    public Client Person => _person;
    
    public ObservableCollection<ClientLanguage> PersonLanguage
    {
        get => _personLanguage;
        set
        {
            _personLanguage = value;
            this.RaisePropertyChanged("PersonLanguage");
        }
    }
    
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
        PersonLanguage = new ObservableCollection<ClientLanguage>(_itemsFromDatabase.Where(it => it.ClientId == _person.Id));
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
                    Id = reader.GetInt32("id"),
                    ClientId = reader.GetInt32("client_id"),
                    LanguageLevelId = reader.GetInt32("language_level_id"), 
                    LanguageId = reader.GetInt32("language_id"),
                    LanguageLevelName = reader.GetString("language_level_name"),
                    LanguageName = reader.GetString("language_name"),
                };
                currentItem.LastExperience = (reader.IsDBNull("last_experience") ? null : reader.GetString("last_experience"));
                currentItem.Needs = (reader.IsDBNull("needs") ? null : reader.GetString("needs"));

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
    
    public ClientInfoCardViewModel(Client client) : this()
    {
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

    public void AddItemButton()
    {
        var view = new ClientLanguageInfoCard();
        var vm = new ClientLanguageInfoCardViewModel(UpdateItems, Person);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void EditItemButton()
    {
        if (CurrentItem == null)
            return;
        var view = new ClientLanguageInfoCard();
        var vm = new ClientLanguageInfoCardViewModel(UpdateItems, Person, CurrentItem);
        view.DataContext = vm;
        view.ShowDialog(_parentWindow);
    }

    public void DeleteItemButton()
    {
        string sql = $"delete from client_language where client_language.id = {CurrentItem.Id}";

        using Database db = new Database();
        
        db.SetData(sql);

        UpdateItems();
    }
}