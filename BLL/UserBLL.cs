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

