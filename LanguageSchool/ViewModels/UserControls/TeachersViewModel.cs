using System;

namespace LanguageSchool.ViewModels.UserControls;

public class TeachersViewModel : IDisposable
{
    private string _searchQuery = "";
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            _searchQuery = value;
        }
    }
    
    public void AddTeacherButton()
    {
        
    }

    public void EditTeacherButton()
    {
        
    }

    public void OpenCardTeacherButton()
    {
        
    }

    public void TakeItems(Utils.TakeItemsEnum takeItemsEnum)
    {
        
    }
    
    public void Dispose()
    {
    }
}