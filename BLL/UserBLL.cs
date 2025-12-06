using WitchTrialSystem.DAL;

namespace WitchTrialSystem.BLL
{
    public class UserBLL
    {
        private readonly UserDAL _dal = new();

        public (int UserID, string Username, int RoleID)? Login(string username, string password)
        {
            var u = _dal.GetByUsername(username);
            if (u == null) return null;

            // 如果还没初始化盐/哈希，直接判失败（或者先 EnsureDefaults）
            if (string.Equals(u.Value.Salt, "PENDING", System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Value.Hash, "PENDING", System.StringComparison.OrdinalIgnoreCase))
                return null;

            var ok = Security.Verify(password, u.Value.Salt, u.Value.Hash);
            if (!ok) return null;

            return (u.Value.UserID, u.Value.Username, u.Value.RoleID);
        }

        // 你已有的 EnsureDefaults 可继续保留，如需也可用 InitPendingAccounts 封装
        public void EnsureDefaults(IEnumerable<string> usernames, string defaultPassword = "123456")
        {
            var (salt, hash) = Security.HashPassword(defaultPassword);
            new UserDAL().InitPendingAccounts(usernames, salt, hash);
        }

        public bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            var u = _dal.GetByUsername(username);
            if (u == null) return false;

            // 还未初始化盐/哈希的直接拒绝
            if (string.Equals(u.Value.Salt, "PENDING", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Value.Hash, "PENDING", StringComparison.OrdinalIgnoreCase))
                return false;

            // 校验旧密码
            if (!Security.Verify(oldPassword, u.Value.Salt, u.Value.Hash))
                return false;

            // 生成新盐+新哈希并写回
            var (salt, hash) = Security.HashPassword(newPassword);
            var rows = _dal.UpdatePassword(username, salt, hash);
            return rows > 0;
        }

        /// <summary>
        /// 检查魔女是否符合创建账号的条件
        /// </summary>
        /// <param name="status">魔女状态</param>
        /// <param name="prisonerNo">囚犯编号</param>
        /// <param name="batchId">批次ID</param>
        /// <param name="witchIslandId">魔女所属岛屿ID</param>
        /// <param name="regulatorIslandId">监管员所属岛屿ID</param>
        /// <returns>符合条件返回true，否则返回false</returns>
        public bool IsAccountEligible(
            string status,
            string? prisonerNo,
            int? batchId,
            int witchIslandId,
            int regulatorIslandId)
        {
            // 1. 状态必须是"分配至岛屿"
            if (status != "分配至岛屿")
                return false;

            // 2. 囚犯编号不能为空
            if (string.IsNullOrWhiteSpace(prisonerNo))
                return false;

            // 3. 批次ID不能为空
            if (!batchId.HasValue)
                return false;

            // 4. 魔女必须属于监管员的岛屿
            if (witchIslandId != regulatorIslandId)
                return false;

            // 5. 账号不能已存在
            if (_dal.UserExists(prisonerNo))
                return false;

            return true;
        }

        /// <summary>
        /// 为魔女创建用户账号
        /// </summary>
        /// <param name="prisonerNo">囚犯编号（作为用户名）</param>
        /// <param name="islandId">岛屿ID</param>
        /// <param name="batchId">批次ID</param>
        /// <param name="witchId">魔女ID</param>
        /// <param name="regulatorIslandId">监管员所属岛屿ID</param>
        /// <returns>成功返回(true, 成功消息)，失败返回(false, 错误消息)</returns>
        public (bool Success, string Message) CreateWitchAccount(
            string prisonerNo,
            int islandId,
            int batchId,
            int witchId,
            int regulatorIslandId)
        {
            try
            {
                // 1. 验证权限：魔女必须属于监管员的岛屿
                if (islandId != regulatorIslandId)
                {
                    return (false, "您只能为本岛屿的魔女创建账号");
                }

                // 2. 检查账号是否已存在
                if (_dal.UserExists(prisonerNo))
                {
                    return (false, $"账号创建失败：用户名已存在（{prisonerNo}）");
                }

                // 3. 获取Witch角色ID
                const string getRoleSql = "SELECT RoleID FROM wt.Role WHERE Name = N'Witch'";
                var roleResult = DBHelper.ExecScalar(getRoleSql);
                if (roleResult == null)
                {
                    return (false, "系统错误：无法找到Witch角色");
                }
                int witchRoleId = Convert.ToInt32(roleResult);

                // 4. 使用固定的盐值和哈希（对应密码"123456"）
                const string fixedSalt = "Yipintianxia_MiddleRingRoad_2025";
                const string fixedHash = "0A98E098B42638B461C3C4E820D1D325F896928BB5DB655DA3BDDDD97F1DC976";

                // 5. 创建账号和关联
                int newUserId = _dal.CreateWitchAccountWithAssociation(
                    prisonerNo,
                    witchRoleId,
                    islandId,
                    batchId,
                    fixedSalt,
                    fixedHash,
                    witchId);

                return (true, $"账号创建成功！\n用户名：{prisonerNo}\n默认密码：123456\n\n请提醒魔女首次登录后修改密码。");
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                // 唯一约束冲突
                return (false, $"账号创建失败：用户名已存在（{prisonerNo}）");
            }
            catch (Exception ex)
            {
                return (false, $"账号创建失败：{ex.Message}");
            }
        }

    }
}


// using WitchTrialSystem.DAL;

// namespace WitchTrialSystem.BLL
// {
//     public class UserBLL
//     {
//         private readonly UserDAL _dal = new();

//         // ✅ 唯一方法：给指定用户名写默认口令（仅当该用户为 PENDING）
//         public void EnsureDefaultFor(string username, string defaultPassword = "123456")
//         {
//             var (salt, hash) = Security.HashPassword(defaultPassword);
//             _dal.UpsertIfPending(username, salt, hash);//这里亮红线了
//         }

//         // 可选：批量初始化（内部仍调用同一个方法）
//         public void EnsureDefaults(IEnumerable<string> usernames, string defaultPassword = "123456")
//         {
//             foreach (var u in usernames)
//                 EnsureDefaultFor(u, defaultPassword);
//         }

//         public (int UserID, int RoleID)? Login(string username, string password)
//         {
//             var user = _dal.GetByUsername(username);
//             if (user == null) return null;

//             var ok = Security.Verify(password, user.Value.Salt, user.Value.Hash);
//             return ok ? (user.Value.UserID, user.Value.RoleID) : null;
//         }

//         public bool ChangePassword(string username, string oldPassword, string newPassword)
//         {
//             // 1) 取用户
//             var user = _dal.GetByUsername(username);
//             if (user == null) return false;

//             // 2) 校验旧密码
//             var ok = Security.Verify(oldPassword, user.Value.Salt, user.Value.Hash);
//             if (!ok) return false;

//             // 3) 生成新盐新哈希并写回
//             var (salt, hash) = Security.HashPassword(newPassword);
//             var n = _dal.UpdatePassword(username, salt, hash);
//             return n > 0;
//         }

//     }
// }

