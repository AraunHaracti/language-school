﻿using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Controls;
using LanguageSchool.Utils;

namespace LanguageSchool.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged = delegate { };
    
    public List<IModule> Modules { get; private set; }
    
    private IModule _SelectedModule;
    public IModule SelectedModule
    {
        get { return _SelectedModule; }
        set
        {
            if (value == _SelectedModule) return;
            if (_SelectedModule != null) _SelectedModule.Deactivate();
            _SelectedModule = value;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedModule)));
            PropertyChanged(this, new PropertyChangedEventArgs("UserInterface"));
        }
    }
    
    public MainWindowViewModel(){}
    
    
    public MainWindowViewModel(IEnumerable<IModule> modules)
    {
        Modules = new List<IModule>(modules);
        
        if (this.Modules.Count > 0)
        {
            SelectedModule = this.Modules[0];
        }
    }
    
    public UserControl UserInterface
    {
        get
        {
            if (SelectedModule == null) 
                return null;
            return SelectedModule.UserInterface;
        }
    }
}