using System;
using System.ComponentModel;

namespace Lab6_Starter.Model;

/// <summary>
/// Every airport has an id, city, userId, dateVisited, Rating, latitude longitude
/// Really, this is a VisitedAirport, and might be better off called such
/// </summary>
[Serializable()]
public class Airport : INotifyPropertyChanged
{
    String id;
    String userId;
    String city;
    DateTime dateVisited;
    int rating;
    Double latitude;
    Double longitude;
    int distanceToNextAirport;

    //Each Airport has a dictionary of every other Airport(Id) and the distance from itself to that airport
    public Dictionary<string, int> distances = new();

    public String Id
    {
        get { return id; }
        set
        {
            id = value;
            OnPropertyChanged(nameof(Id));
        }
    }

    public String UserId {
        get { return userId;}
        set {
            userId = value;
            OnPropertyChanged(nameof(UserId));
        }
    }

    public String City
    {
        get { return city; }
        set
        {
            city = value;
            OnPropertyChanged(nameof(City));
        }
    }

    public DateTime DateVisited
    {
        get { return dateVisited; }
        set
        {
            dateVisited = value;
            OnPropertyChanged(nameof(DateVisited));
        }
    }

    public int Rating
    {
        get { return rating; }
        set
        {
            rating = value;
            OnPropertyChanged(nameof(Rating));
        }
    }

    public Double Latitude
    {
        get { return latitude; }
        set { latitude = value; }
    }
    public Double Longitude
    {
        get { return longitude; }
        set { longitude = value; }
    }
    public int DistanceFromNextAirport
    {
        get { return distanceToNextAirport; }
        set { distanceToNextAirport = value; }
    }

    public Airport(String id, String userId, String city, DateTime dateVisited, int rating)
    {
        Id = id;
        UserId = userId;
        City = city;
        DateVisited = dateVisited;
        Rating = rating;
    }

    public Airport(String id, String userId, String city, DateTime dateVisited, int rating, Double latitude, Double longitude) : this(id,userId,city,dateVisited,rating)
    {
        Latitude = latitude;
        Longitude = longitude;
        DistanceFromNextAirport = 0;
    }

    public Airport() { }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public double GetDistance(string otherAirportId)
    {
        if (distances.ContainsKey(otherAirportId))
        {
            return distances[otherAirportId];
        }
        else
        {
            return -1;
        }

    }


    public override bool Equals(object obj)
    {
        var otherAirport = obj as Airport;
        return Id == otherAirport.Id && UserId == otherAirport.UserId;
    }

    public override int GetHashCode()
    {
        return id.GetHashCode();
    }
}
