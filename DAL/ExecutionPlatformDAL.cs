using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 处刑台数据访问层
    /// </summary>
    public class ExecutionPlatformDAL
    {
        /// <summary>
        /// 按岛屿查询所有处刑台
        /// </summary>
        public List<ExecutionPlatformModel> GetByIsland(int islandID)
        {
            const string sql = @"
SELECT PlatformID, IslandID, PlatformNumber, HomePosition, CurrentPosition,
       ToolName, ToolType, ToolDescription, Status, CreatedAt, UpdatedAt
FROM wt.ExecutionPlatform
WHERE IslandID = @islandID
ORDER BY PlatformNumber";

            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@islandID", islandID));
            return DataTableToList(dt);
        }

        /// <summary>
        /// 按ID查询单个处刑台
        /// </summary>
        public ExecutionPlatformModel? GetByID(int platformID)
        {
            const string sql = @"
SELECT PlatformID, IslandID, PlatformNumber, HomePosition, CurrentPosition,
       ToolName, ToolType, ToolDescription, Status, CreatedAt, UpdatedAt
FROM wt.ExecutionPlatform
WHERE PlatformID = @platformID";

            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@platformID", platformID));
            if (dt.Rows.Count == 0) return null;
            return DataRowToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 按位置查询处刑台
        /// </summary>
        public ExecutionPlatformModel? GetByPosition(int islandID, int position)
        {
            const string sql = @"
SELECT PlatformID, IslandID, PlatformNumber, HomePosition, CurrentPosition,
       ToolName, ToolType, ToolDescription, Status, CreatedAt, UpdatedAt
FROM wt.ExecutionPlatform
WHERE IslandID = @islandID AND CurrentPosition = @position";

            var dt = DBHelper.ExecDataTable(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@position", position));

            if (dt.Rows.Count == 0) return null;
            return DataRowToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 检查位置是否被占用
        /// </summary>
        public bool IsPositionOccupied(int islandID, int position)
        {
            const string sql = @"
SELECT COUNT(1)
FROM wt.ExecutionPlatform
WHERE IslandID = @islandID AND CurrentPosition = @position";

            var result = DBHelper.ExecScalar(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@position", position));

            return result != null && Convert.ToInt32(result) > 0;
        }

        /// <summary>
        /// 获取指定位置的处刑台数量
        /// </summary>
        public int GetPlatformCountAtPosition(int islandID, int position)
        {
            const string sql = @"
SELECT COUNT(1)
FROM wt.ExecutionPlatform
WHERE IslandID = @islandID AND CurrentPosition = @position";

            var result = DBHelper.ExecScalar(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@position", position));

            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 插入新处刑台
        /// </summary>
        public int Insert(ExecutionPlatformModel platform)
        {
            const string sql = @"
INSERT INTO wt.ExecutionPlatform (IslandID, PlatformNumber, HomePosition, CurrentPosition,
                                   ToolName, ToolType, ToolDescription, Status, CreatedAt, UpdatedAt)
VALUES (@islandID, @platformNumber, @homePosition, @currentPosition,
        @toolName, @toolType, @toolDescription, @status, @createdAt, @updatedAt);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var result = DBHelper.ExecScalar(sql,
                new SqlParameter("@islandID", platform.IslandID),
                new SqlParameter("@platformNumber", platform.PlatformNumber),
                new SqlParameter("@homePosition", platform.HomePosition),
                new SqlParameter("@currentPosition", platform.CurrentPosition),
                new SqlParameter("@toolName", (object?)platform.ToolName ?? DBNull.Value),
                new SqlParameter("@toolType", (object?)platform.ToolType ?? DBNull.Value),
                new SqlParameter("@toolDescription", (object?)platform.ToolDescription ?? DBNull.Value),
                new SqlParameter("@status", platform.Status),
                new SqlParameter("@createdAt", platform.CreatedAt),
                new SqlParameter("@updatedAt", platform.UpdatedAt));

            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 更新处刑台信息
        /// </summary>
        public int Update(ExecutionPlatformModel platform)
        {
            const string sql = @"
UPDATE wt.ExecutionPlatform
SET CurrentPosition = @currentPosition,
    ToolName = @toolName,
    ToolType = @toolType,
    ToolDescription = @toolDescription,
    Status = @status,
    UpdatedAt = @updatedAt
WHERE PlatformID = @platformID";

            return DBHelper.ExecNonQuery(sql,
                new SqlParameter("@currentPosition", platform.CurrentPosition),
                new SqlParameter("@toolName", (object?)platform.ToolName ?? DBNull.Value),
                new SqlParameter("@toolType", (object?)platform.ToolType ?? DBNull.Value),
                new SqlParameter("@toolDescription", (object?)platform.ToolDescription ?? DBNull.Value),
                new SqlParameter("@status", platform.Status),
                new SqlParameter("@updatedAt", DateTime.Now),
                new SqlParameter("@platformID", platform.PlatformID));
        }

        /// <summary>
        /// 删除处刑台
        /// </summary>
        public int Delete(int platformID)
        {
            const string sql = @"
DELETE FROM wt.ExecutionPlatform
WHERE PlatformID = @platformID";

            return DBHelper.ExecNonQuery(sql, new SqlParameter("@platformID", platformID));
        }

        /// <summary>
        /// 批量插入处刑台
        /// </summary>
        public int InsertBatch(List<ExecutionPlatformModel> platforms)
        {
            int count = 0;
            foreach (var platform in platforms)
            {
                Insert(platform);
                count++;
            }
            return count;
        }

        #region 辅助方法

        /// <summary>
        /// DataTable 转换为 List
        /// </summary>
        private List<ExecutionPlatformModel> DataTableToList(DataTable dt)
        {
            var list = new List<ExecutionPlatformModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(DataRowToModel(row));
            }
            return list;
        }

        /// <summary>
        /// DataRow 转换为 Model
        /// </summary>
        private ExecutionPlatformModel DataRowToModel(DataRow row)
        {
            return new ExecutionPlatformModel
            {
                PlatformID = Convert.ToInt32(row["PlatformID"]),
                IslandID = Convert.ToInt32(row["IslandID"]),
                PlatformNumber = Convert.ToInt32(row["PlatformNumber"]),
                HomePosition = Convert.ToInt32(row["HomePosition"]),
                CurrentPosition = Convert.ToInt32(row["CurrentPosition"]),
                ToolName = row["ToolName"] == DBNull.Value ? null : Convert.ToString(row["ToolName"]),
                ToolType = row["ToolType"] == DBNull.Value ? null : Convert.ToString(row["ToolType"]),
                ToolDescription = row["ToolDescription"] == DBNull.Value ? null : Convert.ToString(row["ToolDescription"]),
                Status = Convert.ToString(row["Status"]) ?? "空闲",
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                UpdatedAt = Convert.ToDateTime(row["UpdatedAt"])
            };
        }

        #endregion
    }
}
