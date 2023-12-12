using CommunityToolkit.Maui.Core.Platform;
using Lab6_Starter.Model;
using System.Reflection;

namespace Lab6_Starter;

public partial class Weather : ContentPage
{
    const int CODE_LENGTH = 4;
    const String CODE_PREFIX = "K";
    List<string> ids;
    public Weather()
    {
        InitializeComponent();
        try
        {
            BindingContext = MauiProgram.BusinessLogic;
            ids = MauiProgram.BusinessLogic.SelectAllWiAirportIds();
        }
        catch(TargetInvocationException tie)
        {
            throw; 
        }
    }
    /// <summary>
    /// searches for metar and taf based on information in the entry
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnSearch_Clicked(object sender, EventArgs e)
    {
        if (entry.Text == null)
        {
            DisplayAlert("Error", "No airport id entered", "Ok");
        }
        else
        {
            _ = entry.HideKeyboardAsync(CancellationToken.None); //close the keyboard after searching
                                                                 //Check length of Wisconsin ICAO airport is correct length of 4 characters.
            if (entry.Text.Length == CODE_LENGTH)
            {
                //Check if it is a valid Wisconsin ICAO airport (First character must be a 'K')
                if (entry.Text.ToUpper().IndexOf(CODE_PREFIX) == 0)
                {
                    string searchId = entry.Text.ToUpper().Substring(1) + " ";//ids are stored with space at the end
                    if (!ids.Contains(searchId)) //ids contains IATA(3characters) not ICAO(3characters)
                    {
                        DisplayAlert("Error", "Airport id does not exist", "Ok");
                        return;
                    }
                    try
                    {
                        String metar = Meteorologist.GetMetar(entry.Text);
                        if (metar == "Unexpected JSON structure")
                        {
                            MetarLabel.Text = "";
                            DisplayAlert("Error: Invalid METAR", "Error while reading METAR data", "OK");
                        }
                        else if (metar == "0")
                        {
                            MetarLabel.Text = "No METAR available for this airport";
                        }
                        else
                        {
                            MetarLabel.Text = metar;
                        }
                    }
                    catch (Exception ex)
                    {
                        MetarLabel.Text = "";
                        DisplayAlert("Error: Invalid METAR", ex.Message, "OK");
                    }
                    try
                    {
                        String taf = Meteorologist.GetTaf(entry.Text);
                        if (taf == "Unexpected JSON structure")
                        {
                            TafLabel.Text = "";
                            DisplayAlert("Error: Invalid TAF", "Error while reading TAF data", "OK");
                        }
                        else if (taf == "0")
                        {
                            TafLabel.Text = "No TAF available for this airport";
                        }
                        else
                        {
                            TafLabel.Text = taf;
                        }
                    }
                    catch (Exception ex)
                    {
                        TafLabel.Text = "";
                        DisplayAlert("Error: Invalid TAF", ex.Message, "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error: Invalid Wisconsin ICAO airport", "Not an airport starting with K character", "OK");
                }
            }
            else
            {
                DisplayAlert("Error: Invalid ICAO airport", "Not a length of 4 characters", "OK");
            }

        }
    }
}