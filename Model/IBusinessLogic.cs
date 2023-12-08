using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lab6_Starter.Model;
public interface IBusinessLogic
{
    public String UserId {get; set;} // all airport operations will use this userId
    AirportAdditionError AddAirport(String id, String city, DateTime dateVisited, int rating);
    AirportDeletionError DeleteAirport(String id);
    AirportEditError EditAirport(String id, String city, DateTime dateVisited, int rating);
    Airport FindAirport(String id);
    String CalculateStatistics();
    Double ToRadians(Double x);
    Double CalculateDistance(Tuple<Double, Double> latLong1, Tuple<Double, Double> latLong2);

    ObservableCollection<NearbyAirport> CalculateAllAirportDist(String startAirportId, int distance);
    ObservableCollection<Airport> GetAirports();
    ObservableCollection<Resource> GetResources();

    // RoutingStrategies
    ObservableCollection<Airport> GetWisconsinAirports();

    Route CalculateRoute(String id, int maxDist, bool isVisited);

    public ObservableCollection<Airport> FillDistances(ObservableCollection<Airport> airports, Airport startingAirport, int maxDistance, bool onlyStartingAirport);
    double CalculateDistance(Airport start, Airport end);

    public Airport FindWisconsinAirport(String id);


}
