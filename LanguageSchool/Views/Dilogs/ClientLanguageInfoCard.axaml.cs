using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace LanguageSchool.Views.Dialogs;

public partial class ClientLanguageInfoCard : Window
{
    public ClientLanguageInfoCard()
    {
        InitializeComponent();
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.ClientLanguageInfoCardViewModel;
        bool result = dataContext.ActionClientLanguage();
        
        if (result)
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
        var dataContext = DataContext as ViewModels.Dialogs.ClientLanguageInfoCardViewModel;
        dataContext.LanguagesComboBoxChanged();
    }
}