using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LanguageSchool.Demos;
using LanguageSchool.Utils;
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
            IEnumerable<IModule> modules = new List<IModule>()
            {
                new WelcomeDemo(), new AttendancesDemo(), new ClientsDemo(), new CoursesDemo(),
                new GroupsDemo(), new PaymentsDemo(), new SchedulesDemo(), new TeachersDemo()
            };
            var vm = new MainWindowViewModel(modules);
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm,
            };

            desktop.MainWindow.Closing += (s, args) => vm.SelectedModule.Deactivate();
        }

        base.OnFrameworkInitializationCompleted();
    }
}