using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 智慧可视化大屏业务逻辑层
    /// 处理数据转换、权限控制和业务逻辑
    /// </summary>
    public class DashboardService
    {
        private readonly DashboardDAL _dal = new();

        /// <summary>
        /// 获取全局统计数据
        /// </summary>
        public GlobalStats GetGlobalStats(string username, string role, int? userIslandId = null)
        {
            var dt = _dal.GetGlobalStatistics();
            if (dt.Rows.Count == 0)
                return new GlobalStats();

            var row = dt.Rows[0];
            return new GlobalStats
            {
                TotalWitches = Convert.ToInt32(row["TotalWitches"]),
                TotalIslands = Convert.ToInt32(row["TotalIslands"]),
                ActiveIslands = Convert.ToInt32(row["TotalIslands"]),
                TotalBatches = Convert.ToInt32(row["TotalBatches"]),
                ActiveBatches = Convert.ToInt32(row["ActiveBatches"])
            };
        }

        /// <summary>
        /// 获取全局状态分布（根据权限过滤）
        /// </summary>
        public List<StatusCount> GetStatusDistribution(string username, string role, int? userIslandId = null)
        {
            DataTable dt;
            if (role == "Admin")
            {
                dt = _dal.GetStatusCounts();
            }
            else if (role == "Meruru" && userIslandId.HasValue)
            {
                dt = _dal.GetIslandStatusCounts(userIslandId.Value);
            }
            else
            {
                return new List<StatusCount>();
            }

            return ConvertToStatusCounts(dt);
        }


        /// <summary>
        /// 获取指定岛屿的状态分布
        /// </summary>
        public List<StatusCount> GetIslandStatusDistribution(int islandId)
        {
            var dt = _dal.GetIslandStatusCounts(islandId);
            return ConvertToStatusCounts(dt);
        }

        /// <summary>
        /// 获取所有岛屿信息（根据权限过滤）
        /// </summary>
        public List<IslandInfo> GetIslands(string username, string role, int? userIslandId = null)
        {
            var dt = _dal.GetIslands();
            var islands = new List<IslandInfo>();

            foreach (DataRow row in dt.Rows)
            {
                int islandId = Convert.ToInt32(row["IslandID"]);
                
                // Meruru只能看自己的岛屿
                if (role == "Meruru" && userIslandId.HasValue && islandId != userIslandId.Value)
                    continue;

                var island = new IslandInfo
                {
                    IslandId = islandId,
                    Name = row["Name"]?.ToString() ?? "",
                    WitchCount = Convert.ToInt32(row["WitchCount"]),
                    StatusDistribution = GetIslandStatusDistribution(islandId),
                    BatchCapacities = GetBatchCapacityData(islandId)
                };
                islands.Add(island);
            }

            return islands;
        }

        /// <summary>
        /// 获取指定岛屿的批次容量数据
        /// </summary>
        public List<BatchCapacity> GetBatchCapacityData(int islandId)
        {
            var dt = _dal.GetBatchCapacity(islandId);
            var batches = new List<BatchCapacity>();

            foreach (DataRow row in dt.Rows)
            {
                batches.Add(new BatchCapacity
                {
                    IslandId = Convert.ToInt32(row["IslandID"]),
                    LocalBatchId = Convert.ToInt32(row["LocalBatchID"]),
                    BatchId = Convert.ToInt32(row["BatchID"]),
                    CurrentCount = Convert.ToInt32(row["CurrentCount"]),
                    MaxCapacity = Convert.ToInt32(row["MaxCapacity"])
                });
            }

            return batches;
        }

        /// <summary>
        /// 获取所有批次容量数据（根据权限过滤）
        /// </summary>
        public List<BatchCapacity> GetAllBatchCapacityData(string username, string role, int? userIslandId = null)
        {
            var dt = _dal.GetAllBatchCapacity();
            var batches = new List<BatchCapacity>();

            foreach (DataRow row in dt.Rows)
            {
                int islandId = Convert.ToInt32(row["IslandID"]);
                
                // Meruru只能看自己的岛屿
                if (role == "Meruru" && userIslandId.HasValue && islandId != userIslandId.Value)
                    continue;

                batches.Add(new BatchCapacity
                {
                    IslandId = islandId,
                    LocalBatchId = Convert.ToInt32(row["LocalBatchID"]),
                    BatchId = Convert.ToInt32(row["BatchID"]),
                    CurrentCount = Convert.ToInt32(row["CurrentCount"]),
                    MaxCapacity = Convert.ToInt32(row["MaxCapacity"])
                });
            }

            return batches;
        }

        /// <summary>
        /// 获取批次状态矩阵（根据权限过滤）
        /// </summary>
        public Dictionary<int, Dictionary<string, int>> GetBatchStatusMatrix(string username, string role, int? userIslandId = null)
        {
            DataTable dt;
            if (role == "Admin")
            {
                dt = _dal.GetBatchStatusMatrix();
            }
            else if (role == "Meruru" && userIslandId.HasValue)
            {
                dt = _dal.GetBatchStatusMatrix(userIslandId.Value);
            }
            else
            {
                return new Dictionary<int, Dictionary<string, int>>();
            }

            return ConvertToBatchStatusMatrix(dt);
        }

        /// <summary>
        /// 获取批次状态矩阵单元格列表
        /// </summary>
        public List<BatchStatusCell> GetBatchStatusCells(string username, string role, int? userIslandId = null)
        {
            DataTable dt;
            if (role == "Admin")
            {
                dt = _dal.GetBatchStatusMatrix();
            }
            else if (role == "Meruru" && userIslandId.HasValue)
            {
                dt = _dal.GetBatchStatusMatrix(userIslandId.Value);
            }
            else
            {
                return new List<BatchStatusCell>();
            }

            var cells = new List<BatchStatusCell>();
            foreach (DataRow row in dt.Rows)
            {
                cells.Add(new BatchStatusCell
                {
                    IslandId = dt.Columns.Contains("IslandID") ? Convert.ToInt32(row["IslandID"]) : (userIslandId ?? 0),
                    LocalBatchId = Convert.ToInt32(row["LocalBatchID"]),
                    Status = row["Status"]?.ToString() ?? "无",
                    Count = Convert.ToInt32(row["Count"])
                });
            }

            return cells;
        }

        /// <summary>
        /// 获取指定状态的魔女列表
        /// </summary>
        public DataTable GetWitchesByStatus(string status, string role, int? userIslandId = null)
        {
            if (role == "Admin")
            {
                return _dal.GetWitchesByStatus(status);
            }
            else if (role == "Meruru" && userIslandId.HasValue)
            {
                return _dal.GetWitchesByStatus(status, userIslandId.Value);
            }
            return new DataTable();
        }

        /// <summary>
        /// 获取指定批次的魔女列表
        /// </summary>
        public DataTable GetWitchesByBatch(int islandId, int localBatchId)
        {
            return _dal.GetWitchesByBatch(islandId, localBatchId);
        }

        /// <summary>
        /// 获取指定批次和状态的魔女列表
        /// </summary>
        public DataTable GetWitchesByBatchAndStatus(int islandId, int localBatchId, string status)
        {
            return _dal.GetWitchesByBatchAndStatus(islandId, localBatchId, status);
        }

        #region 私有辅助方法

        private List<StatusCount> ConvertToStatusCounts(DataTable dt)
        {
            var result = new List<StatusCount>();
            foreach (DataRow row in dt.Rows)
            {
                string status = row["Status"]?.ToString() ?? "未知";
                result.Add(new StatusCount
                {
                    Status = status,
                    Count = Convert.ToInt32(row["Count"]),
                    Percentage = row["Percentage"] != DBNull.Value ? Convert.ToDouble(row["Percentage"]) : 0,
                    Color = DashboardColors.GetStatusColor(status)
                });
            }
            return result;
        }

        private Dictionary<int, Dictionary<string, int>> ConvertToBatchStatusMatrix(DataTable dt)
        {
            var matrix = new Dictionary<int, Dictionary<string, int>>();
            
            foreach (DataRow row in dt.Rows)
            {
                int localBatchId = Convert.ToInt32(row["LocalBatchID"]);
                string status = row["Status"]?.ToString() ?? "无";
                int count = Convert.ToInt32(row["Count"]);

                if (!matrix.ContainsKey(localBatchId))
                {
                    matrix[localBatchId] = new Dictionary<string, int>();
                }
                matrix[localBatchId][status] = count;
            }

            return matrix;
        }

        #endregion
    }
}
