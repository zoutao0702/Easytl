using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Easytl.SqlHelper
{
    /// <summary>
    /// SqlServer数据库帮助类
    /// </summary>
    public class SqlServerHelper
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        public SqlServerHelper(string connectionString)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 测试数据库连接情况
        /// </summary>
        /// <returns></returns>
        public static bool ConnectConn(string ConnectionStr)
        {
            SqlConnection conn_new = new SqlConnection();
            conn_new.ConnectionString = ConnectionStr;
            try
            {
                conn_new.Open();
            }
            catch
            {
                conn_new.Dispose();
                return false;
            }
            finally
            {
                conn_new.Close();
                conn_new.Dispose();
            }
            return true;
        }

        /// <summary>
        ///  用于对数据库进行增，删，改(返回受影响的行数)。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 用于对数据库进行增，删，改(返回受影响的行数)。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns>int</returns>
        public int ExecuteNonQuery(string sql, SqlParameter[] paras, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            for (int i = 0; i < paras.Length; i++)
            {
                cmd.Parameters.Add(paras[i]);
            }
            try
            {
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 用于对数据库进行查询并获取一个数据表。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>DataSet</returns>
        public DataSet GetDataSet(string sql, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            sda.SelectCommand = cmd;
            DataSet ds = new DataSet();
            try
            {
                sda.Fill(ds.Tables.Add("Tb"));
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                sda.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 用于对数据库进行查询并获取一个数据表。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, SqlParameter[] paras, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            SqlDataAdapter sda = new SqlDataAdapter();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            sda.SelectCommand = cmd;
            for (int i = 0; i < paras.Length; i++)
            {
                cmd.Parameters.Add(paras[i]);
            }
            DataSet ds = new DataSet();
            try
            {
                sda.Fill(ds.Tables.Add("Tb"));
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                sda.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 用于对数据库进行查询并返回第一行第一列。
        /// </summary>
        /// <param name="sql"></param>
        /// <returns>bool</returns>
        public object ExecuteScalar(string sql, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 用于对数据库进行查询并返回第一行第一列。
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paras"></param>
        /// <returns>bool</returns>
        public object ExecuteScalar(string sql, SqlParameter[] paras, CommandType commandType)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sql;
            cmd.CommandType = commandType;
            cmd.Parameters.Clear();
            for (int i = 0; i < paras.Length; i++)
            {
                cmd.Parameters.Add(paras[i]);
            }
            try
            {
                conn.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
                cmd.Dispose();
                conn.Dispose();
            }
        }

        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize, int PageIndex, string strWhere)
        {
            SqlParameter[] parameters = {
					new SqlParameter("@tblName", SqlDbType.VarChar, 255),
					new SqlParameter("@fldName", SqlDbType.VarChar, 255),
					new SqlParameter("@PageSize", SqlDbType.Int),
					new SqlParameter("@PageIndex", SqlDbType.Int),
					new SqlParameter("@IsReCount", SqlDbType.Bit),
					new SqlParameter("@OrderType", SqlDbType.Bit),
					new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
					};
            parameters[0].Value = "MJ_A_Operator";
            parameters[1].Value = "id";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;
            return GetDataSet("UP_GetRecordByPage", parameters, CommandType.StoredProcedure);
        }



        //--用途：支持任意排序的分页存储过程 
        //--说明：
        //------------------------------------
        //CREATE PROCEDURE [dbo].[UP_GetRecordByPageOrder]

        //@tblName varchar(255), -- 表名 
        //@fldName varchar(255), -- 显示字段名 
        //@OrderfldName varchar(255), -- 排序字段名 
        //@StatfldName varchar(255), -- 统计字段名 
        //@PageSize int = 10, -- 页尺寸 
        //@PageIndex int = 1, -- 页码 
        //@IsReCount bit = 0, -- 返回记录总数, 非 0 值则返回 
        //@OrderType bit = 0, -- 设置排序类型, 非 0 值则降序 
        //@strWhere varchar(1000) = '' -- 查询条件 (注意: 不要加 where) 
        //AS
        //declare @strSQL varchar(6000) -- 主语句 
        //declare @strTmp varchar(100) -- 临时变量(查询条件过长时可能会出错，可修改100为1000)
        //declare @strOrder varchar(400) -- 排序类型
        //if @OrderType != 0 
        //begin 
        //set @strTmp = '<(select min' 
        //set @strOrder = ' order by [' + @OrderfldName +'] desc' 
        //end 
        //else 
        //begin 
        //set @strTmp = '>(select max' 
        //set @strOrder = ' order by [' + @OrderfldName +'] asc' 
        //end
        //set @strSQL = 'select top ' + str(@PageSize) + ' ' + @fldName + ' from [' 
        //+ @tblName + '] where [' + @OrderfldName + ']' + @strTmp + '([' 
        //+ @OrderfldName + ']) from (select top ' + str((@PageIndex-1)*@PageSize) + ' [' 
        //+ @OrderfldName + '] from [' + @tblName + ']' + @strOrder + ') as tblTmp)' 
        //+ @strOrder
        //if @strWhere != '' 
        //set @strSQL = 'select top ' + str(@PageSize) + ' ' + @fldName + ' from [' 
        //+ @tblName + '] where [' + @OrderfldName + ']' + @strTmp + '([' 
        //+ @OrderfldName + ']) from (select top ' + str((@PageIndex-1)*@PageSize) + ' [' 
        //+ @OrderfldName + '] from [' + @tblName + '] where ' + @strWhere + ' ' 
        //+ @strOrder + ') as tblTmp) and ' + @strWhere + ' ' + @strOrder
        //if @PageIndex = 1 
        //begin 
        //set @strTmp = '' 
        //if @strWhere != '' 
        //set @strTmp = ' where ' + @strWhere
        //set @strSQL = 'select top ' + str(@PageSize) + ' ' + @fldName + ' from [' 
        //+ @tblName + ']' + @strTmp + ' ' + @strOrder 
        //end

        //if @IsReCount != 0 
        //set @strSQL = @strSQL+' select count(1) as Total from [' + @tblName + ']'
        //if @strWhere!=''
        //set @strSQL = @strSQL+' where ' + @strWhere
        //exec (@strSQL)

         

        //------------------------------------
        //--用途：分页存储过程(对有主键的表效率极高) 
        //--说明：
        //------------------------------------
        //CREATE PROCEDURE [dbo].[UP_GetRecordByPage]
        //@tblName varchar(255), -- 表名
        //@fldName varchar(255), -- 主键字段名
        //@PageSize int = 10, -- 页尺寸
        //@PageIndex int = 1, -- 页码
        //@IsReCount bit = 0, -- 返回记录总数, 非 0 值则返回
        //@OrderType bit = 0, -- 设置排序类型, 非 0 值则降序
        //@strWhere varchar(1000) = '' -- 查询条件 (注意: 不要加 where)
        //AS
        //declare @strSQL varchar(6000) -- 主语句
        //declare @strTmp varchar(100) -- 临时变量(查询条件过长时可能会出错，可修改100为1000)
        //declare @strOrder varchar(400) -- 排序类型
        //if @OrderType != 0
        //begin
        //set @strTmp = '<(select min'
        //set @strOrder = ' order by [' + @fldName +'] desc'
        //end
        //else
        //begin
        //set @strTmp = '>(select max'
        //set @strOrder = ' order by [' + @fldName +'] asc'
        //end
        //set @strSQL = 'select top ' + str(@PageSize) + ' * from ['
        //+ @tblName + '] where [' + @fldName + ']' + @strTmp + '(['
        //+ @fldName + ']) from (select top ' + str((@PageIndex-1)*@PageSize) + ' ['
        //+ @fldName + '] from [' + @tblName + ']' + @strOrder + ') as tblTmp)'
        //+ @strOrder
        //if @strWhere != ''
        //set @strSQL = 'select top ' + str(@PageSize) + ' * from ['
        //+ @tblName + '] where [' + @fldName + ']' + @strTmp + '(['
        //+ @fldName + ']) from (select top ' + str((@PageIndex-1)*@PageSize) + ' ['
        //+ @fldName + '] from [' + @tblName + '] where ' + @strWhere + ' '
        //+ @strOrder + ') as tblTmp) and ' + @strWhere + ' ' + @strOrder
        //if @PageIndex = 1
        //begin
        //set @strTmp =''
        //if @strWhere != ''
        //set @strTmp = ' where ' + @strWhere
        //set @strSQL = 'select top ' + str(@PageSize) + ' * from ['
        //+ @tblName + ']' + @strTmp + ' ' + @strOrder
        //end
        //if @IsReCount != 0
        //set @strSQL = 'select count(*) as Total from [' + @tblName + ']'+' where ' + @strWhere
        //exec (@strSQL)
    }
}
