
namespace Microsoft.Exchange.WebServices.Data.TimeZoneData
{


    // used in Microsoft.Exchange.WebServices.Data\ComplexProperties\TimeZones\TimeZoneDefinition.cs
    internal class TimeZoneTranslator
    {
        private static WindowsZonesMap s_zm;


        static TimeZoneTranslator()
        {
            s_zm = WindowsZonesMap.FromEmbeddedRessource("windowsZones.xml");
        }



        private static string WindowsToLinux(string windowsTimeZoneName, string regionCode)
        {
            foreach (MapZone mapZone in s_zm.WindowsZones.MapTimezones.MapZone)
            {
                if (string.Equals(mapZone.Other, windowsTimeZoneName, System.StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(mapZone.Territory, regionCode, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return mapZone.Type;
                } // End if timezonematch 

            } // Next mapZone 

            // If none with region, use default 
            foreach (MapZone mapZone in s_zm.WindowsZones.MapTimezones.MapZone)
            {
                if (string.Equals(mapZone.Other, windowsTimeZoneName, System.StringComparison.InvariantCultureIgnoreCase)
                    && string.Equals(mapZone.Territory, "001", System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return mapZone.Type;
                } // End if timezonematch 

            } // Next mapZone 

            // if not found, return original 
            return windowsTimeZoneName;
        } // End Function WindowsToLinux 


        private static string WindowsToLinux(string windowsTimeZoneName)
        {
            string regionCode = System.Globalization.RegionInfo.CurrentRegion.TwoLetterISORegionName;
            return WindowsToLinux(windowsTimeZoneName, regionCode);
        } // End Function WindowsToLinux 


        private static string LinuxToWindows(string linuxTimeZoneName)
        {
            foreach (MapZone mapZone in s_zm.WindowsZones.MapTimezones.MapZone)
            {
                if (string.Equals(mapZone.Type, linuxTimeZoneName, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    return mapZone.Other;
                } // End if timezonematch 

            } // Next mapZone 

            // if not found, return original 
            return linuxTimeZoneName;
        } // End Function LinuxToWindows 


        public static string TranslateWrite(string timeZoneName)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return timeZoneName;

            return LinuxToWindows(timeZoneName);
        } // End Function TranslateWrite 


        public static string TranslateRead(string timeZoneName)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                return timeZoneName;

            return WindowsToLinux(timeZoneName);
        } // End Function TranslateRead 


    } // End Class TimeZoneTranslator 


} // End Namespace Microsoft.Exchange.WebServices.Data.TimeZoneData
