using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Converter
    {
        static string[] manu = Manufacturer.manu;
        static string[] signer = { "SIX-QA", "SIX-ATM", "MANU", "SIX-ATM-DEVICE" };
        static string[] manu_abr = { "MVS", "SIX", "NCR", "WNI", "GGS", "DSB","MSIX","PRE","SUZ" };

        static MANUFACTURER[] manus = { MANUFACTURER.SIX, MANUFACTURER.DIEBOLD, MANUFACTURER.GLORY, MANUFACTURER.MVS, 
            MANUFACTURER.NCR, MANUFACTURER.WINCOR,MANUFACTURER.MSIX,MANUFACTURER.PREMA,MANUFACTURER.SUZOH,MANUFACTURER.MVSDN };
        public static MANUFACTURER[] Manufactures
        {
            get
            {
                return manus;
            }
        }
        public static string[] ManufacturesString
        {
            get
            {
                return manu;
            }
        }
        public static string Abrevation(string manu) 
        {
            var r = Manufacturer.ManuAbrMapping.First(p => p.Key == manu);
            return r.Value;
        }
        static public string Vendor(string abr)
        {            
            var r = Manufacturer.ManuAbrMapping.First(p=>p.Value==abr);
            return r.Key;
        }
        public static SIGNER Signer(string sig)
        {
            if (sig == "ATM-DEVICE")
                return SIGNER.ATM_DEVICE;
            if (sig == "SIX-ATM-DEVICE")
                return SIGNER.ATM_DEVICE;
            if (sig == "SIX-ATM")
                return SIGNER.ATM;
            if (sig == "SIX-QA")
                return SIGNER.QA;
            if (sig == "ATM")
                return SIGNER.ATM;
            if (sig == "QA")
                return SIGNER.QA;
            if (sig == "MANU")
                return SIGNER.MANU;
            else
                return SIGNER.MANU;
        }
        public static string Signer(SIGNER sig)
        {
            switch(sig)
            {
                case SIGNER.ATM:
                    return signer[1];
                case SIGNER.QA:
                    return signer[0];
                case SIGNER.MANU:
                    return signer[2];
                case SIGNER.ATM_DEVICE:
                    return signer[3];
            }
            return "";
        }
        public static MANUFACTURER Manu(string manu)
        {
            
            switch (manu)
            {
                
                case "SIX":
                    return MANUFACTURER.SIX;                    
                case "WINCOR":
                   return MANUFACTURER.WINCOR;                    
                case "DIEBOLD":
                   return MANUFACTURER.DIEBOLD;                    
                case "NCR":
                    return MANUFACTURER.NCR;                    
                case "GLORY":
                    return MANUFACTURER.GLORY;                   
                case "MVS":
                    return MANUFACTURER.MVS;
                case "MSIX":
                    return MANUFACTURER.MSIX;
                case "PREMA":
                    return MANUFACTURER.PREMA;
                case "SUZOH":
                    return MANUFACTURER.SUZOH;
                case "MVSDN":
                    return MANUFACTURER.MVSDN;

            }
            throw new InvalidOperationException("wrong manufacturer");
        }
        public static string Manu(MANUFACTURER m)
        {
           
            switch (m)
            {
                case MANUFACTURER.SIX:
                    return "SIX";                   
                case MANUFACTURER.WINCOR:
                    return manu[0];
                case MANUFACTURER.DIEBOLD:
                    return manu[1];                   
                case MANUFACTURER.NCR:
                    return manu[2];                  
                case MANUFACTURER.GLORY:
                   return manu[3];
                case MANUFACTURER.MVS:
                    return manu[4];
                case MANUFACTURER.MSIX:
                    return manu[5];
                case MANUFACTURER.PREMA:
                    return manu[6];
                case MANUFACTURER.SUZOH:
                    return manu[7];
                case MANUFACTURER.MVSDN:
                    return manu[8];
            }
            return string.Empty;
        }
        public static ENVIROMENT Env(string env)
        {
            ENVIROMENT e = ENVIROMENT.TEST;
            switch (env)
            {
                case "TEST":
                    e = ENVIROMENT.TEST;
                    break;
                case "PROD":
                    e = ENVIROMENT.PROD;
                    break;
            }
            return e;
        }
        public static string Env(ENVIROMENT env)
        {
            
            switch (env)
            {
                case ENVIROMENT.TEST:
                   return "TEST";
                case ENVIROMENT.PROD:
                    return "PROD";
            }
            return string.Empty;
        }

        public static STORETYPE ST(string st)
        {
            STORETYPE m = STORETYPE.UNMANAGED;
            switch (st)
            {
                case "UNMANAGED":
                    m = STORETYPE.UNMANAGED;
                    break;
                case "KMS":
                    m = STORETYPE.KMS;
                    break;
            }
            return m;
        }
        public static string ST(STORETYPE st)
        {
            
            switch (st)
            {
                case STORETYPE.UNMANAGED:
                    return "UNMANAGED";
                case STORETYPE.KMS:
                    return "KMS";
            }
            return string.Empty;
           
        }
        public static string[] SplitManuCertype(string manu_cert)
        {
            char[] delimiter = { '-' };
            string[] r = manu_cert.Split(delimiter);
            if (r.Length == 1)
            {
                string[] m_c = new string[2];
                m_c[0] = r[0];
                m_c[1] = "MANU";
                return m_c;
            }                
            if(r[0] == signer[2])
            {
                string[] s = new string[2];
                s[0] = r[1];
                s[1] = "MANU";
                return s;
            }
            return r;
        }
        public static string CertType(CERTTYPE ct)
        {
            string st = "ATM";
            switch (ct)
            {
                case CERTTYPE.ATM:
                    st = "ATM";
                    break;
                case CERTTYPE.QA:
                    st = "QA";
                    break;
                case CERTTYPE.CA:
                    st = "CA";
                    break;
                case CERTTYPE.MANU:
                    st = "MANU";
                    break;
            }
            return st;
        }
        public static CERTTYPE CertType(string s)
        {
            CERTTYPE st = CERTTYPE.ATM;
            switch (s)
            {
                case "ATM":
                    st = CERTTYPE.ATM;
                    break;
                case "QA":
                    st = CERTTYPE.QA;
                    break;
                case "CA":
                    st = CERTTYPE.CA;
                    break;
                case "MANU":
                    st = CERTTYPE.MANU;
                    break;
            }
            return st;
        }
        /// <summary>
        /// Constant hex digits array.
        /// </summary>
        static char[] hexDigits = { '0', '1', '2', '3', '4', '5', '6', '7',
                                    '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        /// <summary>
        /// Convert a byte array to hex string.
        /// </summary>
        /// <param name="bytes">source array.</param>
        /// <returns>hex string.</returns>
		public static string ToHexString(byte[] bytes)
        {
            if (bytes == null) return "";
            char[] chars = new char[bytes.Length * 2];
            int b, i;
            for (i = 0; i < bytes.Length; i++)
            {
                b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }
        /// <summary>
        /// Convert hex string to byte array.
        /// </summary>
        /// <param name="hexStr">Source hex string.</param>
        /// <returns>return byte array.</returns>
        public static byte[] HexStrToBytes(string hexStr)
        {
            hexStr = hexStr.Replace(" ", "");
            hexStr = hexStr.Replace("\r", "");
            hexStr = hexStr.Replace("\n", "");
            hexStr = hexStr.ToUpper();
            if ((hexStr.Length % 2) != 0) throw new Exception("Invalid Hex string: odd length.");
            int i;
            for (i = 0; i < hexStr.Length; i++)
            {
                if (!IsValidHexDigits(hexStr[i]))
                {
                    throw new Exception("Invalid Hex string: included invalid character [" +
                        hexStr[i] + "]");
                }
            }
            int bc = hexStr.Length / 2;
            byte[] retval = new byte[bc];
            int b1, b2, b;
            for (i = 0; i < bc; i++)
            {
                b1 = GetHexDigitsVal(hexStr[i * 2]);
                b2 = GetHexDigitsVal(hexStr[i * 2 + 1]);
                b = ((b1 << 4) | b2);
                retval[i] = (byte)b;
            }
            return retval;
        }
        /// <summary>
        /// Check if the source string is a valid hex string.
        /// </summary>
        /// <param name="hexStr">source string.</param>
        /// <returns>true:Valid, false:Invalid.</returns>
        public static bool IsHexStr(string hexStr)
        {
            byte[] bytes = null;
            try
            {
                bytes = HexStrToBytes(hexStr);
            }
            catch
            {
                return false;
            }
            if (bytes == null || bytes.Length < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        /// <summary>
        /// Get hex digits value.
        /// </summary>
        /// <param name="ch">source character.</param>
        /// <returns>hex digits value.</returns>
        public static byte GetHexDigitsVal(char ch)
        {
            byte retval = 0;
            for (int i = 0; i < hexDigits.Length; i++)
            {
                if (hexDigits[i] == ch)
                {
                    retval = (byte)i;
                    break;
                }
            }
            return retval;
        }
        /// <summary>
        /// Check if the character is a valid hex digits.
        /// </summary>
        /// <param name="ch">source character.</param>
        /// <returns>true:Valid, false:Invalid.</returns>
        public static bool IsValidHexDigits(char ch)
        {
            bool retval = false;
            for (int i = 0; i < hexDigits.Length; i++)
            {
                if (hexDigits[i] == ch)
                {
                    retval = true;
                    break;
                }
            }
            return retval;
        }
    }
}
