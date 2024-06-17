using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LipingShare.LCLib.Asn1Processor;
using Org.BouncyCastle.Math;
using System.IO;


namespace Infrastructure.cms
{
    public abstract class Asn1Object
    {
        public abstract void Decode(byte[] DEREncoded);
        public abstract byte[] Encode();
        protected string m_oid = string.Empty;
        protected Asn1Node m_root = new Asn1Node();
        public string OID
        {
            get
            {
                return m_oid;
            }
        }
        protected void MakeRootSequence()
        {
            m_root.ClearAll();
            m_root.Tag = Asn1Tag.SEQUENCE | Asn1TagClasses.CONSTRUCTED;
        }
        protected void MakeNamedRootSequence(Asn1Node n)
        {
            m_root.ClearAll();
            m_root.Tag = Asn1Tag.SEQUENCE | Asn1TagClasses.CONSTRUCTED;
            AddOID();
            m_root.AddChild(n);
        }
        protected Asn1Node MakeSequence()
        {
            Asn1Node node = new Asn1Node();
            node.Tag = Asn1Tag.SEQUENCE | Asn1TagClasses.CONSTRUCTED;
            return node;
        }
        protected Asn1Node MakeOID(string od)
        {
            Oid o = new Oid();
            byte[] boid = o.Encode(od);
            Asn1Node oid = new Asn1Node();
            oid.Tag = Asn1Tag.OBJECT_IDENTIFIER;
            oid.Data = boid;
            return oid;
        }
        protected Asn1Node MakeInteger(int v)
        {
            Asn1Node node = new Asn1Node();
            node.Tag = Asn1Tag.INTEGER;
            node.Data = ConvertToByteArray(v);
            return node;
        }
        protected Asn1Node MakeOctet(byte[] v)
        {
            Asn1Node node = new Asn1Node();
            node.Tag = Asn1Tag.OCTET_STRING;
            node.Data = v;
            return node;
        }
        protected Asn1Node MakeSet()
        {
            Asn1Node node = new Asn1Node();
            node.Tag = Asn1Tag.SET | Asn1TagClasses.CONSTRUCTED;
            return node;
        }
        protected void AddOID()
        {
            Oid o = new Oid();
            byte[] boid = o.Encode(OID);
            Asn1Node oid = new Asn1Node();
            oid.Tag = Asn1Tag.OBJECT_IDENTIFIER;
            oid.Data = boid;
            m_root.AddChild(oid);
        }
        protected bool LoadData(byte[] DEREncoded)
        {
            return m_root.LoadData(DEREncoded);
        }
        protected bool CheckOID()
        {      
            Asn1Node NodeOid= m_root.GetChildNode(0);
            if (NodeOid == null)
                return false;
            if(NodeOid.Tag!=Asn1Tag.OBJECT_IDENTIFIER)
                return false;
            Oid oid = new Oid();
            string strOid = oid.Decode(NodeOid.Data);
            if(strOid.Equals(m_oid)==false)
                return false;
            return true;
        }
        protected byte[] ConvertToByteArray(int intValue)
        {
            byte[] integer = BigInteger.ValueOf(intValue).ToByteArray();
            return integer;
            
        }
        protected byte[] DEREncodeNode(Asn1Node n)
        {
            MemoryStream lengthStream = new MemoryStream();
            n.SaveData(lengthStream);
            byte[] t = lengthStream.ToArray();
            return t;
        }
        protected byte[] DEREncodeRootNode()
        {
            MemoryStream lengthStream = new MemoryStream();
            m_root.SaveData(lengthStream);
            byte[] t = lengthStream.ToArray();
            return t;            
        }
        public bool CompareObjectName(string oid)
        {
            if (this.m_oid == oid)
                return true;
            else
                return false;
        }
    }
}
