using CommunityToolkit.Maui.Views;
using Lab6_Solution;
using Lab6_Starter.Model;
namespace Lab6_Starter;

public partial class AirportsPage : ContentPage
{
    public AirportsPage()
    {
        InitializeComponent();

        // We've set the BindingContext for the entire page to be the BusinessLogic layer
        // So any control on the page can bind to the BusinessLogic layer
        // There's really only one control that needs to talk to the BusinessLogic layer, and that's the CollectionView
        BindingContext = MauiProgram.BusinessLogic.GetAirports();
    }

    // Various event handlers for the buttons on the main page

    async void AddAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        var popup = new AddNewAirportPopup();

        var result = await this.ShowPopupAsync(popup);
        await DisplayAlert("Result", result.ToString(), "OK");
    }

    void DeleteAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        Airport currentAirport = (sender as Button).BindingContext as Airport; // wow, one line of code to get the current Airport object :-)
        AirportDeletionError result = MauiProgram.BusinessLogic.DeleteAirport(currentAirport.Id);
        if (result != AirportDeletionError.NoError)
        {
            DisplayAlert("Ruhroh", result.ToString(), "OK");
        }
    }

    void EditAirport_Clicked(System.Object sender, System.EventArgs e)
    {
        Airport currentAirport = CV.SelectedItem as Airport;
        DateTime dateVisited;

        if (DateTime.TryParse(DateVisitedENT.Text, out dateVisited) == false)
        {
            DisplayAlert("Ruhroh", "Illegal date format", "OK");
        }
        else
        {
            AirportEditError result = MauiProgram.BusinessLogic.EditAirport(currentAirport.Id, CityENT.Text, DateTime.Parse(DateVisitedENT.Text), int.Parse(RatingENT.Text));
            if (result != AirportEditError.NoError)
            {
                DisplayAlert("Ruhroh", result.ToString(), "OK");
            }
        }
    }

    void CalculateStatistics_Clicked(System.Object sender, System.EventArgs e)
    {
        String result = MauiProgram.BusinessLogic.CalculateStatistics();

        DisplayAlert("Your Progress", result.ToString(), "Good to know");
    }

    /// <summary>
    /// This is the event handler for the Logout button. All it does is bring us back to the Login page
    /// </summary>
    /// <param name="sender">button that triggered this</param>
    /// <param name="e">no events worth worrying about</param>
    void Logout_Clicked(System.Object sender, System.EventArgs e)
    {
        //MauiProgram.BusinessLogic.Logout();
        Application.Current.MainPage = new LoginPage();
    }
}


