using Lab6_Starter.Model;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Lab6_Starter;
public partial class Map : ContentPage
{
	public Map()
	{
		InitializeComponent();
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