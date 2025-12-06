using System;
using System.Collections.Generic;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 处刑台业务逻辑服务
    /// </summary>
    public class ExecutionPlatformService
    {
        private readonly ExecutionPlatformDAL _platformDal = new();
        private readonly MovementLogDAL _logDal = new();

        #region 查询操作

        /// <summary>
        /// 获取岛屿的所有处刑台
        /// </summary>
        public List<ExecutionPlatformModel> GetPlatformsByIsland(int islandID)
        {
            return _platformDal.GetByIsland(islandID);
        }

        /// <summary>
        /// 获取单个处刑台
        /// </summary>
        public ExecutionPlatformModel? GetPlatformByID(int platformID)
        {
            return _platformDal.GetByID(platformID);
        }

        /// <summary>
        /// 获取指定位置的处刑台
        /// </summary>
        public ExecutionPlatformModel? GetPlatformAtPosition(int islandID, int position)
        {
            return _platformDal.GetByPosition(islandID, position);
        }

        /// <summary>
        /// 检查位置是否被占用
        /// </summary>
        public bool IsPositionOccupied(int islandID, int position)
        {
            return _platformDal.IsPositionOccupied(islandID, position);
        }

        /// <summary>
        /// 检查审判庭是否被占用
        /// </summary>
        public bool IsTrialHallOccupied(int islandID)
        {
            return IsPositionOccupied(islandID, 50);
        }

        #endregion

        #region 移动操作

        /// <summary>
        /// 移动处刑台到审判庭
        /// </summary>
        /// <param name="platformID">处刑台ID</param>
        /// <param name="userIslandID">用户所属岛屿ID</param>
        /// <param name="customTime">自定义时间（可选）</param>
        /// <returns>成功返回(true, 成功消息)，失败返回(false, 错误消息)</returns>
        public (bool Success, string Message) MoveToTrialHall(int platformID, int userIslandID, DateTime? customTime = null)
        {
            try
            {
                // 1. 获取处刑台信息
                var platform = _platformDal.GetByID(platformID);
                if (platform == null)
                {
                    return (false, "处刑台不存在");
                }

                // 2. 验证岛屿权限
                if (platform.IslandID != userIslandID)
                {
                    return (false, "您只能操作本岛屿的处刑台");
                }

                // 3. 检查处刑台当前位置
                if (platform.CurrentPosition == 50)
                {
                    return (false, "该处刑台已经在审判庭");
                }

                // 4. 检查审判庭是否为空
                if (IsTrialHallOccupied(platform.IslandID))
                {
                    return (false, "审判庭已被占用，请先将其他处刑台返回原位");
                }

                // 5. 记录移动前的位置
                int fromPosition = platform.CurrentPosition;

                // 6. 更新处刑台位置
                platform.CurrentPosition = 50;
                platform.Status = "使用中";
                platform.UpdatedAt = DateTime.Now;
                _platformDal.Update(platform);

                // 7. 记录移动日志
                var movementTime = customTime ?? DateTime.Now;
                var log = new PlatformMovementLogModel
                {
                    IslandID = platform.IslandID,
                    PlatformID = platform.PlatformID,
                    PlatformNumber = platform.PlatformNumber,
                    FromPosition = fromPosition,
                    ToPosition = 50,
                    ToolName = platform.ToolName,
                    MovementTime = movementTime,
                    IsManualTime = customTime.HasValue,
                    MovementType = "升起"
                };
                _logDal.Insert(log);

                return (true, $"{platform.PlatformNumber}号处刑台已成功移动到审判庭");
            }
            catch (Exception ex)
            {
                return (false, $"移动失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 处刑台返回原位
        /// </summary>
        /// <param name="platformID">处刑台ID</param>
        /// <param name="userIslandID">用户所属岛屿ID</param>
        /// <param name="customTime">自定义时间（可选）</param>
        /// <returns>成功返回(true, 成功消息)，失败返回(false, 错误消息)</returns>
        public (bool Success, string Message) ReturnToHome(int platformID, int userIslandID, DateTime? customTime = null)
        {
            try
            {
                // 1. 获取处刑台信息
                var platform = _platformDal.GetByID(platformID);
                if (platform == null)
                {
                    return (false, "处刑台不存在");
                }

                // 2. 验证岛屿权限
                if (platform.IslandID != userIslandID)
                {
                    return (false, "您只能操作本岛屿的处刑台");
                }

                // 3. 检查处刑台当前位置
                if (platform.CurrentPosition != 50)
                {
                    return (false, "该处刑台不在审判庭，无需返回");
                }

                // 4. 检查原位是否为空
                if (IsPositionOccupied(platform.IslandID, platform.HomePosition))
                {
                    return (false, $"原位（{platform.HomePosition}号位）已被占用");
                }

                // 5. 记录移动前的位置
                int fromPosition = platform.CurrentPosition;

                // 6. 更新处刑台位置
                platform.CurrentPosition = platform.HomePosition;
                platform.Status = "空闲";
                platform.UpdatedAt = DateTime.Now;
                _platformDal.Update(platform);

                // 7. 记录移动日志
                var movementTime = customTime ?? DateTime.Now;
                var log = new PlatformMovementLogModel
                {
                    IslandID = platform.IslandID,
                    PlatformID = platform.PlatformID,
                    PlatformNumber = platform.PlatformNumber,
                    FromPosition = fromPosition,
                    ToPosition = platform.HomePosition,
                    ToolName = platform.ToolName,
                    MovementTime = movementTime,
                    IsManualTime = customTime.HasValue,
                    MovementType = "返回"
                };
                _logDal.Insert(log);

                return (true, $"{platform.PlatformNumber}号处刑台已成功返回原位");
            }
            catch (Exception ex)
            {
                return (false, $"返回失败：{ex.Message}");
            }
        }

        #endregion

        #region 刑具管理

        /// <summary>
        /// 添加刑具
        /// </summary>
        /// <param name="platformID">处刑台ID</param>
        /// <param name="toolName">刑具名称</param>
        /// <param name="toolType">刑具类型</param>
        /// <param name="description">刑具描述</param>
        /// <param name="userIslandID">用户所属岛屿ID</param>
        /// <returns>成功返回(true, 成功消息)，失败返回(false, 错误消息)</returns>
        public (bool Success, string Message) AddTool(int platformID, string toolName, string toolType, string? description, int userIslandID)
        {
            try
            {
                // 1. 验证输入
                if (string.IsNullOrWhiteSpace(toolName))
                {
                    return (false, "刑具名称不能为空");
                }

                if (string.IsNullOrWhiteSpace(toolType))
                {
                    return (false, "刑具类型不能为空");
                }

                // 2. 获取处刑台信息
                var platform = _platformDal.GetByID(platformID);
                if (platform == null)
                {
                    return (false, "处刑台不存在");
                }

                // 3. 验证岛屿权限
                if (platform.IslandID != userIslandID)
                {
                    return (false, "您只能操作本岛屿的处刑台");
                }

                // 4. 检查是否已有刑具
                if (platform.HasTool)
                {
                    return (false, $"该处刑台已有刑具（{platform.ToolName}），请先移除或更换");
                }

                // 5. 添加刑具
                platform.ToolName = toolName.Trim();
                platform.ToolType = toolType.Trim();
                platform.ToolDescription = description?.Trim();
                platform.UpdatedAt = DateTime.Now;
                _platformDal.Update(platform);

                return (true, $"已成功为{platform.PlatformNumber}号处刑台添加刑具：{toolName}");
            }
            catch (Exception ex)
            {
                return (false, $"添加刑具失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 更换刑具
        /// </summary>
        public (bool Success, string Message) UpdateTool(int platformID, string toolName, string toolType, string? description, int userIslandID)
        {
            try
            {
                // 1. 验证输入
                if (string.IsNullOrWhiteSpace(toolName))
                {
                    return (false, "刑具名称不能为空");
                }

                if (string.IsNullOrWhiteSpace(toolType))
                {
                    return (false, "刑具类型不能为空");
                }

                // 2. 获取处刑台信息
                var platform = _platformDal.GetByID(platformID);
                if (platform == null)
                {
                    return (false, "处刑台不存在");
                }

                // 3. 验证岛屿权限
                if (platform.IslandID != userIslandID)
                {
                    return (false, "您只能操作本岛屿的处刑台");
                }

                // 4. 更换刑具
                string oldToolName = platform.ToolName ?? "无";
                platform.ToolName = toolName.Trim();
                platform.ToolType = toolType.Trim();
                platform.ToolDescription = description?.Trim();
                platform.UpdatedAt = DateTime.Now;
                _platformDal.Update(platform);

                return (true, $"已成功更换{platform.PlatformNumber}号处刑台的刑具：{oldToolName} → {toolName}");
            }
            catch (Exception ex)
            {
                return (false, $"更换刑具失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 移除刑具
        /// </summary>
        public (bool Success, string Message) RemoveTool(int platformID, int userIslandID)
        {
            try
            {
                // 1. 获取处刑台信息
                var platform = _platformDal.GetByID(platformID);
                if (platform == null)
                {
                    return (false, "处刑台不存在");
                }

                // 2. 验证岛屿权限
                if (platform.IslandID != userIslandID)
                {
                    return (false, "您只能操作本岛屿的处刑台");
                }

                // 3. 检查是否有刑具
                if (!platform.HasTool)
                {
                    return (false, "该处刑台没有刑具");
                }

                // 4. 移除刑具
                string oldToolName = platform.ToolName ?? "";
                platform.ToolName = null;
                platform.ToolType = null;
                platform.ToolDescription = null;
                platform.UpdatedAt = DateTime.Now;
                _platformDal.Update(platform);

                return (true, $"已成功移除{platform.PlatformNumber}号处刑台的刑具：{oldToolName}");
            }
            catch (Exception ex)
            {
                return (false, $"移除刑具失败：{ex.Message}");
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化岛屿的处刑台
        /// </summary>
        public void InitializePlatforms(int islandID)
        {
            var platforms = new List<ExecutionPlatformModel>();
            
            for (int i = 1; i <= 49; i++)
            {
                platforms.Add(new ExecutionPlatformModel
                {
                    IslandID = islandID,
                    PlatformNumber = i,
                    HomePosition = i,
                    CurrentPosition = i,
                    Status = "空闲",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            _platformDal.InsertBatch(platforms);
        }

        #endregion
    }
}
