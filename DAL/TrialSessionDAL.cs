using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 审判会话数据访问层
    /// </summary>
    public class TrialSessionDAL
    {
        /// <summary>
        /// 插入新审判会话
        /// </summary>
        public static int Insert(TrialSessionModel session)
        {
            const string sql = @"
                INSERT INTO wt.TrialSession (IslandID, BatchID, Status, CreatedBy, CreatedAt)
                VALUES (@IslandID, @BatchID, @Status, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new[]
            {
                new SqlParameter("@IslandID", session.IslandID),
                new SqlParameter("@BatchID", session.BatchID),
                new SqlParameter("@Status", session.Status),
                new SqlParameter("@CreatedBy", session.CreatedBy),
                new SqlParameter("@CreatedAt", session.CreatedAt)
            };

            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 更新审判会话
        /// </summary>
        public static int Update(TrialSessionModel session)
        {
            const string sql = @"
                UPDATE wt.TrialSession
                SET Status = @Status,
                    VotingStartTime = @VotingStartTime,
                    VotingEndTime = @VotingEndTime,
                    ExecutionTargetWitchID = @ExecutionTargetWitchID,
                    ExecutionConfirmedAt = @ExecutionConfirmedAt,
                    CompletedAt = @CompletedAt
                WHERE SessionID = @SessionID";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", session.SessionID),
                new SqlParameter("@Status", session.Status),
                new SqlParameter("@VotingStartTime", (object?)session.VotingStartTime ?? DBNull.Value),
                new SqlParameter("@VotingEndTime", (object?)session.VotingEndTime ?? DBNull.Value),
                new SqlParameter("@ExecutionTargetWitchID", (object?)session.ExecutionTargetWitchID ?? DBNull.Value),
                new SqlParameter("@ExecutionConfirmedAt", (object?)session.ExecutionConfirmedAt ?? DBNull.Value),
                new SqlParameter("@CompletedAt", (object?)session.CompletedAt ?? DBNull.Value)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 更新审判状态
        /// </summary>
        public static int UpdateStatus(int sessionID, string status)
        {
            const string sql = "UPDATE wt.TrialSession SET Status = @Status WHERE SessionID = @SessionID";
            
            var parameters = new[]
            {
                new SqlParameter("@SessionID", sessionID),
                new SqlParameter("@Status", status)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 更新处刑对象
        /// </summary>
        public static int UpdateExecutionTarget(int sessionID, int targetWitchID)
        {
            const string sql = "UPDATE wt.TrialSession SET ExecutionTargetWitchID = @TargetWitchID WHERE SessionID = @SessionID";
            
            var parameters = new[]
            {
                new SqlParameter("@SessionID", sessionID),
                new SqlParameter("@TargetWitchID", targetWitchID)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 按ID查询审判会话
        /// </summary>
        public static TrialSessionModel? GetByID(int sessionID)
        {
            const string sql = "SELECT * FROM wt.TrialSession WHERE SessionID = @SessionID";
            
            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };
            
            DataTable dt = DBHelper.ExecDataTable(sql, parameters);
            
            if (dt.Rows.Count == 0)
                return null;

            return MapToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 查询岛屿的进行中审判
        /// </summary>
        public static TrialSessionModel? GetActiveByIsland(int islandID)
        {
            const string sql = @"
                SELECT TOP 1 * FROM wt.TrialSession 
                WHERE IslandID = @IslandID 
                  AND Status NOT IN (N'Completed', N'Cancelled')
                ORDER BY CreatedAt DESC";
            
            var parameters = new[] { new SqlParameter("@IslandID", islandID) };
            
            DataTable dt = DBHelper.ExecDataTable(sql, parameters);
            
            if (dt.Rows.Count == 0)
                return null;

            return MapToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 查询岛屿的历史审判
        /// </summary>
        public static List<TrialSessionModel> GetByIsland(int islandID, int limit = 10)
        {
            string sql = $@"
                SELECT TOP {limit} * FROM wt.TrialSession 
                WHERE IslandID = @IslandID 
                ORDER BY CreatedAt DESC";
            
            var parameters = new[] { new SqlParameter("@IslandID", islandID) };
            
            DataTable dt = DBHelper.ExecDataTable(sql, parameters);
            
            var list = new List<TrialSessionModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapToModel(row));
            }
            
            return list;
        }

        /// <summary>
        /// 将DataRow映射到Model
        /// </summary>
        private static TrialSessionModel MapToModel(DataRow row)
        {
            return new TrialSessionModel
            {
                SessionID = Convert.ToInt32(row["SessionID"]),
                IslandID = Convert.ToInt32(row["IslandID"]),
                BatchID = Convert.ToInt32(row["BatchID"]),
                Status = row["Status"].ToString() ?? "",
                CreatedBy = Convert.ToInt32(row["CreatedBy"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"]),
                VotingStartTime = row["VotingStartTime"] == DBNull.Value ? null : Convert.ToDateTime(row["VotingStartTime"]),
                VotingEndTime = row["VotingEndTime"] == DBNull.Value ? null : Convert.ToDateTime(row["VotingEndTime"]),
                ExecutionTargetWitchID = row["ExecutionTargetWitchID"] == DBNull.Value ? null : Convert.ToInt32(row["ExecutionTargetWitchID"]),
                ExecutionConfirmedAt = row["ExecutionConfirmedAt"] == DBNull.Value ? null : Convert.ToDateTime(row["ExecutionConfirmedAt"]),
                CompletedAt = row["CompletedAt"] == DBNull.Value ? null : Convert.ToDateTime(row["CompletedAt"])
            };
        }
    }
}
