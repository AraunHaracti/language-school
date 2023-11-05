using Avalonia.Controls;
using LanguageSchool.Utils;

namespace LanguageSchool.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        Database.ConnectionStringBuilder = new()
        {
            Server = "10.10.1.24",
            Port = 3306,
            Database = "pro1_12",
            UserID = "user_01",
            Password = "user01pro"
        };
        
        InitializeComponent();
    }
}