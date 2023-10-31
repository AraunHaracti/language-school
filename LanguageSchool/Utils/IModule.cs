using Avalonia.Controls;

namespace LanguageSchool.Utils;

public interface IModule
{ 
    string Name { get; }
    
    UserControl UserInterface { get; }
    
    void Deactivate();
}