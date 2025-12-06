using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 处刑台移动记录数据访问层
    /// </summary>
    public class MovementLogDAL
    {
        /// <summary>
        /// 按岛屿查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetByIsland(int islandID)
        {
            const string sql = @"
SELECT LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE IslandID = @islandID
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@islandID", islandID));
            return DataTableToList(dt);
        }

        /// <summary>
        /// 按处刑台查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetByPlatform(int platformID)
        {
            const string sql = @"
SELECT LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE PlatformID = @platformID
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@platformID", platformID));
            return DataTableToList(dt);
        }

        /// <summary>
        /// 按时间范围查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetByTimeRange(int islandID, DateTime startTime, DateTime endTime)
        {
            const string sql = @"
SELECT LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE IslandID = @islandID 
  AND MovementTime >= @startTime 
  AND MovementTime <= @endTime
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@startTime", startTime),
                new SqlParameter("@endTime", endTime));

            return DataTableToList(dt);
        }

        /// <summary>
        /// 按位置查询移动记录（起始或目标位置匹配）
        /// </summary>
        public List<PlatformMovementLogModel> GetByPosition(int islandID, int position)
        {
            const string sql = @"
SELECT LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE IslandID = @islandID 
  AND (FromPosition = @position OR ToPosition = @position)
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@position", position));

            return DataTableToList(dt);
        }

        /// <summary>
        /// 插入移动记录
        /// </summary>
        public int Insert(PlatformMovementLogModel log)
        {
            const string sql = @"
INSERT INTO wt.PlatformMovementLog (IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
                                     ToolName, MovementTime, IsManualTime, MovementType)
VALUES (@islandID, @platformID, @platformNumber, @fromPosition, @toPosition,
        @toolName, @movementTime, @isManualTime, @movementType);
SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var result = DBHelper.ExecScalar(sql,
                new SqlParameter("@islandID", log.IslandID),
                new SqlParameter("@platformID", log.PlatformID),
                new SqlParameter("@platformNumber", log.PlatformNumber),
                new SqlParameter("@fromPosition", log.FromPosition),
                new SqlParameter("@toPosition", log.ToPosition),
                new SqlParameter("@toolName", (object?)log.ToolName ?? DBNull.Value),
                new SqlParameter("@movementTime", log.MovementTime),
                new SqlParameter("@isManualTime", log.IsManualTime),
                new SqlParameter("@movementType", log.MovementType));

            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 按处刑台编号查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetByPlatformNumber(int islandID, int platformNumber)
        {
            const string sql = @"
SELECT LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE IslandID = @islandID AND PlatformNumber = @platformNumber
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql,
                new SqlParameter("@islandID", islandID),
                new SqlParameter("@platformNumber", platformNumber));

            return DataTableToList(dt);
        }

        /// <summary>
        /// 获取最近N条移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetRecent(int islandID, int count = 100)
        {
            string sql = $@"
SELECT TOP {count} LogID, IslandID, PlatformID, PlatformNumber, FromPosition, ToPosition,
       ToolName, MovementTime, IsManualTime, MovementType
FROM wt.PlatformMovementLog
WHERE IslandID = @islandID
ORDER BY MovementTime DESC";

            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@islandID", islandID));
            return DataTableToList(dt);
        }

        #region 辅助方法

        /// <summary>
        /// DataTable 转换为 List
        /// </summary>
        private List<PlatformMovementLogModel> DataTableToList(DataTable dt)
        {
            var list = new List<PlatformMovementLogModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(DataRowToModel(row));
            }
            return list;
        }

        /// <summary>
        /// DataRow 转换为 Model
        /// </summary>
        private PlatformMovementLogModel DataRowToModel(DataRow row)
        {
            return new PlatformMovementLogModel
            {
                LogID = Convert.ToInt32(row["LogID"]),
                IslandID = Convert.ToInt32(row["IslandID"]),
                PlatformID = Convert.ToInt32(row["PlatformID"]),
                PlatformNumber = Convert.ToInt32(row["PlatformNumber"]),
                FromPosition = Convert.ToInt32(row["FromPosition"]),
                ToPosition = Convert.ToInt32(row["ToPosition"]),
                ToolName = row["ToolName"] == DBNull.Value ? null : Convert.ToString(row["ToolName"]),
                MovementTime = Convert.ToDateTime(row["MovementTime"]),
                IsManualTime = Convert.ToBoolean(row["IsManualTime"]),
                MovementType = Convert.ToString(row["MovementType"]) ?? ""
            };
        }

        #endregion
    }
}
