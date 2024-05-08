namespace DataCollector.Helpers
{
    public class Helper
    {
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1); // deg2rad below
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
            ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in km
            return d;
        }

        public static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        public static double CalculateDistance(string lat1, string lon1, string lat2, string lon2)
        {
           var d= CalculateDistance(Convert.ToDouble(lat1), Convert.ToDouble(lon1), Convert.ToDouble(lat2), Convert.ToDouble(lon2));
            return d;
        }


    }
}
