using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    //public class HSMProdEnviroment : HSMBaseEnviroment, IEnviroment
    //{
    //    protected HSMProdKeys pk = new HSMProdKeys();
    //    public static string default_atm_prod_ip = "10.152.16.36 10.152.144.45";

    //    public List<KEY_STATUS> KeyStatus
    //    {
    //        get
    //        {
    //            return pk.KeyStatus;
    //        }
    //    }
    //    public HSMProdEnviroment()
    //    {
    //        SetIPEnviroment();
    //        IpStatus(cfa.CK_ATM_IP_ADR, default_atm_prod_ip);
    //        SetUpSlots();
    //    }
    //    public void DetermineKeyStatus()
    //    {
    //        var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
    //        pk.DetermineKeyStatus(s.Slot);
    //    }
    //    public bool CheckPwd(string pwd)
    //    {
    //        var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
    //        pk.TokenPwd = pwd;
    //        int r = pk.OpenSession(s.Slot);
    //        pk.CloseSession();
    //        if (r != 0)
    //        {
    //            pk.TokenPwd = string.Empty;
    //            return false;
    //        }
    //        else
    //            return true;

    //    }

    //    public string PublicKey(int slot, string cert_type)
    //    {
    //        string kn = pk.DeterminePublicKeyName(cert_type);
    //        if (pk.OpenSession(slot) > 0)
    //            return string.Empty;
    //        string key = InteropHSM.ReadPublicKey(kn);
    //        pk.CloseSession();
    //        return key;
    //    }

    //    public string SigningKeyName(string cert_type)
    //    {
    //        return pk.DetermineSigningKeyName(cert_type);
    //    }

    //    public string Sign(int slot, byte[] data, string kn)
    //    {
    //        return pk.Sign(slot, data, kn);
    //    }

    //    public bool IsConnected()
    //    {
    //        try
    //        {
    //            var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED").First();
    //            if (s == null)
    //                return false;
    //            else
    //                return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public bool TokenIsAvailable()
    //    {
    //        try
    //        {
    //            var s = HSMStatus.Values.Where(e => e.Connection == "CONNECTED" && e.Slot >= 0).First();
    //            if (s == null)
    //                return false;
    //            else
    //                return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }
    //}
}
