
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Security.Cryptography.X509Certificates;


// https://gist.github.com/ststeiger/aa7da5031e689fd66527af86ad962cc0
// https://dotnetcodr.com/2016/01/21/using-client-certificates-in-net-part-4-working-with-client-certificates-in-code/
namespace RedmineMailService.CertSSL
{


    public class Certificator
    {

        /// <summary>
        ///     Ensure certificates are setup (creates root if required).
        ///     Also makes root certificate trusted based on initial setup from proxy constructor for user/machine trust.
        /// </summary>
        public void EnsureRootCertificate()
        {
            X509Certificate2 rootCertificate = null;
            bool machineTrustRoot = false;

            if (rootCertificate == null)
            {
                rootCertificate = CreateRootCertificate();
            }

            TrustRootCertificate(rootCertificate, machineTrustRoot);
        }

        public static X509Certificate2 CreateRootCertificate()
        {
            return null;
        }


        /// <summary>
        ///     Trusts the root certificate in user store, optionally also in machine store.
        ///     Machine trust would require elevated permissions (will silently fail otherwise).
        /// </summary>
        public void TrustRootCertificate(X509Certificate2 cert, bool machineTrusted = false)
        {
            // currentUser\personal
            InstallCertificate(cert, StoreName.My, StoreLocation.CurrentUser);

            if (!machineTrusted)
            {
                // currentUser\Root
                InstallCertificate(cert, StoreName.Root, StoreLocation.CurrentUser);
            }
            else
            {
                // current system
                InstallCertificate(cert, StoreName.My, StoreLocation.LocalMachine);

                // this adds to both currentUser\Root & currentMachine\Root
                InstallCertificate(cert, StoreName.Root, StoreLocation.LocalMachine);
            }
        }


        private static X509Certificate Find(string serialNumber, StoreLocation location)
        {
            using (X509Store store = new X509Store(location))
            {
                store.Open(OpenFlags.OpenExistingOnly);
                X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySerialNumber, serialNumber, true);
                return certs.OfType<X509Certificate>().FirstOrDefault();
            }
        }

        public static bool certExists()
        {
            // TODO: 
            // StoreLocation.CurrentUser)

            using (X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine))
            {
                store.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certificates = store.Certificates.Find(
                    X509FindType.FindBySubjectName,
                    "subjectName",
                    false);

                if (certificates != null && certificates.Count > 0)
                {
                    System.Console.WriteLine("Certificate already exists");
                    return true;
                }
            }

            return false;
        }


        public static void PrintCertificateInfo(X509Certificate2 certificate)
        {
            Console.WriteLine("Name: {0}", certificate.FriendlyName);
            Console.WriteLine("Issuer: {0}", certificate.IssuerName.Name);
            Console.WriteLine("Subject: {0}", certificate.SubjectName.Name);
            Console.WriteLine("Version: {0}", certificate.Version);
            Console.WriteLine("Valid from: {0}", certificate.NotBefore);
            Console.WriteLine("Valid until: {0}", certificate.NotAfter);
            Console.WriteLine("Serial number: {0}", certificate.SerialNumber);
            Console.WriteLine("Signature Algorithm: {0}", certificate.SignatureAlgorithm.FriendlyName);
            Console.WriteLine("Thumbprint: {0}", certificate.Thumbprint);
            Console.WriteLine();
        }

        public static void EnumCertificates(StoreName name, StoreLocation location)
        {
            using (X509Store store = new X509Store(name, location))
            {
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    foreach (X509Certificate2 certificate in store.Certificates)
                    {
                        PrintCertificateInfo(certificate);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                finally
                {
                    store.Close();
                }
            }
        }

        public static void EnumCertificates(string name, StoreLocation location)
        {
            using (X509Store store = new X509Store(name, location))
            {
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    foreach (X509Certificate2 certificate in store.Certificates)
                    {
                        PrintCertificateInfo(certificate);
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
                finally
                {
                    store.Close();
                }
            }
        }


        // <summary>
        ///     For CertificateEngine.DefaultWindows to work we need to also check in personal store
        /// </summary>
        /// <param name="storeLocation"></param>
        /// <returns></returns>
        private bool RootCertificateInstalled(StoreLocation storeLocation, string issuer)
        {
            //string issuer = $"{RootCertificate.Issuer}";
            // && (CertificateEngine != CertificateEngine.DefaultWindows
            return FindCertificates(StoreName.Root, storeLocation, issuer).Count > 0 
                       || FindCertificates(StoreName.My, storeLocation, issuer).Count > 0;
        } // End Function FindCertificates 


        private static X509Certificate2Collection FindCertificates(
              StoreName storeName
            , StoreLocation storeLocation
            , string findValue)
        {
            X509Certificate2Collection results = null;

            using (X509Store x509Store = new X509Store(storeName, storeLocation))
            {
                try
                {
                    x509Store.Open(OpenFlags.OpenExistingOnly);
                    results = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, findValue, false);
                }
                finally
                {
                    x509Store.Close();
                }

            } // End Using x509Store 

            return results;
        } // End Function FindCertificates 

        // https://www.codeguru.com/csharp/csharp/cs_misc/security/article.php/c16491/Working-with-Digital-Certificates-in-NET.htm
        public static bool ExportCertificate(
           string certificateName,
           string path,
           StoreName storeName,
           StoreLocation location)
        {
            bool success = false;

            X509Store store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadOnly);
            try
            {
                X509Certificate2Collection certs
                   = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, true);

                if (certs != null && certs.Count > 0)
                {
                    byte[] data = certs[0].Export(X509ContentType.Cert);
                    success = WriteFile(data, path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                store.Close();
            }

            return success;
        }

        public static bool ExportCertificate(
           string certificateName,
           string path,
           string storeName,
           StoreLocation location)
        {
            bool success = false;

            using (X509Store store = new X509Store(storeName, location))
            {
                store.Open(OpenFlags.ReadOnly);
                try
                {
                    X509Certificate2Collection certs
                       = store.Certificates.Find(X509FindType.FindBySubjectName, certificateName, true);

                    if (certs != null && certs.Count > 0)
                    {
                        byte[] data = certs[0].Export(X509ContentType.Cert);
                        success = WriteFile(data, path);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    store.Close();
                }
            }

            return success;
        }

        private static bool WriteFile(byte[] data, string filename)
        {
            bool ret = false;
            try
            {
                using (FileStream f = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    f.Write(data, 0, data.Length);
                    f.Flush();
                    f.Close();
                }
                
                ret = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return ret;
        }



        /// <summary>
        ///     Loads root certificate from current executing assembly location with expected name rootCert.pfx.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 LoadRootCertificate(string fileName
            , string pfxPassword)
        {
            return LoadRootCertificate(fileName, pfxPassword, X509KeyStorageFlags.Exportable);
        } // End Function LoadRootCertificate 


        /// <summary>
        ///     Loads root certificate from current executing assembly location with expected name rootCert.pfx.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 LoadRootCertificate(string fileName
        ,string pfxPassword
            ,X509KeyStorageFlags StorageFlag )
        {
            bool pfxFileExists = System.IO.File.Exists(fileName);
            if (!pfxFileExists)
            {
                return null;
            }

            try
            {
                return new X509Certificate2(fileName, pfxPassword, StorageFlag);
            }
            catch (Exception e)
            {
                throw e;
            }
        } // End Function LoadRootCertificate 



        /// <summary>
        ///     Remove the Root Certificate trust
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="certificate"></param>
        public static void UninstallCertificate(
              X509Certificate2 certificate
            , StoreName storeName
            , StoreLocation storeLocation
            )
        {
            if (certificate == null)
            {
                throw new Exception("Could not remove certificate as it is null or empty.");
            }

            using (X509Store x509Store = new X509Store(storeName, storeLocation))
            {
                try
                {
                    x509Store.Open(OpenFlags.ReadWrite);
                    x509Store.Remove(certificate);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to remove root certificate trust "
                                      + $" for {storeLocation} store location. You may need admin rights.", e);
                }
                finally
                {
                    x509Store.Close();
                }
            } // End Using x509Store 

        } // End Sub UninstallCertificate 



        /// <summary>
        ///     Make current machine trust the Root Certificate used by this proxy
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="certificate"></param>
        public static void InstallCertificate(
              System.Security.Cryptography.X509Certificates.X509Certificate2 certificate
            , StoreName storeName
            , StoreLocation storeLocation
            
        )
        {
            
            if (certificate == null)
            {
                throw new Exception("Could not install certificate as it is null or empty.");
            }

            using (X509Store x509Store = new X509Store(storeName, storeLocation))
            {
                // todo
                // also it should do not duplicate if certificate already exists
                try
                {
                    x509Store.Open(OpenFlags.ReadWrite);
                    x509Store.Add(certificate);
                }
                catch (Exception e)
                {
                    throw new Exception("Failed to make system trust root certificate "
                                      + $" for {storeName}\\{storeLocation} store location. You may need admin rights.",
                            e);
                }
                finally
                {
                    x509Store.Close();
                }
            } // End Using x509Store 

        } // End Sub InstallCertificate 


    }
    
    
}
