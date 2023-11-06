using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dialogs;

public partial class PaymentInfoCard : Window
{
    public PaymentInfoCard()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void Ok_OnClick(object? sender, RoutedEventArgs e)
    {
        var dataContext = DataContext as ViewModels.Dialogs.PaymentInfoCardViewModel;
        bool result = dataContext.ActionPayment();
        
        if (result)
            Close();
    }

    private void Exit_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void Groups_OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (DataContext == null || e.Property.Name != "SelectedIndex")
            return;
        var dataContext = DataContext as ViewModels.Dialogs.PaymentInfoCardViewModel;
        dataContext.GroupsComboBoxChanged();
    }
}