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

public class ScheduleInfoCardViewModel : ViewModelBase
{
    private bool _isEdit;
    
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
    
    private List<string> _groupsName = new();

    public List<string> GroupsName => _groupsName;

    private List<Group> _groups = new();
    
    private Schedule _item;

    public Schedule Item => _item;
    
    public ScheduleInfoCardViewModel()
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
    }
    public ScheduleInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new Schedule();

        _isEdit = false;
    }
    public ScheduleInfoCardViewModel(Action action, Schedule item) : this()
    {
        _action = action;
        _item = item;

        _isEdit = true;
    }
    
    public bool ActionSchedule()
    {
        if (Item.About == null || Item.About == "")
            return false;
        
        if (_isEdit)
        {
            EditSchedule();
        }
        else
        {
            AddSchedule();
        }
        
        _action.Invoke();

        return true;
    }

    public void AddSchedule()
    {
        string sql = $"insert into schedule (datetime, about, group_id) values (" +
                     $"'{Item.Datetime.ToString("yyyy-MM-dd hh:mm:ss")}', " +
                     $"'{Item.About}', " +
                     $"{_groups.Where(it => 
                         it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id})";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
    
    public void EditSchedule()
    {
        string sql = $"update schedule set " +
                     $"datetime = '{Item.Datetime.ToString("yyyy-MM-dd hh:mm:ss")}', " +
                     $"about = '{Item.About}', " +
                     $"group_id = {_groups.Where(it => 
                         it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id} " +
                     $"where id = {Item.Id}";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}