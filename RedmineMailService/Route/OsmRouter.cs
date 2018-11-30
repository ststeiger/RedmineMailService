
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
witter

namespace RedmineMailService.Route
{


    // Altstätten-Weinfelden
    // duration":3924.5,"distance":67010.7 (duration in seconds, distance in meters)
    // http://router.project-osrm.org/route/v1/driving/9.541277,47.378347;9.109754,47.566919?overview=false
    public partial class RootNode
    {
        [JsonProperty("routes")]
        public List<Route> Routes { get; set; }

        [JsonProperty("waypoints")]
        public List<Waypoint> Waypoints { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public partial class Route
    {
        [JsonProperty("legs")]
        public List<Leg> Legs { get; set; }

        [JsonProperty("weight_name")]
        public string WeightName { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }

    public partial class Leg
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("weight")]
        public double Weight { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("steps")]
        public List<object> Steps { get; set; }

        [JsonProperty("distance")]
        public double Distance { get; set; }
    }

    public partial class Waypoint
    {
        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("location")]
        public List<double> Location { get; set; }
    }
    
    
    public partial class RootNode
    {
        public static RootNode FromJson(string json)
        {
            return JsonConvert.DeserializeObject<RootNode>(json, Converter.Settings);
        }
    }
    
    
    public static class Serialize
    {
        public static string ToJson(this RootNode self)
        {
            return JsonConvert.SerializeObject(self, Converter.Settings);
        }
    }
    

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal }
            },
        };
    }
    
    
}
