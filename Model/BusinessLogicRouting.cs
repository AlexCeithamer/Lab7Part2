namespace Lab6_Starter.Model;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Lab6_Starter.Model;

public partial class BusinessLogic : IBusinessLogic, INotifyPropertyChanged
{
    public ObservableCollection<Airport> WisconsinAirports
    {
        get { return GetWisconsinAirports(); }
    }

    public ObservableCollection<Airport> GetWisconsinAirports()
    {
        return db.SelectAllWiAirports();
    }
    public Airport FindWisconsinAirport(String id)
    {
        if (db.SelectWisconsinAirport(id) == null)
        {
            return null;
        }
        return db.SelectWisconsinAirport(id);
    }

    /// <summary>
    /// Calculates all possible routes from a starting airport within a given distance
    /// </summary>
    /// <param name="id"></param>
    /// <param name="maxDist"></param>
    /// <param name="isVisited"></param>
    /// <returns></returns>
    public Route CalculateRoute(string id, int maxDist, bool isVisited)
    {
        //find the starting airport
        Airport startingAirport = FindWisconsinAirport(id);

        //if the starting airport is null or not in wisconsin, return an empty collection
        if (startingAirport == null)
        {
            return new Route();
        }

        //get list of all airports and get the distance from the starting airport to each airport
        ObservableCollection<Airport> airports = db.SelectAllWiAirports();
        airports =  FillDistances(airports, startingAirport, maxDist, true);

        //get a list of the airports in the radius and that match the isVisited parameter
        ObservableCollection<Airport> airportsInRadius = new ObservableCollection<Airport>() { startingAirport };
        foreach (Airport airport in airports)
        {
            bool airportVisited = airport.DateVisited != DateTime.MinValue;
            if (airport.Id != startingAirport.Id &&
                startingAirport.distances[airport.Id] <= maxDist &&
                airportVisited == isVisited)
            {
                airportsInRadius.Add(airport);
            }
        }

        //calculate all distances for each airport now that we have a list of airports to consider
        foreach (Airport airport in airportsInRadius)
        {
            airportsInRadius = FillDistances(airportsInRadius, airport, maxDist, false);
        }

        // Call ExploreRoutes to find the shortest path
        return ExploreRoutes(startingAirport, airportsInRadius, maxDist);
    }

    /// <summary>
    /// Iterates over the airports, using the FindNearestAirports helper method to find the closest unvisited
    /// airport. Once all airports have been visited, the method closes by returning to the original airport
    /// and returns the route.
    /// 
    /// uses the Nearest Neighbor algorithm
    /// </summary>
    /// <param name="startingAirport"></param>
    /// <param name="airports"></param>
    /// <param name="maxDistance"></param>
    /// <returns></returns>
    private Route ExploreRoutes(Airport startingAirport, ObservableCollection<Airport> airports, int maxDistance)
    {
        // A temporary variable to store the current airport
        Airport currentAirport = startingAirport;

        // We use a HashSet to keep track of visited airports
        HashSet<string> visitedAirports = new HashSet<string>();

        // Start constructing the route
        ObservableCollection<Airport> routeAirports = new ObservableCollection<Airport>();
        routeAirports.Add(currentAirport);

        visitedAirports.Add(currentAirport.Id);

        while (visitedAirports.Count < airports.Count)
        {
            Airport nearestAirport = FindNearestAirport(currentAirport, airports, visitedAirports);

            if (nearestAirport == null)
                break; // No further airport found within maxDistance

            //update airport lists
            routeAirports.Add(nearestAirport);
            visitedAirports.Add(nearestAirport.Id);

            // Set distance variable the indicates the distance to get to this airport from the previous airport
            nearestAirport.DistanceFromNextAirport = (int)currentAirport.GetDistance(nearestAirport.Id);

            currentAirport = nearestAirport;
        }

        // Add the starting airport to complete the loop
        Airport lastAirport = new Airport(startingAirport.Id, startingAirport.UserId, startingAirport.City, startingAirport.DateVisited, startingAirport.Rating);
        lastAirport.DistanceFromNextAirport = (int)currentAirport.GetDistance(startingAirport.Id);
        lastAirport.Latitude = startingAirport.Latitude;
        lastAirport.Longitude = startingAirport.Longitude;
        routeAirports.Add(lastAirport);

        // return a Route object
        return new Route(routeAirports);
    }

    /// <summary>
    /// Private helper method for ExploreRoutes. Finds the nearest airport to the current airport
    /// </summary>
    /// <param name="currentAirport"></param>
    /// <param name="airports"></param>
    /// <param name="visitedAirports"></param>
    /// <returns></returns>
    private Airport FindNearestAirport(Airport currentAirport, ObservableCollection<Airport> airports, HashSet<string> visitedAirports)
    {
        Airport nearest = null;
        double minDistance = double.MaxValue;

        foreach (Airport airport in airports)
        {
            if (!visitedAirports.Contains(airport.Id))
            {
                double distance = currentAirport.GetDistance(airport.Id);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = airport;
                }
            }
        }

        return nearest;
    }



    /// <summary>
    /// Fills the distance property for all airports or just the starting airport
    /// </summary>
    /// <param name="airports"></param>
    /// <param name="startingAirport"></param>
    /// <param name="maxDistance"></param>
    /// <param name="onlyStartingAirport"></param>
    /// <returns></returns>
    public ObservableCollection<Airport> FillDistances(ObservableCollection<Airport> airports, Airport startingAirport, int maxDistance, bool onlyStartingAirport)
     {
        //run this if we only want to fill the distances for the starting airport
        if (onlyStartingAirport)
        {
            //Fill distance to each airport from our starting airport
            foreach (Airport airport in airports)
            {
                if (airport.Id != startingAirport.Id)
                {
                    startingAirport.distances.Add(airport.Id, CalculateDistance(startingAirport, airport));
                }
            }
        }
        //run this if we want to fill the distances for every airport (should only be passing in airports in the radius)
        else
        {
            foreach (Airport airport in airports)
            {
                //clear the distances dictionary so we don't have any old data
                if (airport.distances != null || airport.distances.Count > 0)
                {
                    airport.distances.Clear();
                }
                //calculate distance to other airports
                foreach (Airport otherAirport in airports)
                {
                    //make sure we don't calculate distance from an airport to itself
                    if (airport.Id != otherAirport.Id)
                    {
                        airport.distances.Add(otherAirport.Id, CalculateDistance(airport, otherAirport));
                    }
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
    public int CalculateDistance(Airport startingAirport, Airport otherAirport)
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
         return (int)Math.Round(R * c); // Distance in kilometers
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
