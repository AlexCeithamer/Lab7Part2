using Lab6_Starter.Model;

namespace Lab6_Starter;
public partial class Map : ContentPage
{
	public Map()
	{
		InitializeComponent();
        PopulateMap();
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
	void PopulateMap()
	{
		var pinsToAdd = MauiProgram.BusinessLogic.GetAirportPins();
		foreach (var pin in pinsToAdd)
		{
			map.Pins.Add(pin);
		}
		var visitedPinsToAdd = MauiProgram.BusinessLogic.GetVisitedAirportPins();
        foreach (var pin in pinsToAdd)
        {
            map.Pins.Add(pin);
        }
    }
	
}