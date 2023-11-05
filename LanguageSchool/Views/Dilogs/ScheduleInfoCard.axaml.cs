using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LanguageSchool.Utils;

namespace LanguageSchool.Views.Dilogs;

public partial class ScheduleInfoCard : Window
{
    public ScheduleInfoCard(InfoCardEnum action)
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

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}