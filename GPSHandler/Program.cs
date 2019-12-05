using System;
using System.IO.Ports;
using System.Linq;

namespace GPSHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var availableComPorts = SerialPort.GetPortNames();

            availableComPorts.ToList().ForEach(Console.WriteLine);

            Console.WriteLine();
            Console.WriteLine("Enter port to start GPS handler :");
            var comPort = Console.ReadLine()?.ToUpper();

            if (!availableComPorts.Contains(comPort))
            {
                Console.WriteLine("Not valid COM port");
                Console.Read();
                return;
            }
            Console.Clear();

            var gps = new GpsHandler(comPort);
            try
            {
                gps.GeoLocation += OnNewPosition;
                gps.Start();

                Console.Read();
                gps.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void OnNewPosition(string lat, string lon)
        {
            Console.WriteLine($"https://www.google.com/maps/@{lat},{lon},19z");
        }
    }
}