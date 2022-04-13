using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections;
using Microsoft.Extensions.Configuration;
using CMAdmin.API.Helpers;

namespace CMAdmin.API.Data
{
    public class DBAccess
    {
        private SqlConnection OBJConn = null;
        private SqlTransaction Trans = null;
        private int liCommandTimeout = 0;
        public StringBuilder SqlBuffer = new StringBuilder();
        public string SessionKey = "";
        private readonly string _connectionString;
        private readonly string _SQLQueryTimeout;
        public DBAccess()
        {
            _connectionString = Config.MCQBConnectionString;
            try
            {
                _connectionString = CommonLibrary.Security.lfnDecryptString(Config.MCQBConnectionString);
            }
            catch
            {
                _connectionString = Config.MCQBConnectionString; ;
            }
            _SQLQueryTimeout = "30";
            ConnectDB();
        }

        public void ConnectDB()
        {
            DateTime dtStart = DateTime.Now;
            if (isConnectionOpen())
                return;
            SQLOutMessage(EventNames.SQLDBCONNECT.ToString(), dtStart, "");

            //OBJConn = new SqlConnection(Config.MCQBConnectionString);
            OBJConn = new SqlConnection(_connectionString);
            OBJConn.Open();

            DateTime dtEnd = DateTime.Now;
            if (_SQLQueryTimeout != "")
                liCommandTimeout = Convert.ToInt32(_SQLQueryTimeout);
            SQLInMessage(EventNames.SQLDBCONNECT.ToString(), dtStart, dtEnd);
        }

        public void ConnectDB(string ConnString)
        {
            DateTime dtStart = DateTime.Now;
            if (isConnectionOpen())
                return;
            SQLOutMessage(EventNames.SQLDBCONNECT.ToString(), dtStart, "");
            OBJConn.Open();
            DateTime dtEnd = DateTime.Now;
            if (_SQLQueryTimeout != "")
                liCommandTimeout = Convert.ToInt32(_SQLQueryTimeout);
            SQLInMessage(EventNames.SQLDBCONNECT.ToString(), dtStart, dtEnd);
        }
        public void BeginTransaction()
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQLTRANSBEGIN.ToString(), dtStart, "");
            Trans = OBJConn.BeginTransaction();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQLTRANSBEGIN.ToString(), dtStart, dtEnd);
        }
        public void CommitTransaction()
        {
            if (Trans != null)
            {
                DateTime dtStart = DateTime.Now;
                SQLOutMessage(EventNames.SQLTRANSCOMMIT.ToString(), dtStart, "");
                Trans.Commit();
                DateTime dtEnd = DateTime.Now;
                Trans = null;
                SQLInMessage(EventNames.SQLTRANSCOMMIT.ToString(), dtStart, dtEnd);
            }
        }
        public void RollbackTransaction()
        {
            if (Trans != null)
            {
                DateTime dtStart = DateTime.Now;
                SQLOutMessage(EventNames.ROLLBACKTRANS.ToString(), dtStart, "");
                Trans.Rollback();
                DateTime dtEnd = DateTime.Now;
                Trans = null;
                SQLInMessage(EventNames.ROLLBACKTRANS.ToString(), dtStart, dtEnd);
            }
        }
        public void CloseDB()
        {
            if (OBJConn != null)
            {
                if (Trans != null)
                {
                    Trans.Rollback();
                    Trans = null;
                }
                DateTime dtStart = DateTime.Now;
                SQLOutMessage(EventNames.SQLDBCLOSE.ToString(), dtStart, "");
                OBJConn.Close();
                DateTime dtEnd = DateTime.Now;
                SQLInMessage(EventNames.SQLDBCLOSE.ToString(), dtStart, dtEnd);
                // CMAdmin.LogWriter.WriteSQLLog(this.SqlBuffer.ToString());
                OBJConn.Dispose();
                OBJConn = null;
            }
        }
        public bool isConnectionOpen()
        {
            if (OBJConn == null) return false;

            if (OBJConn.State == ConnectionState.Open)
                return true;
            else
                return false;
        }
        public DataTable lfnGetDataTable(string lsOleDb)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlDataAdapter OBJDataAdapter;
            OBJDataAdapter = new SqlDataAdapter(lsOleDb, OBJConn);
            DataSet OBJDataSet = new DataSet();
            if (Trans != null)
                OBJDataAdapter.SelectCommand.Transaction = Trans;
            OBJDataAdapter.SelectCommand.CommandTimeout = liCommandTimeout;
            OBJDataAdapter.Fill(OBJDataSet, "tblTEMP");
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);

            return OBJDataSet.Tables["tblTEMP"];
        }

        public DataTable lfnGetDataTable(string lsOleDb, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlDataReader OBJDataReader = null;
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandTimeout = 0;
            string str = OBJCommand.CommandText;

            OBJDataReader = OBJCommand.ExecuteReader();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;

            DataTable OBJDataTable = new DataTable();
            OBJDataTable.Load(OBJDataReader);

            return OBJDataTable;
        }

        public DataTable lfnGetDataTableProcedure(string ProcedureName, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, ProcedureName);

            SqlDataReader OBJDataReader = null;
            SqlCommand OBJCommand = new SqlCommand(ProcedureName, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandTimeout = 0;
            string str = OBJCommand.CommandText;
            OBJCommand.CommandType = System.Data.CommandType.StoredProcedure;
            OBJDataReader = OBJCommand.ExecuteReader();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;

            DataTable OBJDataTable = new DataTable();
            OBJDataTable.Load(OBJDataReader);

            return OBJDataTable;
        }

        public DataSet lfnGetDataSetFromProcedure(string ProcedureName, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, ProcedureName);

            SqlDataAdapter objDataAdapter = new SqlDataAdapter();

            SqlCommand OBJCommand = new SqlCommand(ProcedureName, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;

            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);

            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandTimeout = 120;
            string str = OBJCommand.CommandText;
            OBJCommand.CommandType = System.Data.CommandType.StoredProcedure;

            //OBJDataReader = OBJCommand.ExecuteReader();
            objDataAdapter.SelectCommand = OBJCommand;

            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;

            DataSet objDataSet = new DataSet();
            objDataAdapter.Fill(objDataSet);

            return objDataSet;
        }

        public void lfnGetDataSet(string lsOleDb, string TableName, out DataSet OBJDataSet)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            DataSet OBJDataSet1 = new DataSet();
            SqlDataAdapter OBJDataAdapter;
            OBJDataAdapter = new SqlDataAdapter(lsOleDb, OBJConn);
            if (Trans != null)
                OBJDataAdapter.SelectCommand.Transaction = Trans;
            OBJDataAdapter.SelectCommand.CommandTimeout = liCommandTimeout;

            OBJDataAdapter.Fill(OBJDataSet1, TableName);
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            OBJDataSet = OBJDataSet1;
            OBJDataSet1.Dispose();
            OBJDataAdapter.Dispose();
        }

        public DataSet lfnGetDataSetSP(string SpName, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, SpName);

            SqlDataAdapter OBJDataAdapter;
            SqlCommand OBJCommand = new SqlCommand(SpName, OBJConn);
            OBJCommand.CommandType = CommandType.StoredProcedure;
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);

            OBJDataAdapter = new SqlDataAdapter(OBJCommand);
            DataSet OBJDataSet = new DataSet();
            if (Trans != null)
                OBJDataAdapter.SelectCommand.Transaction = Trans;
            OBJDataAdapter.SelectCommand.CommandTimeout = liCommandTimeout;
            OBJDataAdapter.Fill(OBJDataSet);
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            return OBJDataSet;
        }

        public DataTable lfnGetDataTable(string lsOleDb, string TableName)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlDataAdapter OBJDataAdapter;
            OBJDataAdapter = new SqlDataAdapter(lsOleDb, OBJConn);
            DataTable OBJDataTable = new DataTable(TableName);
            if (Trans != null)
                OBJDataAdapter.SelectCommand.Transaction = Trans;
            OBJDataAdapter.SelectCommand.CommandTimeout = liCommandTimeout;

            OBJDataAdapter.Fill(OBJDataTable);
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            return OBJDataTable;
        }
        public SqlCommand lfnGetCommandObject(string lsOleDb)
        {
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            return OBJCommand;
        }

        #region UpdateData Method Overloads
        public void lfnUpdateData(string lsOleDb, out object Identity)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            Identity = OBJCommand.ExecuteScalar();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
        }
        public void lfnUpdateData(string lsOleDb)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            OBJCommand.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
        }
        public void lfnUpdateData(string lsOleDb, out int NoOfRowsUpdated)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            NoOfRowsUpdated = OBJCommand.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
        }
        public void lfnUpdateData(string lsOleDb, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;
        }
        public void lfnUpdateData(string lsOleDb, ArrayList ParameterObjectsArrayList, string stroreProc)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandType = System.Data.CommandType.StoredProcedure;
            OBJCommand.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;
        }
        public void lfnUpdateData(string lsOleDb, ArrayList ParameterObjectsArrayList, out object Identity)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            Identity = OBJCommand.ExecuteScalar();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;
        }
        public void lfnUpdateData(string lsOleDb, ArrayList ParameterObjectArrayList, System.Data.ParameterDirection Direction)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            for (int i = 0; i < ParameterObjectArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;

            OBJCommand.ExecuteNonQuery();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectArrayList.Clear();
            ParameterObjectArrayList = null;
        }
        public void lfnUpdateData(string sTableName, string[] sColName, string[] sColValue, string sCondition)
        {
            string lsOleDb, lsUpdateColumns = "";
            lsOleDb = "UPDATE " + sTableName + " SET ";

            for (int i = 0; i < sColName.Length; i++)
                lfnColumnsForUpdate(sColName[i], sColValue[i], ref lsUpdateColumns);
            if (lsUpdateColumns.Trim().Length > 0)
            {
                lsOleDb = lsOleDb + lsUpdateColumns.Trim().TrimStart(',').TrimEnd(',') + " WHERE " + sCondition;
                lfnUpdateData(lsOleDb);
            }
        }
        #endregion
        private void lfnColumnsForUpdate(string sColumnName, string sColumnValue, ref string lsUpdateColumns)
        {
            if (!sColumnValue.Equals("-1"))
            {
                if (!lsUpdateColumns.Trim().EndsWith(","))
                    lsUpdateColumns = lsUpdateColumns + ", ";
                lsUpdateColumns = lsUpdateColumns + sColumnName + "='" + sColumnValue + "', ";
            }
        }
        public SqlDataReader lfnGetDataReader(string lsOleDb, ArrayList ParameterObjectsArrayList)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlDataReader OBJDataReader = null;
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;
            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);
            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandTimeout = 0;
            string str = OBJCommand.CommandText;

            OBJDataReader = OBJCommand.ExecuteReader();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;
            return OBJDataReader;
        }
        public SqlDataReader lfnGetDataReader(string lsOleDb)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);
            SqlDataReader OBJDataReader = null;
            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);

            if (Trans != null)
                OBJCommand.Transaction = Trans;
            OBJCommand.CommandTimeout = liCommandTimeout;

            OBJDataReader = OBJCommand.ExecuteReader();
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            return OBJDataReader;
        }
        public DataSet lfnGetDataSet(string lsOleDb)
        {
            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlDataAdapter OBJDataAdapter;
            OBJDataAdapter = new SqlDataAdapter(lsOleDb, OBJConn);
            DataSet OBJDataSet = new DataSet();
            if (Trans != null)
                OBJDataAdapter.SelectCommand.Transaction = Trans;
            OBJDataAdapter.SelectCommand.CommandTimeout = liCommandTimeout;

            OBJDataAdapter.Fill(OBJDataSet);
            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);
            return OBJDataSet;
        }

        public T lfnExecuteScaler<T>(string lsOleDb)
        {
            T retVal;

            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;
            if (Trans != null) OBJCommand.Transaction = Trans;
            retVal = (T)OBJCommand.ExecuteScalar();

            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);

            return retVal;
        }

        public T lfnExecuteScaler<T>(string lsOleDb, ArrayList ParameterObjectsArrayList)
        {
            T retVal;

            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;

            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);

            if (Trans != null) OBJCommand.Transaction = Trans;
            retVal = (T)OBJCommand.ExecuteScalar();

            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);

            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;

            return retVal;
        }

        public T lfnExecuteScalerProcedure<T>(string lsOleDb, ArrayList ParameterObjectsArrayList)
        {
            T retVal;

            DateTime dtStart = DateTime.Now;
            SQLOutMessage(EventNames.SQL.ToString(), dtStart, lsOleDb);

            SqlCommand OBJCommand = new SqlCommand(lsOleDb, OBJConn);
            OBJCommand.CommandTimeout = liCommandTimeout;

            for (int i = 0; i < ParameterObjectsArrayList.Count; i++)
                OBJCommand.Parameters.Add(ParameterObjectsArrayList[i]);

            if (Trans != null) OBJCommand.Transaction = Trans;
            OBJCommand.CommandType = System.Data.CommandType.StoredProcedure;
            retVal = (T)OBJCommand.ExecuteScalar();

            DateTime dtEnd = DateTime.Now;
            SQLInMessage(EventNames.SQL.ToString(), dtStart, dtEnd);

            ParameterObjectsArrayList.Clear();
            ParameterObjectsArrayList = null;

            return retVal;
        }

        private void SQLInMessage(string EvtName, DateTime dtStart, DateTime dtEnd)
        {
            SqlBuffer.Append("<Evt Name=\"" + EvtName + "\" ST=\"" + dtStart.Minute.ToString() + ":" + dtStart.Second.ToString() + ":" + dtStart.Millisecond.ToString() + "\" ET=\"" + dtEnd.Minute.ToString() + ":" + dtEnd.Second.ToString() + ":" + dtEnd.Millisecond.ToString() + "\" ElSec=\"" + ((TimeSpan)dtEnd.Subtract(dtStart)).TotalSeconds + "\" Type=\"" + MessageType.In.ToString() + "\"/>\r\n");
        }
        private void SQLOutMessage(string EvtName, DateTime dtStart, string lsOleDb)
        {
            if (lsOleDb == "")
                SqlBuffer.Append("<Evt Name=\"" + EvtName + "\" ST=\"" + dtStart.Minute.ToString() + ":" + dtStart.Second.ToString() + ":" + dtStart.Millisecond.ToString() + "\" Type=\"" + MessageType.Out.ToString() + "\"/>\r\n");
            else
                SqlBuffer.Append("<Evt Name=\"" + EvtName + "\" ST=\"" + dtStart.Minute.ToString() + ":" + dtStart.Second.ToString() + ":" + dtStart.Millisecond.ToString() + "\" Type=\"" + MessageType.Out.ToString() + "\"><![CDATA[" + lsOleDb + "]]></Evt>\r\n");
        }

        public string lfnGetSQLServerVersion()
        {
            string version = "";

            string lssql = "SELECT @@Version as Version";
            SqlDataReader oDataReader = lfnGetDataReader(lssql);
            if (oDataReader.HasRows)
            {
                oDataReader.Read();
                version = oDataReader["Version"].ToString();
            }
            oDataReader.Close();

            if (version.ToUpper().IndexOf("MICROSOFT SQL SERVER  2000") >= 0)
                return "2000";
            else if (version.ToUpper().IndexOf("MICROSOFT SQL SERVER  2005") >= 0)
                return "2005";
            else if (version.ToUpper().IndexOf("MICROSOFT SQL SERVER  2008") >= 0)
                return "2008";
            else if (version.ToUpper().IndexOf("MICROSOFT SQL SERVER  2012") >= 0)
                return "20012";
            else
                return "2005";
        }

        public object GetDate()
        {
            string lsOleDb = "";
            lsOleDb = "Select getdate() as date1";
            DataTable oDt = lfnGetDataTable(lsOleDb);
            object oDate;
            if (oDt.Rows.Count > 0)
                oDate = oDt.Rows[0][0];
            else
            {
                throw new Exception("Error getting date format from database");
            }
            oDt.Dispose();
            return oDate;
        }

        public enum EventNames
        {
            DAMIMG, IMGPRO, SQLDBCONNECT, SQL, SQLTRANSBEGIN, SQLTRANSCOMMIT, ROLLBACKTRANS, SQLDBCLOSE, RENDER, WINZIP, FILEIO, ERROR, SWFPRO, QRCODE
        }
        public enum MessageType
        {
            In, Out
        }
    }
}
