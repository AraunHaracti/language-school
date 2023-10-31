using System;
using MySql.Data.MySqlClient;

namespace LanguageSchool;

public class Database : IDisposable
{
    private MySqlConnection? _connection;
    private static MySqlConnectionStringBuilder? _stringBuilder;

    public static MySqlConnectionStringBuilder? ConnectionStringBuilder
    {
        get => _stringBuilder;
        set => _stringBuilder = value;
    }
    
    public Database()
    {
        if (_stringBuilder == null)
        {
            throw new Exception("Connection string is empty");
        }

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
        _connection = new MySqlConnection(_stringBuilder.ConnectionString);
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
}