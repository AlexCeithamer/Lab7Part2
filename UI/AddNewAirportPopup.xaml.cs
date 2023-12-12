using CommunityToolkit.Maui.Views;
using Lab6_Starter;
using Lab6_Starter.Model;

namespace Lab6_Solution;

/// <summary>
/// Authors: Vincent Morrill, Alex Wolff, Jordyn Henrich, Keith 
/// </summary>
public partial class AddNewAirportPopup : Popup
{
	/// <summary>
	/// Initializes the popup component.
	/// </summary>
	public AddNewAirportPopup()
	{
		InitializeComponent();
	}

	/// <summary>
	/// Adds an airport with the specified information to the database.
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Arguments</param>
	public void AddAirportButtonClicked(System.Object sender, System.EventArgs args)
	{
		// Group 1's contributions
		DateTime dateVisited;
        int rating;
		string id = IdENT.Text;
		string city = CityENT.Text;
        bool validDate = DateTime.TryParse(DateVisitedENT.Text, out dateVisited);
		bool validRating = int.TryParse(RatingENT.Text, out rating);
        AirportAdditionError result = AirportAdditionError.NoError;

		if (!validDate || !validRating)
		{
			// Parsing operations failed
			result = AirportAdditionError.InvalidDate;
		} else if (rating > 5 || rating < 1)
		{
			result = AirportAdditionError.InvalidRating;
		} else if (city.Length > 200)
		{
			result = AirportAdditionError.InvalidCityLength;
		} else if (id.Length > 200)
		{
			result = AirportAdditionError.InvalidIdLength;
		}
         else {
            //no issues in input
            result = MauiProgram.BusinessLogic.AddAirport(id, city, dateVisited, rating);
        }
        if (DateTime.TryParse(DateVisitedENT.Text, out dateVisited) && int.TryParse(RatingENT.Text, out rating))
        {
            // Both parsing operations were successful.
            result = MauiProgram.BusinessLogic.AddAirport(IdENT.Text, CityENT.Text, dateVisited, rating);
        } else {
		result = AirportAdditionError.InvalidDate;
		}

        Close(result);
     }
	

	/// <summary>
	/// Closes the "Add New Airport" popup.
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="args">Arguments</param>
	public void ClosePopupButtonClicked(System.Object sender, System.EventArgs args)
	{
		try
		{
			this.Close();
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: Unable to close the popup {ex.InnerException.Message}");
		}
	}
}
