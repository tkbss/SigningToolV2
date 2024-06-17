using Infrastructure.Interfaces;
using Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Security
{
    public class SecurityFactory
    {
        public static ISecurity CreateSecurity(STORETYPE kt,ENVIROMENT e,IUnityContainer container)
        {
            ISecurity s = null;
            switch (kt)
            {
                case STORETYPE.UNMANAGED:
                    s = new UnmanagedSecurityProvider(container) as ISecurity;
                    break;
                case STORETYPE.KMS:
                    s = new KMSSecurityProvider(e,container) as ISecurity;
                    break;
            }
            return s;
           }
        
    }
}
