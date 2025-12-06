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

        /// <summary>
        /// 检查用户名是否已存在
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>存在返回true，否则返回false</returns>
        public bool UserExists(string username)
        {
            const string sql = @"
SELECT COUNT(1)
FROM wt.[User]
WHERE Username = @u";
            var result = DBHelper.ExecScalar(sql, new SqlParameter("@u", username));
            return result != null && Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 为魔女创建用户账号并建立关联
        /// </summary>
        /// <param name="username">用户名（囚犯编号）</param>
        /// <param name="roleId">角色ID（Witch角色）</param>
        /// <param name="islandId">岛屿ID</param>
        /// <param name="batchId">批次ID</param>
        /// <param name="salt">密码盐值</param>
        /// <param name="hash">密码哈希</param>
        /// <param name="witchId">魔女ID</param>
        /// <returns>新创建的UserID</returns>
        public int CreateWitchAccountWithAssociation(
            string username,
            int roleId,
            int islandId,
            int batchId,
            string salt,
            string hash,
            int witchId)
        {
            using var conn = new SqlConnection(DBHelper.GetConn().ConnectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. 创建User记录
                const string insertUserSql = @"
INSERT INTO wt.[User] (Username, RoleID, IslandID, BatchID, Salt, PasswordHash, GomokuScore)
VALUES (@username, @roleId, @islandId, @batchId, @salt, @hash, 0);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

                int newUserId;
                using (var cmd = new SqlCommand(insertUserSql, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@roleId", roleId);
                    cmd.Parameters.AddWithValue("@islandId", islandId);
                    cmd.Parameters.AddWithValue("@batchId", batchId);
                    cmd.Parameters.AddWithValue("@salt", salt);
                    cmd.Parameters.AddWithValue("@hash", hash);
                    
                    var result = cmd.ExecuteScalar();
                    newUserId = Convert.ToInt32(result);
                }

                // 2. 创建UserWitch关联记录
                const string insertUserWitchSql = @"
INSERT INTO wt.UserWitch (UserID, WitchID)
VALUES (@userId, @witchId);";

                using (var cmd = new SqlCommand(insertUserWitchSql, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@userId", newUserId);
                    cmd.Parameters.AddWithValue("@witchId", witchId);
                    cmd.ExecuteNonQuery();
                }

                // 3. 提交事务
                transaction.Commit();
                return newUserId;
            }
            catch
            {
                // 回滚事务
                transaction.Rollback();
                throw;
            }
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
