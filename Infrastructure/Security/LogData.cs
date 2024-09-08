using Infrastructure;
using System;


namespace SoftwareSigning.Model
{
    public class LogData
    {
        public enum OPERATION {LOGIN,SIGNING,VERIFICATION,IMPORT,EXPORT,KEY_GEN,MANU_CERT_SIGN,REMOVE,PKCS10_REQ }
        public enum RESULT { LOGIN_SUCCESS,LOGIN_ERROR,P_VERIFY_SUCCESS, P_VERIFY_ERROR,IMPORT_ERROR, IMPORT_SUCCESS,
                              EXPORT_SUCCESS,EXPORT_ERROR,SIGNING_ERROR,SIGNING_SUCCESS,REMOVE_SUCCESS,SUCCESS,ERROR}

        public OPERATION Operation { get; set; }
        public ENVIROMENT e { get; set; }
        public STORETYPE StoreType{ get; set; }

        public string FirstName { get; set; }
        public string SessionId { get; set; }
        public string Name { get; set; }
        public DateTime SessionDate { get; set; }
        public RESULT OperationResult { get; set; }

        public SIGNER SignerType { get; set; }

        public MANUFACTURER Signer { get; set; }

        public string ErrorMessage { get; set; }

        
        
        private string RES_CONV(RESULT r)
        {
            switch(r)
            {
                case RESULT.LOGIN_ERROR:
                    return "ERROR";
                case RESULT.LOGIN_SUCCESS:
                    return "SUCCESS";
                case RESULT.P_VERIFY_SUCCESS:
                    return "SUCCESS";
                case RESULT.P_VERIFY_ERROR:
                    return "ERROR";
                case RESULT.IMPORT_SUCCESS:
                    return "SUCCESS";
                case RESULT.REMOVE_SUCCESS:
                    return "SUCCESS";
                case RESULT.IMPORT_ERROR:
                    return "ERROR";
                case RESULT.EXPORT_ERROR:
                    return "ERROR";
                case RESULT.EXPORT_SUCCESS:
                    return "SUCCESS";
                case RESULT.ERROR:
                    return "ERROR";
                case RESULT.SUCCESS:
                    return "SUCCESS";
                case RESULT.SIGNING_SUCCESS:
                    return "SUCCESS";
                case RESULT.SIGNING_ERROR:
                    return "ERROR";
                    
            }
            return string.Empty;
        }
        private string OP_CONV(OPERATION o)
        {
            switch (o)
            {
                case OPERATION.EXPORT:
                    return "EXPORT";
                case OPERATION.IMPORT:
                    return "IMPORT";
                case OPERATION.KEY_GEN:
                    return "KEY_GENERATION";
                case OPERATION.LOGIN:
                    return "LOGIN";
                case OPERATION.MANU_CERT_SIGN:
                    return "MANU_CERT_SIGN";
                case OPERATION.SIGNING:
                    return "SIGNING";
                case OPERATION.VERIFICATION:
                    return "VERIFICATION";
                case OPERATION.REMOVE:
                    return "REMOVE_PACKAGE";
                case OPERATION.PKCS10_REQ:
                    return "PKCS10_REQ";
            }
            return string.Empty;
        }
    }
}
