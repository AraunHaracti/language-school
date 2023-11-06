using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LanguageSchool.Views.Dilogs;

public partial class GroupClientInfoCard : Window
{
    public GroupClientInfoCard()
    {
        InitializeComponent();
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.GroupClientInfoCardViewModel;
        bool result = dataContext.ActionGroupClient();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}