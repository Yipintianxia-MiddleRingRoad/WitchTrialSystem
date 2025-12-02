using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 权限控制数据访问层
    /// 根据用户角色控制数据访问范围
    /// </summary>
    public class PermissionDAL
    {
        /// <summary>
        /// 根据用户权限获取可访问的魔女数据
        /// </summary>
        /// <param name="username">当前用户名</param>
        /// <param name="nameLike">搜索关键词</param>
        /// <returns>符合权限的魔女数据</returns>
        public DataTable GetWitchesByPermission(string username, string? nameLike = null)
        {
            // 1. 获取用户信息和权限
            const string userPermissionSql = @"
SELECT 
    u.UserID,
    u.Username,
    r.Name AS RoleName,
    u.IslandID,
    u.BatchID,
    ir.UserID AS IsRegulator,
    iw.UserID AS IsWarden
FROM wt.[User] u
LEFT JOIN wt.Role r ON r.RoleID = u.RoleID
LEFT JOIN wt.IslandRegulator ir ON ir.UserID = u.UserID
LEFT JOIN wt.IslandWarden iw ON iw.UserID = u.UserID
WHERE u.Username = @username";

            var userDt = DBHelper.ExecDataTable(userPermissionSql, new SqlParameter("@username", username));
            if (userDt.Rows.Count == 0) return new DataTable();
            
            var userRow = userDt.Rows[0];
            var roleName = userRow["RoleName"].ToString();
            var userId = Convert.ToInt32(userRow["UserID"]);
            var isRegulator = userRow["IsRegulator"] != DBNull.Value;
            var isWarden = userRow["IsWarden"] != DBNull.Value;
            var userIslandId = userRow["IslandID"] == DBNull.Value ? (int?)null : Convert.ToInt32(userRow["IslandID"]);
            var userBatchId = userRow["BatchID"] == DBNull.Value ? (int?)null : Convert.ToInt32(userRow["BatchID"]);

            // 2. 根据角色构建查询
            string sql = @"
SELECT 
    WitchID,
    PrisonerNo,
    PersonalNo,
    Name,
    Gender,
    BirthDate,
    DATEDIFF(YEAR, BirthDate, GETDATE()) AS Age,
    Height,
    Weight,
    BloodType,
    Magic,
    [Status],
    HighestEducation,
    Birthplace,
    Phone,
    Email,
    Skills,
    Hobbies,
    Dreams,
    Trauma,
    IslandID,
    BatchID,
    AvatarPath,
    DescriptionPublic
FROM wt.Witch WHERE 1=1";

            var parameters = new System.Collections.Generic.List<SqlParameter>();
            
            switch (roleName)
            {
                case "Admin":
                    // 管理员：可以查看所有岛屿所有批次
                    // 无额外限制
                    break;
                    
                case "Meruru":
                    if (isRegulator && userIslandId.HasValue)
                    {
                        // 管理者：只能查看本岛屿所有批次
                        sql += " AND IslandID = @islandId";
                        parameters.Add(new SqlParameter("@islandId", userIslandId.Value));
                    }
                    break;
                    
                case "Warden":
                    if (isWarden && userIslandId.HasValue)
                    {
                        // 典狱长：只能查看本岛屿所有批次，受本岛屿管理者控制
                        sql += " AND IslandID = @islandId";
                        parameters.Add(new SqlParameter("@islandId", userIslandId.Value));
                    }
                    break;
                    
                case "Witch":
                    if (userIslandId.HasValue && userBatchId.HasValue)
                    {
                        // 普通魔女：只能查看本岛屿本批次（13人）
                        sql += " AND IslandID = @islandId AND BatchID = @batchId";
                        parameters.Add(new SqlParameter("@islandId", userIslandId.Value));
                        parameters.Add(new SqlParameter("@batchId", userBatchId.Value));
                    }
                    break;
                    
                default:
                    // 未知角色：返回空结果
                    return new DataTable();
            }

            // 3. 添加搜索条件
            if (!string.IsNullOrWhiteSpace(nameLike))
            {
                sql += " AND Name LIKE @nameLike";
                parameters.Add(new SqlParameter("@nameLike", "%" + nameLike.Trim() + "%"));
            }

            sql += " ORDER BY PrisonerNo";

            return DBHelper.ExecDataTable(sql, parameters.ToArray());
        }

        /// <summary>
        /// 根据用户权限获取可访问的岛屿列表
        /// </summary>
        public DataTable GetIslandsByPermission(string username)
        {
            string sql = @"
SELECT DISTINCT 
    i.IslandID,
    i.Name
FROM wt.Island i
WHERE 1=1";

            // 获取用户权限
            var userPermissionSql = @"
SELECT r.Name AS RoleName,
       u.IslandID,
       ir.UserID AS IsRegulator,
       iw.UserID AS IsWarden
FROM wt.[User] u
LEFT JOIN wt.Role r ON r.RoleID = u.RoleID
LEFT JOIN wt.IslandRegulator ir ON ir.UserID = u.UserID
LEFT JOIN wt.IslandWarden iw ON iw.UserID = u.UserID
WHERE u.Username = @username";

            var userDt = DBHelper.ExecDataTable(userPermissionSql, new SqlParameter("@username", username));
            if (userDt.Rows.Count == 0) return new DataTable();

            var userRow = userDt.Rows[0];
            var roleName = userRow["RoleName"].ToString();
            var userIslandId = userRow["IslandID"] == DBNull.Value ? (int?)null : Convert.ToInt32(userRow["IslandID"]);
            var isRegulator = userRow["IsRegulator"] != DBNull.Value;
            var isWarden = userRow["IsWarden"] != DBNull.Value;

            switch (roleName)
            {
                case "Admin":
                    // 管理员：可以看到所有岛屿
                    break;
                    
                case "Meruru":
                    if (userIslandId.HasValue && isRegulator)
                    {
                        sql += " AND i.IslandID = @islandId";
                        return DBHelper.ExecDataTable(sql, new SqlParameter("@islandId", userIslandId.Value));
                    }
                    break;
                    
                case "Warden":
                    if (userIslandId.HasValue && isWarden)
                    {
                        sql += " AND i.IslandID = @islandId";
                        return DBHelper.ExecDataTable(sql, new SqlParameter("@islandId", userIslandId.Value));
                    }
                    break;
                    
                case "Witch":
                    if (userIslandId.HasValue)
                    {
                        sql += " AND i.IslandID = @islandId";
                        return DBHelper.ExecDataTable(sql, new SqlParameter("@islandId", userIslandId.Value));
                    }
                    break;
            }

            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 检查用户是否有权限访问特定魔女
        /// </summary>
        public bool CanAccessWitch(string username, int witchId)
        {
            const string sql = @"
SELECT CASE 
    WHEN r.Name = 'Admin' THEN 1
    WHEN r.Name = 'Meruru' AND ir.UserID IS NOT NULL AND ir.IslandID = w.IslandID THEN 1
    WHEN r.Name = 'Warden' AND iw.UserID IS NOT NULL AND iw.IslandID = w.IslandID THEN 1
    WHEN r.Name = 'Witch' AND u.IslandID = w.IslandID AND u.BatchID = w.BatchID THEN 1
    ELSE 0
END AS CanAccess
FROM wt.Witch w
CROSS JOIN wt.[User] u
LEFT JOIN wt.Role r ON r.RoleID = u.RoleID
LEFT JOIN wt.IslandRegulator ir ON ir.UserID = u.UserID
LEFT JOIN wt.IslandWarden iw ON iw.UserID = u.UserID
WHERE u.Username = @username AND w.WitchID = @witchId";

            var dt = DBHelper.ExecDataTable(sql, 
                new SqlParameter("@username", username),
                new SqlParameter("@witchId", witchId));

            return dt.Rows.Count > 0 && Convert.ToBoolean(dt.Rows[0]["CanAccess"]);
        }
    }
}