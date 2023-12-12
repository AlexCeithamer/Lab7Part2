using Microsoft.Maui.Controls.Maps;
using System.Collections.Generic;
using System.Collections.ObjectModel;

//TODO: Refactor to account for change in name of table - airports => visited_airports and introduction of user_id
namespace Lab6_Starter.Model
{
    public interface IDatabase
    {

        ObservableCollection<Airport> SelectAllAirports(String userId);
        Airport SelectAirport(String id, String userId);
        ObservableCollection<Airport> SelectAllWiAirports();
        List<string> SelectAllWiAirportIds();
        Airport SelectWisconsinAirport(String id);
        AirportAdditionError InsertAirport(Airport airport);
        AirportDeletionError DeleteAirport(Airport airport);
        AirportEditError UpdateAirport(Airport replacementAirport);
        ObservableCollection<Resource> SelectAllResources();

        ObservableCollection<Pin> GenerateAllAirportPins();
        ObservableCollection<Pin> GenerateAllVisitedAirportPins();
    }
}