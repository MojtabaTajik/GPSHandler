using System;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace GPSHandler
{
    public class GpsHandler
    {
        public Action<string, string> GeoLocation;
        private readonly SerialPort _port;

        public GpsHandler(string comPort)
        {
            if (string.IsNullOrEmpty(comPort))
                throw new ArgumentNullException(comPort);

            _port = new SerialPort
            {
                PortName = comPort,
                BaudRate = 4800,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One
            };

            _port.DataReceived += (sender, args) =>
            {
                try
                {
                    var buffer = new byte[_port.BytesToRead];

                    _port.Read(buffer, 0, buffer.Length);

                    var gpsData = Encoding.ASCII.GetString(buffer);
                    if (string.IsNullOrEmpty(gpsData))
                        return;

                    string[] gpsDataLines = gpsData.Split(Environment.NewLine);
                    string latLonLoc = gpsDataLines.LastOrDefault(w => w.StartsWith("$GPGLL"));

                    if (latLonLoc == null)
                        return;

                    string[] splitedLatLon = latLonLoc.Split(",");
                    if (splitedLatLon.Length < 8)
                        return;
                    
                    // Calculate lat string based on GLL
                    string latString = splitedLatLon?[1];
                    string latHemisphereString = splitedLatLon?[2];
                    string calculatedLat = NmeaHelper.NmeaToDecDeg(latString, latHemisphereString);

                    // Calculate lon string based on GLL
                    string lonString = splitedLatLon?[3];
                    string lonHemisphereString = splitedLatLon?[4];
                    string calculatedLon = NmeaHelper.NmeaToDecDeg(lonString, lonHemisphereString);

                    GeoLocation?.Invoke(calculatedLat, calculatedLon);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            };
        }

        public void Start()
        {
            _port.Open();
        }

        public void Stop()
        {
            _port.Close();
        }
    }
}