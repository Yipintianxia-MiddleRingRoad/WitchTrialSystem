using System;
using System.Collections.Generic;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 处刑台移动记录业务逻辑服务
    /// </summary>
    public class MovementLogService
    {
        private readonly MovementLogDAL _logDal = new();

        #region 查询操作

        /// <summary>
        /// 获取岛屿的移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetLogsByIsland(int islandID)
        {
            return _logDal.GetByIsland(islandID);
        }

        /// <summary>
        /// 获取处刑台的移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetLogsByPlatform(int platformID)
        {
            return _logDal.GetByPlatform(platformID);
        }

        /// <summary>
        /// 按时间范围查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetLogsByTimeRange(int islandID, DateTime startTime, DateTime endTime)
        {
            return _logDal.GetByTimeRange(islandID, startTime, endTime);
        }

        /// <summary>
        /// 按位置查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetLogsByPosition(int islandID, int position)
        {
            return _logDal.GetByPosition(islandID, position);
        }

        /// <summary>
        /// 按处刑台编号查询移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetLogsByPlatformNumber(int islandID, int platformNumber)
        {
            return _logDal.GetByPlatformNumber(islandID, platformNumber);
        }

        /// <summary>
        /// 获取最近的移动记录
        /// </summary>
        public List<PlatformMovementLogModel> GetRecentLogs(int islandID, int count = 100)
        {
            return _logDal.GetRecent(islandID, count);
        }

        /// <summary>
        /// 组合筛选查询移动记录
        /// </summary>
        /// <param name="islandID">岛屿ID</param>
        /// <param name="platformNumber">处刑台编号（0表示全部）</param>
        /// <param name="position">位置（0表示全部）</param>
        /// <param name="startTime">开始时间（null表示不限）</param>
        /// <param name="endTime">结束时间（null表示不限）</param>
        /// <returns>筛选后的移动记录列表</returns>
        public List<PlatformMovementLogModel> GetLogsWithFilters(
            int islandID,
            int platformNumber = 0,
            int position = 0,
            DateTime? startTime = null,
            DateTime? endTime = null)
        {
            // 先获取岛屿的所有记录
            var logs = _logDal.GetByIsland(islandID);

            // 应用筛选条件
            var filtered = new List<PlatformMovementLogModel>();
            foreach (var log in logs)
            {
                bool match = true;

                // 处刑台编号筛选
                if (platformNumber > 0 && log.PlatformNumber != platformNumber)
                {
                    match = false;
                }

                // 位置筛选
                if (position > 0 && log.FromPosition != position && log.ToPosition != position)
                {
                    match = false;
                }

                // 时间范围筛选
                if (startTime.HasValue && log.MovementTime < startTime.Value)
                {
                    match = false;
                }

                if (endTime.HasValue && log.MovementTime > endTime.Value)
                {
                    match = false;
                }

                if (match)
                {
                    filtered.Add(log);
                }
            }

            return filtered;
        }

        #endregion

        #region 记录操作

        /// <summary>
        /// 记录移动日志
        /// </summary>
        /// <param name="islandID">岛屿ID</param>
        /// <param name="platformID">处刑台ID</param>
        /// <param name="platformNumber">处刑台编号</param>
        /// <param name="fromPosition">起始位置</param>
        /// <param name="toPosition">目标位置</param>
        /// <param name="toolName">刑具名称</param>
        /// <param name="movementType">移动类型</param>
        /// <param name="customTime">自定义时间（可选）</param>
        public void LogMovement(
            int islandID,
            int platformID,
            int platformNumber,
            int fromPosition,
            int toPosition,
            string? toolName,
            string movementType,
            DateTime? customTime = null)
        {
            var log = new PlatformMovementLogModel
            {
                IslandID = islandID,
                PlatformID = platformID,
                PlatformNumber = platformNumber,
                FromPosition = fromPosition,
                ToPosition = toPosition,
                ToolName = toolName,
                MovementTime = customTime ?? DateTime.Now,
                IsManualTime = customTime.HasValue,
                MovementType = movementType
            };

            _logDal.Insert(log);
        }

        #endregion

        #region 统计分析

        /// <summary>
        /// 获取处刑台使用频率统计
        /// </summary>
        public Dictionary<int, int> GetPlatformUsageStatistics(int islandID)
        {
            var logs = _logDal.GetByIsland(islandID);
            var statistics = new Dictionary<int, int>();

            foreach (var log in logs)
            {
                if (!statistics.ContainsKey(log.PlatformNumber))
                {
                    statistics[log.PlatformNumber] = 0;
                }
                statistics[log.PlatformNumber]++;
            }

            return statistics;
        }

        /// <summary>
        /// 获取指定时间段内的移动次数
        /// </summary>
        public int GetMovementCount(int islandID, DateTime startTime, DateTime endTime)
        {
            var logs = _logDal.GetByTimeRange(islandID, startTime, endTime);
            return logs.Count;
        }

        /// <summary>
        /// 获取最常用的处刑台
        /// </summary>
        public List<(int PlatformNumber, int UsageCount)> GetMostUsedPlatforms(int islandID, int topN = 10)
        {
            var statistics = GetPlatformUsageStatistics(islandID);
            var sorted = new List<(int, int)>();

            foreach (var kvp in statistics)
            {
                sorted.Add((kvp.Key, kvp.Value));
            }

            // 简单排序（冒泡排序）
            for (int i = 0; i < sorted.Count - 1; i++)
            {
                for (int j = 0; j < sorted.Count - i - 1; j++)
                {
                    if (sorted[j].Item2 < sorted[j + 1].Item2)
                    {
                        var temp = sorted[j];
                        sorted[j] = sorted[j + 1];
                        sorted[j + 1] = temp;
                    }
                }
            }

            // 返回前N个
            var result = new List<(int, int)>();
            for (int i = 0; i < Math.Min(topN, sorted.Count); i++)
            {
                result.Add(sorted[i]);
            }

            return result;
        }

        #endregion
    }
}
