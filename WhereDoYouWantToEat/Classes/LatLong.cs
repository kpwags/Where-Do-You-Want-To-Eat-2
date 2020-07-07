namespace WhereDoYouWantToEat2.Classes
{
    public class LatLong
    {
        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public LatLong(decimal lat = 0, decimal lng = 0)
        {
            Latitude = lat;
            Longitude = lng;
        }
    }
}