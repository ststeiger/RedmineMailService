
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Cms;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;


namespace RedmineMailService.CertSSL.Trash
{


    class Trash
    {



        private static Asn1Set buildSignedAttributesJunk(byte[] hash, System.DateTime dateTime, X509Certificate cert)
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            v.Add(new Attribute(CmsAttributes.ContentType, new DerSet(PkcsObjectIdentifiers.Data)));
            if (dateTime != null)
                v.Add(new Attribute(CmsAttributes.SigningTime, new DerSet(new Org.BouncyCastle.Asn1.Cms.Time(dateTime))));
            v.Add(new Attribute(CmsAttributes.MessageDigest, new DerSet(new DerOctetString(hash))));

            // CADES support section
            Asn1EncodableVector aaV2 = new Asn1EncodableVector();


            AlgorithmIdentifier algoId = new AlgorithmIdentifier(new DerObjectIdentifier(Org.BouncyCastle.Cms.CmsSignedDataGenerator.DigestSha256), null);
            aaV2.Add(algoId);
            byte[] dig = null; // SignUtils.calculateHASH(Org.BouncyCastle.Cms.CmsSignedDataGenerator.DigestSha256, cert.GetEncoded());
            aaV2.Add(new DerOctetString(dig));
            Attribute cades = new Attribute(PkcsObjectIdentifiers.IdAASigningCertificateV2, new DerSet(new DerSequence(new DerSequence(new DerSequence(aaV2)))));
            v.Add(cades);

            Asn1Set signedAttributes = new DerSet(v);
            return signedAttributes;
        }



        public static void Asn1SetTrialJunk()
        {
            Asn1Encodable foo = null;
            Asn1Encodable bar = null;

            DerSequence subjectAlternativeNames = new DerSequence(new Asn1Encodable[] {
                new GeneralName(GeneralName.DnsName, "localhost"),
                new GeneralName(GeneralName.DnsName, System.Environment.MachineName),
                new GeneralName(GeneralName.DnsName, "127.0.0.1")
            });


            Asn1EncodableVector v = new Asn1EncodableVector();


            v.Add(Org.BouncyCastle.Asn1.X509.X509Extensions.SubjectAlternativeName);
            // v.Add(DerBoolean.True);
            v.Add(subjectAlternativeNames);
            // var ds = new DerSet(v);
            //var ds = new DerSet(new DerSequence(new DerSequence(new DerSequence(v))));

            // byte[] der = new DerSequence(v).GetDerEncoded();
            // var ds = new DerSet(new DerSequence(v));


            // System.Collections.Generic.Dictionary<DerObjectIdentifier, string> dict = new System.Collections.Generic.Dictionary<DerObjectIdentifier, string>();
            // AttributeTable att = new AttributeTable(dict);

            // Asn1Set derSet = new DerSet(foo, bar, subjectAlternativeNames);

            Asn1Set derSet = new DerSet(subjectAlternativeNames);
            // Asn1Set derSet1 = new DerSet(att.to);

            string asn1st = subjectAlternativeNames.ToString();
            System.Console.WriteLine(asn1st);
        }


    }
}
