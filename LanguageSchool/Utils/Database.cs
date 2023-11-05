using System;
using MySql.Data.MySqlClient;

namespace LanguageSchool.Utils;

public class Database : IDisposable
{
    private MySqlConnection? _connection;

    public static MySqlConnectionStringBuilder? ConnectionStringBuilder { get; set; }

    public Database()
    {
        Open();
    }

    public void SetData(string sql)
    {
        var command = new MySqlCommand(sql, _connection);
        command.ExecuteNonQuery();
    }

    public MySqlDataReader GetData(string sql)
    {
        var command = new MySqlCommand(sql, _connection);
        var reader = command.ExecuteReader();

        return reader;
    }

    private void Open()
    {
        if (ConnectionStringBuilder == null)
            throw new Exception("Connection string is empty");
        _connection = new MySqlConnection(ConnectionStringBuilder.ConnectionString);
        _connection.Open();
    }
    
    private void Close()
    {
        _connection?.Close();
    }
    
    public void Dispose()
    {
        Close();
    }
    
    ~Database()
    {
        Dispose();
    }
}