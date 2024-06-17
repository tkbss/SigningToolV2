using Infrastructure;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using System.Collections;
using System.IO;

namespace infrastructure.security.provider
{
    class SIXCertificateManagement 
    {
        AsymmetricCipherKeyPair CERTKeyPair;
        AsymmetricCipherKeyPair CAKeyPair;
        Org.BouncyCastle.X509.X509Certificate CACertificate;
        Org.BouncyCastle.X509.X509Certificate Certificate;
        public AsymmetricCipherKeyPair DeviceKeyPair
        {
            get 
            {
                return CERTKeyPair;
            }
        }
        public Org.BouncyCastle.X509.X509Certificate GetCACertificate() 
        {
            return CACertificate;
        }
        public Org.BouncyCastle.X509.X509Certificate GetCertificate() 
        {
            return Certificate;
        }
        public void LoadCAKeys(string fn,string pwd)
        {            
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            char[] passwd = pwd.ToCharArray();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.Load(fileStream, passwd);
            fileStream.Close();
            string pName = null;
            foreach (string n in store.Aliases)
            {
                if (store.IsKeyEntry(n))
                {
                    pName = n;
                    break;
                }
            }
            AsymmetricKeyEntry key = store.GetKey(pName);
            X509CertificateEntry cert = store.GetCertificate(pName);
            CACertificate = cert.Certificate;
            CAKeyPair = new AsymmetricCipherKeyPair(CACertificate.GetPublicKey(), key.Key);
        }
        public void LoadCertificateKeys(string fn,string pwd)
        {
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            char[] passwd = pwd.ToCharArray();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.Load(fileStream, passwd);
            fileStream.Close();
            string pName = null;
            foreach (string n in store.Aliases)
            {
                if (store.IsKeyEntry(n))
                {
                    pName = n;
                    break;
                }
            }
            AsymmetricKeyEntry key = store.GetKey(pName);
            X509CertificateEntry cert = store.GetCertificate(pName);
            Certificate = cert.Certificate;
            CERTKeyPair = new AsymmetricCipherKeyPair(Certificate.GetPublicKey(), key.Key);
        }
        public void LoadCertificateKeys(string fn)
        {
            string pwd = "1234";
            LoadCertificateKeys(fn,pwd);
        }
        public void ExportCACertificate(string path,string cert_name)
        {
            string fn = Path.Combine(path, cert_name);
            byte[] encoded_cert = CACertificate.GetEncoded();
            FileStream writeStream = new FileStream(fn, FileMode.Create);
            writeStream.Write(encoded_cert, 0, encoded_cert.Length);
            writeStream.Close();
        }
        public void ExportCACertificate(string target_path)
        {
            if (File.Exists(target_path) == true)
                File.Delete(target_path);            
            byte[] encoded_cert = CACertificate.GetEncoded();
            FileStream writeStream = new FileStream(target_path, FileMode.Create);
            writeStream.Write(encoded_cert, 0, encoded_cert.Length);
            writeStream.Close();
        }
        public System.Security.Cryptography.X509Certificates.X509Certificate ExportMicrosoftCertificate(string fn)
        {
            FileStream fileStream = new FileStream(fn, FileMode.Open);
            char[] passwd = "1234".ToCharArray();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.Load(fileStream, passwd);
            fileStream.Close();
            string pName = null;
            foreach (string n in store.Aliases)
            {
                if (store.IsKeyEntry(n))
                {
                    pName = n;
                    break;
                }
            }            
            X509CertificateEntry cert = store.GetCertificate(pName);
            Org.BouncyCastle.X509.X509Certificate c = cert.Certificate;
            System.Security.Cryptography.X509Certificates.X509Certificate mscert = DotNetUtilities.ToX509Certificate(c);
            return mscert;
        }
        public SubjectPublicKeyInfo MakeSubjectPubliKeyInfo(string public_key)
        {
            byte[] seq = Converter.HexStrToBytes(public_key);            
            Asn1Object pk = Asn1Object.FromByteArray(seq);
            RsaPublicKeyStructure pk_struct = RsaPublicKeyStructure.GetInstance(pk);
            RsaKeyParameters key = new RsaKeyParameters(false, pk_struct.Modulus, pk_struct.PublicExponent);
            return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key);
        }
        public string MakeCACommonName(string env)
        {
            string cn = env + "_" + "CA_ATM_SOFTWARE_SIGNING";
            return cn;
        }
        public string MakeSigningCertCommonName(string st,string env,string cert_type)
        {
            string cn = st + "_";
            if (cert_type.ToUpper() == "QA")
            {
                cn += "QA_SOFTWARE_SIGNING";
            }
            else
                cn += env + "_" + cert_type + "_SOFTWARE_SIGNING";
            return cn;
        }
        public void MakeKMSCACert(string common_name)
        {
            CreateCAKeys(2048);
            string c = "CH";
            string o = "SIX";
            string ou = "ATM";
            string l = "ZH";
            string cn = common_name;
            IDictionary subject = new Hashtable();
            subject[X509Name.C] = c;
            subject[X509Name.O] = o;
            subject[X509Name.OU] = ou;
            subject[X509Name.L] = l;
            subject[X509Name.CN] = cn;
            ArrayList ord = new ArrayList();
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.OU);
            ord.Add(X509Name.L);
            ord.Add(X509Name.CN);
            X509Name sn = new X509Name(ord, subject);

            //set key usage to signature for this certificate            
            KeyUsage ku = new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment | KeyUsage.KeyCertSign);
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;


            //basic constraints set to CA = true;
            BasicConstraints bc = new BasicConstraints(true);
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator CAcertGen = new X509V3CertificateGenerator();
            BigInteger snr = new BigInteger("75689324567113");
            CAcertGen.SetSerialNumber(snr);
            CAcertGen.SetIssuerDN(sn);
            CAcertGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            CAcertGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            CAcertGen.SetSubjectDN(sn);
            CAcertGen.SetPublicKey(CAKeyPair.Public);

            //add extensions to certificate which are marked as not critical           
            CAcertGen.AddExtension(oidKu, false, ku);
            CAcertGen.AddExtension(oidBc, false, abc);

            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            CACertificate = CAcertGen.Generate(sf);

        }
        public void MakeKMSSigningCert(string common_name,CERTTYPE ct,string issuer_cn)
        {
            CreateKeys(2048);
            string c = "CH";
            string o = "SIX";
            string ou = "ATM";
            string l = "ZH";
            string cn = common_name;
            IDictionary subject = new Hashtable();
            subject[X509Name.C] = c;
            subject[X509Name.O] = o;
            subject[X509Name.OU] = ou;
            subject[X509Name.L] = l;
            subject[X509Name.CN] = cn;

            ArrayList ord = new ArrayList();
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.OU);
            ord.Add(X509Name.L);
            ord.Add(X509Name.CN);

            X509Name sn = new X509Name(ord, subject);
            if (ct != CERTTYPE.QA )            
            {
                c = "CH";
                o = "SIX";
                ou = "ATM";
                l = "ZH";
                cn = issuer_cn;
            }
            IDictionary issuer = new Hashtable();
            issuer[X509Name.C] = c;
            issuer[X509Name.O] = o;
            issuer[X509Name.OU] = ou;
            issuer[X509Name.L] = l;
            issuer[X509Name.CN] = cn;
            
            X509Name isn = new X509Name(ord, issuer);

            //set key usage to signature for this certificate 
            int usage = 0;
            if (ct == CERTTYPE.QA)
                usage = KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment | KeyUsage.KeyCertSign;
            else
                usage = KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment;
            KeyUsage ku = new KeyUsage(usage);
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;


            //basic constraints set to CA = true;
            BasicConstraints bc;
            BigInteger snr;
            if (ct == CERTTYPE.QA)
            {
                bc = new BasicConstraints(true);
                snr = new BigInteger("97653295187345608992384571");
            }
            else
            {
                bc = new BasicConstraints(false);
                snr = new BigInteger("1234789034578093478734687");
            }
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
             
            certGen.SetSerialNumber(snr);
            certGen.SetIssuerDN(isn);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            certGen.SetSubjectDN(sn);
            certGen.SetPublicKey(CERTKeyPair.Public);

            //add extensions to certificate which are marked as not critical           
            certGen.AddExtension(oidKu, false, ku);
            certGen.AddExtension(oidBc, false, abc);

            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CERTKeyPair.Private);
            Certificate = certGen.Generate(sf);

        }

        public void MakeCACertificate(string c,string o,string ou,string l,string cn)
        {
            if (CAKeyPair == null)
                return;
            IDictionary subject = new Hashtable();
            subject[X509Name.C] = c;
            subject[X509Name.O] = o;
            subject[X509Name.OU] = ou;
            subject[X509Name.L] = l;
            subject[X509Name.CN] = cn;
            ArrayList ord = new ArrayList();
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.OU);
            ord.Add(X509Name.L);
            ord.Add(X509Name.CN);
            X509Name sn = new X509Name(ord, subject);

            //set key usage to signature for this certificate            
            KeyUsage ku = new KeyUsage(KeyUsage.DigitalSignature|KeyUsage.NonRepudiation|KeyUsage.DataEncipherment|KeyUsage.KeyCertSign);
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;

            //add public key identifier to certificate            
            SubjectKeyIdentifierStructure ski = new SubjectKeyIdentifierStructure(CAKeyPair.Public);
            DerObjectIdentifier oidSki = X509Extensions.SubjectKeyIdentifier;
            //add issuer key identifier to certificate
            AuthorityKeyIdentifierStructure iki = new AuthorityKeyIdentifierStructure(CAKeyPair.Public);
            DerObjectIdentifier oidIki = X509Extensions.AuthorityKeyIdentifier;

            //basic constraints set to CA = true;
            BasicConstraints bc = new BasicConstraints(true);
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator CAcertGen = new X509V3CertificateGenerator();
            BigInteger snr= new BigInteger(128, new SecureRandom());
            CAcertGen.SetSerialNumber(snr);
            CAcertGen.SetIssuerDN(sn);
            CAcertGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            CAcertGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            CAcertGen.SetSubjectDN(sn);            
            CAcertGen.SetPublicKey(CAKeyPair.Public);
            //CAcertGen.SetSignatureAlgorithm("SHA256WITHRSAENCRYPTION");

            //add extensions to certificate which are marked as not critical           
            CAcertGen.AddExtension(oidKu, false, ku);
            CAcertGen.AddExtension(oidSki, false, ski);
            CAcertGen.AddExtension(oidIki, false, iki);
            CAcertGen.AddExtension(oidBc, false, abc);

            //CACertificate = CAcertGen.Generate(CAKeyPair.Private);
            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            CACertificate = CAcertGen.Generate(sf);
            
        }
        public void MakeCertificate(string c, string o, string ou, string l, string cn,bool self_signed)
        {
            if (self_signed == false)
            {
                if (CAKeyPair == null || CACertificate == null)
                    throw new Exception("NO CA KEYS LOADED");
            }
            IDictionary subject = new Hashtable();
            subject[X509Name.C] = c;
            subject[X509Name.O] = o;
            subject[X509Name.OU] = ou;
            subject[X509Name.L] = l;
            subject[X509Name.CN] = cn;
            ArrayList ord = new ArrayList();
            ord.Add(X509Name.C);
            ord.Add(X509Name.O);
            ord.Add(X509Name.OU);
            ord.Add(X509Name.L);
            ord.Add(X509Name.CN);
            X509Name sn = new X509Name(ord, subject);

            //set key usage to signature for this certificate            
            KeyUsage ku = new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment );
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;

            //add public key identifier to certificate            
            SubjectKeyIdentifierStructure ski = new SubjectKeyIdentifierStructure(CERTKeyPair.Public);
            DerObjectIdentifier oidSki = X509Extensions.SubjectKeyIdentifier;
            //add issuer key identifier to certificate
            AuthorityKeyIdentifierStructure iki;           
            if (self_signed == true)
            {
                iki= new AuthorityKeyIdentifierStructure(CERTKeyPair.Public);
            }
            else
            {
                iki= new AuthorityKeyIdentifierStructure(CAKeyPair.Public);               
            }
            DerObjectIdentifier oidIki = X509Extensions.AuthorityKeyIdentifier;

            //basic constraints set to CA = false;
            BasicConstraints bc = new BasicConstraints(false);
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
            BigInteger snr = new BigInteger(128, new SecureRandom());
            certGen.SetSerialNumber(snr);
            if(self_signed==false)
                certGen.SetIssuerDN(CACertificate.SubjectDN);
            else
                certGen.SetIssuerDN(sn);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            certGen.SetSubjectDN(sn);
            certGen.SetPublicKey(CERTKeyPair.Public);
            //certGen.SetSignatureAlgorithm("SHA256WITHRSAENCRYPTION");

            //add extensions to certificate which are marked as not critical           
            certGen.AddExtension(oidKu, false, ku);
            certGen.AddExtension(oidSki, false, ski);
            certGen.AddExtension(oidIki, false, iki);
            certGen.AddExtension(oidBc, false, abc);
            Asn1SignatureFactory sf;            
            if(self_signed == true)
                sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CERTKeyPair.Private);
            else
                sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            Certificate=certGen.Generate(sf);
            
        }

        public void CreateKeys(int len)
        {
            RsaKeyPairGenerator g = new RsaKeyPairGenerator();
            g.Init(new KeyGenerationParameters(new SecureRandom(), len));
            CERTKeyPair = g.GenerateKeyPair();
        }
        public void CreateCAKeys(int len)
        {
            RsaKeyPairGenerator g = new RsaKeyPairGenerator();
            g.Init(new KeyGenerationParameters(new SecureRandom(), len));
            CAKeyPair = g.GenerateKeyPair();
        }
        public string SerializePrivateKey()
        {
            if (CERTKeyPair == null)
                return string.Empty;
            PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(CERTKeyPair.Private);
            byte[] serializedPrivateBytes = privateKeyInfo.ToAsn1Object().GetDerEncoded();
            string serializedPrivate = Convert.ToBase64String(serializedPrivateBytes);
            return serializedPrivate;
        }
        public string SerializePublicKey()
        {
            if (CERTKeyPair == null)
                return string.Empty;
            SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(CERTKeyPair.Public);
            byte[] serializedPublicBytes = publicKeyInfo.ToAsn1Object().GetDerEncoded();
            string serializedPublic = Convert.ToBase64String(serializedPublicBytes);
            return serializedPublic;
        }
        public void DeserializingKey(string serializedPublic, string serializedPrivate) 
        {
            RsaKeyParameters publicKey = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(serializedPublic));
            RsaPrivateCrtKeyParameters privateKey = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(serializedPrivate));
            AsymmetricCipherKeyPair CAKeyPair = new AsymmetricCipherKeyPair(publicKey, privateKey);
        }

        public X509Certificate SaveCACeritifcatesWithPrivateKey(string fn,string storename,string pwd)
        {
            //make a pkcs12 structure with private key inside  
            if (File.Exists(fn) == true)
                File.Delete(fn);
            X509CertificateEntry pkcs12Entry = new X509CertificateEntry(CACertificate);
            X509CertificateEntry[] chain = new X509CertificateEntry[] { pkcs12Entry };
            string store_name= storename;
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry(store_name, new AsymmetricKeyEntry(CAKeyPair.Private), chain);
            FileStream fileStream = new FileStream(fn, FileMode.Create);            
            char[] passwd = pwd.ToCharArray();
            store.Save(fileStream,passwd, new SecureRandom());
            fileStream.Close();
            return CACertificate;


        }
        public void SaveCeritifcatesWithPrivateKey(string fn, string storename,string pwd)
        {
            //make a pkcs12 structure with private key inside 
            if (File.Exists(fn) == true)
                File.Delete(fn);
            X509CertificateEntry pkcs12Entry = new X509CertificateEntry(Certificate);
            X509CertificateEntry[] chain = new X509CertificateEntry[] { pkcs12Entry };
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            string store_name=storename;
            store.SetKeyEntry(store_name, new AsymmetricKeyEntry(CERTKeyPair.Private), chain);
            FileStream fileStream = new FileStream(fn, FileMode.Create);
            char[] passwd = pwd.ToCharArray();
            store.Save(fileStream, passwd, new SecureRandom());
            fileStream.Close();

        }
        public byte[] BuildPKCS10Request(string fn_store,string store_name,string pwd)
        {
            FileStream fileStream = new FileStream(fn_store, FileMode.Open);
            char[] passwd = pwd.ToCharArray();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.Load(fileStream, passwd);            
            fileStream.Close();
            
            AsymmetricKeyEntry key = store.GetKey(store_name);
            X509CertificateEntry cert = store.GetCertificate(store_name);
            CACertificate = cert.Certificate;
            CAKeyPair = new AsymmetricCipherKeyPair(CACertificate.GetPublicKey(), key.Key);
            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            Pkcs10CertificationRequest r = new Pkcs10CertificationRequest(sf,CACertificate.SubjectDN,CAKeyPair.Public,null,CAKeyPair.Private);
            byte[] request=r.GetDerEncoded();            
            return request;
        }
        public void InsertCertificateToStore(string cert_path,string store_path,string store_name,string pwd)
        {
            FileStream fileStream = new FileStream(store_path, FileMode.Open);
            char[] passwd = pwd.ToCharArray();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.Load(fileStream, passwd);
            fileStream.Close();

            

            X509CertificateParser p = new X509CertificateParser();
            fileStream = new FileStream(cert_path, FileMode.Open);
            X509Certificate c = p.ReadCertificate(fileStream);
            fileStream.Close();

            RsaKeyParameters k1= c.GetPublicKey() as RsaKeyParameters;
            RsaKeyParameters k2= store.GetCertificate(store_name).Certificate.GetPublicKey() as RsaKeyParameters;
            if (k1.Equals(k2) == false)
                throw new System.Security.Cryptography.CryptographicException("certificate with wrong public key imported");

            File.Delete(store_path);
            X509CertificateEntry ce = new X509CertificateEntry(c);
            AsymmetricKeyEntry key = store.GetKey(store_name);                        
            X509CertificateEntry[] chain = new X509CertificateEntry[] { ce };
            Pkcs12Store new_store = new Pkcs12StoreBuilder().Build();            
            new_store.SetKeyEntry(store_name, key, chain);
            
            fileStream = new FileStream(store_path, FileMode.Create);            
            new_store.Save(fileStream, passwd, new SecureRandom());
            fileStream.Close();

        }
        public X509Certificate BuildSimpleCertificateFromRequest(string req_n,X509Name issuer_name)
        {
            CreateCAKeys(2048);
            FileStream fileStream = new FileStream(req_n, FileMode.Open);
            Pkcs10CertificationRequest r = new Pkcs10CertificationRequest(fileStream);
            CertificationRequestInfo info = r.GetCertificationRequestInfo();

            AsymmetricKeyParameter pk = r.GetPublicKey();
            bool v = r.Verify();
            X509Name subject = info.Subject;
            X509Name issuer = issuer_name;

            //set key usage to signature for this certificate            
            KeyUsage ku = new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment);
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;
            BasicConstraints bc = new BasicConstraints(false);
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
            BigInteger snr = new BigInteger(128, new SecureRandom());
            certGen.SetSerialNumber(snr);
            certGen.SetIssuerDN(issuer);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            certGen.SetSubjectDN(subject);
            certGen.SetPublicKey(pk);
            //certGen.SetSignatureAlgorithm("SHA256WITHRSAENCRYPTION");

            //add extensions to certificate which are marked as not critical           
            certGen.AddExtension(oidKu, false, ku);            
            certGen.AddExtension(oidBc, false, abc);

            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            X509Certificate cert = certGen.Generate(sf);
            return cert;

        }
        public X509Certificate BuildCertificateFromRequest(string req_n,string cert_n)
        {
            FileStream fileStream = new FileStream(req_n, FileMode.Open);
            Pkcs10CertificationRequest r= new Pkcs10CertificationRequest(fileStream);
            CertificationRequestInfo info = r.GetCertificationRequestInfo();
            
            AsymmetricKeyParameter pk = r.GetPublicKey();
            bool v = r.Verify();
            X509Name subject = info.Subject;
            X509Name issuer = CACertificate.SubjectDN; 

            //set key usage to signature for this certificate            
            KeyUsage ku = new KeyUsage(KeyUsage.DigitalSignature | KeyUsage.NonRepudiation | KeyUsage.DataEncipherment);
            Asn1Object ao = ku.ToAsn1Object();
            DerObjectIdentifier oidKu = X509Extensions.KeyUsage;

            //add public key identifier to certificate            
            SubjectKeyIdentifierStructure ski = new SubjectKeyIdentifierStructure(pk);

            DerObjectIdentifier oidSki = X509Extensions.SubjectKeyIdentifier;
            //add issuer key identifier to certificate
            AuthorityKeyIdentifierStructure iki = new AuthorityKeyIdentifierStructure(CAKeyPair.Public);
            DerObjectIdentifier oidIki = X509Extensions.AuthorityKeyIdentifier;

            //basic constraints set to CA = false;
            BasicConstraints bc = new BasicConstraints(false);
            Asn1Object abc = bc.ToAsn1Object();
            DerObjectIdentifier oidBc = X509Extensions.BasicConstraints;

            X509V3CertificateGenerator certGen = new X509V3CertificateGenerator();
            BigInteger snr = new BigInteger(128, new SecureRandom());
            certGen.SetSerialNumber(snr);
            certGen.SetIssuerDN(issuer);
            certGen.SetNotBefore(DateTime.UtcNow.AddDays(-1));
            certGen.SetNotAfter(DateTime.UtcNow.AddDays(5000));
            certGen.SetSubjectDN(subject);
            certGen.SetPublicKey(pk);
            //certGen.SetSignatureAlgorithm("SHA256WITHRSAENCRYPTION");

            //add extensions to certificate which are marked as not critical           
            certGen.AddExtension(oidKu, false, ku);
            certGen.AddExtension(oidSki, false, ski);
            certGen.AddExtension(oidIki, false, iki);
            certGen.AddExtension(oidBc, false, abc);

            Asn1SignatureFactory sf = new Asn1SignatureFactory("SHA256WITHRSAENCRYPTION", CAKeyPair.Private);
            X509Certificate cert= certGen.Generate(sf);
            byte[] encoded_cert=cert.GetEncoded();
            byte[] sig = cert.GetSignature();
            if (File.Exists(cert_n) == true)
                File.Delete(cert_n);
            FileStream writeStream = new FileStream(cert_n, FileMode.Create);
            writeStream.Write(encoded_cert, 0, encoded_cert.Length);
            writeStream.Close();

            FileStream rs = new FileStream(cert_n, FileMode.Open);
            X509CertificateParser p = new X509CertificateParser();
            X509Certificate c= p.ReadCertificate(rs);
            c.Verify(CAKeyPair.Public);
            fileStream.Close();
            File.Delete(req_n);
            return c;
        }
        public string Thumbprint(Org.BouncyCastle.X509.X509Certificate c) 
        {
            Sha1Digest digest = new Sha1Digest();
            byte[] der = c.GetEncoded();            
            byte[] resBuf = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(der, 0, der.Length);
            digest.DoFinal(resBuf, 0);
            string thumbprint = Hex.ToHexString(resBuf);
            return thumbprint;
        }
    }
}
