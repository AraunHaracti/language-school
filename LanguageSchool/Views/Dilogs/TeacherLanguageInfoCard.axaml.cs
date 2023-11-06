using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LanguageSchool.Views.Dialogs;

public partial class TeacherLanguageInfoCard : Window
{
    public TeacherLanguageInfoCard()
    {
        InitializeComponent();
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.TeacherLanguageInfoCardViewModel;
        dataContext.ActionTeacherLanguage();
        
        Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Languages_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (DataContext == null || e.Property.Name != "SelectedIndex")
            return;
        var dataContext = DataContext as ViewModels.Dialogs.TeacherLanguageInfoCardViewModel;
        dataContext.LanguagesComboBoxChanged();
    }
}