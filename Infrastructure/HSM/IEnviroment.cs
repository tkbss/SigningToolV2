using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.HSM
{
    public interface IEnviroment
    {
        int GetSlot();
        string PublicKey(int slot,string cert_type,string env);
        string SigningKeyName(string cert_type,string env);

        string Sign(int slot, byte[] data, string kn);
        bool IsConnected();
        bool TokenIsAvailable();
    }
}
