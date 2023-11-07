using System;
using System.Collections.Generic;
using LanguageSchool.Utils;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using MySql.Data.MySqlClient;
using SkiaSharp;

namespace LanguageSchool.ViewModels.Statistics;

public class PaymentViewModel : IDisposable
{
    public PaymentViewModel()
    {
        GetData();
    }
    
    public ISeries[] Series { get; set; } =
    {
        new ColumnSeries<double>
        {
            Values = _values,
            
        }
    };

    public ICartesianAxis[] XAxis { get; set; } =
    {
        new Axis
        {
            Labels = _names
        }
    };

    private static List<string> _names = new();
    private static List<double> _values = new();
    
    private void GetData()
    {
        _names = new();
        _values = new();
        
        string sql = "select " +
                     "concat(client.name, ' ', client.surname) as name, " +
                     "client_in_group.id, " +
                     "SUM(count) as sum " +
                     "from payment " +
                     "join client_in_group " +
                     "on payment.client_in_group_id = client_in_group.id " +
                     "join client " +
                     "on client_in_group.client_id = client.id " +
                     "group by client_in_group.id, name";

        using (Database db = new Database())
        {
            MySqlDataReader reader = db.GetData(sql);

            while (reader.Read() && reader.HasRows)
            {
                _names.Add(reader.GetString("name"));
                _values.Add(reader.GetDouble("sum"));
            }
        }
    }

    public LabelVisual Title { get; set; } = new LabelVisual
    {
        Text = "My chart title",
        TextSize = 25,
        Padding = new LiveChartsCore.Drawing.Padding(15),
        Paint = new SolidColorPaint(SKColors.DarkSlateGray)
    };
    
    public void Dispose() { }
}