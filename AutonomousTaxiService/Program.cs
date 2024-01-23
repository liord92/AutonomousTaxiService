
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

Dictionary<int, Detailes> Taxi = new Dictionary<int, Detailes>();
Queue<Distance> ordering_list = new Queue<Distance>();
Task<bool> task = Orders();
AddTaxiToList();
Main();
void Main()
{
    Distance order = new Distance();
    while (true)
    {
        if (ordering_list.Count > 0 && CheckTaxisAvailable())
        {
            order = ordering_list.Dequeue();
            int taxi = GetMinDistance(order);
            Task Change = ChangeTaxiStatus(taxi, order);
            Console.WriteLine("--> Taxi " + taxi + " taking an order: START POINT " + order.Start.X + "km, " + order.Start.Y + "km TO END POINT " + order.End.X + "km, " + order.End.Y + "km\n");
            PrintTaxi();
        }
    }
}
bool CheckTaxisAvailable()
{
    foreach (var taxi in Taxi)
    {
        if (taxi.Value.Status == (int)Status.standing)
        {
            return true;
        }
    }
    return false;
}
async Task ChangeTaxiStatus(int taxi, Distance order)
{
    Taxi[taxi].Status = (int)Status.driving;
    double d_TAXI_TO_START_ORDER = StepCalculator(Taxi[taxi].location, order.Start);
    double d_START_TO_END = StepCalculator(order.Start, order.End);
    double time = (d_TAXI_TO_START_ORDER + d_START_TO_END) / 72;
    await Task.Delay((int)(time * 1000));
    Taxi[taxi].Status = (int)Status.standing;
    UpdateLocationTaxi(taxi, order);
}
void UpdateLocationTaxi(int taxi, Distance order)
{
    if (Taxi.TryGetValue(taxi, out Detailes value))
    {
        Taxi[taxi].location = order.End;
    }
}
async Task<bool> Orders()
{
    bool succeeded = true;
    while (succeeded)
    {
        succeeded = CreateOrder();
        await Task.Delay(20000);
        PrintOrder();
    }
    return true;
}
void PrintOrder()
{
    Console.WriteLine("Orders List: ");
    if (ordering_list.Count > 0)
    {
        foreach (var order in ordering_list)
        {
            Console.WriteLine("ORDER: START POINT - " + order.Start.X + "km , " + order.Start.Y + "km END POINT - " + order.End.X + "km, " + order.End.Y + "km");
        }
    }
    else
    {
        Console.WriteLine("Empty");
    }
}
bool CreateOrder()
{
    double max = 19;
    double min = 0;
    try
    {
        bool succeeded = false;
        Distance distance = new Distance();
        distance.Start = new Point();
        distance.End = new Point();
        while (!succeeded)
        {
            distance.Start.X = GetRandomNumber(min, max);
            distance.Start.Y = GetRandomNumber(min, max);
            distance.End.X = GetRandomNumber(min, max);
            distance.End.Y = GetRandomNumber(min, max);
            if (CheckDistance2KM(distance))
            {
                succeeded = true;
                ordering_list.Enqueue(distance);
                Console.WriteLine("NEW ORDER: START POINT - " + distance.Start.X + "km," + distance.Start.Y + "km END POINT - " + distance.End.X + "km," + distance.End.Y + "km");
            }
        }
        return true;
    }
    catch (Exception)
    {
        return false;
    }
}
bool CheckDistance2KM(Distance d)
{
    if (StepCalculator(d.Start, d.End) <= 2)
    {
        return true;
    }
    else return false;
}
double GetRandomNumber(double min, double max)
{
    Random random = new Random();
    return Math.Round(Math.Abs(random.NextDouble() * (max - min) + min), 1);
}
void AddTaxiToList()
{
    Random rnd = new Random();
    int name = 1;
    for (int i = 0; i < 10; i++)
    {
        Detailes detailes = new Detailes();
        Point t = new Point();
        t.X = GetRandomNumber(0, 20);
        t.Y = GetRandomNumber(0, 20);
        detailes.location = t;
        detailes.Status = (int)Status.standing;
        Taxi.Add(name, detailes);
        name++;
    }
    Console.WriteLine("Initial taxi locations:");
    PrintTaxi();
}
void PrintTaxi()
{
    foreach (var taxi in Taxi)
    {
        Status status = (Status)taxi.Value.Status;
        Console.WriteLine("Taxi " + taxi.Key + " location: " + taxi.Value.location.X + "km," + taxi.Value.location.Y + "km (" + status + ")");
    }
    Console.WriteLine("\n");
}
int GetMinDistance(Distance d)
{
    int min_name_taxi = 0;
    double dis_min = 100;
    foreach (var t in Taxi)
    {
        if (t.Value.Status == (int)Status.standing)
        {
            double temp = StepCalculator(d.Start, t.Value.location);
            if (temp < dis_min)
            {
                dis_min = temp;
                min_name_taxi = t.Key;
            }
        }
    }
    return min_name_taxi;
}
double StepCalculator(Point first, Point second)
{
    double x_temp = Math.Abs(first.X - second.X);
    double y_temp = Math.Abs(first.Y - second.Y);
    return Math.Round((x_temp + y_temp), 1);
}
internal class Point
{
    public double X { get; set; }
    public double Y { get; set; }
}
internal class Distance
{
    public Point Start { get; set; }
    public Point End { get; set; }
}
internal class Detailes
{
    public Point location { get; set; }
    public int Status { get; set; }
}
enum Status
{
    driving = 1,
    standing
}