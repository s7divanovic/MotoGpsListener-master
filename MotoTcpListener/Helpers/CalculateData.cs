using System;

namespace MotoTcpListener.Lib
{
    public class CalculateData
    {
		public static decimal DegreeToDecimal(decimal coordinates_in_degrees, string direction)
        {
            int degrees = (int)(coordinates_in_degrees / 100);
			decimal minutes = coordinates_in_degrees - (degrees * 100);
			decimal seconds = minutes / 60;
			decimal coordinates_in_decimal = degrees + seconds;

            if(direction == "S" || direction == "W")
            {
                coordinates_in_decimal = coordinates_in_decimal * (-1);
            }

			return Decimal.Round(coordinates_in_decimal, 10);
        }
    }
}
