using System.Collections.Generic;
using LanguageSchool.Interfaces;

namespace LanguageSchool.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private IPageViewModel _selectedPageViewModel;
    private IList<IPageViewModel> _pageViewModels;

    public string Title => $"MultiPage demo - {SelectedPageViewModel?.Title}";

    public IList<IPageViewModel> PageViewModels
    {
        get => _pageViewModels;
        set
        {
            _pageViewModels = value;
            OnPropertyChanged();
        }
    }

    public IPageViewModel SelectedPageViewModel 
    {
        get => _selectedPageViewModel; 
        set 
        {
            _selectedPageViewModel = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Title));
        } 
    }
}