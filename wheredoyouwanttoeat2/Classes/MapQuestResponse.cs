using System.Collections.Generic;

namespace wheredoyouwanttoeat2.Classes
{
    public class MapQuestResponse
    {
        public MapQuestResponseInfo info { get; set; }
        public MapQuestResponseOptions options { get; set; }
        public List<MapQuestResponseResult> results { get; set; }
    }

    public class MapQuestResponseInfo
    {
        public int statuscode { get; set; }
    }

    public class MapQuestResponseInfoCopyright
    {
        public string text { get; set; }
        public string imageUrl { get; set; }
        public string imageAltText { get; set; }
    }

    public class MapQuestResponseOptions
    {
        public int maxResults { get; set; }
        public bool thumbMaps { get; set; }
        public bool ignoreLatLngInput { get; set; }
    }

    public class MapQuestResponseResult
    {
        public List<MapQuestResponseResultLocation> locations { get; set; }
    }

    public class MapQuestResponseResultLocation
    {
        public string street { get; set; }
        public string adminArea6 { get; set; }
        public string adminArea6Type { get; set; }
        public string adminArea5 { get; set; }
        public string adminArea5Type { get; set; }
        public string adminArea4 { get; set; }
        public string adminArea4Type { get; set; }
        public string adminArea3 { get; set; }
        public string adminArea3Type { get; set; }
        public string adminArea1 { get; set; }
        public string adminArea1Type { get; set; }
        public string postalCode { get; set; }
        public string geocodeQualityCode { get; set; }
        public string geocodeQuality { get; set; }
        public bool dragPoint { get; set; }
        public string sideOfStreet { get; set; }
        public string linkId { get; set; }
        public string unknownInput { get; set; }
        public string type { get; set; }
        public MapQuestResponseResultLocationLatLng latLng { get; set; }
        public MapQuestResponseResultLocationLatLng displayLatLng { get; set; }
        public string mapUrl { get; set; }
    }

    public class MapQuestResponseResultLocationLatLng
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }
}