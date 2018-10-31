
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


namespace RedmineMailService.CertSSL
{
    public class Certificator
    {


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
                       || FindCertificates(StoreName.My, storeLocation, issuer).Count > 0);
        }
        
        
        private static X509Certificate2Collection FindCertificates(
              StoreName storeName
            , StoreLocation storeLocation
            , string findValue)
        {
            var x509Store = new X509Store(storeName, storeLocation);
            try
            {
                x509Store.Open(OpenFlags.OpenExistingOnly);
                return x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, findValue, false);
            }
            finally
            {
                x509Store.Close();
            }
        }

        
        

        /// <summary>
        ///     Loads root certificate from current executing assembly location with expected name rootCert.pfx.
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 LoadRootCertificate(string fileName
            , string pfxPassword)
        {
            return LoadRootCertificate(fileName, pfxPassword, X509KeyStorageFlags.Exportable);
        }
        
        
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
        }

        
        
        /// <summary>
        ///     Remove the Root Certificate trust
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="certificate"></param>
        public static void UninstallCertificate(StoreName storeName, StoreLocation storeLocation,
            X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw new Exception("Could not remove certificate as it is null or empty.");
            }

            var x509Store = new X509Store(storeName, storeLocation);

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
        }

        
        
        /// <summary>
        ///     Make current machine trust the Root Certificate used by this proxy
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="certificate"></param>
        public static void InstallCertificate(StoreName storeName, StoreLocation storeLocation
            ,System.Security.Cryptography.X509Certificates.X509Certificate2 certificate;
        )
        {
            
            if (certificate == null)
            {
                throw new Exception("Could not install certificate as it is null or empty.");
                return;
            }

            X509Store x509Store = new X509Store(storeName, storeLocation);
            
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
        }
        
        
    }
    
    
}
