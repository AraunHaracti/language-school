using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LanguageSchool.Interfaces;
using LanguageSchool.ViewModels;
using LanguageSchool.Views;

namespace LanguageSchool;

public partial class App : Application
{
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel
                {
                    PageViewModels = new List<IPageViewModel>
                    {
                        new TestViewModel(),
                    }
                },
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}