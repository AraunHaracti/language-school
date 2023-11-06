using Avalonia.Controls;
using Avalonia.Interactivity;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dialogs;

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
                this.FindControl<Button>("Ok").IsVisible = false;
                break;
        }
    }
    
    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.ClientInfoCardViewModel;
        bool result = dataContext.ActionClient();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}