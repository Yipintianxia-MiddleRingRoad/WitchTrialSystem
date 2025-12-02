using Microsoft.Data.SqlClient;


namespace WitchTrialSystem.DAL
{
    public static class AuditDAL
    {
        public static void Log(int? userId, string username, string action, string? target = null, string? detail = null)
        {
            const string sql = @"EXEC wt.sp_LogOperation @UserID=@uid, @Username=@un, @Action=@ac, @Target=@tg, @Detail=@dt";
            DBHelper.ExecNonQuery(sql,
                new SqlParameter("@uid", (object?)userId ?? System.DBNull.Value),
                new SqlParameter("@un", username),
                new SqlParameter("@ac", action),
                new SqlParameter("@tg", (object?)target ?? System.DBNull.Value),
                new SqlParameter("@dt", (object?)detail ?? System.DBNull.Value));
        }
    }
}
