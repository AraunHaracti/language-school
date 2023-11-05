using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;

namespace LanguageSchool.ViewModels.Dialogs;

public class ScheduleInfoCardViewModel
{
    private readonly Window _parentWindow;
    
    private bool _isEdit;
    
    private Action _action;
    private Schedule _item;
    
    public ScheduleInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
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
    
    public ScheduleInfoCardViewModel(Schedule item) : this()
    {
        _item = item;

        _isEdit = true;
    }
}