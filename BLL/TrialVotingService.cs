using System;
using System.Collections.Generic;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 审判投票业务逻辑层
    /// </summary>
    public class TrialVotingService
    {
        /// <summary>
        /// 提交投票（魔女操作）
        /// </summary>
        public static (bool Success, string Message) SubmitVote(int sessionID, int voterWitchID, int votedForWitchID)
        {
            try
            {
                // 1. 获取审判会话
                var session = TrialSessionService.GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                // 2. 检查审判状态
                if (session.Status != "Voting")
                    return (false, $"当前状态为{session.Status}，无法投票");

                // 3. 获取投票者的参与记录
                var participant = TrialParticipantDAL.GetBySessionAndWitch(sessionID, voterWitchID);
                if (participant == null)
                    return (false, "您不是本次审判的参与者");

                // 4. 检查是否已投票
                if (participant.HasVoted)
                    return (false, "您已经投过票了，不能重复投票");

                // 5. 验证投票对象是否是参与者
                var votedForParticipant = TrialParticipantDAL.GetBySessionAndWitch(sessionID, votedForWitchID);
                if (votedForParticipant == null)
                    return (false, "投票对象不是本次审判的参与者");

                // 6. 更新投票记录
                int result = TrialParticipantDAL.UpdateVote(participant.ParticipantID, votedForWitchID);
                if (result > 0)
                    return (true, "投票成功");
                else
                    return (false, "投票失败，请重试");
            }
            catch (Exception ex)
            {
                return (false, $"投票时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 确认处刑（魔女操作 - 点击处刑按钮）
        /// </summary>
        public static (bool Success, string Message) ConfirmExecution(int sessionID, int witchID)
        {
            try
            {
                // 1. 获取审判会话
                var session = TrialSessionService.GetSessionByID(sessionID);
                if (session == null)
                    return (false, "审判会话不存在");

                // 2. 检查审判状态
                if (session.Status != "Executing")
                    return (false, $"当前状态为{session.Status}，无法确认处刑");

                // 3. 获取参与记录
                var participant = TrialParticipantDAL.GetBySessionAndWitch(sessionID, witchID);
                if (participant == null)
                    return (false, "您不是本次审判的参与者");

                // 4. 检查是否已确认
                if (participant.HasConfirmedExecution)
                    return (false, "您已经确认过处刑了");

                // 5. 更新确认记录
                int result = TrialParticipantDAL.UpdateExecutionConfirmation(participant.ParticipantID);
                if (result > 0)
                    return (true, "确认处刑成功");
                else
                    return (false, "确认处刑失败，请重试");
            }
            catch (Exception ex)
            {
                return (false, $"确认处刑时发生错误：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取参与者列表
        /// </summary>
        public static List<TrialParticipantModel> GetParticipants(int sessionID)
        {
            return TrialParticipantDAL.GetBySession(sessionID);
        }

        /// <summary>
        /// 获取参与者记录
        /// </summary>
        public static TrialParticipantModel? GetParticipant(int sessionID, int userID)
        {
            return TrialParticipantDAL.GetBySessionAndUser(sessionID, userID);
        }

        /// <summary>
        /// 检查是否已投票
        /// </summary>
        public static bool HasVoted(int sessionID, int userID)
        {
            var participant = GetParticipant(sessionID, userID);
            return participant?.HasVoted ?? false;
        }

        /// <summary>
        /// 检查是否已确认处刑
        /// </summary>
        public static bool HasConfirmedExecution(int sessionID, int userID)
        {
            var participant = GetParticipant(sessionID, userID);
            return participant?.HasConfirmedExecution ?? false;
        }

        /// <summary>
        /// 检查是否可以投票
        /// </summary>
        public static bool CanVote(int sessionID, int userID)
        {
            var session = TrialSessionService.GetSessionByID(sessionID);
            if (session == null || session.Status != "Voting")
                return false;

            var participant = GetParticipant(sessionID, userID);
            if (participant == null)
                return false;

            return !participant.HasVoted;
        }

        /// <summary>
        /// 检查是否可以确认处刑
        /// </summary>
        public static bool CanConfirmExecution(int sessionID, int userID)
        {
            var session = TrialSessionService.GetSessionByID(sessionID);
            if (session == null || session.Status != "Executing")
                return false;

            var participant = GetParticipant(sessionID, userID);
            if (participant == null)
                return false;

            return !participant.HasConfirmedExecution;
        }
    }
}
