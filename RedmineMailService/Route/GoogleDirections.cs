
namespace RedmineMailService.CertSSL
{


    public class GeocodedWaypoint
    {
        public string geocoder_status { get; set; }
        public string place_id { get; set; }
        public System.Collections.Generic.List<string> types { get; set; }
    }


    public class LatLng
    {
        public decimal lat { get; set; }
        public decimal lng { get; set; }
    }


    public class Bounds
    {
        public LatLng northeast { get; set; }
        public LatLng southwest { get; set; }
    }


    public class TextValuePair
    {
        public string text { get; set; }
        public int value { get; set; }
    }
    

    public class Polyline
    {
        public string points { get; set; }
    }


    public class Step
    {
        public TextValuePair distance { get; set; }
        public TextValuePair duration { get; set; }
        public LatLng end_location { get; set; }
        public string html_instructions { get; set; }
        public Polyline polyline { get; set; }
        public LatLng start_location { get; set; }
        public string travel_mode { get; set; }
        public string maneuver { get; set; }
    }


    public class Leg
    {
        public TextValuePair distance { get; set; }
        public TextValuePair duration { get; set; }
        public string end_address { get; set; }
        public LatLng end_location { get; set; }
        public string start_address { get; set; }
        public LatLng start_location { get; set; }
        public System.Collections.Generic.List<Step> steps { get; set; }
        public System.Collections.Generic.List<object> traffic_speed_entry { get; set; }
        public System.Collections.Generic.List<object> via_waypoint { get; set; }
    }


    public class OverviewPolyline
    {
        public string points { get; set; }
    }


    public class Route
    {
        public Bounds bounds { get; set; }
        public string copyrights { get; set; }
        public System.Collections.Generic.List<Leg> legs { get; set; }
        public OverviewPolyline overview_polyline { get; set; }
        public string summary { get; set; }
        public System.Collections.Generic.List<object> warnings { get; set; }
        public System.Collections.Generic.List<object> waypoint_order { get; set; }
    }

    // https://developers.google.com/maps/documentation/directions/intro#TravelModes
    // http://maps.googleapis.com/maps/api/directions/outputFormat?parameters
    // https://maps.googleapis.com/maps/api/directions/json?origin=Toronto&destination=Montreal&key=YOUR_API_KEY
    // origin=24+Sussex+Drive+Ottawa+ON
    // departure_time
    // arrival_time
    // You can specify the time as an integer in seconds since midnight, January 1, 1970 UTC.
    // Alternatively, you can specify a value of "now"

    // traffic_model(defaults to best_guess)
    // best_guess
    // pessimistic
    // optimistic


    //&mode=
    //driving
    //walking
    //bicycling
    //transit

    // https://maps.googleapis.com/maps/api/directions/json?
    // origin=Boston,MA&destination=Concord,MA
    // &waypoints=via:Charlestown,MA|via:Lexington,MA
    // &departure_time=now
    // &key=YOUR_API_KEY



    // https://maps.googleapis.com/maps/api/directions/json?origin=Toronto&destination=Montreal&mode=transit&departure_time=1542790175&key=YOUR_API_KEY
    // https://maps.googleapis.com/maps/api/directions/json?origin=Toronto&destination=Montreal&mode=transit&departure_time=1542790175&key=AIzaSyBhQ8oF-u8f050rdwnlZSIERvCT6HZfo-g


    internal static class DateTimeHelper
    {
        private static readonly System.DateTime Jan1st1970 = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);


        public static long CurrentUnixTime()
        {
            return (long)(System.DateTime.UtcNow - Jan1st1970).TotalSeconds;
        }


        public static long ToUnixTime(System.DateTime dateTime)
        {
            return (long)(dateTime.ToUniversalTime() - Jan1st1970).TotalSeconds;
        }
        

        public static long ToUnixTime(int hour, int minute, int second)
        {
            System.DateTime thisTime = new System.DateTime(
                  System.DateTime.Now.Year
                , System.DateTime.Now.Month 
                , System.DateTime.Now.Day
                , hour, minute, second
                , System.DateTimeKind.Local
            );

            return ToUnixTime(thisTime);
        }

        // , System.DateTimeKind kind
        public static long ToUnixTime(int year, int month, int day, int hour, int minute, int second)
        {
            System.DateTime thisTime = new System.DateTime(year, month, day, hour, minute, second, System.DateTimeKind.Local);
            
            return ToUnixTime(thisTime);
        }
    }


    public class DirectionsUrlBuilder
    {
        // https://maps.googleapis.com/maps/api/directions/json?origin=Toronto&destination=Montreal&mode=transit&departure_time=1542790175&key=YOUR_API_KEY
        public string BaseUrl { get; set; }

        public enum TransitMode_t
        {
            Walking,
            Bicycling,
            Transit,
            Driving
        }


        protected string m_format;
        public string Format
        {
            get { return System.Uri.UnescapeDataString(this.m_format); }
            set { this.m_format = System.Uri.EscapeDataString(value); }
        }

        protected string m_origin;
        public string Origin
        {
            get { return System.Uri.UnescapeDataString(this.m_origin); }
            set { this.m_origin = System.Uri.EscapeDataString(value); }
        }

        protected string m_destination;
        public string Destination
        {
            get { return System.Uri.UnescapeDataString(this.m_destination); }
            set { this.m_destination = System.Uri.EscapeDataString(value); }
        }

        public TransitMode_t Mode { get; set; }

        public long Departure_time { get; set; }

        protected string m_key;
        public string Key
        {
            get { return System.Uri.UnescapeDataString(this.m_key); }
            set { this.m_key = System.Uri.EscapeDataString(value); }
        }


        public DirectionsUrlBuilder()
        {
            this.BaseUrl = "https://maps.googleapis.com/maps/api/directions/";
            this.Format = "json";
            this.Origin = "Basel SBB";
            this.Destination = "Zürich HB";
            this.Mode = TransitMode_t.Transit;
            this.Departure_time = DateTimeHelper.ToUnixTime(7, 0, 0);
            this.Key = RedmineMailService.Trash.UserData.GoogleDirectionsApiKey;
        }


        public override string ToString()
        {
            string mode = System.Uri.EscapeDataString( this.Mode.ToString().ToLowerInvariant());
            return $"{this.BaseUrl}{this.m_format}?origin={this.m_origin}&destination={this.m_destination}&mode={mode}&departure_time={this.Departure_time}&key={this.m_key}";


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Type t = this.GetType();
            System.Reflection.FieldInfo[] fis = t.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            sb.Append(this.BaseUrl);
            sb.Append(this.Format);
            sb.Append("?");

            for(int i = 0; i < fis.Length; ++i)
            {
                if(i!= 0)
                    sb.Append("&");

                sb.Append(fis[i].Name.ToLowerInvariant().Substring(2));
                sb.Append("=");

                string value = System.Convert.ToString(fis[i].GetValue(this));
                sb.Append(System.Uri.EscapeDataString(value));
            }

            string retValue = sb.ToString();
            sb.Clear();
            sb = null;
            return retValue;
        }

        
    }



    public class GoogleDirectionsReturnValue
    {
        public System.Collections.Generic.List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public System.Collections.Generic.List<Route> routes { get; set; }
        public string status { get; set; } // "OVER_QUERY_LIMIT", "OVER_DAILY_LIMIT", "REQUEST_DENIED"
        public string error_message { get; set; }


        /*
        // https://developers.google.com/maps/documentation/javascript/directions
        OK indicates the response contains a valid DirectionsResult.
        NOT_FOUND indicates at least one of the locations specified in the request's origin, destination, or waypoints could not be geocoded.
        ZERO_RESULTS indicates no route could be found between the origin and destination.
        MAX_WAYPOINTS_EXCEEDED indicates that too many DirectionsWaypoint fields were provided in the DirectionsRequest. See the section below on limits for way points.
        MAX_ROUTE_LENGTH_EXCEEDED indicates the requested route is too long and cannot be processed. This error occurs when more complex directions are returned. Try reducing the number of waypoints, turns, or instructions.
        INVALID_REQUEST indicates that the provided DirectionsRequest was invalid. The most common causes of this error code are requests that are missing either an origin or destination, or a transit request that includes waypoints.
        OVER_QUERY_LIMIT indicates the webpage has sent too many requests within the allowed time period.
        OVER_DAILY_LIMIT: The webpage has gone over the daily limit of its request quota.
        REQUEST_DENIED indicates the webpage is not allowed to use the directions service.
        UNKNOWN_ERROR indicates a directions request could not be processed due to a server error. The request may succeed if you try again.

        ERROR: There was a problem contacting the Google servers.
        */

        public static GoogleDirectionsReturnValue FromJson(string json)
        {
            GoogleDirectionsReturnValue retValue = Newtonsoft.Json.JsonConvert.DeserializeObject<GoogleDirectionsReturnValue>(json, Converter.Settings);

            if (!"OK".Equals(retValue.status, System.StringComparison.OrdinalIgnoreCase))
                throw new GoogleDirectionsException(retValue);

            return retValue;
        }


        public static GoogleDirectionsReturnValue GetDirections(DirectionsUrlBuilder urlBuilder)
        {
            GoogleDirectionsReturnValue retVal = null;

            string url = urlBuilder.ToString();
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                string result = wc.DownloadString(url);
                retVal = GoogleDirectionsReturnValue.FromJson(result);
            }

            return retVal;
        }



        public static void Test()
        {
            DirectionsUrlBuilder url = new RedmineMailService.CertSSL.DirectionsUrlBuilder();
        }


    } // End Class GoogleDirectionsReturnValue 


    public class GoogleDirectionsException
        : System.Exception
    {
        public GoogleDirectionsException()
            : base()
        { }
        public GoogleDirectionsException(string message)
            : base(message)
        { }
        public GoogleDirectionsException(string message, System.Exception innerException)
            : base(message, innerException)
        { }


        public GoogleDirectionsException(GoogleDirectionsReturnValue api_result)
            : this(api_result.status + ": " + api_result.error_message)
        { }

    } // End Class GoogleDirectionsException


    internal static class Converter
    {
        public static readonly Newtonsoft.Json.JsonSerializerSettings Settings = 
            new Newtonsoft.Json.JsonSerializerSettings
            {
                MetadataPropertyHandling = Newtonsoft.Json.MetadataPropertyHandling.Ignore,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                Converters =
                {
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter {
                        DateTimeStyles = System.Globalization.DateTimeStyles.AssumeUniversal
                    }
                },
            };
    }


}

