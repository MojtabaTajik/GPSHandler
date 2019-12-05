using System;
using System.Globalization;

namespace GPSHandler
{
    public static class NmeaHelper
    {
        public static string NmeaToDecDeg(string nmeaPosition, string hemisphere)
        {
            int inx = nmeaPosition.IndexOf(".", StringComparison.Ordinal);
            if (inx == -1)
            {
                return 0.ToString();
            }

            string minutesStr = nmeaPosition.Substring(inx - 2);
            double minutes = Double.Parse(minutesStr, new CultureInfo("en-US"));

            string degreesStr = nmeaPosition.Substring(0, inx - 2);
            double degrees = Convert.ToDouble(degreesStr) + minutes / 60.0;

            if (hemisphere.Equals("W") || hemisphere.Equals("S"))
            {
                degrees = -degrees;
            }

            return degrees.ToString(CultureInfo.InvariantCulture);
        }
    }
}