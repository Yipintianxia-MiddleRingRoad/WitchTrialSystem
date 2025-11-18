using System.Data;
using Microsoft.Data.SqlClient;

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
    w.IslandID,                 -- ★ 新增：岛
    w.BatchID                   -- ★ 新增：批
FROM wt.[User]      AS u       -- ★ 一定要 wt.[User]
LEFT JOIN wt.Role    AS r  ON r.RoleID  = u.RoleID
LEFT JOIN wt.UserWitch AS uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch   AS w  ON w.WitchID = uw.WitchID
WHERE u.Username = @u;";

            return DBHelper.ExecDataTable(sql, new SqlParameter("@u", username));
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
