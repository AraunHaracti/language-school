using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.UserControls;
using LanguageSchool.Views.UserControls;

namespace LanguageSchool.Demos;

public class SchedulesDemo : IModule
{
    private SchedulesView _view;
    private SchedulesViewModel _viewModel;

    public string Name => "Расписание";

    public Bitmap Picture => new Bitmap(AssetLoader.Open(new Uri("avares://LanguageSchool/Assets/schedule.png")));
    
    public UserControl UserInterface
    {
        get
        {
            if (_view == null) 
                CreateView();
            return _view;
        }
    }

    private void CreateView()
    {
        _view = new ();
        _viewModel = new ();
        _view.DataContext = _viewModel;
    }

    public void Deactivate()
    {
        _viewModel.Dispose();
        _view = null;
    }
}