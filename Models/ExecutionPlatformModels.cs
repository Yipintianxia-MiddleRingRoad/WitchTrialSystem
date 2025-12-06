using System;

namespace WitchTrialSystem.Models
{
    /// <summary>
    /// 处刑台数据模型
    /// </summary>
    public class ExecutionPlatformModel
    {
        public int PlatformID { get; set; }
        public int IslandID { get; set; }
        public int PlatformNumber { get; set; }
        public int HomePosition { get; set; }
        public int CurrentPosition { get; set; }
        public string? ToolName { get; set; }
        public string? ToolType { get; set; }
        public string? ToolDescription { get; set; }
        public string Status { get; set; } = "空闲";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 计算属性
        /// <summary>
        /// 是否在审判庭
        /// </summary>
        public bool IsInTrialHall => CurrentPosition == 50;

        /// <summary>
        /// 是否在原位
        /// </summary>
        public bool IsAtHome => CurrentPosition == HomePosition;

        /// <summary>
        /// 是否有刑具
        /// </summary>
        public bool HasTool => !string.IsNullOrEmpty(ToolName);

        /// <summary>
        /// 位置描述
        /// </summary>
        public string LocationDescription => CurrentPosition == 50 ? "审判庭" : $"地下室-{CurrentPosition}号位";

        /// <summary>
        /// 完整描述
        /// </summary>
        public string FullDescription
        {
            get
            {
                var desc = $"{PlatformNumber}号处刑台 - {LocationDescription}";
                if (HasTool)
                {
                    desc += $" - 刑具：{ToolName}";
                }
                return desc;
            }
        }
    }

    /// <summary>
    /// 处刑台移动记录数据模型
    /// </summary>
    public class PlatformMovementLogModel
    {
        public int LogID { get; set; }
        public int IslandID { get; set; }
        public int PlatformID { get; set; }
        public int PlatformNumber { get; set; }
        public int FromPosition { get; set; }
        public int ToPosition { get; set; }
        public string? ToolName { get; set; }
        public DateTime MovementTime { get; set; }
        public bool IsManualTime { get; set; }
        public string MovementType { get; set; } = "";

        // 计算属性
        /// <summary>
        /// 起始位置描述
        /// </summary>
        public string FromLocationDescription => FromPosition == 50 ? "审判庭" : $"地下室-{FromPosition}号位";

        /// <summary>
        /// 目标位置描述
        /// </summary>
        public string ToLocationDescription => ToPosition == 50 ? "审判庭" : $"地下室-{ToPosition}号位";

        /// <summary>
        /// 移动描述
        /// </summary>
        public string MovementDescription => $"{PlatformNumber}号处刑台从{FromLocationDescription}移动到{ToLocationDescription}";

        /// <summary>
        /// 时间来源描述
        /// </summary>
        public string TimeSourceDescription => IsManualTime ? "手动输入" : "系统记录";

        /// <summary>
        /// 完整描述
        /// </summary>
        public string FullDescription
        {
            get
            {
                var desc = $"{MovementTime:yyyy-MM-dd HH:mm:ss} - {MovementDescription}";
                if (!string.IsNullOrEmpty(ToolName))
                {
                    desc += $" - 刑具：{ToolName}";
                }
                desc += $" ({TimeSourceDescription})";
                return desc;
            }
        }
    }
}
