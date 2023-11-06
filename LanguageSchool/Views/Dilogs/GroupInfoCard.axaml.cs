using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dialogs;

public partial class GroupInfoCard : Window
{
    public GroupInfoCard(InfoCardEnum action)
    {
        InitializeComponent();
        
        var languageMenu = this.FindControl<StackPanel>("ClientMenu");
        var languageDataGrid = this.FindControl<StackPanel>("ClientDataGrid");

        switch (action)
        {
            case InfoCardEnum.Add:
                languageMenu.IsVisible = false;
                languageDataGrid.IsVisible = false;
                break;
            case InfoCardEnum.Edit:
                languageMenu.IsVisible = true;
                languageDataGrid.IsVisible = true;
                break;
            case InfoCardEnum.Info:
                languageMenu.IsVisible = false;
                languageDataGrid.IsVisible = true;
                this.FindControl<Button>("Ok").IsVisible = false;
                break;
        }
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.GroupInfoCardViewModel;
        bool result = dataContext.ActionGroup();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}