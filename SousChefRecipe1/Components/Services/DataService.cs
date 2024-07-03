using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using SousChefRecipe1.Components;

public class DataService
{
    private readonly string _connectionString;

    public DataService(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    public async Task InsertUserAsync(UsersClass user)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = new NpgsqlCommand("INSERT INTO users (username, email, password) VALUES (@username, @email, @password)", conn))
        {
            cmd.Parameters.AddWithValue("username", user.username);
            cmd.Parameters.AddWithValue("email", user.email);
            cmd.Parameters.AddWithValue("password", user.password);

            Console.WriteLine(cmd);
            
            await cmd.ExecuteNonQueryAsync();
        }
    }
    
    public async Task<List<UsersClass>> CheckUserAsync(string email, string password)
    {
        var users = new List<UsersClass>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = new NpgsqlCommand("SELECT * FROM users WHERE email = @email AND password = @password", conn))
        {
            cmd.Parameters.AddWithValue("email", email);
            cmd.Parameters.AddWithValue("password", password);

            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new UsersClass
                    {
                        id = reader.GetInt32(0),
                        username = reader.GetString(1),
                        email = reader.GetString(2),
                        password = reader.GetString(3)
                    });
                }
            }
        }

        return users;
    }


    public async Task<List<UsersClass>> GetDataAsync(string inCMD)
    {
        var data = new List<UsersClass>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using (var cmd = new NpgsqlCommand(inCMD, conn))
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var user = new UsersClass
                {
                    id = reader.GetInt32(0),
                    username = reader.GetString(1),
                    email = reader.GetString(2),
                    password = reader.GetString(3)
                };
                data.Add(user);
            }
        }

        return data;
    }
}