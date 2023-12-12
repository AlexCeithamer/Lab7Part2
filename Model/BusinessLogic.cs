using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Runtime.Intrinsics.Arm;
using System.ComponentModel;
using System.Linq;
using Lab6_Starter;
using Microsoft.Maui.Controls.Maps;

namespace Lab6_Starter.Model;

/// <summary>
/// This class implements the BusinessLogic
/// </summary>
public partial class BusinessLogic : IBusinessLogic, INotifyPropertyChanged
{
    const int BRONZE_LEVEL = 42;
    const int SILVER_LEVEL = 84;
    const int GOLD_LEVEL = 128;


    IDatabase db;
    private const int MIN_RATING = 1;
    private const int MAX_RATING = 5;
    private const int MIN_ID_LENGTH = 3;
    private const int MAX_ID_LENGTH = 4;

    public String UserId {get;set;} // by introducing this as a property, we don't have to change the UI too much
    public event PropertyChangedEventHandler PropertyChanged;

    public ObservableCollection<Airport> Airports
    {
        get { return GetAirports(); }

    }

    public ObservableCollection<Resource> Resources
    {
        get { return GetResources(); }

    }

    /// <summary>
    /// Gets all of the airport pins from the database
    /// </summary>
    /// <returns>an observable collection of airport pins</returns>
    public ObservableCollection<Pin> AirportPins
    {
        get { return GetAirportPins(); }
    }
    /// <summary>
    /// Displays Visited Airport Pins
    /// </summary>
    public ObservableCollection<Pin> VisitedAirportPins
    {
        get { return GetVisitedAirportPins(); }
    }


    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    

    public Airport FindAirport(String id)
    {
        return db.SelectAirport(id, UserId);
    }
    
/// <summary>
/// Checks to make sure that all airport fields are legitimate
/// </summary>
/// <param name="id">id of airport to ad"></param>
/// <param name="dateVisited">date visited</param>
/// <param name="rating">rating, from 1-5</param>
/// <returns></returns>
    private AirportAdditionError CheckAirportFields(String id, String city, DateTime dateVisited, int rating)
    {
        if (id.Length <  MIN_ID_LENGTH || id.Length > MAX_ID_LENGTH)
        {
            return AirportAdditionError.InvalidIdLength;
        }
        if (city.Length < MIN_ID_LENGTH)
        {
            return AirportAdditionError.InvalidCityLength;
        }
        if (rating < MIN_RATING || rating > MAX_RATING)
        {
            return AirportAdditionError.InvalidRating;
        }

        return AirportAdditionError.NoError;
    }


/// <summary>
/// Adds a new airport to the DB. It grabs these fields from the popup, but since it knows the UserId, we add that from elsewhere
/// </summary>
/// <param name="id"></param>
/// <param name="city"></param>
/// <param name="dateVisited"></param>
/// <param name="rating"></param>
/// <returns>An error if the pilot has already visited the airport. You can't stamp it twice</returns>
    public AirportAdditionError AddAirport(String id, String city, DateTime dateVisited, int rating)
    {

        var result = CheckAirportFields(id, city, dateVisited, rating);
        if (result != AirportAdditionError.NoError)
        {
            return result;
        }

        if (db.SelectAirport(id, UserId) != null)           // we do not allow duplicate airport visits by one user ... we might regret that
        {
            return AirportAdditionError.DuplicateAirportId;
        }
        Airport airport = new Airport(id, UserId, city, dateVisited, rating);
        db.InsertAirport(airport);

        return AirportAdditionError.NoError;
    }

    public AirportDeletionError DeleteAirport(String id)
    {

        var entry = db.SelectAirport(id, UserId);

        if (entry != null)
        {
            AirportDeletionError success = db.DeleteAirport(entry);
            if (success == AirportDeletionError.NoError)
            {
                return AirportDeletionError.NoError;

            }
            else
            {
                return AirportDeletionError.DBDeletionError;
            }
        }
        else
        {
            return AirportDeletionError.AirportNotFound;
        }
    }


    public AirportEditError EditAirport(String id, String city, DateTime dateVisited, int rating)
    {

        var fieldCheck = CheckAirportFields(id, city, dateVisited, rating);
        if (fieldCheck != AirportAdditionError.NoError)
        {
            return AirportEditError.InvalidFieldError;
        }

        var airport = db.SelectAirport(id, UserId);
        airport.Id = id;
        airport.City = city;
        airport.DateVisited = dateVisited;
        airport.Rating = rating;

        AirportEditError success = db.UpdateAirport(airport);
        if (success != AirportEditError.NoError)
        {
            return AirportEditError.DBEditError;
        }

        return AirportEditError.NoError;
    }


    public String CalculateStatistics()
    {
        FlyWisconsinLevel nextLevel;
        int numAirportsUntilNextLevel;

        int numAirportsVisited = db.SelectAllAirports(UserId).Count;
        if (numAirportsVisited < BRONZE_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Bronze;
            numAirportsUntilNextLevel = BRONZE_LEVEL - numAirportsVisited;
        }
        else if (numAirportsVisited < SILVER_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Silver;
            numAirportsUntilNextLevel = SILVER_LEVEL - numAirportsVisited;
        }
        else if (numAirportsVisited < GOLD_LEVEL)
        {
            nextLevel = FlyWisconsinLevel.Gold;
            numAirportsUntilNextLevel = GOLD_LEVEL - numAirportsVisited;
        }
        else
        {
            nextLevel = FlyWisconsinLevel.None;
            numAirportsUntilNextLevel = 0;
        }

        return String.Format("{0} airport{1} visited; {2} airports remaining until achieving {3}",
              numAirportsVisited, numAirportsVisited != 1 ? "s" : "", numAirportsUntilNextLevel, nextLevel);
    }

    public ObservableCollection<Airport> GetAirports()
    {
        return db.SelectAllAirports(UserId);
    }

    public ObservableCollection<Resource> GetResources()
    {
        return db.SelectAllResources();
    }

    /// <summary>
    /// Get all airport pins for all airports in Wisconsin
    /// </summary>
    /// <returns>ObservableCollection<Pin></returns>
    public ObservableCollection<Pin> GetAirportPins()
    {
        return db.GenerateAllAirportPins();
    }
    /// <summary>
    /// Gets all visited airports and populates the pins.
    /// </summary>
    /// <returns>ObservableCollection<Pin></returns>
    public ObservableCollection<Pin> GetVisitedAirportPins()
    {
        return db.GenerateAllVisitedAirportPins();
    }
    


    /// <summary>
    /// Call the DB layer to get the IDs of the Wisconsin airports; mostly for error checking.
    /// </summary>
    /// <returns> a list object containing all IATA-format Wisconsin airport identifiers </returns>
    public List<string> SelectAllWiAirportIds()
    {
        return db.SelectAllWiAirportIds();
    }

}

