using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dilogs;

public partial class ClientInfoCard : Window
{
    public ClientInfoCard(InfoCardEnum action)
    {
        InitializeComponent();

        var languageMenu = this.FindControl<StackPanel>("LanguageMenu");
        var languageDataGrid = this.FindControl<DataGrid>("LanguageDataGrid");

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
                break;
        }
    }
    
    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.ClientInfoCardViewModel;
        dataContext.ActionClient();
        
        Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}