using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace Easytl.SqlHelper
{
    /// <summary>
    /// OleDb数据库帮助类
    /// </summary>
    public class OleDbHelper
    {
        /// <summary>
        /// 数据库连接类
        /// </summary>
        OleDbConnection ole_connection = new OleDbConnection();

        /// <summary>
        /// 数据库命令类
        /// </summary>
        OleDbCommand ole_command = new OleDbCommand();

        /// <summary>
        /// 数据表填充类
        /// </summary>
        OleDbDataAdapter ole_Adapter = new OleDbDataAdapter();

        /// <summary>
        /// 构造函数
        /// </summary>  
        /// <param name="db_path">数据库路径</param>  
        public OleDbHelper(string ConnectionString)
        {
            //Access: ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + db_path + "';Persist Security Info=False;Jet OLEDB:Database Password=" + Password;
            ole_connection.ConnectionString = ConnectionString;
            ole_command.Connection = ole_connection;
            ole_Adapter.SelectCommand = ole_command;
        }

        /// <summary>
        /// 测试数据库连接情况
        /// </summary>
        /// <param name="db_path">数据库路径</param>  
        public static bool ConnectConn(string ConnectionString)
        {
            OleDbConnection ole_connection_new = new OleDbConnection();
            ole_connection_new.ConnectionString = ConnectionString;
            try
            {
                ole_connection_new.Open();
            }
            catch
            {
                ole_connection_new.Dispose();
                return false;
            }
            finally
            {
                ole_connection_new.Close();
                ole_connection_new.Dispose();
            }
            return true;
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            try
            {
                ole_connection.Close();
                ole_command.Dispose();
                ole_Adapter.Dispose();
                ole_connection.Dispose();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>  
        /// 执行sql语句(返回受影响的行数)
        /// </summary>  
        /// <param name="strSql">sql语句</param>  
        /// <returns>返回结果</returns>  
        public int ExecuteNonQuery(string sql)
        {
            lock (this)
            {
                ole_command.CommandText = sql;
                try
                {
                    if (ole_connection.State == ConnectionState.Closed)
                    {
                        ole_connection.Open(); //打开数据库连接
                    }
                    return ole_command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>  
        /// 执行sql语句(返回结果的第一行第一列)
        /// </summary>  
        /// <param name="strSql">sql语句</param>  
        /// <returns>返回结果</returns>  
        public object ExecuteScalar(string sql)
        {
            lock (this)
            {
                ole_command.Connection = ole_connection;
                ole_command.CommandText = sql;
                try
                {
                    if (ole_connection.State == ConnectionState.Closed)
                    {
                        ole_connection.Open(); //打开数据库连接
                    }
                    return ole_command.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 从数据库里面获取数据  
        /// </summary>  
        /// <param name="strSql">查询语句</param>  
        /// <returns>数据列表</returns>  
        public DataSet GetDataSet(string sql)
        {
            lock (this)
            {
                ole_command.Connection = ole_connection;
                ole_command.CommandText = sql;
                DataSet ds = new DataSet();
                try
                {
                    if (ole_connection.State == ConnectionState.Closed)
                    {
                        ole_connection.Open(); //打开数据库连接
                    }
                    ole_Adapter.Fill(ds.Tables.Add("Tb"));
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
