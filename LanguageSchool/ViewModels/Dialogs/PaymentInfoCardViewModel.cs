using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using LanguageSchool.Models;

namespace LanguageSchool.ViewModels.Dialogs;

public class PaymentInfoCardViewModel
{
    private readonly Window _parentWindow;
    
    private bool _isEdit;
    
    private Action _action;
    private Payment _item;
    
    public PaymentInfoCardViewModel()
    {
        if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _parentWindow = desktop.MainWindow;
        }
    }
    public PaymentInfoCardViewModel(Action action) : this()
    {
        _action = action;
        _item = new Payment();

        _isEdit = false;
    }
    public PaymentInfoCardViewModel(Action action, Payment client) : this()
    {
        _action = action;
        _item = client;

        _isEdit = true;
    }
    
    public PaymentInfoCardViewModel(Payment client) : this()
    {
        _item = client;

        _isEdit = true;
    }
}