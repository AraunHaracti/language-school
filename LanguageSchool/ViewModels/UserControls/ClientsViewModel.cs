using System;
using Avalonia.Controls;

namespace LanguageSchool.ViewModels.UserControls;

public class ClientsViewModel : IDisposable
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
    
    public void AddClientButton()
    {
        
    }

    public void EditClientButton()
    {
        
    }

    public void OpenCardClientButton()
    {
        
    }

    public void TakeItems(Utils.TakeItemsEnum takeItemsEnum)
    {
        
    }
    
    public void Dispose()
    {
    }
}