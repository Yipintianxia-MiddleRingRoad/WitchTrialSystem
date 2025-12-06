using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 审判参与者数据访问层
    /// </summary>
    public class TrialParticipantDAL
    {
        /// <summary>
        /// 插入参与者记录
        /// </summary>
        public static int Insert(TrialParticipantModel participant)
        {
            const string sql = @"
                INSERT INTO wt.TrialParticipant (SessionID, WitchID, UserID, HasVoted, HasConfirmedExecution)
                VALUES (@SessionID, @WitchID, @UserID, @HasVoted, @HasConfirmedExecution);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", participant.SessionID),
                new SqlParameter("@WitchID", participant.WitchID),
                new SqlParameter("@UserID", participant.UserID),
                new SqlParameter("@HasVoted", participant.HasVoted),
                new SqlParameter("@HasConfirmedExecution", participant.HasConfirmedExecution)
            };

            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 批量插入参与者
        /// </summary>
        public static int InsertBatch(List<TrialParticipantModel> participants)
        {
            int count = 0;
            foreach (var participant in participants)
            {
                int result = Insert(participant);
                if (result > 0)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 更新参与者记录
        /// </summary>
        public static int Update(TrialParticipantModel participant)
        {
            const string sql = @"
                UPDATE wt.TrialParticipant
                SET HasVoted = @HasVoted,
                    VotedForWitchID = @VotedForWitchID,
                    VotedAt = @VotedAt,
                    HasConfirmedExecution = @HasConfirmedExecution,
                    ExecutionConfirmedAt = @ExecutionConfirmedAt
                WHERE ParticipantID = @ParticipantID";

            var parameters = new[]
            {
                new SqlParameter("@ParticipantID", participant.ParticipantID),
                new SqlParameter("@HasVoted", participant.HasVoted),
                new SqlParameter("@VotedForWitchID", (object?)participant.VotedForWitchID ?? DBNull.Value),
                new SqlParameter("@VotedAt", (object?)participant.VotedAt ?? DBNull.Value),
                new SqlParameter("@HasConfirmedExecution", participant.HasConfirmedExecution),
                new SqlParameter("@ExecutionConfirmedAt", (object?)participant.ExecutionConfirmedAt ?? DBNull.Value)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 更新投票
        /// </summary>
        public static int UpdateVote(int participantID, int votedForWitchID)
        {
            const string sql = @"
                UPDATE wt.TrialParticipant
                SET HasVoted = 1,
                    VotedForWitchID = @VotedForWitchID,
                    VotedAt = GETDATE()
                WHERE ParticipantID = @ParticipantID";

            var parameters = new[]
            {
                new SqlParameter("@ParticipantID", participantID),
                new SqlParameter("@VotedForWitchID", votedForWitchID)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 更新处刑确认
        /// </summary>
        public static int UpdateExecutionConfirmation(int participantID)
        {
            const string sql = @"
                UPDATE wt.TrialParticipant
                SET HasConfirmedExecution = 1,
                    ExecutionConfirmedAt = GETDATE()
                WHERE ParticipantID = @ParticipantID";

            var parameters = new[] { new SqlParameter("@ParticipantID", participantID) };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 查询会话的所有参与者
        /// </summary>
        public static List<TrialParticipantModel> GetBySession(int sessionID)
        {
            const string sql = @"
                SELECT 
                    p.*,
                    w.Name AS WitchName,
                    w.AvatarPath,
                    u.Username,
                    vw.Name AS VotedForWitchName
                FROM wt.TrialParticipant p
                INNER JOIN wt.Witch w ON p.WitchID = w.WitchID
                INNER JOIN wt.[User] u ON p.UserID = u.UserID
                LEFT JOIN wt.Witch vw ON p.VotedForWitchID = vw.WitchID
                WHERE p.SessionID = @SessionID
                ORDER BY p.ParticipantID";

            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            var list = new List<TrialParticipantModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapToModel(row));
            }

            return list;
        }

        /// <summary>
        /// 查询特定用户的参与记录
        /// </summary>
        public static TrialParticipantModel? GetBySessionAndUser(int sessionID, int userID)
        {
            const string sql = @"
                SELECT 
                    p.*,
                    w.Name AS WitchName,
                    w.AvatarPath,
                    u.Username,
                    vw.Name AS VotedForWitchName
                FROM wt.TrialParticipant p
                INNER JOIN wt.Witch w ON p.WitchID = w.WitchID
                INNER JOIN wt.[User] u ON p.UserID = u.UserID
                LEFT JOIN wt.Witch vw ON p.VotedForWitchID = vw.WitchID
                WHERE p.SessionID = @SessionID AND p.UserID = @UserID";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", sessionID),
                new SqlParameter("@UserID", userID)
            };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 查询特定魔女的参与记录
        /// </summary>
        public static TrialParticipantModel? GetBySessionAndWitch(int sessionID, int witchID)
        {
            const string sql = @"
                SELECT 
                    p.*,
                    w.Name AS WitchName,
                    w.AvatarPath,
                    u.Username,
                    vw.Name AS VotedForWitchName
                FROM wt.TrialParticipant p
                INNER JOIN wt.Witch w ON p.WitchID = w.WitchID
                INNER JOIN wt.[User] u ON p.UserID = u.UserID
                LEFT JOIN wt.Witch vw ON p.VotedForWitchID = vw.WitchID
                WHERE p.SessionID = @SessionID AND p.WitchID = @WitchID";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", sessionID),
                new SqlParameter("@WitchID", witchID)
            };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            if (dt.Rows.Count == 0)
                return null;

            return MapToModel(dt.Rows[0]);
        }

        /// <summary>
        /// 获取已投票人数
        /// </summary>
        public static int GetVotedCount(int sessionID)
        {
            const string sql = "SELECT COUNT(*) FROM wt.TrialParticipant WHERE SessionID = @SessionID AND HasVoted = 1";
            
            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };
            
            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 获取已确认人数
        /// </summary>
        public static int GetConfirmedCount(int sessionID)
        {
            const string sql = "SELECT COUNT(*) FROM wt.TrialParticipant WHERE SessionID = @SessionID AND HasConfirmedExecution = 1";
            
            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };
            
            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 获取投票统计（每个魔女的得票数）
        /// </summary>
        public static Dictionary<int, int> GetVoteStatistics(int sessionID)
        {
            const string sql = @"
                SELECT VotedForWitchID, COUNT(*) AS VoteCount
                FROM wt.TrialParticipant
                WHERE SessionID = @SessionID AND VotedForWitchID IS NOT NULL
                GROUP BY VotedForWitchID";

            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            var statistics = new Dictionary<int, int>();
            foreach (DataRow row in dt.Rows)
            {
                int witchID = Convert.ToInt32(row["VotedForWitchID"]);
                int voteCount = Convert.ToInt32(row["VoteCount"]);
                statistics[witchID] = voteCount;
            }

            return statistics;
        }

        /// <summary>
        /// 将DataRow映射到Model
        /// </summary>
        private static TrialParticipantModel MapToModel(DataRow row)
        {
            return new TrialParticipantModel
            {
                ParticipantID = Convert.ToInt32(row["ParticipantID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                WitchID = Convert.ToInt32(row["WitchID"]),
                UserID = Convert.ToInt32(row["UserID"]),
                HasVoted = Convert.ToBoolean(row["HasVoted"]),
                VotedForWitchID = row["VotedForWitchID"] == DBNull.Value ? null : Convert.ToInt32(row["VotedForWitchID"]),
                VotedAt = row["VotedAt"] == DBNull.Value ? null : Convert.ToDateTime(row["VotedAt"]),
                HasConfirmedExecution = Convert.ToBoolean(row["HasConfirmedExecution"]),
                ExecutionConfirmedAt = row["ExecutionConfirmedAt"] == DBNull.Value ? null : Convert.ToDateTime(row["ExecutionConfirmedAt"]),
                WitchName = row["WitchName"].ToString() ?? "",
                Username = row["Username"].ToString() ?? "",
                AvatarPath = row["AvatarPath"].ToString() ?? "",
                VotedForWitchName = row["VotedForWitchName"] == DBNull.Value ? "" : row["VotedForWitchName"].ToString() ?? ""
            };
        }
    }
}
