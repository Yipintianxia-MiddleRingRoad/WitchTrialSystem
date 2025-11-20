using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.DAL.Models;

namespace WitchTrialSystem.DAL
{
    public class UserProfileDAL
    {
        public DataTable GetProfile(string username)
        {
            const string sql = @"
SELECT
    u.UserID,
    u.Username,
    r.Name AS RoleName,
    w.WitchID,
    w.Name       AS CnName,
    w.PrisonerNo,
    w.Magic,
    w.AvatarPath,
    w.IslandID,
    w.BatchID,
    ISNULL(u.GomokuScore, 0) AS GomokuScore
FROM wt.[User]      AS u
LEFT JOIN wt.Role    AS r  ON r.RoleID  = u.RoleID
LEFT JOIN wt.UserWitch AS uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch   AS w  ON w.WitchID = uw.WitchID
WHERE u.Username = @u;";

            return DBHelper.ExecDataTable(sql, new SqlParameter("@u", username));
        }

        /// <summary>
        /// 获取用户档案对象
        /// </summary>
        public UserProfile? GetUserProfile(string username)
        {
            var dt = GetProfile(username);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new UserProfile
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"].ToString() ?? "",
                RoleName = row["RoleName"].ToString() ?? "",
                WitchID = row["WitchID"] == DBNull.Value ? null : Convert.ToInt32(row["WitchID"]),
                CnName = row["CnName"] == DBNull.Value ? null : row["CnName"].ToString(),
                PrisonerNo = row["PrisonerNo"] == DBNull.Value ? null : row["PrisonerNo"].ToString(),
                Magic = row["Magic"] == DBNull.Value ? null : row["Magic"].ToString(),
                CharacterImage = row["AvatarPath"] == DBNull.Value ? null : row["AvatarPath"].ToString(),
                IslandID = row["IslandID"] == DBNull.Value ? null : Convert.ToInt32(row["IslandID"]),
                BatchID = row["BatchID"] == DBNull.Value ? null : Convert.ToInt32(row["BatchID"]),
                GomokuScore = row["GomokuScore"] == DBNull.Value ? 0 : Convert.ToInt32(row["GomokuScore"])
            };
        }

        /// <summary>
        /// 更新用户档案（主要用于更新五子棋积分）
        /// </summary>
        public bool UpdateUserProfile(UserProfile profile)
        {
            const string sql = @"
UPDATE wt.[User]
SET GomokuScore = @GomokuScore
WHERE Username = @Username;";

            var parameters = new[]
            {
                new SqlParameter("@GomokuScore", profile.GomokuScore),
                new SqlParameter("@Username", profile.Username)
            };

            return DBHelper.ExecNonQuery(sql, parameters) > 0;
        }
    }
}

// using System.Data;
// using Microsoft.Data.SqlClient;

// namespace WitchTrialSystem.DAL
// {
//     public class UserProfileDAL
//     {
//         public DataTable GetProfile(string username)
//         {
//             const string sql = @"
// SELECT u.UserID, u.Username,
//        r.Name   AS RoleName,
//        w.WitchID, w.Name AS CnName, w.PrisonerNo, w.Magic, w.AvatarPath
//        w.IslandID, w.BatchID
// FROM wt.[User] u
// LEFT JOIN wt.Role r      ON r.RoleID = u.RoleID
// LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
// LEFT JOIN wt.Witch w      ON w.WitchID = uw.WitchID
// WHERE u.Username = @u";

//             return DBHelper.ExecDataTable(sql, new SqlParameter("@u", username));
//         }
//     }
// }
