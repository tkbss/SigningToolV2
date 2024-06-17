using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace TracingModule
{
    public class TraceDataTable
    {
        public static string DT_TRACE = "TracingData";
        public static string DT_PACKAGE_TRACE = "PackageData";
        
        
        public static DataTable GetTable(string TableName)
        {
            DataTable dt = new DataTable(TableName);
            return dt;
            //string constr = Settings.Default.TracingDatabaseConnectionString;
            //string cmdSelect = "SELECT * FROM "+TableName;
            //using (SqlConnection con = new SqlConnection(constr))
            //{

            //    SqlCommand cmd = new SqlCommand(cmdSelect, con);
            //    SqlDataAdapter sa = new SqlDataAdapter(cmd);
            //    DataTable dt = new DataTable(TableName);
            //    sa.Fill(dt);
            //    return dt;
            //}
        }
        public static int GetIndex(DataTable dt)
        {

            //DataRow row = dt.Select("Index=MAX(Index)")[0];
            //int nextId = (int)row["Index"];
            //return nextId;
            return 1;
        }
        //public static TracePackageData ReadPackage(int index)
        //{
        //    return TracingDataContext.ReadPackage(index);
        //}
        //public static DataTable GetNames()
        //{
        //    string constr = Settings.Default.TracingDatabaseConnectionString;
        //    string cmdSelect = "SELECT DISTINCT Prename,Name FROM TracingData";
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {

        //        SqlCommand cmd = new SqlCommand(cmdSelect, con);
        //        SqlDataAdapter sa = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable(DT_TRACE);
        //        sa.Fill(dt);
        //        return dt;
        //    }
        //}
        //public static DataTable GetOperations()
        //{
        //    string constr = Settings.Default.TracingDatabaseConnectionString;
        //    string cmdSelect = "SELECT DISTINCT Operation FROM TracingData";
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {

        //        SqlCommand cmd = new SqlCommand(cmdSelect, con);
        //        SqlDataAdapter sa = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable(DT_TRACE);
        //        sa.Fill(dt);
        //        return dt;
        //    }
        //}
        //public static DataTable SelectData(string pn,string n,string op,string sid,bool time_range,DateTime StartDate,DateTime EndDate)
        //{
        //    string query = string.Empty;            
        //    string[] attributes = new string[] { "Prename", "Name","Operation","SessionId" };
        //    string[] values = new string[] { pn, n, op, sid };
        //    for (int i = 0; i < 4; i++)
        //    {
        //        if (string.IsNullOrEmpty(query) == false)
        //        {
        //            if (string.IsNullOrEmpty(values[i]) == false)
        //                query = query + " AND " + attributes[i]+ "='" + values[i] + "'";
        //        }
        //        else
        //        {
        //            if (string.IsNullOrEmpty(values[i]) == false)
        //                query = attributes[i]+"='" + values[i] + "'";
        //        }
        //    }
        //    string constr = Settings.Default.TracingDatabaseConnectionString;
        //    string cmdSelect;
        //    if(string.IsNullOrEmpty(query)==true)
        //        cmdSelect = "SELECT * FROM TracingData";
        //    else
        //        cmdSelect = "SELECT * FROM TracingData WHERE "+query;
        //    if (time_range == true)
        //    {
        //        if (string.IsNullOrEmpty(query) == true)
        //            cmdSelect = cmdSelect + " WHERE SessionDate>=@sd AND SessionDate<=@ed";
        //        else
        //            cmdSelect = cmdSelect + " AND SessionDate>=@sd AND SessionDate<=@ed";
        //    }
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {

        //        SqlCommand cmd = new SqlCommand(cmdSelect, con);
        //        if(time_range == true)
        //        {
        //            SqlParameter p;
        //            p = cmd.Parameters.Add("@sd", SqlDbType.DateTime);
        //            p.Value = StartDate;
        //            p = cmd.Parameters.Add("@ed", SqlDbType.DateTime);
        //            p.Value = EndDate;
        //        }
        //        SqlDataAdapter sa = new SqlDataAdapter(cmd);
        //        DataTable dt = new DataTable(DT_TRACE);
        //        sa.Fill(dt);
        //        return dt;
        //    }

        //}


    }
}
