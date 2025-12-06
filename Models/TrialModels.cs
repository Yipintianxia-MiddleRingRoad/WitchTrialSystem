using System;
using System.Collections.Generic;

namespace WitchTrialSystem.Models
{
    /// <summary>
    /// 审判会话模型
    /// </summary>
    public class TrialSessionModel
    {
        public int SessionID { get; set; }
        public int IslandID { get; set; }
        public int BatchID { get; set; }
        public string Status { get; set; } = "";
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? VotingStartTime { get; set; }
        public DateTime? VotingEndTime { get; set; }
        public int? ExecutionTargetWitchID { get; set; }
        public DateTime? ExecutionConfirmedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        // 计算属性
        public bool IsPending => Status == "Pending";
        public bool IsVoting => Status == "Voting";
        public bool IsConfirmed => Status == "Confirmed";
        public bool IsExecuting => Status == "Executing";
        public bool IsCompleted => Status == "Completed";
        public bool IsActive => Status != "Completed" && Status != "Cancelled";
    }

    /// <summary>
    /// 审判参与者模型
    /// </summary>
    public class TrialParticipantModel
    {
        public int ParticipantID { get; set; }
        public int SessionID { get; set; }
        public int WitchID { get; set; }
        public int UserID { get; set; }
        public bool HasVoted { get; set; }
        public int? VotedForWitchID { get; set; }
        public DateTime? VotedAt { get; set; }
        public bool HasConfirmedExecution { get; set; }
        public DateTime? ExecutionConfirmedAt { get; set; }
        
        // 扩展属性（从其他表JOIN获取）
        public string WitchName { get; set; } = "";
        public string Username { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public string VotedForWitchName { get; set; } = "";
    }

    /// <summary>
    /// 审判通知模型
    /// </summary>
    public class TrialNotificationModel
    {
        public int NotificationID { get; set; }
        public int SessionID { get; set; }
        public int UserID { get; set; }
        public string Message { get; set; } = "";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// 审判状态枚举
    /// </summary>
    public enum TrialState
    {
        /// <summary>
        /// 无审判
        /// </summary>
        Idle = 0,
        
        /// <summary>
        /// 不参与此审判
        /// </summary>
        NotParticipating = 1,
        
        /// <summary>
        /// 等待投票开始（Pending状态）
        /// </summary>
        WaitingToStart = 2,
        
        /// <summary>
        /// 投票中（未投票）
        /// </summary>
        Voting = 3,
        
        /// <summary>
        /// 等待其他人投票（已投票）
        /// </summary>
        WaitingForOthersToVote = 4,
        
        /// <summary>
        /// 等待宣布处刑对象（Confirmed状态）
        /// </summary>
        WaitingForExecutionAnnouncement = 5,
        
        /// <summary>
        /// 确认处刑中（未确认）
        /// </summary>
        ConfirmingExecution = 6,
        
        /// <summary>
        /// 等待其他人确认（已确认）
        /// </summary>
        WaitingForOthersToConfirm = 7,
        
        /// <summary>
        /// 审判完成
        /// </summary>
        Completed = 8
    }

    /// <summary>
    /// 投票统计模型
    /// </summary>
    public class VotingStatisticsModel
    {
        public int WitchID { get; set; }
        public string WitchName { get; set; } = "";
        public string AvatarPath { get; set; } = "";
        public int VoteCount { get; set; }
        public List<string> VoterNames { get; set; } = new List<string>();
    }
}
