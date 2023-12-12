using System.Collections.ObjectModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Controls.Maps;
using Npgsql; // To install this, add dotnet add package Npgsql 


namespace Lab6_Starter.Model;

public partial class Database : IDatabase
{
    private static System.Random rng = new();
    private String connString;

    ObservableCollection<Airport> airports = new();
    ObservableCollection<Airport> wiAirports = new();
    ObservableCollection<Resource> resources = new();
    ObservableCollection<Pin> airportPins = new();
    ObservableCollection<Pin> visitedAirportPins = new();

    public Database()
    {
        connString = GetConnectionString();
    }



    // Fills our local Airports ObservableCollection with all the airports in the database
    // We don't cache the airports in the database, so we have to go to the database to get them
    // This is one of those "tradeoffs" we have to make when we use a database
   public ObservableCollection<Airport> SelectAllAirports(String userId)
{
    try
    {
        airports.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        Console.WriteLine("hello from here");
        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM visited_airports WHERE user_id = @user_id", conn);
        cmd.Parameters.AddWithValue("user_id", userId);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            String id = (String)reader["id"];
            String city = (String)reader["city"];
            DateTime dateVisited = (DateTime)reader["date_visited"];
            int rating = Convert.ToInt32(reader["rating"]);
            Airport airportToAdd = new(id, userId, city, dateVisited, rating);
            airports.Add(airportToAdd);
            Console.WriteLine(airportToAdd);
        }
    }
    catch (NpgsqlException ex)
    {
        // Handle Npgsql-specific exceptions here
        Console.WriteLine("Database operation failed: " + ex.Message);
        // Depending on your requirements, you might want to rethrow the exception or handle it differently
    }
    catch (Exception ex)
    {
        // Handle non-database related exceptions here
        Console.WriteLine("An error occurred: " + ex.Message);
    }
    finally
    {
        // This block is executed regardless of whether an exception was thrown or not.
        // Here you can close connection, dispose objects etc.
    }

    return airports;
}


    // Fills wiAirports ObservableCollection with all Wisconsin airports in the database
    public ObservableCollection<Airport> SelectAllWiAirports()
    {
        wiAirports.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        // using() ==> disposable types are properly disposed of, even if there is an exception thrown 
        using var cmd = new NpgsqlCommand("SELECT id, name, lat, long FROM wi_airports", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object

        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            String id = reader.GetString(0);
            String city = reader.GetString(1);
            Double latitude = reader.GetDouble(2);
            Double longitude = reader.GetDouble(3);
            Airport airportToAdd = new(id, null, city, DateTime.MinValue, 1, latitude, longitude); // borrowing same Airport class, yet another sign we should've thought about users earlier!!
            wiAirports.Add(airportToAdd);
            Console.WriteLine(airportToAdd);
        }

        return wiAirports;
    }

    /// <summary>
    /// Get only the IDs of the Wisconsin airports from the DB; mostly for error checking.
    /// </summary>
    /// <returns> a list object containing all IATA-format Wisconsin airport identifiers </returns>
    public List<string> SelectAllWiAirportIds()
    {
        wiAirports.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT id FROM wi_airports", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object
        List<string> ids = new List<string>();
        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            string id = reader.GetString(0);
            ids.Add(id);
        }

        return ids;
    }


    // Fills resources ObservableCollection with all the resources in the database
    public ObservableCollection<Resource> SelectAllResources()
    {
        resources.Clear();
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT link, name FROM resources", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object

        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            String link = reader.GetString(0);
            String name = reader.GetString(1);
            Resource resourceToAdd = new(link, name);
            resources.Add(resourceToAdd);
            Console.WriteLine(resourceToAdd);
        }

        return resources;
    }

    // Finds the airport (among those visited) with the given id, null if not found
    public Airport SelectAirport(String id, String userId)
    {
        Airport airportToAdd = null;
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT id, city, date_visited, rating FROM visited_airports WHERE id = @id AND user_id = @user_id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("user_id", userId);

        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object
        if (reader.Read())
        { // there should be only one row, so we don't need a while loop TODO: Sanity check

            id = reader["id"] as String; // reader["id"] is a boxed String, so we have to cast it to a String
            String city = reader["city"] as String;
            DateTime dateVisited = reader["date_visited"] as DateTime? ?? default; // reader["date_visited"] is a boxed DateTime, so we have to cast it to a DateTime
            Int32 rating = reader["rating"] as Int32? ?? default; // reader["rating"] is a boxed Int32, so we have to cast it to an Int32
            airportToAdd = new(id, userId, city, dateVisited, rating);
        }
        return airportToAdd;
    }

/// <summary>This inserts a airport into the database, or prints out an error message and returns false if the airport already exists. Notice the try-catch block, how could INSERT possibly fail?
/// </summary>
/// <param name="airport"airport to insert></param>
/// <returns></returns>
    // 
    public AirportAdditionError InsertAirport(Airport airport)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database

            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "INSERT INTO visited_airports (id, city, date_visited, rating, user_id) VALUES (@id, @city, @date_visited, @rating, @user_id)";
            cmd.Parameters.AddWithValue("id", airport.Id);
            cmd.Parameters.AddWithValue("city", airport.City);
            cmd.Parameters.AddWithValue("date_visited", airport.DateVisited);
            cmd.Parameters.AddWithValue("rating", airport.Rating);
            cmd.Parameters.AddWithValue("user_id", airport.UserId);
            cmd.ExecuteNonQuery(); // used for INSERT, UPDATE & DELETE statements - returns # of affected rows 

            SelectAllAirports(airport.UserId);
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Insert failed, {0}", pe);
            return AirportAdditionError.DBAdditionError;
        }
        return AirportAdditionError.NoError;
    }



    // The UI >> BusinessLogic >> Database the currently selected airport (selected in the CollectionView) for upating 
    public AirportEditError UpdateAirport(Airport airportToUpdate)
    {
        try
        {
            using var conn = new NpgsqlConnection(connString); // conn, short for connection, is a connection to the database

            conn.Open(); // open the connection ... now we are connected!
            var cmd = new NpgsqlCommand(); // create the sql commaned
            cmd.Connection = conn; // commands need a connection, an actual command to execute
            cmd.CommandText = "UPDATE visited_airports SET city = @city, date_visited = @date_visited, rating = @rating WHERE id = @id;";

            cmd.Parameters.AddWithValue("id", airportToUpdate.Id);
            cmd.Parameters.AddWithValue("city", airportToUpdate.City);
            cmd.Parameters.AddWithValue("date_visited", airportToUpdate.DateVisited);
            cmd.Parameters.AddWithValue("rating", airportToUpdate.Rating);
            var numAffected = cmd.ExecuteNonQuery();

            SelectAllAirports(airportToUpdate.UserId);
        }
        catch (Npgsql.PostgresException pe)
        {
            Console.WriteLine("Update failed, {0}", pe);
            return AirportEditError.DBEditError;
        }
        return AirportEditError.NoError;
    }

/// <summary>
/// Deletes an airport -- not clear why a user would want to do this but we offer the functionality just in case
/// </summary>
/// <param name="airportToDelete">Airport to delete</param>
/// <returns>AirportDeletionError (or none if succes)</returns>
    public AirportDeletionError DeleteAirport(Airport airportToDelete)
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "DELETE FROM visited_airports WHERE id = @id";
        cmd.Parameters.AddWithValue("id", airportToDelete.Id);
        int numDeleted = cmd.ExecuteNonQuery();

        if (numDeleted > 0)
        {
            SelectAllAirports(airportToDelete.UserId); // go and fetch the airports again, otherwise Airports will be out of sync with the database
            return AirportDeletionError.NoError;
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }

/// <summary>
/// This looks up a Wisconsin airport (1 out of 142)
/// </summary>
/// <param name="id">id to look up, e.g., ATW</param>
/// <returns>the airport, null if not found</returns>
    public Airport SelectWisconsinAirport(String id)
    {
        Airport airportToAdd = null;
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        using var cmd = new NpgsqlCommand("SELECT id, name, lat, long FROM wi_airports WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);

        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object
        if (reader.Read())
        { // there should be only one row, so we don't need a while loop TODO: Sanity check

            id = reader.GetString(0);
            String city = reader.GetString(1);
            double lat = reader.GetDouble(2);
            double long_ = reader.GetDouble(3);
            airportToAdd = new(id, null, city, DateTime.Now, 5);
            airportToAdd.Latitude = lat;
            airportToAdd.Longitude = long_;
        }
        return airportToAdd;
    }

    // Builds a ConnectionString, which is used to connect to the database
    static String GetConnectionString()
    {
        var connStringBuilder = new NpgsqlConnectionStringBuilder();
        connStringBuilder.Host = "stormy-ocelot-12775.5xj.cockroachlabs.cloud";
        connStringBuilder.Port = 26257;
        connStringBuilder.SslMode = SslMode.VerifyFull;
        connStringBuilder.Username = "mprogers"; // won't hardcode this in your app
        connStringBuilder.Password = FetchPassword();
        connStringBuilder.Database = "defaultdb";
        connStringBuilder.ApplicationName = "whatever";
        connStringBuilder.IncludeErrorDetail = true;

        return connStringBuilder.ConnectionString;
    }

    // Fetches the password from the user secrets store (um, this works in VS, but not in the beta of VSC's C# extension)
    // This assumes the NuGet package is installed -- dotnet add package Microsoft.Extensions.Configuration.UserSecrets
    static String FetchPassword()
    {
        IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Database>().Build();
        return config["CockroachDBPassword"] ?? "xYvR09EUtNehzMnSpJlojA"; // if it can't find the password, returns ... the password (this works in VS, not VSC) 
    }

    /// <summary>
    /// This generates all of the airport pins
    /// </summary>
    /// <returns>an observable collection of airport pins</returns>
    public ObservableCollection<Pin> GenerateAllAirportPins()
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        // using() ==> disposable types are properly disposed of, even if there is an exception thrown 
        using var cmd = new NpgsqlCommand("SELECT id, name, lat, long FROM wi_airports", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object

        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            String id = reader.GetString(0);
            String name = reader.GetString(1);
            double lat = reader.GetDouble(2);
            double longi = reader.GetDouble(3);
            Location location = new(lat, longi);
            
            Pin airportPinToAdd = new Pin { 
                Label = id, 
                Address = name, 
                Type = PinType.Place, 
                Location = location };
            airportPins.Add(airportPinToAdd);
            Console.WriteLine(airportPinToAdd);
        }

        return airportPins;
    }
    public ObservableCollection<Pin> GenerateAllVisitedAirportPins()
    {
        var conn = new NpgsqlConnection(connString);
        conn.Open();

        // using() ==> disposable types are properly disposed of, even if there is an exception thrown 
        using var cmd = new NpgsqlCommand("SELECT va.id,  wa.id, wa.name, wa.lat, wa.long FROM wi_airports wa JOIN visited_airports va ON wa.id = va.id WHERE va.id = wa.id", conn);
        using var reader = cmd.ExecuteReader(); // used for SELECT statement, returns a forward-only traversable object

        while (reader.Read()) // each time through we get another row in the table (i.e., another Airport)
        {
            String id = reader.GetString(1);
            String name = reader.GetString(2);
            double lat = reader.GetDouble(3);
            double longi = reader.GetDouble(4);
            Location location = new(lat, longi);

            Pin airportPinToAdd = new Pin
            {
                Label = "Visited Airport",
                Address = name,
                Type = PinType.Place,
                Location = location
            };
            visitedAirportPins.Add(airportPinToAdd);
            Console.WriteLine(airportPinToAdd);
        }

        return airportPins;
    }

}

