using System;
using System.Collections.Generic;
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

public class PaymentInfoCardViewModel : ViewModelBase
{
    private Action _action;
    
    private int _groupsNameIndex = 0;

    public int GroupsNameIndex
    {
        get => _groupsNameIndex;
        set
        {
            _groupsNameIndex = value;
            this.RaisePropertyChanged();
        }
    }

    private int _groupClientsNameIndex = 0;

    public int GroupClientsNameIndex
    {
        get => _groupsNameIndex;
        set
        {
            _groupsNameIndex = value;
            this.RaisePropertyChanged();
        }
    }

    private List<string> _groupsName = new();
    
    public List<string> GroupsName => _groupsName;

    private List<string> _groupClientsName = new();
    
    public List<string> GroupClientsName => _groupClientsName;

    private List<Group> _groups = new();

    private List<ClientInGroup> _groupClients = new();
    
    private Payment _item;
    public Payment Item => _item;
    
    public PaymentInfoCardViewModel()
    {
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from `group`");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Group()
                {
                    Id = reader.GetInt32("id"),
                    TeacherId = reader.GetInt32("teacher_id"),
                    CourseId = reader.GetInt32("course_id"),
                    Name = reader.GetString("name"),
                };

                _groups.Add(currentItem);
            }
        }
        
        foreach (var item in _groups)
        {
            GroupsName.Add(item.Name);
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select " +
                                                "client_in_group.id as id, " +
                                                "client_in_group.group_id as group_id, " +
                                                "client_in_group.client_id as client_id, " +
                                                "client.name as client_name, " +
                                                "client.surname as client_surname, " +
                                                "client.birthday as client_birthday " +
                                                "from client_in_group " +
                                                "join client " +
                                                "on client_in_group.client_id = client.id");
            
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

                _groupClients.Add(currentItem);
            }
        }
    }

    public void GroupsComboBoxChanged()
    {
        _groupClientsName = new List<string>();
        var group = _groups.Where(it => it.Name == GroupsName[GroupsNameIndex]).ToList()[0];

        foreach (var item in _groupClients.Where(it => it.GroupId == group.Id))
        {
            GroupClientsName.Add(item.ClientName);
        }
        
        this.RaisePropertyChanged("GroupsName");
        this.RaisePropertyChanged("GroupClientsName");
    }
    
    public PaymentInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new Payment();
    }

    public bool ActionPayment()
    {
        if (GroupClientsNameIndex == -1)
            return false;

        AddClient();
        
        _action.Invoke();

        return true;
    }

    private void AddClient()
    {
        string sql = $"insert into payment (client_in_group_id, date, count) values (" +
                     $"{_groupClients.
                         Where(it => 
                             it.GroupId == _groups.
                                 Where(it => 
                                     it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.ClientName == GroupClientsName[GroupClientsNameIndex]).ToList()[0].Id}, " +
                     $"'{Item.Date.ToString("yyyy-MM-dd hh:mm:ss")}', " +
                     $"'{Item.Count.ToString().Replace(",", ".")}')";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}