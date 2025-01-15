using System.Data.Common;
using System.Data;
using System.Data.Odbc;
using Application.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Threading;

namespace Infrastructure.Common
{
    public static class IBMiAS400
    {
        private static readonly CommonClass _commonClass;

        public static List<AccountDetailsStatus> GetAccountNameStatusPostGre(this List<string> value,IConfiguration _config)
        {
            List<AccountDetailsStatus> result = new List<AccountDetailsStatus>();
            string where = "", field = "foracid, acct_status, cif_id,acct_name", table = "gam g";
            string sLib = "tbaadm";
            value.ForEach(a => where += $" '{a}', ");
            where = where.Remove(where.LastIndexOf(','));
            where = $" where acct_ownership = 'C' AND foracid in ({where})";
            string sCmdText = $"SELECT {field} FROM {sLib}.{table} {where}" +
                              $" JOIN tbaadm.smt s ON g.acid = s.acid";
            result = PostgresQuery(sCmdText, _config);

            string restriction, status;

            //Mas prevail ang account closed.
            foreach (var acctDets in result)
            {
                var StatusName = GetAccountStatusDesc(acctDets.AccountStatus);
                status = acctDets.AccountStatus;
                restriction = (StatusName == "CLOSED" || StatusName == "INACTIVE" || StatusName == "DORMANT") ? "" : acctDets.frez_code ? "Post No Debit" : "";
                acctDets.AccountName = acctDets.AccountName;
                acctDets.AccountStatus = $"{StatusName} {restriction}";
            }

            return result;
        }
        private static List<AccountDetailsStatus> PostgresQuery(string sCommandText, IConfiguration _config)
        {
            List<AccountDetailsStatus> accountDetailsStatus = new List<AccountDetailsStatus>();
            try
            {
                string connectionString = _config["FinacleSVSContext"].ToString();
                //string connectionString = _config["PostGre"].ToString();
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    try
                    {

                        using (var cmd = new NpgsqlCommand(sCommandText, connection))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        accountDetailsStatus.Add(
                                            new AccountDetailsStatus
                                            {
                                                AccountNumber = reader["foracid"].ToString(),
                                                AccountStatus = reader["acct_status"].ToString(),
                                                AccountName = reader["acct_name"].ToString(),
                                                CIFID = reader["cif_id"].ToString(),
                                               frez_code = reader["frez_code"]?.ToString().Equals("D") == true,
                                    });
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No rows found.");
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return accountDetailsStatus;
        }
        private static string GetAccountStatusDesc(string statcode)
        {
            string result = "";
            //1       ACTIVE ACCOUNT                      ACTIVE          ACTIVE ACCOUNT                                   
            //2       CLOSED ACCOUNT                      CLOSED          CLOSED ACCOUNT                                   
            //3       MATURED ACCOUNT                     MATURED         MATURED ACCOUNT                                  
            //4       NEW ACCOUNT                         NEW             NEW ACCOUNT                                      
            //5       DO NOT CLOSE WITH ZERO BALANCE      NO CLOSE        DO NOT CLOSE ACCOUNT WITH ZERO BALANCE           
            //6       RESTRICTED ACCT-NO DEBITING         RESTRICTED      RESTRICTED ACCT - NO DEBITING ALLOWED            
            //7       FROZEN A/C-NO DEBIT,NO CREDIT       FROZEN          FROZEN A/C-NO DEBITING AND NO CREDITING ALLOWED  
            //9       DORMANT A/C-NO DEBIT,NO CREDIT      DORMANT         DORMANT A/C-NO DEBITING AND NO CREDITING ALLOWED 
            switch (statcode.ToUpper())
            {
                case "1": result = "ACTIVE"; break; //  ACTIVE ACCOUNT"; break;
                case "2": result = "CLOSED"; break; //  CLOSED ACCOUNT"; break;
                case "3": result = "MATURED"; break; //  MATURED ACCOUNT"; break;
                case "4": result = "NEW"; break; //  NEW ACCOUNT"; break;
                case "5": result = "NO CLOSE"; break; //  DO NOT CLOSE ACCOUNT WITH ZERO BALANCE"; break;
                case "6": result = "RESTRICTED"; break; //  RESTRICTED ACCT - NO DEBITING ALLOWED"; break;
                case "7": result = "FROZEN"; break; //  FROZEN A/C-NO DEBITING AND NO CREDITING ALLOWED"; break;
                case "9": result = "DORMANT"; break; //  DORMANT A/C-NO DEBITING AND NO CREDITING ALLOWED"; break;

                //PostGre
                case "A": result = "ACTIVE"; break; //  ACTIVE ACCOUNT"; break;
                case "I": result = "INACTIVE"; break; //  ACTIVE ACCOUNT"; break;
                case "C": result = "CLOSED"; break; //  CLOSED ACCOUNT"; break;
                case "M": result = "MATURED"; break; //  MATURED ACCOUNT"; break;
                case "N": result = "NEW"; break; //  NEW ACCOUNT"; break;
                case "": result = "NO CLOSE"; break; //  DO NOT CLOSE ACCOUNT WITH ZERO BALANCE"; break;
                case "R": result = "RESTRICTED"; break; //  RESTRICTED ACCT - NO DEBITING ALLOWED"; break;
                case "F": result = "FROZEN"; break; //  FROZEN A/C-NO DEBITING AND NO CREDITING ALLOWED"; break;
                case "D": result = "DORMANT"; break; //  DORMANT A/C-NO DEBITING AND NO CREDITING ALLOWED"; break;
                default: result = statcode; break;
            }
            return result;

        }
        private static DataTable GetDataPostgres(string query, IConfiguration _config)
        {
            string connectionString = _config["FinacleSVSContext"].ToString();
            //string connectionString = _config["PostGre"].ToString();
            DataTable _Result = new DataTable();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    try
                    {

                        using (var cmd = new NpgsqlCommand(query, connection))
                        {
                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                _Result.Load(reader);
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return _Result;
        }
        private static bool CheckAccountPNDPostGre(string value, IConfiguration _config)
        {
            bool result = false;
            DataTable res = GetData(
                "TLSC04",
                 "TLASTS", "tlsact", value,
                 _config);
            if (res.HasErrorColumn())
            {
                result = false;
            }
            else
            {
                if (res.Rows.Count >= 0)
                {
                    result = res.Rows[0]["frez_code"].Equals("D") ? true : false;
                }
            }
            return result;
        }

        #region SIBSs
        private const string CLIENTACCESS = "{Client Access ODBC Driver (32-bit)}";
        private const string ODBCCONNSTRING = "Driver={0};SYSTEM={1}; UID={2}; PWD={3};";
        private static DataTable ODBCQuery(string sCommandText, IConfiguration _config)
        {
            DataTable _Result = new DataTable();
            string error = "";
            try
            {
                using (OdbcConnection _DB2Conn = MakeConnString(_config))
                {
                    _DB2Conn.ConnectionTimeout = 60;
                    _DB2Conn.Open();
                    using (OdbcCommand _DB2Comd = new OdbcCommand(sCommandText, _DB2Conn))
                    {
                        using (OdbcDataReader _DB2DataRdr = _DB2Comd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            _Result.Load(_DB2DataRdr);
                            _DB2DataRdr.Close();
                        }
                    }
                    _DB2Conn.Close();
                    OdbcConnection.ReleaseObjectPool();
                }
            }
            catch (OdbcException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    error += "Index #" + i + "\n" +
                                     "Message: " + ex.Errors[i].Message + "\n" +
                                     "NativeError: " + ex.Errors[i].NativeError.ToString() + "\n" +
                                     "Source: " + ex.Errors[i].Source + "\n" +
                                     "SQL: " + ex.Errors[i].SQLState + "\n";
                }

                //error = string.Format("[Error]: {0} {1}  {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
            }
            finally
            {
                OdbcConnection.ReleaseObjectPool();
            }

            if (_Result.Rows.Count == 0)//&& error != "")
            {
                _Result.Columns.Clear();
                _Result.Columns.Add("Error");
                _Result.Rows.Add(new string[] { error != "" ? error : "" });
            }
            return _Result;
        }
        public static List<AccountDetailsStatus> GetAccountNameStatus2(this List<string> value, IConfiguration _config)
        {
            List<AccountDetailsStatus> result = new List<AccountDetailsStatus>();
            string where = "", field = "acctno, status, CIFNO", table = "ddmast";
            string sLib = _config["DBAS400:Database"].ToString().Decrypt();

            value.ForEach(a => where += $" '{a}', ");
            where = where.Remove(where.LastIndexOf(','));
            where = $" where acctno in ({where})";
            string sCmdText = $"select {field} from {sLib}.{table} {where}";
            result = ODBCQuery2(sCmdText, _config);

            string restriction, status;

            //Mas prevail ang account closed.
            foreach (var acctDets in result)
            {
                status = acctDets.AccountStatus;
                restriction = (status == "2" || status == "9") ? "" : CheckAccountPND(acctDets.AccountNumber, _config) ? "Post No Debit" : "";
                acctDets.AccountName = GetLongName(acctDets.AccountName, _config);
                acctDets.AccountStatus = $"{GetAccountStatusDesc(status)} {restriction}";
            }

            return result;
        }
        private static List<AccountDetailsStatus> ODBCQuery2(string sCommandText, IConfiguration _config)
        {
            List<AccountDetailsStatus> accountDetailsStatus = new List<AccountDetailsStatus>();
            try
            {
                using (OdbcConnection _DB2Conn = MakeConnString(_config))
                {
                    _DB2Conn.ConnectionTimeout = 60;
                    _DB2Conn.Open();
                    using (OdbcCommand _DB2Comd = new OdbcCommand(sCommandText, _DB2Conn))
                    {
                        OdbcDataReader _DB2DataRdr = _DB2Comd.ExecuteReader(CommandBehavior.CloseConnection);


                        while (_DB2DataRdr.Read())
                        {
                            accountDetailsStatus.Add(
                                new AccountDetailsStatus
                                {
                                    AccountNumber = _DB2DataRdr.GetValue(0).ToString(),
                                    AccountStatus = _DB2DataRdr.GetValue(1).ToString(),
                                    AccountName = _DB2DataRdr.GetValue(2).ToString(),
                                });
                        }

                        _DB2DataRdr.Close();
                    }
                    _DB2Conn.Close();
                    OdbcConnection.ReleaseObjectPool();
                }
            }
            catch (OdbcException ex)
            {
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    Console.WriteLine("Index #" + i + "\n" +
                                     "Message: " + ex.Errors[i].Message + "\n" +
                                     "NativeError: " + ex.Errors[i].NativeError.ToString() + "\n" +
                                     "Source: " + ex.Errors[i].Source + "\n" +
                                     "SQL: " + ex.Errors[i].SQLState + "\n"
                                     );
                }
            }
            finally
            {
                OdbcConnection.ReleaseObjectPool();
            }

            return accountDetailsStatus;
        }
        private static string GetLongName(string CFINO, IConfiguration _config)
        {
            string result = "", field = "CFNA1", key = "CFCIF#";
            try
            {
                DataTable res = GetData(field, "cfmast", key, $"'{CFINO}'", _config);
                if (res.HasErrorColumn())
                {
                    res = GetData(field, "cftnew", key, CFINO, _config);
                    if (!res.HasErrorColumn())
                    {
                        result = res.Rows[0][0].ToString();
                    }
                }
                else
                    result = res.Rows[0][0].ToString();
            }
            catch (Exception)
            {
                result = "";
            }
            return result;
        }
        private static bool CheckAccountPND(string value, IConfiguration _config)
        {
            //select TLSC04 from TLASTS where tlsact = ''
            //dr["TLSC04"].Equals("Y") == "Post No Debit"
            bool result = false;
            DataTable res = GetData(
                "TLSC04",
                 "TLASTS", "tlsact", value,
                 _config);
            if (res.HasErrorColumn())
            {
                result = false;
            }
            else
            {
                if (res.Rows.Count >= 0)
                {
                    result = res.Rows[0]["TLSC04"].Equals("Y") ? true : false;
                }
            }
            return result;
        }
        private static OdbcConnection MakeConnString(IConfiguration _config)
        {
            try
            {
                string UserName = _config["DBAS400:UserName"].ToString().Decrypt();
                string DBServer = _config["DBAS400:DBServer"].ToString().Decrypt();
                string Password = _config["DBAS400:Password"].ToString().Decrypt();
                string con = string.Format(
                            ODBCCONNSTRING,
                            CLIENTACCESS,
                            DBServer,
                            UserName,
                            Password
                            );
                return new OdbcConnection(con);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DataTable GetData(string field, string table, string key, string value, IConfiguration _config)
        {
            string Database = _config["DBAS400:Database"].ToString().Decrypt();
            string sCmdText = "select {2} from {0}.{1}  where {3} = {4} ";
            sCmdText = string.Format(sCmdText, Database, table, field, key, value);

            return ODBCQuery(sCmdText, _config);
        }
        private static bool HasErrorColumn(this DataTable value)
        {
            return value.Columns.Contains("Error");
        }
        #endregion
        #region Unused
        //private const string IBMIACCESS = "{IBM i Access ODBC Driver}";
        //private const string IDB2CONNSTIRNG = "DataSource = {0}; UserID = {1}; Password = {2}; Pooling=false; EnablePreFetch = false;";
        //public static MAccountDetails GetAccountNameStatus(this string value)
        //{
        //    MAccountDetails result = new MAccountDetails() { AccountName = "", AccountStatus = "" };
        //    DataTable res = GetData("status, CIFNO", "ddmast", "acctno", value);
        //    string restriction, status;
        //    if (res.HasErrorColumn())
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        //Mas prevail ang account closed.
        //        if (res.Rows.Count >= 0)
        //        {
        //            DataRow dr = res.Rows[0];
        //            status = dr["status"].ToString();

        //            restriction = (status == "2" || status == "9") ? "" : CheckAccountPND(value) ? "Post No Debit" : "";
        //            result.AccountName = GetLongName(dr["CIFNO"].ToString());
        //            result.AccountStatus = $"{GetAccountStatusDesc(status)} {restriction}";
        //        }
        //    }
        //    return result;
        //}
        //private static DataTable iDB2Query(string sCommandText)
        //{
        //    string IDB2CONNSTIRNG = "DataSource = {0}; UserID = {1}; Password = {2}; Pooling=false; EnablePreFetch = false;";
        //    DataTable dt = new DataTable();
        //    string error = "";
        //    try
        //    {
        //        IDB2CONNSTIRNG = string.Format(
        //                IDB2CONNSTIRNG,
        //                _config.CredConns[DBCredConns.DBAS400].DBServer.DeCrypt(),
        //                _config.CredConns[DBCredConns.DBAS400].UserName.DeCrypt(),
        //                _config.CredConns[DBCredConns.DBAS400].Password.DeCrypt()
        //                );
        //        using (iDB2Connection iDB2Conn = new iDB2Connection(IDB2CONNSTIRNG))//_config.DictDataSrvcConn[DBCredConns.DBAS400]))
        //        {
        //            iDB2Conn.Open();
        //            using (iDB2Command iDB2Comd = new iDB2Command(sCommandText, CommandType.Text, iDB2Conn))
        //            {
        //                iDB2Comd.CommandTimeout = 60;
        //                using (iDB2DataReader iDB2DataRdr = iDB2Comd.ExecuteReader(CommandBehavior.CloseConnection))
        //                {
        //                    if (iDB2DataRdr.HasRows)
        //                        dt.Load(iDB2DataRdr);

        //                    iDB2DataRdr.Close();
        //                }
        //            }
        //            iDB2Conn.Close();
        //            //iDB2Connection..CleanupPooledConnections();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        error = string.Format("[Error]: {0} {1}  {2}", ex.GetType().Name, ex.Message, ex.StackTrace);
        //    }
        //    finally
        //    {
        //        //iDB2Connection.CleanupPooledConnections();
        //    }

        //    if (dt.Rows.Count == 0)//&& error != "")
        //    {
        //        dt.Clear();
        //        dt.Columns.Add("Error");
        //        dt.Rows.Add(new string[] { error != "" ? error : "" });
        //    }

        //    return dt;
        //}

        //private static string CheckAccountRestrictions(string value)
        //{
        //    string result = "";
        //    DataTable res = GetData(
        //        "TLSC01, TLSC02, TLSC03, TLSC04, TLSC05, TLSC06, TLSC07, TLSC08, TLSC09, TLSC10, TLSC11, TLSC12, TLSC13, TLSC14, TLSC15, TLSC16, TLSC17, TLSC18, TLSC19, TLSC20, TLSC21, TLSC22",
        //         "TLASTS", "tlsact", value);
        //    if (res.HasErrorColumn())
        //    {
        //        result = "";
        //    }
        //    else
        //    {
        //        foreach (DataRow dr in res.Rows)
        //        {
        //            result = (dr["TLSC01"].Equals("Y")) ? "Cash Deposit Only" : "";
        //            result += (dr["TLSC02"].Equals("Y")) ? "Non-Drawing Account" : "";
        //            result += (dr["TLSC03"].Equals("Y")) ? "Deposit Only" : "";
        //            result += (dr["TLSC04"].Equals("Y")) ? "Post No Debit" : "";
        //            result += (dr["TLSC05"].Equals("Y")) ? "Post No Credit" : "";
        //            result += (dr["TLSC06"].Equals("Y")) ? "Lost Passbook" : "";
        //            result += (dr["TLSC07"].Equals("Y")) ? "Refer To Officer" : "";
        //            result += (dr["TLSC08"].Equals("Y")) ? "Lost Card" : "";
        //            result += (dr["TLSC09"].Equals("Y")) ? "Refer All Debits" : "";
        //            result += (dr["TLSC10"].Equals("Y")) ? "Block Account" : "";
        //            result += (dr["TLSC11"].Equals("Y")) ? "House Check Drawn on Now Acct" : "";
        //            result += (dr["TLSC12"].Equals("Y")) ? "Refer to Officer w/IBT Agreemt" : "";
        //            result += (dr["TLSC13"].Equals("Y")) ? "Escrow Account" : "";
        //            result += (dr["TLSC14"].Equals("Y")) ? "14" : "";
        //            result += (dr["TLSC15"].Equals("Y")) ? "15" : "";
        //            result += (dr["TLSC16"].Equals("Y")) ? "16" : "";
        //            result += (dr["TLSC17"].Equals("Y")) ? "17" : "";
        //            result += (dr["TLSC18"].Equals("Y")) ? "DAUD facility is under review" : "";
        //            result += (dr["TLSC19"].Equals("Y")) ? "DAUD facility is expired" : "";
        //            result += (dr["TLSC20"].Equals("Y")) ? "Account has DAUD facility" : "";
        //            result += (dr["TLSC21"].Equals("Y")) ? "DAUD past due" : "";
        //            result += (dr["TLSC22"].Equals("Y")) ? "Account has BP facility" : "";
        //            result += (dr["TLSC23"].Equals("Y")) ? "BP past due" : "";
        //            result += (dr["TLSC24"].Equals("Y")) ? "BP facility is under review" : "";
        //            result += (dr["TLSC25"].Equals("Y")) ? "BP facility is expired" : "";
        //            result += (dr["TLSC26"].Equals("Y")) ? "Restricted in OTC Transaction" : "";
        //            result += (dr["TLSC27"].Equals("Y")) ? "27" : "";
        //            result += (dr["TLSC28"].Equals("Y")) ? "28" : "";
        //            result += (dr["TLSC29"].Equals("Y")) ? "29" : "";
        //            result += (dr["TLSC30"].Equals("Y")) ? "30" : "";
        //            result += (dr["TLSC31"].Equals("Y")) ? "31" : "";
        //            result += (dr["TLSC32"].Equals("Y")) ? "32" : "";
        //            result += (dr["TLSC33"].Equals("Y")) ? "33" : "";
        //            result += (dr["TLSC34"].Equals("Y")) ? "34" : "";
        //            result += (dr["TLSC35"].Equals("Y")) ? "35" : "";
        //            result += (dr["TLSC36"].Equals("Y")) ? "36" : "";
        //            result += (dr["TLSC37"].Equals("Y")) ? "Account with owing amount" : "";
        //            result += (dr["TLSC38"].Equals("Y")) ? "Mishandled Account" : "";
        //            result += (dr["TLSC39"].Equals("Y")) ? "Past Due Account" : "";
        //            result += (dr["TLSC40"].Equals("Y")) ? "Clearing Pending Items" : "";
        //        }
        //    }
        //    return result;
        //}


        //public static MReponseObj AcctRelName(this string value)
        //{
        //    bool isValid = false, isExist = false;
        //    string result = "", tmpName = "";
        //    string field = "CFCIF#, CFATYP, CFRELA, CFSNME", key = "CFACC#";
        //    try
        //    {
        //        DataTable res = GetData(field, "cfacct", key, value);
        //        if (res.HasErrorColumn())
        //        {
        //            isValid = false; isExist = false;
        //            result = res.Rows[0][0].ToString();
        //        }
        //        else
        //        {
        //            isValid = true; isExist = true;
        //            foreach (DataRow dr in res.Rows)
        //            {
        //                tmpName = GetLongName(dr["CFCIF#"].ToString());
        //                result += (result == "") ? "" : "|";
        //                result += "(" + dr["CFCIF#"].ToString() + ")";
        //                result += "(" + dr["CFRELA"].ToString() + ")";
        //                result += (tmpName == "") ? dr["CFSNME"].ToString() : tmpName;

        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        result = "";
        //    }
        //    return new MReponseObj { Result = result, IsExist = isExist, IsValid = isValid };
        //}

        //private static string GetSName(string value)
        //{
        //    string result = "";
        //    DataTable res = GetData("status, SNAME, cifno", "ddmast", "acctno", value);
        //    if (res.HasErrorColumn())
        //    {
        //        return result;
        //    }
        //    else
        //    {
        //        foreach (DataRow dr in res.Rows)
        //        {
        //            result = $"{dr["SNAME"]}|{dr["status"]}|{dr["cifno"]}";

        //        }
        //    }
        //    return result;
        //}
        //public static MReponseObj AccountName(this string value)
        //{
        //    bool isValid = false, isExist = false;
        //    string result = "", tmpName = "";
        //    DataTable res = GetData("status, SNAME, CIFNO", "ddmast", "acctno", value);
        //    if (res.HasErrorColumn())
        //    {
        //        isValid = false; isExist = false;
        //        result = res.Rows[0][0].ToString();
        //    }
        //    else
        //    {
        //        foreach (DataRow dr in res.Rows)
        //        {
        //            tmpName = GetLongName(dr["CIFNO"].ToString());
        //            result = (tmpName == "") ? dr["SNAME"].ToString() : tmpName;
        //            isValid = dr["status"].ToString().Equals("1");
        //        }
        //        isExist = true;
        //    }
        //    return new MReponseObj { Result = result, IsExist = isExist, IsValid = isValid };
        //}
        #endregion
    }
}
