
namespace RedmineMailService.CertSSL
{


    public class CertificationRequestReader
    {


        // Using BouncyCastle API, how do I sign a Certificate Signing Request (CSR) using my own CA?
        // https://stackoverflow.com/questions/44440974/bouncy-castle-decode-csr-c-sharp
        // You will have to add some code to read the subject's public key info 
        // from the CSR and then generate a version 3 certificate as shown in the example.
        // The OpenSSL equivalent for this is
        // openssl ca –in <CSR> -cert <CA-cert-file> -out <signed-cert>
        public static void ReadCertificationRequest(string csr)
        {
            Org.BouncyCastle.Pkcs.Pkcs10CertificationRequest decodedCsr = null;

            using (System.IO.TextReader sr = new System.IO.StringReader(csr))
            {
                Org.BouncyCastle.OpenSsl.PemReader pr = new Org.BouncyCastle.OpenSsl.PemReader(sr);
                object obj = pr.ReadObject();
                decodedCsr = (Org.BouncyCastle.Pkcs.Pkcs10CertificationRequest)obj;
            } // End Using sr 


            Org.BouncyCastle.Asn1.Pkcs.CertificationRequestInfo csri = decodedCsr.GetCertificationRequestInfo();
            GetX509ExtensionsFromCsr(csri);
            Org.BouncyCastle.Asn1.X509.X509Name subj = csri.Subject;


            // decodedCsr.Signature
            // decodedCsr.SignatureAlgorithm




            GetX509Field(Org.BouncyCastle.Asn1.X509.X509Name.EmailAddress.Id, subj);

            System.Console.WriteLine(csri.Subject);
            // csri.SubjectPublicKeyInfo
            // csri.Version
            // csri.Attributes

            //Org.BouncyCastle.Asn1.Asn1Set attributes =
            //    new Org.BouncyCastle.Asn1.DerSet(
            //        new Org.BouncyCastle.Asn1.Pkcs.AttributePkcs(
            //              Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtExtensionRequest
            //            , new Org.BouncyCastle.Asn1.DerSet(new Org.BouncyCastle.Asn1.X509.X509Extensions(extensions))
            //        )
            //);

        } // End Sub ReadCertificationRequest 



        // https://stackoverflow.com/questions/21912390/decode-read-a-csr-certificate-signing-request-using-java-or-bouncycastle
        private static string GetX509Field(string asn1ObjectIdentifier, Org.BouncyCastle.Asn1.X509.X509Name x500Name)
        {
            string retVal = null;

            System.Collections.IList rdnArray = x500Name.GetValueList(
                new Org.BouncyCastle.Asn1.DerObjectIdentifier(asn1ObjectIdentifier)
            );

            System.Collections.IList oids = x500Name.GetOidList();
            System.Collections.IList foo = x500Name.GetValueList();
            System.Console.WriteLine(oids);
            System.Console.WriteLine(foo);

            foreach (Org.BouncyCastle.Asn1.DerObjectIdentifier thisOID in oids)
            {
                string oidName = System.Convert.ToString(Org.BouncyCastle.Asn1.X509.X509Name.DefaultSymbols[thisOID]);
                System.Console.WriteLine(oidName);
                System.Collections.IList values = x500Name.GetValueList(thisOID);
                System.Console.WriteLine(values);
            } // Next thisOID 

            foreach (object x in rdnArray)
            {
                // System.Console.WriteLine(x);
                retVal = System.Convert.ToString(x);
                return retVal;
            } // Next x 

            return retVal;
        } // End Function GetX509Field 


        // http://unitstep.net/blog/2008/10/27/extracting-x509-extensions-from-a-csr-using-the-bouncy-castle-apis/
        // https://github.com/puppetlabs/jvm-ssl-utils/blob/master/src/java/com/puppetlabs/ssl_utils/ExtensionsUtils.java
        // Gets the X509 Extensions contained in a CSR (Certificate Signing Request).
        // @param certificateSigningRequest the CSR.
        // @return the X509 Extensions in the request.
        // @throws CertificateException if the extensions could not be found.
        private static Org.BouncyCastle.Asn1.X509.X509Extensions GetX509ExtensionsFromCsr(
            Org.BouncyCastle.Asn1.Pkcs.CertificationRequestInfo certificationRequestInfo
        )
        {
            // Org.BouncyCastle.Pkcs.Pkcs10CertificationRequest certificateSigningRequest
            //Org.BouncyCastle.Asn1.Pkcs.CertificationRequestInfo certificationRequestInfo = certificateSigningRequest.GetCertificationRequestInfo();

            Org.BouncyCastle.Asn1.Asn1Set attributesAsn1Set = certificationRequestInfo.Attributes;

            // The `Extension Request` attribute is contained within an ASN.1 Set,
            // usually as the first element.
            Org.BouncyCastle.Asn1.X509.X509Extensions certificateRequestExtensions = null;

            for (int i = 0; i < attributesAsn1Set.Count; ++i)
            {
                // There should be only only one attribute in the set. (that is, only
                // the `Extension Request`, but loop through to find it properly)
                Org.BouncyCastle.Asn1.Asn1Encodable derEncodable = attributesAsn1Set[i];

                if (derEncodable is Org.BouncyCastle.Asn1.X509.X509Extensions)
                {
                    certificateRequestExtensions = (Org.BouncyCastle.Asn1.X509.X509Extensions)derEncodable;
                    break;
                }
                else if (derEncodable is Org.BouncyCastle.Asn1.DerSequence)
                {
                    Org.BouncyCastle.Asn1.DerSequence sequence = (Org.BouncyCastle.Asn1.DerSequence)attributesAsn1Set[i];

                    Org.BouncyCastle.Asn1.Cms.Attribute attribute =
                        new Org.BouncyCastle.Asn1.Cms.Attribute(sequence);

                    // Check if the `Extension Request` attribute is present.
                    if (attribute.AttrType.Equals(
                        Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Pkcs9AtExtensionRequest)
                    )
                    {
                        Org.BouncyCastle.Asn1.Asn1Set attributeValues = attribute.AttrValues;

                        // The X509Extensions are contained as a value of the ASN.1 Set.
                        // Assume that it is the first value of the set.
                        if (attributeValues.Count >= 1)
                        {
                            certificateRequestExtensions = Org.BouncyCastle.Asn1.X509.X509Extensions.GetInstance(
                                attributeValues[0]
                            );
                            // No need to search any more.
                            break;
                        } // End if (attributeValues.Count >= 1) 
                    }

                } // End else if (derEncodable is Org.BouncyCastle.Asn1.DerSequence)

            } // Next i 

            if (null == certificateRequestExtensions)
            {
                throw new Org.BouncyCastle.Security.Certificates.CertificateException(
                    "Could not obtain X509 Extensions from the CSR");
            } // End if (null == certificateRequestExtensions) 

            return certificateRequestExtensions;
        } // End Function 


    } // End Class 


} // End Namespace 
