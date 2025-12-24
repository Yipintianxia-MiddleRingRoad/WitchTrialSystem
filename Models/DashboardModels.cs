using System;
using System.Drawing;
using System.Collections.Generic;

namespace WitchTrialSystem.Models
{
    /// <summary>
    /// 全局统计数据模型
    /// </summary>
    public class GlobalStats
    {
        public int TotalWitches { get; set; }
        public int TotalIslands { get; set; }
        public int ActiveIslands { get; set; }
        public int TotalBatches { get; set; }
        public int ActiveBatches { get; set; }
    }

    /// <summary>
    /// 状态统计数据模型
    /// </summary>
    public class StatusCount
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
        public Color Color { get; set; }
    }

    /// <summary>
    /// 岛屿信息模型
    /// </summary>
    public class IslandInfo
    {
        public int IslandId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int WitchCount { get; set; }
        public List<StatusCount> StatusDistribution { get; set; } = new();
        public List<BatchCapacity> BatchCapacities { get; set; } = new();
    }

    /// <summary>
    /// 批次容量数据模型
    /// </summary>
    public class BatchCapacity
    {
        public int IslandId { get; set; }
        public int LocalBatchId { get; set; }
        public int BatchId { get; set; }
        public int CurrentCount { get; set; }
        public int MaxCapacity { get; set; } = 13;
        public double UsageRate => MaxCapacity > 0 ? (double)CurrentCount / MaxCapacity * 100 : 0;
        public Color BarColor => GetBarColor();

        private Color GetBarColor()
        {
            if (CurrentCount >= MaxCapacity)
                return Color.FromArgb(220, 53, 69);   // 红色 - 已满
            else if (UsageRate >= 80)
                return Color.FromArgb(255, 193, 7);   // 橙色 - 接近满
            else
                return Color.FromArgb(25, 135, 84);   // 绿色 - 正常
        }

        public string DisplayText => $"{CurrentCount}/{MaxCapacity}";
        public bool IsFull => CurrentCount >= MaxCapacity;
    }

    /// <summary>
    /// 批次状态矩阵单元格数据模型
    /// </summary>
    public class BatchStatusCell
    {
        public int IslandId { get; set; }
        public int LocalBatchId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public Color CellColor => GetCellColor();

        private Color GetCellColor()
        {
            if (Count >= 7)
                return Color.FromArgb(220, 53, 69);   // 红色 - 较多
            else if (Count >= 4)
                return Color.FromArgb(255, 193, 7);   // 黄色 - 中等
            else
                return Color.FromArgb(25, 135, 84);   // 绿色 - 少量
        }
    }

    /// <summary>
    /// 趋势数据点模型
    /// </summary>
    public class TrendPoint
    {
        public DateTime Date { get; set; }
        public int TotalCount { get; set; }
        public int TrialCount { get; set; }
        public int ExecutedCount { get; set; }
    }

    /// <summary>
    /// 大屏配色方案
    /// </summary>
    public static class DashboardColors
    {
        // 背景色
        public static readonly Color Background = Color.FromArgb(26, 26, 46);       // #1a1a2e
        public static readonly Color CardBackground = Color.FromArgb(40, 40, 60);   // #28283c
        public static readonly Color PanelBackground = Color.FromArgb(30, 30, 50);  // #1e1e32

        // 主题色
        public static readonly Color Primary = Color.FromArgb(157, 78, 221);        // #9d4edd
        public static readonly Color Secondary = Color.FromArgb(255, 0, 110);       // #ff006e
        public static readonly Color Accent = Color.FromArgb(0, 245, 255);          // #00f5ff

        // 文字色
        public static readonly Color TextPrimary = Color.White;
        public static readonly Color TextSecondary = Color.FromArgb(180, 180, 200);
        public static readonly Color TextMuted = Color.FromArgb(120, 120, 140);

        // 状态色
        public static readonly Color StatusPending = Color.FromArgb(108, 117, 125);     // 待分配 #6c757d
        public static readonly Color StatusAssigned = Color.FromArgb(13, 110, 253);     // 分配至岛屿 #0d6efd
        public static readonly Color StatusTrial = Color.FromArgb(253, 126, 20);        // 审判中 #fd7e14
        public static readonly Color StatusDeathNormal = Color.FromArgb(220, 53, 69);   // 死亡（正常） #dc3545
        public static readonly Color StatusDeathWitch = Color.FromArgb(139, 0, 0);      // 死亡（魔女化） #8b0000
        public static readonly Color StatusOther = Color.FromArgb(157, 78, 221);        // 其它 #9d4edd

        // 容量色
        public static readonly Color CapacityLow = Color.FromArgb(25, 135, 84);         // 绿色 <80%
        public static readonly Color CapacityMedium = Color.FromArgb(255, 193, 7);      // 橙色 80-99%
        public static readonly Color CapacityFull = Color.FromArgb(220, 53, 69);        // 红色 100%

        // 热力图色
        public static readonly Color HeatLow = Color.FromArgb(25, 135, 84);             // 0-3人
        public static readonly Color HeatMedium = Color.FromArgb(255, 193, 7);          // 4-6人
        public static readonly Color HeatHigh = Color.FromArgb(220, 53, 69);            // 7+人

        /// <summary>
        /// 根据状态获取对应颜色
        /// </summary>
        public static Color GetStatusColor(string status)
        {
            return status switch
            {
                "待分配" => StatusPending,
                "分配至岛屿" => StatusAssigned,
                "审判中" => StatusTrial,
                "死亡（正常）" => StatusDeathNormal,
                "死亡（魔女化）" => StatusDeathWitch,
                _ => StatusOther
            };
        }

        /// <summary>
        /// 根据容量使用率获取对应颜色
        /// </summary>
        public static Color GetCapacityColor(double usageRate)
        {
            if (usageRate >= 100)
                return CapacityFull;
            else if (usageRate >= 80)
                return CapacityMedium;
            else
                return CapacityLow;
        }

        /// <summary>
        /// 根据人数获取热力图颜色
        /// </summary>
        public static Color GetHeatmapColor(int count)
        {
            if (count >= 7)
                return HeatHigh;
            else if (count >= 4)
                return HeatMedium;
            else
                return HeatLow;
        }
    }
}
