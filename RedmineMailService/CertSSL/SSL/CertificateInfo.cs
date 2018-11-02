
using AnySqlWebAdmin;
using RedmineMailService.CertSSL;


namespace RedmineMailService.CertSSL
{


    public class CertificateInfo
    {

        public string CountryIso2Characters;
        public string StateOrProvince;
        public string LocalityOrCity;
        public string CompanyName;
        public string Division;
        public string DomainName;
        public string EMail;

        public System.DateTime ValidFrom;
        public System.DateTime ValidTo;

        public PrivatePublicPemKeyPair SubjectKeyPair;
        public PrivatePublicPemKeyPair IssuerKeyPair;


        public System.Collections.Generic.Dictionary<string, Org.BouncyCastle.Asn1.Asn1Encodable> NonCriticalExtensions;
        public System.Collections.Generic.Dictionary<string, Org.BouncyCastle.Asn1.Asn1Encodable> CriticalExtensions;



        public Org.BouncyCastle.Asn1.X509.X509Name Subject
        {
            get
            {
                return CreateSubject(
                      this.CountryIso2Characters
                    , this.StateOrProvince
                    , this.LocalityOrCity
                    , this.CompanyName
                    , this.Division
                    , this.DomainName
                    , this.EMail
                );
            }
        }


        public CertificateInfo()
        {
            this.NonCriticalExtensions = new System.Collections.Generic.Dictionary<string, Org.BouncyCastle.Asn1.Asn1Encodable>(System.StringComparer.OrdinalIgnoreCase);
            this.CriticalExtensions = new System.Collections.Generic.Dictionary<string, Org.BouncyCastle.Asn1.Asn1Encodable>(System.StringComparer.OrdinalIgnoreCase);
        } // End Constructor 


        public CertificateInfo(
              string countryIso2Characters
            , string stateOrProvince
            , string localityOrCity
            , string companyName
            , string division
            , string domainName
            , string email
            , System.DateTime validFrom 
            , System.DateTime validTo 
            ) : this()
        {
            this.CountryIso2Characters = countryIso2Characters;
            this.StateOrProvince = stateOrProvince;
            this.LocalityOrCity = localityOrCity;
            this.CompanyName = companyName;
            this.Division = division;
            this.DomainName = domainName;
            this.EMail = email;
            this.ValidFrom = validFrom;
            this.ValidTo = validTo;
        } // End Constructor 


        // https://codereview.stackexchange.com/questions/84752/net-bouncycastle-csr-and-private-key-generation
        public static Org.BouncyCastle.Asn1.X509.X509Name CreateSubject(
              string countryIso2Characters
            , string stateOrProvince
            , string localityOrCity
            , string companyName
            , string division
            , string domainName
            , string email)
        {
            // https://people.eecs.berkeley.edu/~jonah/bc/org/bouncycastle/asn1/x509/X509Name.html
            KeyValuePairList<Org.BouncyCastle.Asn1.DerObjectIdentifier, string> attrs =
                new KeyValuePairList<Org.BouncyCastle.Asn1.DerObjectIdentifier, string>();
            
            if(!string.IsNullOrEmpty(countryIso2Characters) && countryIso2Characters.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.C, countryIso2Characters);

            if (!string.IsNullOrEmpty(stateOrProvince) && stateOrProvince.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.ST, stateOrProvince);

            if (!string.IsNullOrEmpty(localityOrCity) && localityOrCity.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.L, localityOrCity);

            if (!string.IsNullOrEmpty(companyName) && companyName.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.O, companyName);

            if (!string.IsNullOrEmpty(division) && division.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.OU, division);

            // Must have ? 
            if (!string.IsNullOrEmpty(domainName) && domainName.Trim() != string.Empty)
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.CN, domainName);

            if (!string.IsNullOrEmpty(email) && email.Trim() != string.Empty)
            { 
                //attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.E, email); // email address in Verisign certificates
                attrs.Add(Org.BouncyCastle.Asn1.X509.X509Name.EmailAddress, email); //  Email address (RSA PKCS#9 extension)
            }

            Org.BouncyCastle.Asn1.X509.X509Name subject =
                new Org.BouncyCastle.Asn1.X509.X509Name(attrs.Keys, attrs.Values);

            return subject;
        } // End Function CreateSubject 


        public void AddAlternativeNames(params string[] names)
        {
            Org.BouncyCastle.Asn1.Asn1Encodable[] alternativeNames = new Org.BouncyCastle.Asn1.Asn1Encodable[names.Length];

            for (int i = 0; i < names.Length; ++i)
            {
                alternativeNames[i] = new Org.BouncyCastle.Asn1.X509.GeneralName(Org.BouncyCastle.Asn1.X509.GeneralName.DnsName, names[i]);
            }


            Org.BouncyCastle.Asn1.DerSequence subjectAlternativeNames = new Org.BouncyCastle.Asn1.DerSequence(alternativeNames);
        } // End Sub AddAlternativeNames 


        public void AddExtension(string oid, bool critical, Org.BouncyCastle.Asn1.Asn1Encodable extensionValue)
        {
            if (critical)
                this.CriticalExtensions.Add(oid, extensionValue);
            else
                this.NonCriticalExtensions.Add(oid, extensionValue);

            //certGenerator.AddExtension(
            //              Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName
            //            , false
            //            , subjectAlternativeNames
            //        );
        } // End Sub AddExtension 


    } // End Class 


} // End Namespace 
