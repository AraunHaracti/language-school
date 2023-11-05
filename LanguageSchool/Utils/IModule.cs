using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace LanguageSchool.Utils;

public interface IModule
{ 
    string Name { get; }
    
    UserControl UserInterface { get; }

    Bitmap Picture { get; }
    
    void Deactivate();
}