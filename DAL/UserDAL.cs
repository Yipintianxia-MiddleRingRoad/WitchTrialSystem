using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    public class UserDAL
    {
        // 读取单个用户（含盐与哈希）
        public (int UserID, string Username, int RoleID, string Salt, string Hash)? GetByUsername(string username)
        {
            const string sql = @"
SELECT TOP 1 UserID, Username, RoleID, Salt, PasswordHash
FROM wt.[User]               -- ★ 关键：wt.[User]
WHERE Username = @u";
            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@u", username));
            if (dt.Rows.Count == 0) return null;
            var r = dt.Rows[0];
            return (
                Convert.ToInt32(r["UserID"]),
                Convert.ToString(r["Username"]) ?? "",
                Convert.ToInt32(r["RoleID"]),
                Convert.ToString(r["Salt"]) ?? "",
                Convert.ToString(r["PasswordHash"]) ?? ""
            );
        }

        // 写入新用户
        public int Insert(string username, int roleId, string salt, string hash)
        {
            const string sql = @"
INSERT INTO wt.[User](Username, RoleID, Salt, PasswordHash)
VALUES(@u, @r, @s, @h)";
            return DBHelper.ExecNonQuery(sql,
                new SqlParameter("@u", username),
                new SqlParameter("@r", roleId),
                new SqlParameter("@s", salt),
                new SqlParameter("@h", hash));
        }

        // 修改密码（你之前已加过，留着）
        public int UpdatePassword(string username, string newSalt, string newHash)
        {
            const string sql = @"UPDATE wt.[User]
                                SET Salt=@s, PasswordHash=@h
                                WHERE Username=@u";
            return DBHelper.ExecNonQuery(sql,
                new SqlParameter("@s", newSalt),
                new SqlParameter("@h", newHash),
                new SqlParameter("@u", username));
        }

        // 一次性为 PENDING 账号写入默认口令（供 Bootstrap 调用）
        public int InitPendingAccounts(IEnumerable<string> usernames, string salt, string hash)
        {
            const string sql = @"
UPDATE wt.[User]
SET Salt=@s, PasswordHash=@h
WHERE (Salt='PENDING' OR PasswordHash='PENDING') AND Username=@u";
            var n = 0;
            foreach (var u in usernames)
            {
                n += DBHelper.ExecNonQuery(sql,
                    new SqlParameter("@s", salt),
                    new SqlParameter("@h", hash),
                    new SqlParameter("@u", u));
            }
            return n;
        }
    }
}


// using Microsoft.Data.SqlClient;

// namespace WitchTrialSystem.DAL
// {
//     public class UserDAL
//     {
//         public (int UserID, string Username, string Salt, string Hash, int RoleID)? GetByUsername(string username)
//         {
//             const string sql = @"SELECT TOP 1 UserID, Username, Salt, PasswordHash, RoleID
//                                  FROM wt.[User] WHERE Username=@u";
//             using var conn = DBHelper.GetConn();
//             using var cmd = new SqlCommand(sql, conn);
//             cmd.Parameters.AddWithValue("@u", username);
//             using var rdr = cmd.ExecuteReader();
//             if (!rdr.Read()) return null;
//             return (rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetInt32(4));
//         }

//         // ✅ 唯一通用：若该用户为 PENDING，则写入盐和哈希
//         public void UpsertIfPending(string username, string salt, string hash)
//         {
//             const string sql = @"UPDATE wt.[User]
//                                  SET Salt=@s, PasswordHash=@h
//                                  WHERE Username=@u AND (Salt='PENDING' OR PasswordHash='PENDING')";
//             DBHelper.ExecNonQuery(sql,
//                 new SqlParameter("@s", salt),
//                 new SqlParameter("@h", hash),
//                 new SqlParameter("@u", username));
//         }

//         public int UpdatePassword(string username, string newSalt, string newHash)
//         {
//             const string sql = @"UPDATE wt.[User]
//                                 SET Salt=@s, PasswordHash=@h
//                                 WHERE Username=@u";
//             return DBHelper.ExecNonQuery(sql,
//                 new SqlParameter("@s", newSalt),
//                 new SqlParameter("@h", newHash),
//                 new SqlParameter("@u", username));
//         }

//     }
// }
