using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public enum SIGNER { MANU,QA,ATM,ATM_DEVICE};
    public enum MANUFACTURER { SIX, NCR, DIEBOLD,WINCOR,GLORY,MVS,MSIX,PREMA,SUZOH,MVSDN };
    public enum ENVIROMENT { TEST, PROD };
    public enum STORETYPE { UNMANAGED, KMS };
    public enum CERTTYPE { CA, MANU, QA, ATM };
}
