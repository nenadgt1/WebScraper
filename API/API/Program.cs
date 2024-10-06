using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/api/cars", () =>
{
    var carList = new List<object>();
    string connectionString = "Server=34.107.109.154;Database=car_ads;Uid=scraper_user;Pwd=password;";

    try
    {
        using (MySqlConnection conn = new MySqlConnection(connectionString))
        {
            conn.Open();
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM cars", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    carList.Add(new
                    {
                        Make = reader["make"].ToString(),
                        Model = reader["model"].ToString(),
                        Year = reader["year"].ToString(),
                        Mileage = reader["mileage"].ToString(),
                        Price = reader["price"].ToString(),
                        AdNumber = reader["ad_number"].ToString()
                    });
                }
            }
        }

        // Return car list as JSON if successful
        return Results.Json(carList);
    }
    catch (MySqlException ex)
    {
        // Log error (in production, you'd want to log this to a file or logging system)
        Console.WriteLine($"Database error: {ex.Message}");
        return Results.StatusCode(500); // Internal Server Error
    }
    catch (Exception ex)
    {
        // Log any other errors
        Console.WriteLine($"Error: {ex.Message}");
        return Results.StatusCode(500); // Internal Server Error
    }
});

app.Run();
