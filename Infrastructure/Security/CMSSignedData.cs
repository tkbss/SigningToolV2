using LipingShare.LCLib.Asn1Processor;
using System.Security.Cryptography.X509Certificates;

namespace Infrastructure.cms
{
    public class CMSSignedData : Asn1Object
    {
        const string CERTPATH = "/1/0/3";
        const string SIGNERIDPATH = "/1/0/4/0/1";
        const string TC_OID = "1.3.6.1.4.1.817.17.3";
        const string ATTRIBUTE_SHA_PATH = "/1/0/4/0/3/2/1/0";
        const string SHA_PATH = "/1/0/4/0/3/1/1/0";
        const string SIGNATUREPATH = "/1/0/4/0/4";
        const string CONTENTPATH = "/1/0/2/1/0";
        const string SIGNATURECONTENTPATH = "/1/0/4/0/3";
        const string SIGNERINFO_DIGESTALGO = "/1/0/4/0/2/0";
        const string SIGNERINFO_SIGNATUREALGO = "/1/0/4/0/3/0";
        const string SIGNEDDATA_DIGESTALGO = "/1/0/1/0/0";
        /// <summary>
        /// The content is a SEQUENCE. 
        /// The message digest is computed only over the value of the sequence, not over tag and length!
        /// </summary>
        /// <returns>content without tag and length</returns>
        public byte[] GetContentForMessageDigest()
        {
            Asn1Node content = m_root.GetDescendantNodeByPath(CONTENTPATH);
            return content.Data;
        }
        public byte[] GetContentForSignature()
        {
            Asn1Node SigContent = m_root.GetDescendantNodeByPath(SIGNATURECONTENTPATH);
            byte[] DataToSign = DEREncodeNode(SigContent);
            DataToSign[0] = 0x31;
            return DataToSign;
        }
        public void ReplaceSignature(byte[] sig)
        {
            Asn1Node SigNode = m_root.GetDescendantNodeByPath(SIGNATUREPATH);
            SigNode.ClearAll();
            SigNode.Data = sig;
        }
        public byte[] GetSignature()
        {
            Asn1Node SigNode = m_root.GetDescendantNodeByPath(SIGNATUREPATH);
            return SigNode.Data;
        }
        public void ReplaceCertificates(X509Certificate2[] certs)
        {
            Asn1Node CertNode = m_root.GetDescendantNodeByPath(CERTPATH);
            long NuOfCerts = CertNode.ChildNodeCount;
            for (long i = 0; i < NuOfCerts; i++)
            {
                CertNode.RemoveChild(0);
            }
            foreach (X509Certificate2 c in certs)
            {
                Asn1Node newCert = new Asn1Node();
                newCert.LoadData(c.RawData);
                CertNode.AddChild(newCert);                
            }
        }
        public void ReplaceIssuerAndSerialNumber(X509Certificate2 signingCert)
        {
            Asn1Node SignerInfoIdentifier = m_root.GetDescendantNodeByPath(SIGNERIDPATH);
            byte[] SubjectId = signingCert.IssuerName.RawData;
            Asn1Node newIdentifier = new Asn1Node();
            newIdentifier.LoadData(SubjectId);
            SignerInfoIdentifier.RemoveChild(0);
            SignerInfoIdentifier.InsertChild(newIdentifier, 0);
            Asn1Node serialNumber = SignerInfoIdentifier.GetChildNode(1);
            byte[] snr = Org.BouncyCastle.Security.DotNetUtilities.FromX509Certificate(signingCert).SerialNumber.ToByteArray();             
            serialNumber.Data=snr;
        }
        public void SetSignatureAlgoAndDigestIdentifier(string oidDigestAlgo, string oidSignatureAlgo)
        {
            Oid o = new Oid();
            byte[] oidDA=o.Encode(oidDigestAlgo);
            byte[] oidSA = o.Encode(oidSignatureAlgo);
            Asn1Node oidDigestSignedDataValue = m_root.GetDescendantNodeByPath(SIGNEDDATA_DIGESTALGO);
            oidDigestSignedDataValue.ClearAll();
            oidDigestSignedDataValue.Data = oidDA;
            Asn1Node oidDigestSignerInfoValue = m_root.GetDescendantNodeByPath(SIGNERINFO_DIGESTALGO);
            oidDigestSignerInfoValue.ClearAll();
            oidDigestSignerInfoValue.Data = oidDA;
            Asn1Node oidSigAlgoValue = m_root.GetDescendantNodeByPath(SIGNERINFO_SIGNATUREALGO);
            oidSigAlgoValue.ClearAll();
            oidSigAlgoValue.Data = oidSA;
        }
        public void ReplaceSHAValue(string v)
        {
            string pathSha = string.Empty;
            if (Asn1Node.GetDecendantNodeByOid("1.3.6.1.4.1.817.17.3", m_root) == null)
                pathSha = SHA_PATH;
            else
                pathSha = ATTRIBUTE_SHA_PATH;
            Asn1Node ShaValue = m_root.GetDescendantNodeByPath(pathSha);
            ShaValue.ClearAll();
            ShaValue.Data = Asn1Util.HexStrToBytes(v);
        }
        public override void Decode(byte[] DEREncoded)
        {
            m_root.LoadData(DEREncoded);
        }

        public override byte[] Encode()
        {
            return DEREncodeRootNode();
        }

        
    }
}
