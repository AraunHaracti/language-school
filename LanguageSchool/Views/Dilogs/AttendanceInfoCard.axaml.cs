using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dilogs;

public partial class AttendanceInfoCard : Window
{
    public AttendanceInfoCard()
    {
        InitializeComponent();
    }
    
    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.AttendanceInfoCardViewModel;
        bool result = dataContext.ActionAttendance();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void GroupName_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (DataContext == null || e.Property.Name != "SelectedIndex")
            return;
        var dataContext = DataContext as ViewModels.Dialogs.AttendanceInfoCardViewModel;
        dataContext.GroupsComboBoxChanged();
    }
}