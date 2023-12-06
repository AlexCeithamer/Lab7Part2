namespace Lab6_Starter.Model;

using System.Collections.ObjectModel;
using Lab6_Starter.Model;

public partial class BusinessLogic
{

        public Airport FindWisconsinAirport(String id)
     {
         if (db.SelectWisconsinAirport(id) == null)
         {
             return null;
         }
         return db.SelectWisconsinAirport(id);
     }
     

     public ObservableCollection<Airport> WisconsinAirports
     {
         get { return GetWisconsinAirports(); }
     }

        public ObservableCollection<Airport> GetWisconsinAirports()
     {
         return db.SelectAllWiAirports();
     }

    /// <summary>
    /// Calculates all possible routes from a starting airport within a given distance
    /// </summary>
    /// <param name="id"></param>
    /// <param name="maxDist"></param>
    /// <param name="isVisited"></param>
    /// <returns></returns>
    public ObservableCollection<Route> CalculateRoutes(string id, int maxDist, bool isVisited)
    {
         //find the starting airport
         Airport startingAirport = FindWisconsinAirport(id);
         //if the starting airport is null or not in wisconsin, return an empty collection
         if (startingAirport == null)
         {
             return new ObservableCollection<Route>();
         }
         //fill the distances for all airports
         ObservableCollection<Airport> airports =  FillDistances();
         //create a collection to keep track of which airports have been visited
         ObservableCollection<Route> routes = new ObservableCollection<Route>() {};
         var initialRoute = new ObservableCollection<Airport>() { startingAirport };
         ExploreRoutes(startingAirport, airports, initialRoute, 0, maxDist, isVisited, routes);

         return routes;
     }

     /// <summary>
     /// Private helper method for CalculateRoutes() to recursively explore all possible routes
     /// </summary>
     /// <param name="currentAirport"></param>
     /// <param name="allAirports"></param>
     /// <param name="currentRoute"></param>
     /// <param name="currentDistance"></param>
     /// <param name="maxDist"></param>
     /// <param name="isVisited"></param>
     /// <param name="routes"></param>
     private void ExploreRoutes(Airport currentAirport, ObservableCollection<Airport> allAirports, ObservableCollection<Airport> currentRoute, double currentDistance, int maxDist, bool isVisited, ObservableCollection<Route> routes)
     {
         foreach (var airport in allAirports)
         {
             //if the airport is already in the route, skip it. If the airport is not visited and we are only looking for visited airports, skip it.
             bool airportVisited = airport.DateVisited != DateTime.MinValue;
             if (currentRoute.Contains(airport) || isVisited != airportVisited) { continue; }

             //if the airport is within the max distance, add it to the route and explore from there
             double distanceToAdd = currentAirport.GetDistance(airport.Id);
             if (currentDistance + distanceToAdd <= maxDist)
             {
                 var newRoute = new ObservableCollection<Airport>(currentRoute) { airport };
                 double newDistance = currentDistance + distanceToAdd;
                 //if the route is less than the max distance, add it to the routes collection. Otherwise, explore from the new airport
                 if (newDistance <= maxDist && newDistance > 0)
                 {
                     routes.Add(new Route(newRoute, routes.Count + 1));
                 }
                 else
                 {
                     ExploreRoutes(airport, allAirports, newRoute, newDistance, maxDist, isVisited, routes);

                 }
             }
         }
     }

     /// <summary>
     /// Fills the distance property for all airports
     /// </summary>
     public ObservableCollection<Airport> FillDistances()
     {
         ObservableCollection<Airport> airports = db.SelectAllWiAirports();
         //for each of this airport, calculate the distance to every other airport
         foreach(Airport airport in airports)
         {
             foreach(Airport otherAirport in airports)
             {
                 //make sure we don't calculate distance from an airport to itself
                 if (airport.Id != otherAirport.Id)
                 {
                     airport.distances.Add(otherAirport.Id, CalculateDistance(airport, otherAirport));
                 }
             }
         }
         return airports;
     }

     /// <summary>
     /// Uses the Haversine formula to calculate the distance between two airports
     /// </summary>
     /// <param name="startingAirport"></param>
     /// <param name="otherAirport"></param>
     /// <returns></returns>
     public double CalculateDistance(Airport startingAirport, Airport otherAirport)
     {
         double lat1 = startingAirport.Latitude;
         double lon1 = startingAirport.Longitude;
         double lat2 = otherAirport.Latitude;
         double lon2 = otherAirport.Longitude;

         var R = 6371; // Radius of the Earth in kilometers
         var dLat = ToRadians(lat2 - lat1);
         var dLon = ToRadians(lon2 - lon1);
         var rLat1 = ToRadians(lat1);
         var rLat2 = ToRadians(lat2);

         var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                 Math.Cos(rLat1) * Math.Cos(rLat2) *
                 Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
         var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
         return R * c; // Distance in kilometers
     }

     /// <summary>
     /// used by CalculateDistance() to convert degrees to radians
     /// </summary>
     /// <param name="angle"></param>
     /// <returns></returns>
     private double ToRadians(double angle)
     {
         return Math.PI * angle / 180.0;
     }
}
