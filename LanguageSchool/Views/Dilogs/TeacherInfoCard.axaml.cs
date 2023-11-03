using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dilogs;

public partial class TeacherInfoCard : Window
{
    public TeacherInfoCard(InfoCardEnum action)
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
        var dataContext = DataContext as ViewModels.Dialogs.TeacherInfoCardViewModel;
        bool result = dataContext.ActionTeacher();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}