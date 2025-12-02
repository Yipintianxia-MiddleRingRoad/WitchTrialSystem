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

        /// <summary>
        /// 更新魔女的公开描述
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <param name="description">新的公开描述（可为 null）</param>
        public void UpdateDescription(int witchId, string? description)
        {
            const string sql = @"
                UPDATE wt.Witch 
                SET DescriptionPublic = @description 
                WHERE WitchID = @witchId";
            
            DBHelper.ExecNonQuery(sql,
                new SqlParameter("@description", description ?? (object)DBNull.Value),
                new SqlParameter("@witchId", witchId));
        }

        /// <summary>
        /// 添加魔女的完整详细档案（42个字段，包含时间戳）
        /// </summary>
        public static int AddWitchComplete(
            string name,
            string magic,
            string status,
            string? prisonerNo = null,
            string? personalNo = null,
            string? gender = null,
            DateTime? birthDate = null,
            string? nationality = null,
            string? birthplace = null,
            string? formerName = null,
            decimal? height = null,
            decimal? weight = null,
            string? bloodType = null,
            string? address = null,
            string? phone = null,
            string? email = null,
            string? lineAccount = null,
            string? highestEducation = null,
            string? educationHistory = null,
            string? workHistory = null,
            string? familyStructure = null,
            string? father = null,
            string? mother = null,
            string? otherFamily1 = null,
            string? otherFamily2 = null,
            string? otherFamily3 = null,
            string? skills = null,
            string? hobbies = null,
            string? ideal = null,
            string? dislike = null,
            string? trauma = null,
            string? witchMethod = null,
            string? remarks = null,
            string? publicDescription = null,
            int? islandId = null,
            int? batchId = null,
            string? avatarPath = null,
            DateTime? captureTime = null,
            DateTime? departureTime = null,
            DateTime? arrivalTime = null,
            DateTime? deathTime = null
        )
        {
            const string sql = @"
                DECLARE @WitchID INT;
                
                EXEC wt.sp_AddWitchComplete
                    @Name = @name,
                    @Magic = @magic,
                    @PrisonerNo = @prisonerNo,
                    @Status = @status,
                    @AvatarPath = @avatarPath,
                    @IslandID = @islandId,
                    @BatchID = @batchId,
                    @DescriptionPublic = @publicDescription,
                    @PersonalNo = @personalNo,
                    @FormerName = @formerName,
                    @Gender = @gender,
                    @BirthDate = @birthDate,
                    @Ethnicity = @nationality,
                    @Birthplace = @birthplace,
                    @Height = @height,
                    @Weight = @weight,
                    @BloodType = @bloodType,
                    @Address = @address,
                    @Phone = @phone,
                    @Email = @email,
                    @LineAccount = @lineAccount,
                    @HighestEducation = @highestEducation,
                    @EducationHistory = @educationHistory,
                    @WorkHistory = @workHistory,
                    @FamilyStructure = @familyStructure,
                    @Father = @father,
                    @Mother = @mother,
                    @OtherFamily1 = @otherFamily1,
                    @OtherFamily2 = @otherFamily2,
                    @OtherFamily3 = @otherFamily3,
                    @Skills = @skills,
                    @Hobbies = @hobbies,
                    @Dreams = @ideal,
                    @Dislikes = @dislike,
                    @Trauma = @trauma,
                    @WitchTransformMethod = @witchMethod,
                    @Remarks = @remarks,
                    @CaptureTime = @captureTime,
                    @DepartureTime = @departureTime,
                    @ArrivalTime = @arrivalTime,
                    @DeathTime = @deathTime,
                    @NewWitchID = @WitchID OUTPUT;
                
                SELECT @WitchID AS WitchID;";

            var parameters = new[]
            {
                new SqlParameter("@name", name),
                new SqlParameter("@magic", magic),
                new SqlParameter("@status", status),
                new SqlParameter("@prisonerNo", prisonerNo ?? (object)DBNull.Value),
                new SqlParameter("@personalNo", personalNo ?? (object)DBNull.Value),
                new SqlParameter("@gender", gender ?? (object)DBNull.Value),
                new SqlParameter("@birthDate", birthDate ?? (object)DBNull.Value),
                new SqlParameter("@nationality", nationality ?? (object)DBNull.Value),
                new SqlParameter("@birthplace", birthplace ?? (object)DBNull.Value),
                new SqlParameter("@formerName", formerName ?? (object)DBNull.Value),
                new SqlParameter("@height", height ?? (object)DBNull.Value),
                new SqlParameter("@weight", weight ?? (object)DBNull.Value),
                new SqlParameter("@bloodType", bloodType ?? (object)DBNull.Value),
                new SqlParameter("@address", address ?? (object)DBNull.Value),
                new SqlParameter("@phone", phone ?? (object)DBNull.Value),
                new SqlParameter("@email", email ?? (object)DBNull.Value),
                new SqlParameter("@lineAccount", lineAccount ?? (object)DBNull.Value),
                new SqlParameter("@highestEducation", highestEducation ?? (object)DBNull.Value),
                new SqlParameter("@educationHistory", educationHistory ?? (object)DBNull.Value),
                new SqlParameter("@workHistory", workHistory ?? (object)DBNull.Value),
                new SqlParameter("@familyStructure", familyStructure ?? (object)DBNull.Value),
                new SqlParameter("@father", father ?? (object)DBNull.Value),
                new SqlParameter("@mother", mother ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily1", otherFamily1 ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily2", otherFamily2 ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily3", otherFamily3 ?? (object)DBNull.Value),
                new SqlParameter("@skills", skills ?? (object)DBNull.Value),
                new SqlParameter("@hobbies", hobbies ?? (object)DBNull.Value),
                new SqlParameter("@ideal", ideal ?? (object)DBNull.Value),
                new SqlParameter("@dislike", dislike ?? (object)DBNull.Value),
                new SqlParameter("@trauma", trauma ?? (object)DBNull.Value),
                new SqlParameter("@witchMethod", witchMethod ?? (object)DBNull.Value),
                new SqlParameter("@remarks", remarks ?? (object)DBNull.Value),
                new SqlParameter("@publicDescription", publicDescription ?? (object)DBNull.Value),
                new SqlParameter("@islandId", islandId ?? (object)DBNull.Value),
                new SqlParameter("@batchId", batchId ?? (object)DBNull.Value),
                new SqlParameter("@avatarPath", avatarPath ?? (object)DBNull.Value),
                new SqlParameter("@captureTime", captureTime ?? (object)DBNull.Value),
                new SqlParameter("@departureTime", departureTime ?? (object)DBNull.Value),
                new SqlParameter("@arrivalTime", arrivalTime ?? (object)DBNull.Value),
                new SqlParameter("@deathTime", deathTime ?? (object)DBNull.Value)
            };

            var result = DBHelper.ExecDataTable(sql, parameters);
            
            if (result.Rows.Count > 0)
            {
                return Convert.ToInt32(result.Rows[0]["WitchID"]);
            }
            
            throw new Exception("添加魔女失败：未返回 WitchID");
        }

        /// <summary>
        /// 获取批次容量信息
        /// </summary>
        /// <param name="batchId">批次ID</param>
        /// <returns>返回 (当前数量, 最大容量) 元组</returns>
        public (int currentCount, int maxCapacity) GetBatchCapacity(int batchId)
        {
            const string sql = @"
                SELECT WitchCount 
                FROM wt.Batch 
                WHERE BatchID = @batchId";
            
            var result = DBHelper.ExecScalar(sql, new SqlParameter("@batchId", batchId));
            
            if (result != null && result != DBNull.Value)
            {
                int currentCount = Convert.ToInt32(result);
                return (currentCount, 13);  // 最大容量固定为 13
            }
            
            throw new Exception($"批次 {batchId} 不存在");
        }

        /// <summary>
        /// 获取批次容量信息（重载版本，支持岛屿ID和批次ID）
        /// </summary>
        /// <param name="islandId">岛屿ID</param>
        /// <param name="batchId">批次ID</param>
        /// <returns>返回 (当前数量, 最大容量) 元组</returns>
        public static (int currentCount, int maxCapacity) GetBatchCapacity(int islandId, int batchId)
        {
            const string sql = @"
                SELECT WitchCount 
                FROM wt.Batch 
                WHERE IslandID = @islandId AND BatchID = @batchId";
            
            var result = DBHelper.ExecScalar(sql, 
                new SqlParameter("@islandId", islandId),
                new SqlParameter("@batchId", batchId));
            
            if (result != null && result != DBNull.Value)
            {
                int currentCount = Convert.ToInt32(result);
                return (currentCount, 13);  // 最大容量固定为 13
            }
            
            throw new Exception($"批次 IslandID={islandId}, BatchID={batchId} 不存在");
        }

        /// <summary>
        /// 获取指定岛屿的所有批次ID列表
        /// </summary>
        /// <param name="islandId">岛屿ID</param>
        /// <returns>批次ID列表</returns>
        public static System.Collections.Generic.List<int> GetBatchesByIsland(int islandId)
        {
            const string sql = @"
                SELECT BatchID 
                FROM wt.Batch 
                WHERE IslandID = @islandId 
                ORDER BY BatchID";
            
            var dt = DBHelper.ExecDataTable(sql, new SqlParameter("@islandId", islandId));
            var batches = new System.Collections.Generic.List<int>();
            
            foreach (DataRow row in dt.Rows)
            {
                batches.Add(Convert.ToInt32(row["BatchID"]));
            }
            
            return batches;
        }

        /// <summary>
        /// 更新魔女的完整详细档案（42个字段）
        /// </summary>
        public static void UpdateWitchComplete(
            int witchId,
            string name,
            string magic,
            string status,
            string? prisonerNo = null,
            string? personalNo = null,
            string? gender = null,
            DateTime? birthDate = null,
            string? nationality = null,
            string? birthplace = null,
            string? formerName = null,
            decimal? height = null,
            decimal? weight = null,
            string? bloodType = null,
            string? address = null,
            string? phone = null,
            string? email = null,
            string? lineAccount = null,
            string? highestEducation = null,
            string? educationHistory = null,
            string? workHistory = null,
            string? familyStructure = null,
            string? father = null,
            string? mother = null,
            string? otherFamily1 = null,
            string? otherFamily2 = null,
            string? otherFamily3 = null,
            string? skills = null,
            string? hobbies = null,
            string? ideal = null,
            string? dislike = null,
            string? trauma = null,
            string? witchMethod = null,
            string? remarks = null,
            string? publicDescription = null,
            int? islandId = null,
            int? batchId = null,
            string? avatarPath = null,
            DateTime? captureTime = null,
            DateTime? departureTime = null,
            DateTime? arrivalTime = null,
            DateTime? deathTime = null
        )
        {
            const string sql = @"
                EXEC wt.sp_UpdateWitchComplete
                    @WitchID = @witchId,
                    @Name = @name,
                    @Magic = @magic,
                    @PrisonerNo = @prisonerNo,
                    @Status = @status,
                    @AvatarPath = @avatarPath,
                    @IslandID = @islandId,
                    @BatchID = @batchId,
                    @DescriptionPublic = @publicDescription,
                    @PersonalNo = @personalNo,
                    @FormerName = @formerName,
                    @Gender = @gender,
                    @BirthDate = @birthDate,
                    @Ethnicity = @nationality,
                    @Birthplace = @birthplace,
                    @Height = @height,
                    @Weight = @weight,
                    @BloodType = @bloodType,
                    @Address = @address,
                    @Phone = @phone,
                    @Email = @email,
                    @LineAccount = @lineAccount,
                    @HighestEducation = @highestEducation,
                    @EducationHistory = @educationHistory,
                    @WorkHistory = @workHistory,
                    @FamilyStructure = @familyStructure,
                    @Father = @father,
                    @Mother = @mother,
                    @OtherFamily1 = @otherFamily1,
                    @OtherFamily2 = @otherFamily2,
                    @OtherFamily3 = @otherFamily3,
                    @Skills = @skills,
                    @Hobbies = @hobbies,
                    @Dreams = @ideal,
                    @Dislikes = @dislike,
                    @Trauma = @trauma,
                    @WitchTransformMethod = @witchMethod,
                    @Remarks = @remarks,
                    @CaptureTime = @captureTime,
                    @DepartureTime = @departureTime,
                    @ArrivalTime = @arrivalTime,
                    @DeathTime = @deathTime;";

            var parameters = new[]
            {
                new SqlParameter("@witchId", witchId),
                new SqlParameter("@name", name),
                new SqlParameter("@magic", magic),
                new SqlParameter("@status", status),
                new SqlParameter("@prisonerNo", prisonerNo ?? (object)DBNull.Value),
                new SqlParameter("@personalNo", personalNo ?? (object)DBNull.Value),
                new SqlParameter("@gender", gender ?? (object)DBNull.Value),
                new SqlParameter("@birthDate", birthDate ?? (object)DBNull.Value),
                new SqlParameter("@nationality", nationality ?? (object)DBNull.Value),
                new SqlParameter("@birthplace", birthplace ?? (object)DBNull.Value),
                new SqlParameter("@formerName", formerName ?? (object)DBNull.Value),
                new SqlParameter("@height", height ?? (object)DBNull.Value),
                new SqlParameter("@weight", weight ?? (object)DBNull.Value),
                new SqlParameter("@bloodType", bloodType ?? (object)DBNull.Value),
                new SqlParameter("@address", address ?? (object)DBNull.Value),
                new SqlParameter("@phone", phone ?? (object)DBNull.Value),
                new SqlParameter("@email", email ?? (object)DBNull.Value),
                new SqlParameter("@lineAccount", lineAccount ?? (object)DBNull.Value),
                new SqlParameter("@highestEducation", highestEducation ?? (object)DBNull.Value),
                new SqlParameter("@educationHistory", educationHistory ?? (object)DBNull.Value),
                new SqlParameter("@workHistory", workHistory ?? (object)DBNull.Value),
                new SqlParameter("@familyStructure", familyStructure ?? (object)DBNull.Value),
                new SqlParameter("@father", father ?? (object)DBNull.Value),
                new SqlParameter("@mother", mother ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily1", otherFamily1 ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily2", otherFamily2 ?? (object)DBNull.Value),
                new SqlParameter("@otherFamily3", otherFamily3 ?? (object)DBNull.Value),
                new SqlParameter("@skills", skills ?? (object)DBNull.Value),
                new SqlParameter("@hobbies", hobbies ?? (object)DBNull.Value),
                new SqlParameter("@ideal", ideal ?? (object)DBNull.Value),
                new SqlParameter("@dislike", dislike ?? (object)DBNull.Value),
                new SqlParameter("@trauma", trauma ?? (object)DBNull.Value),
                new SqlParameter("@witchMethod", witchMethod ?? (object)DBNull.Value),
                new SqlParameter("@remarks", remarks ?? (object)DBNull.Value),
                new SqlParameter("@publicDescription", publicDescription ?? (object)DBNull.Value),
                new SqlParameter("@islandId", islandId ?? (object)DBNull.Value),
                new SqlParameter("@batchId", batchId ?? (object)DBNull.Value),
                new SqlParameter("@avatarPath", avatarPath ?? (object)DBNull.Value),
                new SqlParameter("@captureTime", captureTime ?? (object)DBNull.Value),
                new SqlParameter("@departureTime", departureTime ?? (object)DBNull.Value),
                new SqlParameter("@arrivalTime", arrivalTime ?? (object)DBNull.Value),
                new SqlParameter("@deathTime", deathTime ?? (object)DBNull.Value)
            };

            DBHelper.ExecNonQuery(sql, parameters);
        }
    }
}
