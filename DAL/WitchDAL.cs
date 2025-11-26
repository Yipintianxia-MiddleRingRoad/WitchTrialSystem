using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.DAL
{
    public class WitchDAL
    {
        public DataTable GetIslands()
            => DBHelper.ExecDataTable("SELECT IslandID, Name FROM wt.Island ORDER BY IslandID");

        public DataTable GetBatches(int islandId)
            => DBHelper.ExecDataTable("SELECT BatchID FROM wt.Batch WHERE IslandID=@i ORDER BY BatchID",
                                       new SqlParameter("@i", islandId));

        public DataTable GetWitches(int? islandId=null, int? batchId=null, string? nameLike=null)
        {
            var sql = @"SELECT 
                            WitchID,
                            PrisonerNo,
                            PersonalNo,
                            Name,
                            Gender,
                            BirthDate,
                            DATEDIFF(YEAR, BirthDate, GETDATE()) AS Age,
                            Height,
                            Weight,
                            BloodType,
                            Magic,
                            [Status],
                            HighestEducation,
                            Birthplace,
                            Phone,
                            Email,
                            Skills,
                            Hobbies,
                            Dreams,
                            Trauma,
                            IslandID,
                            BatchID,
                            AvatarPath,
                            DescriptionPublic
                        FROM wt.Witch WHERE 1=1";
            var ps = new System.Collections.Generic.List<SqlParameter>();
            if (islandId.HasValue) { sql += " AND IslandID=@is"; ps.Add(new SqlParameter("@is", islandId.Value)); }
            if (batchId.HasValue)  { sql += " AND BatchID=@ba";  ps.Add(new SqlParameter("@ba", batchId.Value)); }
            if (!string.IsNullOrWhiteSpace(nameLike))
            { sql += " AND Name LIKE @nm"; ps.Add(new SqlParameter("@nm", "%"+nameLike.Trim()+"%")); }
            sql += " ORDER BY PrisonerNo";
            return DBHelper.ExecDataTable(sql, ps.ToArray());
        }

        public void AddWitch(string name, string? magic, string? prisonerNo, int islandId, int batchId)
        {
            const string sql = @"EXEC wt.sp_AddWitch
                                 @Name=@n,@Magic=@m,@PrisonerNo=@p,@IslandID=@i,@BatchID=@b";
            DBHelper.ExecNonQuery(sql,
                new SqlParameter("@n", name),
                new SqlParameter("@m", (object?)magic ?? DBNull.Value),
                new SqlParameter("@p", (object?)prisonerNo ?? DBNull.Value),
                new SqlParameter("@i", islandId),
                new SqlParameter("@b", batchId));
        }

        public void UpdateStatus(int witchId, string newStatus, string? execResult)
        {
            const string sql = @"EXEC wt.sp_UpdateWitchStatus @WitchID=@id, @NewStatus=@st, @ExecutionResult=@er";
            DBHelper.ExecNonQuery(sql,
                new SqlParameter("@id", witchId),
                new SqlParameter("@st", newStatus),
                new SqlParameter("@er", (object?)execResult ?? DBNull.Value));
        }

        /// <summary>
        /// 获取单个魔女的完整详细信息
        /// </summary>
        public DataTable GetWitchDetail(int witchId)
        {
            const string sql = @"
                SELECT 
                    WitchID,
                    PrisonerNo,
                    PersonalNo,
                    Name,
                    FormerName,
                    Gender,
                    BirthDate,
                    DATEDIFF(YEAR, BirthDate, GETDATE()) AS Age,
                    Ethnicity,
                    Birthplace,
                    Height,
                    Weight,
                    BloodType,
                    Address,
                    Phone,
                    Email,
                    LineAccount,
                    HighestEducation,
                    EducationHistory,
                    WorkHistory,
                    FamilyStructure,
                    Father,
                    Mother,
                    OtherFamily1,
                    OtherFamily2,
                    OtherFamily3,
                    Skills,
                    Hobbies,
                    Dreams,
                    Dislikes,
                    Trauma,
                    Magic,
                    [Status],
                    ExecutionResult,
                    WitchTransformMethod,
                    Remarks,
                    IslandID,
                    BatchID,
                    AvatarPath,
                    DescriptionPublic
                FROM wt.Witch 
                WHERE WitchID = @id";
            return DBHelper.ExecDataTable(sql, new SqlParameter("@id", witchId));
        }

    }
}
