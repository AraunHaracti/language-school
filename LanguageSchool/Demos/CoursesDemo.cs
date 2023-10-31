using Avalonia.Controls;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.UserControls;
using LanguageSchool.Views.UserControls;

namespace LanguageSchool.Demos;

public class CoursesDemo : IModule
{
    private CoursesView _view;
    private CoursesViewModel _viewModel;

    public string Name => "Курсы";

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