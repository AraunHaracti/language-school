

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LanguageSchool.Models;
using LanguageSchool.Utils;
using MySql.Data.MySqlClient;
using ReactiveUI;

namespace LanguageSchool.ViewModels.Dialogs;

public class AttendanceInfoCardViewModel : ViewModelBase
{
    private bool _isEdit;
    
    private Action _action;
    
    private int _groupsNameIndex;
    private int _cleintsNameIndex;
    private int _schedulesNameIndex;
    private int _valuesNameIndex;
    
    public int GroupsNameIndex
    {
        get => _groupsNameIndex;
        set
        {
            _groupsNameIndex = value;
            this.RaisePropertyChanged();
        }
    }

    public int ClientsNameIndex
    {
        get => _cleintsNameIndex;
        set
        {
            _cleintsNameIndex = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int SchedulesNameIndex
    {
        get => _schedulesNameIndex;
        set
        {
            _schedulesNameIndex = value;
            this.RaisePropertyChanged();
        }
    }
    
    public int ValuesNameIndex
    {
        get => _valuesNameIndex;
        set
        {
            _valuesNameIndex = value;
            this.RaisePropertyChanged();
        }
    }
    
    private List<string> _groupsName = new();
    private List<string> _clientsName = new();
    private List<string> _schedulesName = new();
    private List<string> _valuesName = new();

    public List<string> GroupsName => _groupsName;
    public List<string> ClientsName => _clientsName;
    public List<string> SchedulesName => _schedulesName;
    public List<string> ValuesName => _valuesName;

    private List<Group> _groups = new();
    private List<ClientInGroup> _clients = new();
    private List<Schedule> _schedules = new();
    private List<Value> _values = new();

    private AttendanceLog _item;
    
    public AttendanceInfoCardViewModel()
    {
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select " + 
                                                "`group`.id as `id`, " + 
                                                "`group`.teacher_id as `teacher_id`, " + 
                                                "concat(teacher.name, ' ', teacher.surname) as `teacher_name`, " +
                                                "`group`.course_id as `course_id`, " +
                                                "`course`.name as `course_name`, " +
                                                "`group`.name as `name` " +
                                                "from `group` " +
                                                "join `teacher` " +
                                                "on `group`.teacher_id = `teacher`.id " +
                                                "join `course` " +
                                                "on `group`.course_id = `course`.id");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Group()
                {
                    Id = reader.GetInt32("id"),
                    TeacherId = reader.GetInt32("teacher_id"),
                    CourseId = reader.GetInt32("course_id"),
                    TeacherName = reader.GetString("teacher_name"),
                    CourseName = reader.GetString("course_name"),
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
                
                _clients.Add(currentItem);
            }
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select " +
                                                "`schedule`.id as `id`, " +
                                                "`schedule`.group_id as `group_id`, " +
                                                "`group`.name as `group_name`, " +
                                                "`schedule`.about as `about`, " +
                                                "`schedule`.datetime as `datetime` " +
                                                "from `schedule` " +
                                                "join `group` " +
                                                "on `schedule`.group_id = `group`.id");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Schedule()
                {
                    Id = reader.GetInt32("id"),
                    GroupId = reader.GetInt32("group_id"),
                    GroupName = reader.GetString("group_name"),
                    About = reader.GetString("about"),
                    Datetime = reader.GetDateTime("datetime"),
                };
                
                _schedules.Add(currentItem);
            }
        }
        
        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData("select * from value");
            
            while (reader.Read() && reader.HasRows)
            {
                var currentItem = new Value()
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.GetString("name")
                };
                
                _values.Add(currentItem);
            }
        }
        
        foreach (var item in _values)
        {
            ValuesName.Add(item.Name);
        }
    }
    
    public void GroupsComboBoxChanged()
    {
        _clientsName = new List<string>();
        _schedulesName = new List<string>();
        
        var group = _groups.Where(it => it.Name == GroupsName[GroupsNameIndex]).ToList()[0];
        
        foreach (var item in _clients.Where(it => it.GroupId == group.Id))
        {
            ClientsName.Add(string.Concat(item.ClientName, " ", item.ClientSurname));
        }
        
        foreach (var item in _schedules.Where(it => it.GroupId == group.Id))
        {
            SchedulesName.Add(item.Datetime.ToString());
        }
        
        this.RaisePropertyChanged("GroupsName");
        this.RaisePropertyChanged("ClientsName");
        this.RaisePropertyChanged("SchedulesName");
    }
    
    public AttendanceInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new AttendanceLog();

        _isEdit = false;
    }

    public AttendanceInfoCardViewModel(Action action, AttendanceLog attendanceLog) : this()
    {
        _action = action;
        _item = attendanceLog;

        _isEdit = true;
    }

    public bool ActionAttendance()
    {
        if (ClientsNameIndex == -1 || SchedulesNameIndex == -1)
            return false;
        
        if (_isEdit)
        {
            EditAttendance();
        }
        else
        {
            AddAttendance();
        }
        
        _action.Invoke();

        return true;
    }

    private void AddAttendance()
    {
        string sql = $"insert into attendance_log (value_id, schedule_id, client_in_group_id) values (" +
                     $"{_valuesNameIndex + 1}, " +
                     $"{_schedules.
                         Where(it => 
                             it.GroupId == _groups.
                                 Where(it => 
                                     it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.Datetime.ToString() == SchedulesName[SchedulesNameIndex]).ToList()[0].Id}, " +
                     $"{_clients.
                         Where(it => 
                             it.GroupId == _groups.
                                 Where(it => 
                                     it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id).
                         Where(it => 
                             String.Concat(it.ClientName, " ", it.ClientSurname) == ClientsName[ClientsNameIndex]).ToList()[0].Id})";
        
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
        
    }
    
    private void EditAttendance()
    {
        string sql = $"update attendance_log set " +
                     $"value_id = {_valuesNameIndex + 1}, " +
                     $"schedule_id = {_schedules.
                         Where(it => 
                             it.GroupId == _groups.
                                 Where(it => 
                                     it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id).
                         Where(it => 
                             it.Datetime.ToString() == SchedulesName[SchedulesNameIndex]).ToList()[0].Id}, " +
                     $"client_in_group_id = {_clients.
                         Where(it => 
                             it.GroupId == _groups.
                                 Where(it => 
                                     it.Name == GroupsName[GroupsNameIndex]).ToList()[0].Id).
                         Where(it => 
                             String.Concat(it.ClientName, " ", it.ClientSurname) == ClientsName[ClientsNameIndex]).ToList()[0].Id} " +
                     $"where id = {_item.Id}";
                     
        using (Database db = new Database())
        {
            db.SetData(sql);
        }
    }
}