using System;
using System.Collections.Generic;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 审判通知业务逻辑层
    /// </summary>
    public class TrialNotificationService
    {
        /// <summary>
        /// 创建通知（批量）
        /// </summary>
        public static void CreateNotifications(int sessionID, List<int> userIDs, string message)
        {
            try
            {
                var notifications = new List<TrialNotificationModel>();
                
                foreach (var userID in userIDs)
                {
                    notifications.Add(new TrialNotificationModel
                    {
                        SessionID = sessionID,
                        UserID = userID,
                        Message = message,
                        IsRead = false,
                        CreatedAt = DateTime.Now
                    });
                }

                TrialNotificationDAL.InsertBatch(notifications);
            }
            catch (Exception ex)
            {
                // 记录错误日志，但不影响主流程
                Console.WriteLine($"创建通知失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取未读通知
        /// </summary>
        public static List<TrialNotificationModel> GetUnreadNotifications(int userID)
        {
            try
            {
                return TrialNotificationDAL.GetByUser(userID, unreadOnly: true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取未读通知失败：{ex.Message}");
                return new List<TrialNotificationModel>();
            }
        }

        /// <summary>
        /// 标记为已读
        /// </summary>
        public static void MarkAsRead(int notificationID)
        {
            try
            {
                TrialNotificationDAL.MarkAsRead(notificationID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"标记通知为已读失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 标记所有为已读
        /// </summary>
        public static void MarkAllAsRead(int userID)
        {
            try
            {
                TrialNotificationDAL.MarkAllAsRead(userID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"标记所有通知为已读失败：{ex.Message}");
            }
        }

        /// <summary>
        /// 获取会话的所有通知
        /// </summary>
        public static List<TrialNotificationModel> GetNotificationsBySession(int sessionID)
        {
            try
            {
                return TrialNotificationDAL.GetBySession(sessionID);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取会话通知失败：{ex.Message}");
                return new List<TrialNotificationModel>();
            }
        }

        /// <summary>
        /// 获取未读数量
        /// </summary>
        public static int GetUnreadCount(int userID)
        {
            try
            {
                var unreadNotifications = GetUnreadNotifications(userID);
                return unreadNotifications.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取未读数量失败：{ex.Message}");
                return 0;
            }
        }
    }
}
