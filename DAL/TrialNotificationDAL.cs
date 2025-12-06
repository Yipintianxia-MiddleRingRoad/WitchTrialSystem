using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 审判通知数据访问层
    /// </summary>
    public class TrialNotificationDAL
    {
        /// <summary>
        /// 插入通知
        /// </summary>
        public static int Insert(TrialNotificationModel notification)
        {
            const string sql = @"
                INSERT INTO wt.TrialNotification (SessionID, UserID, Message, IsRead, CreatedAt)
                VALUES (@SessionID, @UserID, @Message, @IsRead, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var parameters = new[]
            {
                new SqlParameter("@SessionID", notification.SessionID),
                new SqlParameter("@UserID", notification.UserID),
                new SqlParameter("@Message", notification.Message),
                new SqlParameter("@IsRead", notification.IsRead),
                new SqlParameter("@CreatedAt", notification.CreatedAt)
            };

            object result = DBHelper.ExecScalar(sql, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// 批量插入通知
        /// </summary>
        public static int InsertBatch(List<TrialNotificationModel> notifications)
        {
            int count = 0;
            foreach (var notification in notifications)
            {
                int result = Insert(notification);
                if (result > 0)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// 查询用户的通知
        /// </summary>
        public static List<TrialNotificationModel> GetByUser(int userID, bool unreadOnly = false)
        {
            string sql = @"
                SELECT * FROM wt.TrialNotification 
                WHERE UserID = @UserID";

            if (unreadOnly)
                sql += " AND IsRead = 0";

            sql += " ORDER BY CreatedAt DESC";

            var parameters = new[] { new SqlParameter("@UserID", userID) };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            var list = new List<TrialNotificationModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapToModel(row));
            }

            return list;
        }

        /// <summary>
        /// 查询会话的通知
        /// </summary>
        public static List<TrialNotificationModel> GetBySession(int sessionID)
        {
            const string sql = @"
                SELECT * FROM wt.TrialNotification 
                WHERE SessionID = @SessionID
                ORDER BY CreatedAt DESC";

            var parameters = new[] { new SqlParameter("@SessionID", sessionID) };

            DataTable dt = DBHelper.ExecDataTable(sql, parameters);

            var list = new List<TrialNotificationModel>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(MapToModel(row));
            }

            return list;
        }

        /// <summary>
        /// 标记为已读
        /// </summary>
        public static int MarkAsRead(int notificationID)
        {
            const string sql = "UPDATE wt.TrialNotification SET IsRead = 1 WHERE NotificationID = @NotificationID";

            var parameters = new[] { new SqlParameter("@NotificationID", notificationID) };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 标记所有为已读
        /// </summary>
        public static int MarkAllAsRead(int userID)
        {
            const string sql = "UPDATE wt.TrialNotification SET IsRead = 1 WHERE UserID = @UserID AND IsRead = 0";

            var parameters = new[] { new SqlParameter("@UserID", userID) };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 将DataRow映射到Model
        /// </summary>
        private static TrialNotificationModel MapToModel(DataRow row)
        {
            return new TrialNotificationModel
            {
                NotificationID = Convert.ToInt32(row["NotificationID"]),
                SessionID = Convert.ToInt32(row["SessionID"]),
                UserID = Convert.ToInt32(row["UserID"]),
                Message = row["Message"].ToString() ?? "",
                IsRead = Convert.ToBoolean(row["IsRead"]),
                CreatedAt = Convert.ToDateTime(row["CreatedAt"])
            };
        }
    }
}
