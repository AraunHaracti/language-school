using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dilogs;

public partial class CourseInfoCard : Window
{
    public CourseInfoCard()
    {
        InitializeComponent();
    }

    private void Languages_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (DataContext == null || e.Property.Name != "SelectedIndex")
            return;
        var dataContext = DataContext as ViewModels.Dialogs.CourseInfoCardViewModel;
        dataContext.LanguagesComboBoxChanged();
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.CourseInfoCardViewModel;
        dataContext.ActionCourseLanguage();
        
        Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}