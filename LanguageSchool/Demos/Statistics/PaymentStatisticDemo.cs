using System;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using LanguageSchool.Utils;
using LanguageSchool.ViewModels.Statistics;
using LanguageSchool.Views.Statistics;

namespace LanguageSchool.Demos.Statistics;

public class PaymentStatisticDemo : IModule
{
    private PaymentView _view;
    private PaymentViewModel _viewModel;

    public string Name => "Оплата";

    public Bitmap Picture => new Bitmap(AssetLoader.Open(new Uri("avares://LanguageSchool/Assets/attendances.png")));

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