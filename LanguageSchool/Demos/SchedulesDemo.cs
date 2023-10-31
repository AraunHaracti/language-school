using Avalonia.Controls;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.UserControls;
using LanguageSchool.Views.UserControls;

namespace LanguageSchool.Demos;

public class SchedulesDemo : IModule
{
    private SchedulesView _view;
    private SchedulesViewModel _viewModel;

    public string Name => "Расписание";

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