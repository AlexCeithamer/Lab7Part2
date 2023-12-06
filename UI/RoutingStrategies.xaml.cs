using System.Collections.ObjectModel;
using System.ComponentModel;
using Lab6_Starter.Model;

namespace Lab6_Starter;

public partial class RoutingStrategies : ContentPage, INotifyPropertyChanged
{   
    public bool IsVisited { get; set; }
    private ObservableCollection<Route> _routes;

    public ObservableCollection<Route> Routes
    {
        get => _routes;
        set
        {
            if (_routes != value)
            {
                _routes = value;
                OnPropertyChanged(nameof(Routes));
            }
        }
    }

    public new event PropertyChangedEventHandler PropertyChanged;

    protected new virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public RoutingStrategies()
    { 
        InitializeComponent();
        Routes = new ObservableCollection<Route>();

        this.BindingContext = this;
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
            if (airportId == null || airportId.Length < 3 || airportId.Length > 4) return;
            if (maxDistance < 0) return;

            //calculate the routes to be displayed
            Routes = MauiProgram.BusinessLogic.CalculateRoutes(airportId, maxDistance, isVisited);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                loadingIndicator.IsRunning = false;
                loadingIndicator.IsVisible = false;
            });
        });

        
        
    }
}

