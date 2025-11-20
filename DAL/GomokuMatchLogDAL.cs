using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 五子棋对局日志数据访问层
    /// </summary>
    public class GomokuMatchLogDAL
    {
        /// <summary>
        /// 保存对局记录
        /// </summary>
        public int SaveMatchLog(
            string player1Username, string player1Name,
            string player2Username, string player2Name,
            DateTime startTime, DateTime endTime,
            string player1Result, int player1ScoreChange,
            string player2Result, int player2ScoreChange,
            int totalMoves, int duration)
        {
            const string sql = @"
INSERT INTO wt.GomokuMatchLog 
(Player1Username, Player1Name, Player2Username, Player2Name, 
 StartTime, EndTime, Player1Result, Player1ScoreChange, 
 Player2Result, Player2ScoreChange, TotalMoves, Duration)
VALUES 
(@Player1Username, @Player1Name, @Player2Username, @Player2Name,
 @StartTime, @EndTime, @Player1Result, @Player1ScoreChange,
 @Player2Result, @Player2ScoreChange, @TotalMoves, @Duration)";

            var parameters = new[]
            {
                new SqlParameter("@Player1Username", player1Username),
                new SqlParameter("@Player1Name", player1Name),
                new SqlParameter("@Player2Username", player2Username),
                new SqlParameter("@Player2Name", player2Name),
                new SqlParameter("@StartTime", startTime),
                new SqlParameter("@EndTime", endTime),
                new SqlParameter("@Player1Result", player1Result),
                new SqlParameter("@Player1ScoreChange", player1ScoreChange),
                new SqlParameter("@Player2Result", player2Result),
                new SqlParameter("@Player2ScoreChange", player2ScoreChange),
                new SqlParameter("@TotalMoves", totalMoves),
                new SqlParameter("@Duration", duration)
            };

            return DBHelper.ExecNonQuery(sql, parameters);
        }

        /// <summary>
        /// 获取所有对局记录
        /// </summary>
        public DataTable GetAllMatchLogs()
        {
            const string sql = @"
SELECT m.MatchID, 
       m.Player1Username, w1.PrisonerNo AS Player1PrisonerNo, m.Player1Name, 
       m.Player2Username, w2.PrisonerNo AS Player2PrisonerNo, m.Player2Name,
       m.StartTime, m.EndTime, 
       m.Player1Result, m.Player1ScoreChange,
       m.Player2Result, m.Player2ScoreChange, 
       m.TotalMoves, m.Duration
FROM wt.GomokuMatchLog m
LEFT JOIN wt.[User] u1 ON u1.Username = m.Player1Username
LEFT JOIN wt.UserWitch uw1 ON uw1.UserID = u1.UserID
LEFT JOIN wt.Witch w1 ON w1.WitchID = uw1.WitchID
LEFT JOIN wt.[User] u2 ON u2.Username = m.Player2Username
LEFT JOIN wt.UserWitch uw2 ON uw2.UserID = u2.UserID
LEFT JOIN wt.Witch w2 ON w2.WitchID = uw2.WitchID
ORDER BY m.StartTime DESC";

            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 根据玩家筛选对局记录
        /// </summary>
        public DataTable GetMatchLogsByPlayer(string username)
        {
            const string sql = @"
SELECT m.MatchID, 
       m.Player1Username, w1.PrisonerNo AS Player1PrisonerNo, m.Player1Name, 
       m.Player2Username, w2.PrisonerNo AS Player2PrisonerNo, m.Player2Name,
       m.StartTime, m.EndTime, 
       m.Player1Result, m.Player1ScoreChange,
       m.Player2Result, m.Player2ScoreChange, 
       m.TotalMoves, m.Duration
FROM wt.GomokuMatchLog m
LEFT JOIN wt.[User] u1 ON u1.Username = m.Player1Username
LEFT JOIN wt.UserWitch uw1 ON uw1.UserID = u1.UserID
LEFT JOIN wt.Witch w1 ON w1.WitchID = uw1.WitchID
LEFT JOIN wt.[User] u2 ON u2.Username = m.Player2Username
LEFT JOIN wt.UserWitch uw2 ON uw2.UserID = u2.UserID
LEFT JOIN wt.Witch w2 ON w2.WitchID = uw2.WitchID
WHERE m.Player1Username = @Username OR m.Player2Username = @Username
ORDER BY m.StartTime DESC";

            return DBHelper.ExecDataTable(sql, new SqlParameter("@Username", username));
        }

        /// <summary>
        /// 根据两个玩家筛选对局记录
        /// </summary>
        public DataTable GetMatchLogsByTwoPlayers(string username1, string username2)
        {
            const string sql = @"
SELECT m.MatchID, 
       m.Player1Username, w1.PrisonerNo AS Player1PrisonerNo, m.Player1Name, 
       m.Player2Username, w2.PrisonerNo AS Player2PrisonerNo, m.Player2Name,
       m.StartTime, m.EndTime, 
       m.Player1Result, m.Player1ScoreChange,
       m.Player2Result, m.Player2ScoreChange, 
       m.TotalMoves, m.Duration
FROM wt.GomokuMatchLog m
LEFT JOIN wt.[User] u1 ON u1.Username = m.Player1Username
LEFT JOIN wt.UserWitch uw1 ON uw1.UserID = u1.UserID
LEFT JOIN wt.Witch w1 ON w1.WitchID = uw1.WitchID
LEFT JOIN wt.[User] u2 ON u2.Username = m.Player2Username
LEFT JOIN wt.UserWitch uw2 ON uw2.UserID = u2.UserID
LEFT JOIN wt.Witch w2 ON w2.WitchID = uw2.WitchID
WHERE (m.Player1Username = @Username1 AND m.Player2Username = @Username2)
   OR (m.Player1Username = @Username2 AND m.Player2Username = @Username1)
ORDER BY m.StartTime DESC";

            var parameters = new[]
            {
                new SqlParameter("@Username1", username1),
                new SqlParameter("@Username2", username2)
            };

            return DBHelper.ExecDataTable(sql, parameters);
        }
    }
}
