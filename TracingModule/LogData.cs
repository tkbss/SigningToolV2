using Infrastructure;
using System;


namespace TracingModule
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

        public int Log()
        {
            TraceData td = new TraceData();
            td.Prename = FirstName;
            td.Name = Name;
            td.Operation = OP_CONV(Operation);
            td.Enviroment = Converter.Env(e);
            td.SessionDate = SessionDate;
            td.Operation_Result = RES_CONV(OperationResult);
            td.Signer_Type = Converter.Signer(SignerType);
            td.Signer = Converter.Manu(Signer);
            td.StoreType = Converter.ST(StoreType);
            td.ErrorMessage = ErrorMessage;
            td.SessionId = SessionId;
            //TracingDataContext.Trace(td);
            int index = 0;//TraceDataTable.GetIndex(TraceDataTable.GetTable(TraceDataTable.DT_TRACE));
            return index;
        }
        public void LogPackage(TracePackageData pd,int TraceDataIndex)
        {
            try
            {
                //TracingDataContext.TracePackage(pd);
                //int PackageDataIndex = TraceDataTable.GetIndex(TraceDataTable.GetTable(TraceDataTable.DT_PACKAGE_TRACE));
                //TracingDataContext.UpdatePackageReference(TraceDataIndex, PackageDataIndex);
            }
            catch { }
        }
        public void SetSessionData(string fn,string n,DateTime sd)
        {
            FirstName = fn;
            Name = n;
            SessionDate = sd;
            int index = TraceDataTable.GetIndex(TraceDataTable.GetTable(TraceDataTable.DT_TRACE))+1;
            SessionId = Convert.ToString(index);
        }
        
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
