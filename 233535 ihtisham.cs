using System;
using System.Collections.Generic;

public abstract class User
{
    public string Name { get; set; }
    public string PhoneNo { get; set; }

    public void Register()
    {
        Console.Write("Enter your name: ");
        Name = Console.ReadLine();
        Console.Write("Enter your phone number: ");
        PhoneNo = Console.ReadLine();
    }

    public void DisplayProfile()
    {
        Console.WriteLine($"Name: {Name}, Phone: {PhoneNo}");
    }
}

public class Rider : User
{
    public List<Trip> RideHistory { get; private set; } = new List<Trip>();

    public void RequestRide(string startLocation, string destination)
    {
        Trip trip = new Trip { RiderName = Name, StartLocation = startLocation, Destination = destination, Status = "Requested" };
        RideHistory.Add(trip);
        Console.WriteLine("Ride request successfully!");
    }

    public void ViewRideHistory()
    {
        Console.WriteLine("Your Ride History:");
        for (int i = 0; i < RideHistory.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Trip ID: {i + 1}, From: {RideHistory[i].StartLocation}, To: {RideHistory[i].Destination}, Status: {RideHistory[i].Status}");
        }
    }
}

public class Driver : User
{
    public bool IsAvailable { get; set; } = true;

    public void AcceptRide(Trip trip)
    {
        if (IsAvailable && trip.Status == "Requested")
        {
            trip.DriverName = Name;
            trip.Status = "Accepted";
            Console.WriteLine($"Driver {Name} accepted the ride request from {trip.RiderName}.");
            IsAvailable = false; 
        }
        else
        {
            Console.WriteLine("Driver is not available or trip is not requested.");
        }
    }

    public void ViewTripHistory(List<Trip> allTrips)
    {
        Console.WriteLine("Driver's Trip History:");
        foreach (var trip in allTrips)
        {
            if (trip.DriverName == Name)
            {
                Console.WriteLine($"Trip ID: {trip.RiderName}, From: {trip.StartLocation}, To: {trip.Destination}, Status: {trip.Status}");
            }
        }
    }
}

public class Trip
{
    public string RiderName { get; set; }
    public string DriverName { get; set; }
    public string StartLocation { get; set; }
    public string Destination { get; set; }
    public string Status { get; set; }

    public void CompleteTrip()
    {
        Status = "Completed";
        Console.WriteLine($"Trip from {StartLocation} to {Destination} has been completed.");
    }

    public void DisplayTripDetails()
    {
        Console.WriteLine($"Rider: {RiderName}, Driver: {DriverName}, From: {StartLocation}, To: {Destination}, Status: {Status}");
    }
}

// RideSharingSystem class to manage the overall system
public class RideSharingSystem
{
    public List<Rider> RegisteredRiders { get; private set; } = new List<Rider>();
    public List<Driver> RegisteredDrivers { get; private set; } = new List<Driver>();
    public List<Trip> AllTrips { get; private set; } = new List<Trip>();

    public void RegisterRider()
    {
        Rider rider = new Rider();
        rider.Register();
        RegisteredRiders.Add(rider);
        Console.WriteLine("Registered successfully as a rider!");
    }

    public void RegisterDriver()
    {
        Driver driver = new Driver();
        driver.Register();
        RegisteredDrivers.Add(driver);
        Console.WriteLine("Registered successfully as a driver!");
    }

    public void RequestRide()
    {
        Console.Write("Enter your name: ");
        string riderName = Console.ReadLine();
        Rider rider = RegisteredRiders.Find(r => r.Name.Equals(riderName, StringComparison.OrdinalIgnoreCase));

        if (rider != null)
        {
            Console.Write("Enter Start Location: ");
            string startLocation = Console.ReadLine();
            Console.Write("Enter Destination: ");
            string destination = Console.ReadLine();
            rider.RequestRide(startLocation, destination);
            AllTrips.Add(new Trip { RiderName = rider.Name, StartLocation = startLocation, Destination = destination, Status = "Requested" });
        }
        else
        {
            Console.WriteLine("Rider not found.");
        }
    }

    public void AcceptRide()
    {
        Console.Write("Enter Driver Name: ");
        string driverName = Console.ReadLine();
        Driver driver = RegisteredDrivers.Find(d => d.Name.Equals(driverName, StringComparison.OrdinalIgnoreCase));

        if (driver != null)
        {
            foreach (var trip in AllTrips)
            {
                if (trip.Status == "Requested" && string.IsNullOrEmpty(trip.DriverName)) // If trip has no driver assigned
                {
                    driver.AcceptRide(trip);
                    return;
                }
            }
            Console.WriteLine("No pending ride requests found.");
        }
        else
        {
            Console.WriteLine("Driver not found.");
        }
    }

    public void CompleteTrip()
    {
        Console.Write("Enter Rider Name to complete the trip: ");
        string riderName = Console.ReadLine();
        Trip trip = AllTrips.Find(t => t.RiderName.Equals(riderName, StringComparison.OrdinalIgnoreCase) && t.Status == "Accepted");

        if (trip != null)
        {
            trip.CompleteTrip();
            trip.Status = "Completed"; // Update the trip status
            Console.WriteLine("Trip marked as completed.");
            // Reset the driver's availability
            Driver driver = RegisteredDrivers.Find(d => d.Name.Equals(trip.DriverName, StringComparison.OrdinalIgnoreCase));
            if (driver != null) driver.IsAvailable = true;
        }
        else
        {
            Console.WriteLine("No accepted trip found for this rider.");
        }
    }

    public void ViewAllTrips()
    {
        Console.WriteLine("All Trips:");
        foreach (var trip in AllTrips)
        {
            trip.DisplayTripDetails();
        }
    }
}

// Main Program
class Program
{
    static void Main(string[] args)
    {
        RideSharingSystem system = new RideSharingSystem();

        while (true)
        {
            Console.WriteLine("\n=== Ride Sharing System Menu ===");
            Console.WriteLine("1. Register as Rider");
            Console.WriteLine("2. Register as Driver");
            Console.WriteLine("3. Request a Ride");
            Console.WriteLine("4. Accept a Ride");
            Console.WriteLine("5. Complete Ride");
            Console.WriteLine("6. View Ride History");
            Console.WriteLine("7. View Trip History");
            Console.WriteLine("8. Display All Trips");
            Console.WriteLine("9. Exit");
            Console.Write("Please choose an option: ");
            int option = int.Parse(Console.ReadLine());

            switch (option)
            {
                case 1:
                    system.RegisterRider();
                    break;
                case 2:
                    system.RegisterDriver();
                    break;
                case 3:
                    system.RequestRide();
                    break;
                case 4:
                    system.AcceptRide();
                    break;
                case 5:
                    system.CompleteTrip();
                    break;
                case 6:
                    Console.Write("Enter your name to view ride history: ");
                    string riderName = Console.ReadLine();
                    Rider rider = system.RegisteredRiders.Find(r => r.Name.Equals(riderName, StringComparison.OrdinalIgnoreCase));
                    rider?.ViewRideHistory();
                    break;
                case 7:
                    Console.Write("Enter your name to view trip history: ");
                    string driverName = Console.ReadLine();
                    Driver driver = system.RegisteredDrivers.Find(d => d.Name.Equals(driverName, StringComparison.OrdinalIgnoreCase));
                    driver?.ViewTripHistory(system.AllTrips);
                    break;
                case 8:
                    system.ViewAllTrips();
                    break;
                case 9:
                    return; // Exit the application
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
