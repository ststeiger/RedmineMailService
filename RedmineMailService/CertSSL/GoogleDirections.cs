
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


    public class GoogleDirectionsReturnValue 
    {
        public System.Collections.Generic.List<GeocodedWaypoint> geocoded_waypoints { get; set; }
        public System.Collections.Generic.List<Route> routes { get; set; }
        public string status { get; set; }
    }


}
