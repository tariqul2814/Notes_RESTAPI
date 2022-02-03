using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Collections;
using System.Transactions;
using InsightRESTAPI.Model.CommonModel;
using InsightRESTAPI.Common;

namespace InsightRESTAPI.Model
{
    public static class DataHelper
    {
        private static async Task<SqlCommand> GetCommand(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    await con.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = con;
            if (sqlCommandParameterList != null)
            {
                if (sqlCommandParameterList.Count > 0)
                {
                    foreach (var item in sqlCommandParameterList)
                    {
                        cmd.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                    }
                }
            }
            return cmd;
        }
        private static async Task<SqlCommand> GetCommandWithOutputParameter(string query, List<SqlCommandParameter> sqlCommandParameterList, List<SqlCommandOutputParameter> sqlCommandOutputParameterList, string connectionString)
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
            {
                if (con.State != ConnectionState.Open)
                {
                    await con.OpenAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            SqlCommand cmd = new SqlCommand(query);
            cmd.Connection = con;
            if (sqlCommandParameterList != null)
            {
                if (sqlCommandParameterList.Count > 0)
                {
                    foreach (var item in sqlCommandParameterList)
                    {
                        cmd.Parameters.AddWithValue(item.ParameterName, item.ParameterValue);
                    }
                }
            }
            if (sqlCommandOutputParameterList != null)
            {
                if (sqlCommandOutputParameterList.Count > 0)
                {
                    foreach (var item in sqlCommandOutputParameterList)
                    {
                        if (item.SqlDbType == SqlDbType.VarChar || item.SqlDbType == SqlDbType.NVarChar)
                        {
                            cmd.Parameters.Add(item.ParameterName, item.SqlDbType, item.Size).Direction = ParameterDirection.Output;
                        }
                        else
                        {
                            cmd.Parameters.Add(item.ParameterName, item.SqlDbType).Direction = ParameterDirection.Output;
                        }
                    }
                }
            }

            return cmd;
        }

        public static async Task<bool> IsValidConnection(string connectionString)
        {
            bool toRet;
            try
            {
                SqlConnection con = new SqlConnection(connectionString);
                await con.OpenAsync();
                toRet = true;
                await con.CloseAsync();
            }
            catch (Exception)
            {
                toRet = false;
            }

            return toRet;
        }
        public static async Task<object> ExecuteNonQuery(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            int NoOfRowsEffected = 0;
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            try
            {
                NoOfRowsEffected = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return NoOfRowsEffected;
        }
        public static async Task<object> ExecuteScalar(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            object value = null;
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            try
            {
                value = await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return value;
        }
        public static async Task<int> NonQuery(string query, string connectionString)
        {
            int NoOfRowsEffected = 0;
            var cmd = await GetCommand(query, null, connectionString);
            try
            {
                NoOfRowsEffected = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return NoOfRowsEffected;
        }
        public static async Task<DataTable> DataTable(string query, string connectionString)
        {
            var cmd = await GetCommand(query, null, connectionString);
            DataTable dt = new DataTable();
            try
            {
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                dt.Load(rdr);
            }
            catch (Exception)
            {
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return dt;
        }
        public static async Task<DataTable> DataTable(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            DataTable dt = new DataTable();
            try
            {
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                dt.Load(rdr);
            }
            catch (Exception)
            {
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return dt;
        }
        public static async Task<object> Scalar(string query, string connectionString)
        {
            var cmd = await GetCommand(query, null, connectionString);
            object value = null;
            try
            {
                value = await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return value;
        }
        public static async Task<object> Scalar(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            object value = null;
            try
            {
                value = await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return value;
        }
        public static async Task<SqlDataAdapter> DataAdapter(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            await cmd.DisposeAsync();
            await cmd.Connection.CloseAsync();
            return da;
        }
        public static async Task<DataTable> ExecuteStoredProcedureDataTable(string spName, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(spName, sqlCommandParameterList, connectionString);
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                dt.Load(rdr);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return dt;
        }
        public static async Task<List<DataTable>> ExecuteStoredProcedureDataTables(string spName, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(spName, sqlCommandParameterList, connectionString);
            var dataTables = new List<DataTable>();
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (!reader.IsClosed)
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        dataTables.Add(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return dataTables;
        }
        public static async Task<List<T>> ExecuteStoredProcedure<T>(string spName, List<SqlCommandParameter> sqlCommandParameterList, string connectionString) where T : new()
        {
            var cmd = await GetCommand(spName, sqlCommandParameterList, connectionString);
            DataTable dt = new DataTable();
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                dt.Load(rdr);

                return Utilities.ConvertFromDataTable<T>(dt);
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
        }
        public static async Task<List<T>> ExecuteQuery<T>(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString) where T : new()
        {
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            DataTable dt = new DataTable();
            try
            {
                SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                dt.Load(rdr);

                return Utilities.ConvertFromDataTable<T>(dt);
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
        }
        public static async Task<SqlDataAdapter> ExecuteStoredProcedureDataAdapter(string spName, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(spName, sqlCommandParameterList, connectionString);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            await cmd.DisposeAsync();
            await cmd.Connection.CloseAsync();
            return da;
        }
        public static async Task<object> ExecuteStoredProcedureNonQuery(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            int NoOfRowsEffected = 0;

            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                NoOfRowsEffected = await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return NoOfRowsEffected;
        }
        public static async Task<object> ExecuteStoredProcedureScalar(string query, List<SqlCommandParameter> sqlCommandParameterList, string connectionString)
        {
            object value = null;
            var cmd = await GetCommand(query, sqlCommandParameterList, connectionString);
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                value = await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                value = ex.Message;
            }
            finally
            {
                await cmd.DisposeAsync();
                await cmd.Connection.CloseAsync();
            }
            return value;
        }
        public static async Task<SqlCommand> ExecuteStoredProcedureNonQueryWithOutputParameter(string query, List<SqlCommandParameter> sqlCommandParameterList, List<SqlCommandOutputParameter> sqlCommandOutputParameterList, string connectionString)
        {
            var cmd = await GetCommandWithOutputParameter(query, sqlCommandParameterList, sqlCommandOutputParameterList, connectionString);
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception)
            {
            }
            finally
            {
                await cmd.Connection.CloseAsync();
            }
            return cmd;
        }

        /// <summary>
        /// Executes the provided callback function within a TransactionScope.
        /// Use whenever multiple queries need to be executed but should all be rolledback if any fail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionString"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        async static public Task<T> ExecuteInTransaction<T>(string connectionString, Func<SqlCommand, Task<T>> callback)
        {
            T result;

            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;

                    result = await callback(cmd);
                }
                scope.Complete();
            }

            return result;
        }
    }
}
