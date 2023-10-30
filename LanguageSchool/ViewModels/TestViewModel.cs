using LanguageSchool.Interfaces;

namespace LanguageSchool.ViewModels;

public class TestViewModel : ViewModelBase, IPageViewModel
{
    public string Title => "page1";
    public string Text => "text";
}