
#if (NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD1_8 || NETSTANDARD1_9 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETSTANDARD2_2)

namespace System.Drawing 
{

    // for \Search\GetUserPhotoResults.cs
    public class Image 
    {

        public System.IO.Stream Stream;

        //public static SixLabors.ImageSharp.Image<SixLabors.ImageSharp.Rgba32> FromStream(System.IO.Stream stream)
        //{
        //    SixLabors.ImageSharp.Image<SixLabors.ImageSharp.Rgba32> img = SixLabors.ImageSharp.Image.Load(stream);
        //    return img;
        //}

        public static Image FromStream(System.IO.Stream stream)
        {
            return new Image()
            {
                Stream = stream
            };
        }

    }

}

#endif


// https://github.com/OfficeDev/ews-managed-api
// from sherlock1982 
// https://github.com/sherlock1982/ews-managed-api
// to officedev/msft-default-repo with personal fixes 
// https://github.com/OfficeDev/ews-managed-api

// Fix: Novell.Directory.Ldap.NETStandard 
// https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard



// Fix TimeZoneTransitionGroup.cs (132, 151, 166)
// Dictionary.add(key, value) to dictionary[key] = value;

// Removed D:\github\Microsoft.Exchange.WebServices.Data\Autodiscover\DirectoryHelper.cs 
// Replaced with new dummy class 

//D:\github\Microsoft.Exchange.WebServices.Data\Core\Requests\SetUserPhotoRequest.cs
//- using System.Drawing.Imaging;



//D:\github\Microsoft.Exchange.WebServices.Data\Misc\SoapFaultDetails.cs
//- 
//using System.IO;
//using System.Net;
//using System.Reflection;
//using System.Text;
//using System.Web.Services.Protocols;


// D:\github\Microsoft.Exchange.WebServices.Data\Core\Requests\HangingServiceRequestBase.cs
//245:
//#if !(NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_7 || NETSTANDARD1_8 || NETSTANDARD1_9 || NETSTANDARD2_0 || NETSTANDARD2_1 || NETSTANDARD2_2)
//                catch (HttpException ex)
//                {
//                    // Stream is closed, so disconnect.
//                    this.Disconnect(HangingRequestDisconnectReason.Exception, ex);
//                    return;
//                }
//#endif


//D:\github\Microsoft.Exchange.WebServices.Data\Autodiscover\AutodiscoverService.cs
// Implement 
// private ICollection<string> DefaultGetScpUrlsForDomain(string domainName)
// is calling the DirectoryHelper-dummy-class

// /// <summary>
// /// Defaults the get autodiscover service urls for domain.
// /// </summary>
// /// <param name="domainName">Name of the domain.</param>
// /// <returns></returns>
// private ICollection<string> DefaultGetScpUrlsForDomain(string domainName)
// {
//     DirectoryHelper helper = new DirectoryHelper(this);
//     return helper.GetAutodiscoverScpUrlsForDomain(domainName);
// }
