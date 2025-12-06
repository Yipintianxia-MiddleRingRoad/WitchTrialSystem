using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 智慧可视化大屏数据访问层
    /// 提供大屏所需的各种统计数据查询
    /// </summary>
    public class DashboardDAL
    {
        /// <summary>
        /// 获取全局统计数据（魔女总数、岛屿数、批次数）
        /// </summary>
        public DataTable GetGlobalStatistics()
        {
            const string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM wt.Witch) AS TotalWitches,
                    (SELECT COUNT(*) FROM wt.Island) AS TotalIslands,
                    (SELECT COUNT(*) FROM wt.Batch) AS TotalBatches,
                    (SELECT COUNT(DISTINCT b.BatchID) 
                     FROM wt.Batch b 
                     INNER JOIN wt.Witch w ON b.BatchID = w.BatchID) AS ActiveBatches";
            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 获取全局状态分布统计
        /// </summary>
        public DataTable GetStatusCounts()
        {
            const string sql = @"
                SELECT 
                    [Status],
                    COUNT(*) AS Count,
                    CAST(COUNT(*) * 100.0 / NULLIF((SELECT COUNT(*) FROM wt.Witch), 0) AS DECIMAL(5,2)) AS Percentage
                FROM wt.Witch
                GROUP BY [Status]
                ORDER BY Count DESC";
            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 获取指定岛屿的状态分布统计
        /// </summary>
        public DataTable GetIslandStatusCounts(int islandId)
        {
            const string sql = @"
                SELECT 
                    [Status],
                    COUNT(*) AS Count,
                    CAST(COUNT(*) * 100.0 / NULLIF((SELECT COUNT(*) FROM wt.Witch WHERE IslandID = @IslandID), 0) AS DECIMAL(5,2)) AS Percentage
                FROM wt.Witch
                WHERE IslandID = @IslandID
                GROUP BY [Status]
                ORDER BY Count DESC";
            return DBHelper.ExecDataTable(sql, new SqlParameter("@IslandID", islandId));
        }


        /// <summary>
        /// 获取所有岛屿列表
        /// </summary>
        public DataTable GetIslands()
        {
            const string sql = @"
                SELECT 
                    i.IslandID,
                    i.Name,
                    ISNULL(COUNT(w.WitchID), 0) AS WitchCount
                FROM wt.Island i
                LEFT JOIN wt.Witch w ON i.IslandID = w.IslandID
                GROUP BY i.IslandID, i.Name
                ORDER BY i.IslandID";
            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 获取指定岛屿的批次容量统计
        /// </summary>
        public DataTable GetBatchCapacity(int islandId)
        {
            const string sql = @"
                SELECT 
                    b.IslandID,
                    b.LocalBatchID,
                    b.BatchID,
                    ISNULL(COUNT(w.WitchID), 0) AS CurrentCount,
                    13 AS MaxCapacity,
                    CAST(ISNULL(COUNT(w.WitchID), 0) * 100.0 / 13 AS DECIMAL(5,2)) AS UsageRate
                FROM wt.Batch b
                LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
                WHERE b.IslandID = @IslandID
                GROUP BY b.IslandID, b.LocalBatchID, b.BatchID
                ORDER BY b.LocalBatchID";
            return DBHelper.ExecDataTable(sql, new SqlParameter("@IslandID", islandId));
        }

        /// <summary>
        /// 获取所有岛屿的批次容量统计
        /// </summary>
        public DataTable GetAllBatchCapacity()
        {
            const string sql = @"
                SELECT 
                    b.IslandID,
                    i.Name AS IslandName,
                    b.LocalBatchID,
                    b.BatchID,
                    ISNULL(COUNT(w.WitchID), 0) AS CurrentCount,
                    13 AS MaxCapacity,
                    CAST(ISNULL(COUNT(w.WitchID), 0) * 100.0 / 13 AS DECIMAL(5,2)) AS UsageRate
                FROM wt.Batch b
                LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
                LEFT JOIN wt.Island i ON b.IslandID = i.IslandID
                GROUP BY b.IslandID, i.Name, b.LocalBatchID, b.BatchID
                ORDER BY b.IslandID, b.LocalBatchID";
            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 获取批次状态矩阵（批次×状态的人数统计）
        /// </summary>
        public DataTable GetBatchStatusMatrix()
        {
            const string sql = @"
                SELECT 
                    b.IslandID,
                    b.LocalBatchID,
                    ISNULL(w.[Status], N'无') AS [Status],
                    COUNT(w.WitchID) AS Count
                FROM wt.Batch b
                LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
                GROUP BY b.IslandID, b.LocalBatchID, w.[Status]
                ORDER BY b.IslandID, b.LocalBatchID, w.[Status]";
            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 获取指定岛屿的批次状态矩阵
        /// </summary>
        public DataTable GetBatchStatusMatrix(int islandId)
        {
            const string sql = @"
                SELECT 
                    b.LocalBatchID,
                    ISNULL(w.[Status], N'无') AS [Status],
                    COUNT(w.WitchID) AS Count
                FROM wt.Batch b
                LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
                WHERE b.IslandID = @IslandID
                GROUP BY b.LocalBatchID, w.[Status]
                ORDER BY b.LocalBatchID, w.[Status]";
            return DBHelper.ExecDataTable(sql, new SqlParameter("@IslandID", islandId));
        }

        /// <summary>
        /// 获取指定状态的魔女列表
        /// </summary>
        public DataTable GetWitchesByStatus(string status, int? islandId = null)
        {
            var sql = @"
                SELECT 
                    w.WitchID,
                    w.PrisonerNo,
                    w.Name,
                    w.[Status],
                    w.Magic,
                    i.Name AS IslandName,
                    b.LocalBatchID
                FROM wt.Witch w
                LEFT JOIN wt.Island i ON w.IslandID = i.IslandID
                LEFT JOIN wt.Batch b ON w.BatchID = b.BatchID
                WHERE w.[Status] = @Status";
            
            var parameters = new System.Collections.Generic.List<SqlParameter>
            {
                new SqlParameter("@Status", status)
            };

            if (islandId.HasValue)
            {
                sql += " AND w.IslandID = @IslandID";
                parameters.Add(new SqlParameter("@IslandID", islandId.Value));
            }

            sql += " ORDER BY w.PrisonerNo";
            return DBHelper.ExecDataTable(sql, parameters.ToArray());
        }

        /// <summary>
        /// 获取指定批次的魔女列表
        /// </summary>
        public DataTable GetWitchesByBatch(int islandId, int localBatchId)
        {
            const string sql = @"
                SELECT 
                    w.WitchID,
                    w.PrisonerNo,
                    w.Name,
                    w.[Status],
                    w.Magic,
                    i.Name AS IslandName,
                    b.LocalBatchID
                FROM wt.Witch w
                LEFT JOIN wt.Island i ON w.IslandID = i.IslandID
                LEFT JOIN wt.Batch b ON w.BatchID = b.BatchID
                WHERE b.IslandID = @IslandID AND b.LocalBatchID = @LocalBatchID
                ORDER BY w.PrisonerNo";
            return DBHelper.ExecDataTable(sql, 
                new SqlParameter("@IslandID", islandId),
                new SqlParameter("@LocalBatchID", localBatchId));
        }

        /// <summary>
        /// 获取指定批次和状态的魔女列表
        /// </summary>
        public DataTable GetWitchesByBatchAndStatus(int islandId, int localBatchId, string status)
        {
            const string sql = @"
                SELECT 
                    w.WitchID,
                    w.PrisonerNo,
                    w.Name,
                    w.[Status],
                    w.Magic,
                    i.Name AS IslandName,
                    b.LocalBatchID
                FROM wt.Witch w
                LEFT JOIN wt.Island i ON w.IslandID = i.IslandID
                LEFT JOIN wt.Batch b ON w.BatchID = b.BatchID
                WHERE b.IslandID = @IslandID 
                  AND b.LocalBatchID = @LocalBatchID 
                  AND w.[Status] = @Status
                ORDER BY w.PrisonerNo";
            return DBHelper.ExecDataTable(sql, 
                new SqlParameter("@IslandID", islandId),
                new SqlParameter("@LocalBatchID", localBatchId),
                new SqlParameter("@Status", status));
        }
    }
}
