using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 数据库连接帮助类
    /// </summary>
    public static class DBHelper
    {
        private static string? _connStr;

        /// <summary>
        /// 获取数据库连接字符串（从配置文件读取）
        /// </summary>
        private static string GetConnectionString()
        {
            if (_connStr != null) return _connStr;

            try
            {
                // 读取 appsettings.json 配置文件
                string configPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                
                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException(
                        "未找到配置文件 appsettings.json！\n" +
                        "请复制 appsettings.example.json 为 appsettings.json 并配置你的数据库连接。");
                }

                string json = File.ReadAllText(configPath);
                using var doc = JsonDocument.Parse(json);
                
                _connStr = doc.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("DefaultConnection")
                    .GetString();

                if (string.IsNullOrWhiteSpace(_connStr))
                {
                    throw new Exception("配置文件中的连接字符串为空！");
                }

                return _connStr;
            }
            catch (Exception ex)
            {
                throw new Exception($"读取数据库配置失败：{ex.Message}\n请检查 appsettings.json 文件是否正确配置。");
            }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        public static SqlConnection GetConn()
        {
            try
            {
                var conn = new SqlConnection(GetConnectionString());
                conn.Open();
                return conn;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL连接失败: {ex.Message}\n连接字符串: {GetConnectionString()}");
            }
        }

        /// <summary>
        /// 执行查询，返回单个值
        /// </summary>
        public static object? ExecScalar(string sql, params SqlParameter[] ps)
        {
            try
            {
                using var conn = GetConn();
                using var cmd = new SqlCommand(sql, conn);
                if (ps is { Length: > 0 }) cmd.Parameters.AddRange(ps);
                return cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL执行失败: {ex.Message}\nSQL: {sql}");
            }
        }

        /// <summary>
        /// 执行增删改操作，返回受影响的行数
        /// </summary>
        public static int ExecNonQuery(string sql, params SqlParameter[] ps)
        {
            try
            {
                using var conn = GetConn();
                using var cmd = new SqlCommand(sql, conn);
                if (ps is { Length: > 0 }) cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL执行失败: {ex.Message}\nSQL: {sql}");
            }
        }

        /// <summary>
        /// 执行查询，返回 DataTable
        /// </summary>
        public static DataTable ExecDataTable(string sql, params SqlParameter[] ps)
        {
            try
            {
                using var conn = GetConn();
                using var cmd = new SqlCommand(sql, conn);
                if (ps is { Length: > 0 }) cmd.Parameters.AddRange(ps);
                using var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
            catch (SqlException ex)
            {
                throw new Exception($"SQL执行失败: {ex.Message}\nSQL: {sql}");
            }
        }
    }
}
