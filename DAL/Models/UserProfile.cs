namespace WitchTrialSystem.DAL.Models
{
    /// <summary>
    /// 用户档案模型
    /// </summary>
    public class UserProfile
    {
        public int UserID { get; set; }
        public string Username { get; set; } = "";
        public string RoleName { get; set; } = "";
        public int? WitchID { get; set; }
        public string? CnName { get; set; }
        public string? PrisonerNo { get; set; }
        public string? Magic { get; set; }
        public string? CharacterImage { get; set; }
        public int? IslandID { get; set; }
        public int? BatchID { get; set; }
        
        // 五子棋积分
        public int GomokuScore { get; set; } = 0;
    }
}
