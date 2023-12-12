using System.Collections.ObjectModel;
using Lab6_Starter.Model;

namespace Lab6_Starter;

public partial class NearbyAirports : ContentPage
{
    public ObservableCollection<NearbyAirport> AirportsNearby { get; set; }
    public NearbyAirports()
    {
        InitializeComponent();

        BindingContext = MauiProgram.BusinessLogic;
    }

    void OnRefreshBtnClicked(object sender, EventArgs e)
    {
        if (airport_idENT.Text == null)
        {
            DisplayAlert("Ruhroh", "No Airport ID given.", "OK");
            return;
        }

        if (!int.TryParse(distanceENT.Text, out int distance) || distance < 0)
        {
            DisplayAlert("Ruhroh", "Invalid distance given.", "OK");
            return;
        }

        ObservableCollection<NearbyAirport> airports = MauiProgram.BusinessLogic.CalculateAllAirportDist(airport_idENT.Text, distance);
        if (airports == null) 
        {
            DisplayAlert("Ruhroh", "Airport ID is not valid.", "OK");
        }

        // Set the AirportsNearby in the code-behind
        AirportsNearby = airports;

        // Explicitly trigger the UI to update
        CV.ItemsSource = null;
        CV.ItemsSource = AirportsNearby;
    }
}