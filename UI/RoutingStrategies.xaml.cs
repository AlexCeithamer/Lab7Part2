using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.Metadata.Ecma335;
using Lab6_Starter.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Lab6_Starter;

public partial class RoutingStrategies : ContentPage, INotifyPropertyChanged
{   
    public bool IsVisited { get; set; }
    public int radius { get; set; }

    private Route _route;

    private ObservableCollection<Airport> _routeAirports;

    public Route Route
    {
        get => _route;
        set
        {
            if (_route != value)
            {
                _route = value;
                OnPropertyChanged(nameof(Route));
            }
        }
    }
    public ObservableCollection<Airport> RouteAirports
    {
        get => _routeAirports;
        set
        {
            if (_routeAirports != value)
            {
                _routeAirports = value;
                OnPropertyChanged(nameof(RouteAirports));
            }
        }
    }

    override
    protected void OnAppearing()
    {
        ShowAirports();
    }


    public new event PropertyChangedEventHandler PropertyChanged;

    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public RoutingStrategies()
    { 
        InitializeComponent();
        Route = new Route();

        this.BindingContext = this;
    }

    public ObservableCollection<Airport> VisitedAirports(ObservableCollection<Airport> airports)
    {
        ObservableCollection<Airport> visitedAirports = new ObservableCollection<Airport>();
        foreach (Airport airport in airports)
        {
            if (MauiProgram.BusinessLogic.FindWisconsinAirport(airport.Id) != null)
            {
                visitedAirports.Add(MauiProgram.BusinessLogic.FindWisconsinAirport(airport.Id));
            }
        }

        return visitedAirports;
    }

    public void VisitedToggled(object sender, ToggledEventArgs e)
    {
        ShowAirports();
    }

    public void ShowAirports()
    {
        map.Pins.Clear();
        bool isVisited = IsVisitedENT.IsToggled;
        var airports = MauiProgram.BusinessLogic.GetWisconsinAirports();
        if (isVisited)
        {
            airports = VisitedAirports(MauiProgram.BusinessLogic.GetAirports());
        }

        foreach (Airport airport in airports)
        {
            Pin airportPin = new Pin()
            {
                Location = new Location(airport.Latitude, airport.Longitude),
                Label = airport.Id,
                Address = airport.City,
                Type = PinType.Place
            };
            map.Pins.Add(airportPin);
        }
    }

    public void ShowRouteAirports()
    {
        map.Pins.Clear();
        foreach (Airport airport in RouteAirports)
        {
            Pin airportPin = new Pin()
            {
                Location = new Location(airport.Latitude, airport.Longitude),
                Label = airport.Id,
                Address = airport.City,
                Type = PinType.Place
            };
            map.Pins.Add(airportPin);
        }
    }


    public async void CalculateRoute(object sender, EventArgs e)
    {
        loadingIndicator.IsRunning = true;
        loadingIndicator.IsVisible = true;
        await Task.Run(() =>
        {
            //set the variables from the entries
            String airportId = AirportIdENT.Text.ToUpper();
            int maxDistance;
            bool result = int.TryParse(MaxDistanceENT.Text, out maxDistance);
            if (!result)
            {
                //run error message on main thread
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Please enter a valid distance", "OK"));
                return;
            }
            bool isVisited = IsVisitedENT.IsToggled;

            //check that AirportID and MaxDistance is not null
            if (airportId == null || airportId.Length < 3 || airportId.Length > 4)
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Please enter a valid airport ID", "OK"));
                return;
            }
            if (maxDistance < 0)
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Please enter a valid distance", "OK"));
                return;
            }

            //calculate the routes to be displayed
            Route = MauiProgram.BusinessLogic.CalculateRoute(airportId, maxDistance, isVisited);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                //display the route
                if (Route.Airports.Count <= 2)
                {
                    NumberAirports.Text = $"Number of Airports: N/A";
                    RouteDistance.Text = $"Total Distance: N/A";
                    if (RouteAirports != null)
                    {
                        RouteAirports.Clear();
                    }
                    Route = null;
                    MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "No routes found", "OK"));
                }
                else
                {
                    NumberAirports.Text = $"Number of Airports: {Route.TotalAirports}";
                    RouteDistance.Text = $"Total Distance: {Route.TotalDistance} km";
                    RouteAirports = Route.Airports;
                }
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;

                ShowRouteAirports();

                for (int i = 0; i < RouteAirports.Count - 1; i++)
                {
                    var polyline = new Polyline
                    {
                        StrokeColor = Colors.Red,
                        StrokeWidth = 12,
                        Geopath =
                        {
                            new Location(RouteAirports[i].Latitude, RouteAirports[i].Longitude),
                            new Location(RouteAirports[i + 1].Latitude, RouteAirports[i + 1].Longitude)
                        }
                    };
                    map.MapElements.Add(polyline);
                }

            });
        });

        
        
    }
}

