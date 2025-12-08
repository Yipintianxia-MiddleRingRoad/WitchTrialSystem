using System;
using System.Collections.Generic;
using System.Linq;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 审判会话业务逻辑层
    /// </summary>
    public class TrialSessionService
    {
        /// <summary>
        /// 获取进行中的审判会话
        /// </summary>
        public static TrialSessionModel? GetActiveSession(int islandID)
        {
            return TrialSessionDAL.GetActiveByIsland(islandID);
        }

        /// <summary>
        /// 按ID获取审判会话
        /// </summary>
        public static TrialSessionModel? GetSessionByID(int sessionID)
        {
            return TrialSessionDAL.GetByID(sessionID);
        }

        /// <summary>
        /// 获取历史审判会话
        /// </summary>
        public static List<TrialSessionModel> GetSessionHistory(int islandID, int limit = 10)
        {
            return TrialSessionDAL.GetByIsland(islandID, limit);
        }

        /// <summary>
        /// 检查是否有进行中的审判
        /// </summary>
        public static bool HasActiveSession(int islandID)
        {
            var session = GetActiveSession(islandID);
            return session != null;
        }

        /// <summary>
        /// 创建审判会话（典狱长发起审判）
        /// </summary>
        public static (bool Success, string Message, int SessionID) CreateSession(
            int islandID, 
            int batchID, 
            int createdBy, 
            List<int> participantWitchIDs)
        {
            try
            {
                // 1. 验证参与人数（2-13人）
                if (participantWitchIDs.Count < 2)
                    return (false, "参与人数不足，至少需要2人", 0);
                
                if (participantWitchIDs.Count > 13)
                    return (false, "参与人数过多，最多13人", 0);

                // 2. 检查是否已有进行中的审判
                if (HasActiveSession(islandID))
                    return (false, "当前岛屿已有进行中的审判，请等待完成后再发起新审判", 0);

                // 3. 创建审判会话
                var session = new TrialSessionModel
                {
                    IslandID = islandID,
                    BatchID = batchID,
                    Status = "Pending",
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.Now
                };

                int sessionID = TrialSessionDAL.Insert(session);
                if (sessionID <= 0)
                    return (false, "创建审判会话失败", 0);

                // 4. 创建参与者记录
                var participants = new List<TrialParticipantModel>();
                foreach (var witchID in participantWitchIDs)
                {
                    // 获取魔女的UserID
                    int userID = GetUserIDByWitchID(witchID);
                    if (userID <= 0)
                        continue;

                    participants.Add(new TrialParticipantModel
                    {
                        SessionID = sessionID,
                        WitchID = witchID,
                        UserID = userID,
                        HasVoted = false,
                        HasConfirmedExecution = false
                    });
                }

                int participantCount = TrialParticipantDAL.InsertBatch(participants);
                if (participantCount != participantWitchIDs.Count)
                    return (false, $"创建参与者记录失败，预期{participantWitchIDs.Count}人，实际{participantCount}人", sessionID);

                // 5. 创建通知记录
                var notifications = new List<TrialNotificationModel>();
                foreach (var participant in participants)
                {
                    notifications.Add(new TrialNotificationModel
                    {
                        SessionID = sessionID,
                        UserID = participant.UserID,
                        Message = "呀咧呀咧，又死人了，真是的，请速速前往审判庭",
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }

                TrialNotificationDAL.InsertBatch(notifications);

                return (true, $"审判会话创建成功，共{participantCount}人参与", sessionID);
            }
            catch (Exception ex)
            {
                return (false, $"创建审判会话时发生错误：{ex.Message}", 0);
            }
        }

        /// <summary>
        /// 开始投票（典狱长操作）
        /// </summary>
        public static (bool Success, string Message) StartVoting(int sessionID, int wardenUserID)
        {
            try
            {
                var session = GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                if (session.Status != "Pending")
                    return (false, $"当前状态为{session.Status}，无法开始投票");

                // 更新状态和投票开始时间
                session.Status = "Voting";
                session.VotingStartTime = DateTime.Now;
                
                int result = TrialSessionDAL.Update(session);
                if (result > 0)
                    return (true, "投票已开始");
                else
                    return (false, "更新状态失败");
            }
            catch (Exception ex)
            {
                return (false, $"开始投票时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 确认处刑对象（典狱长操作）
        /// </summary>
        public static (bool Success, string Message) ConfirmExecutionTarget(int sessionID, int targetWitchID, int wardenUserID)
        {
            try
            {
                var session = GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                if (session.Status != "Voting")
                    return (false, $"当前状态为{session.Status}，无法确认处刑对象");

                // 检查是否所有人已投票
                var (voted, total) = GetVotingProgress(sessionID);
                if (voted < total)
                    return (false, $"还有{total - voted}人未投票，无法确认处刑对象");

                // 更新状态和处刑对象
                session.Status = "Confirmed";
                session.ExecutionTargetWitchID = targetWitchID;
                session.VotingEndTime = DateTime.Now;
                
                int result = TrialSessionDAL.Update(session);
                if (result > 0)
                    return (true, "处刑对象已确认");
                else
                    return (false, "更新状态失败");
            }
            catch (Exception ex)
            {
                return (false, $"确认处刑对象时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 宣布处刑对象（典狱长操作）
        /// </summary>
        public static (bool Success, string Message) AnnounceExecutionTarget(int sessionID, int wardenUserID)
        {
            try
            {
                var session = GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                if (session.Status != "Confirmed")
                    return (false, $"当前状态为{session.Status}，无法宣布处刑对象");

                // 更新状态
                session.Status = "Executing";
                session.ExecutionConfirmedAt = DateTime.Now;
                
                int result = TrialSessionDAL.Update(session);
                if (result > 0)
                    return (true, "处刑对象已宣布");
                else
                    return (false, "更新状态失败");
            }
            catch (Exception ex)
            {
                return (false, $"宣布处刑对象时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 完成处刑（典狱长操作）
        /// </summary>
        public static (bool Success, string Message) CompleteExecution(int sessionID, int wardenUserID)
        {
            try
            {
                var session = GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                if (session.Status != "Executing")
                    return (false, $"当前状态为{session.Status}，无法完成处刑");

                // 检查是否所有人已确认处刑
                var (confirmed, total) = GetConfirmationProgress(sessionID);
                if (confirmed < total)
                    return (false, $"还有{total - confirmed}人未确认处刑，无法完成");

                // 更新审判状态
                session.Status = "Completed";
                session.CompletedAt = DateTime.Now;
                
                int result = TrialSessionDAL.Update(session);
                if (result <= 0)
                    return (false, "更新审判状态失败");

                // 更新魔女状态为"已处刑"
                if (session.ExecutionTargetWitchID.HasValue)
                {
                    UpdateWitchStatus(session.ExecutionTargetWitchID.Value);
                }

                return (true, "处刑已完成");
            }
            catch (Exception ex)
            {
                return (false, $"完成处刑时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 取消审判（典狱长操作）
        /// </summary>
        public static (bool Success, string Message) CancelSession(int sessionID, int wardenUserID)
        {
            try
            {
                var session = GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                if (session.Status == "Completed")
                    return (false, "审判已完成，无法取消");

                session.Status = "Cancelled";
                session.CompletedAt = DateTime.Now;
                
                int result = TrialSessionDAL.Update(session);
                if (result > 0)
                    return (true, "审判已取消");
                else
                    return (false, "取消审判失败");
            }
            catch (Exception ex)
            {
                return (false, $"取消审判时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前状态（魔女端）
        /// </summary>
        public static TrialState GetCurrentState(int userId, int islandID)
        {
            try
            {
                // 1. 查询是否有进行中的审判
                var session = GetActiveSession(islandID);
                if (session == null)
                    return TrialState.Idle;

                // 2. 检查用户是否参与此审判
                var participant = TrialParticipantDAL.GetBySessionAndUser(session.SessionID, userId);
                if (participant == null)
                    return TrialState.NotParticipating;

                // 3. 根据会话状态和参与者状态返回
                return session.Status switch
                {
                    "Pending" => TrialState.WaitingToStart,
                    "Voting" => participant.HasVoted 
                        ? TrialState.WaitingForOthersToVote 
                        : TrialState.Voting,
                    "Confirmed" => TrialState.WaitingForExecutionAnnouncement,
                    "Executing" => participant.HasConfirmedExecution 
                        ? TrialState.WaitingForOthersToConfirm 
                        : TrialState.ConfirmingExecution,
                    "Completed" => TrialState.Completed,
                    _ => TrialState.Idle
                };
            }
            catch
            {
                return TrialState.Idle;
            }
        }

        /// <summary>
        /// 获取投票统计
        /// </summary>
        public static Dictionary<int, int> GetVotingStatistics(int sessionID)
        {
            return TrialParticipantDAL.GetVoteStatistics(sessionID);
        }

        /// <summary>
        /// 获取投票详情
        /// </summary>
        public static List<TrialParticipantModel> GetVotingDetails(int sessionID)
        {
            return TrialParticipantDAL.GetBySession(sessionID);
        }

        /// <summary>
        /// 获取投票进度
        /// </summary>
        public static (int Voted, int Total) GetVotingProgress(int sessionID)
        {
            var participants = TrialParticipantDAL.GetBySession(sessionID);
            int voted = TrialParticipantDAL.GetVotedCount(sessionID);
            return (voted, participants.Count);
        }

        /// <summary>
        /// 获取确认进度
        /// </summary>
        public static (int Confirmed, int Total) GetConfirmationProgress(int sessionID)
        {
            var participants = TrialParticipantDAL.GetBySession(sessionID);
            int confirmed = TrialParticipantDAL.GetConfirmedCount(sessionID);
            return (confirmed, participants.Count);
        }

        /// <summary>
        /// 根据WitchID获取UserID
        /// </summary>
        private static int GetUserIDByWitchID(int witchID)
        {
            const string sql = "SELECT UserID FROM wt.UserWitch WHERE WitchID = @WitchID";
            var parameters = new[] { new Microsoft.Data.SqlClient.SqlParameter("@WitchID", witchID) };
            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 更新魔女状态为"已处刑"
        /// </summary>
        private static void UpdateWitchStatus(int witchID)
        {
            const string sql = @"
                UPDATE wt.Witch 
                SET Status = N'已处刑', 
                    ExecutionResult = N'投票处刑' 
                WHERE WitchID = @WitchID";
            
            var parameters = new[] { new Microsoft.Data.SqlClient.SqlParameter("@WitchID", witchID) };
            DBHelper.ExecNonQuery(sql, parameters);
        }
    }
}
