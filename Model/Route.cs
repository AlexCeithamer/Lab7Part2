using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Lab6_Starter.Model;

namespace Lab6_Starter.Model;
[Serializable()]
public class Route : INotifyPropertyChanged
{
    public ObservableCollection<Airport> Airports { get; set; }
    //route number is the number of the route if used in a list of routes (unique identifier)
    public string RouteNumber { get; set; }
    public int TotalAirports { get; set; }
    public int TotalDistance { 
        get
        {
            return CalculateTotalDistance();
        }
        set { }
    }

    /// <summary>
    /// Constructor with route identifier
    /// </summary>
    /// <param name="airports"></param>
    /// <param name="routeNumber"></param>
    public Route(ObservableCollection<Airport> airports, int routeNumber)
    {
        Airports = airports;
        RouteNumber = $"Route {routeNumber}";
        TotalAirports = airports.Count;
        TotalDistance = CalculateTotalDistance();
    }

    /// <summary>
    /// Constructor without route identifier
    /// </summary>
    /// <param name="airports"></param>
    public Route(ObservableCollection<Airport> airports)
    {
        Airports = airports;
        TotalAirports = airports.Count;
        TotalDistance = CalculateTotalDistance();
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Route()
    {
        Airports = new ObservableCollection<Airport>();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Calculates the total distance of the route
    /// </summary>
    /// <returns></returns>
    private int CalculateTotalDistance()
    {
        double totalDistance = 0;

        for (int i = 0; i < Airports.Count - 1; i++)
        {
            Airport currentAirport = Airports[i];
            Airport nextAirport = Airports[i + 1];

            double distanceBetweenAirports = currentAirport.GetDistance(nextAirport.Id);
            totalDistance += distanceBetweenAirports;
        }

        return (int)Math.Ceiling(totalDistance);
    }
}