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
            => DBHelper.ExecDataTable("SELECT BatchID, LocalBatchID FROM wt.Batch WHERE IslandID=@i ORDER BY LocalBatchID",
                                       new SqlParameter("@i", islandId));

        // 根据岛屿ID和本地批次号获取全局批次ID
        public static int? GetBatchIdByLocal(int islandId, int localBatchId)
        {
            var sql = "SELECT BatchID FROM wt.Batch WHERE IslandID=@islandId AND LocalBatchID=@localBatchId";
            var result = DBHelper.ExecScalar(sql,
                new SqlParameter("@islandId", islandId),
                new SqlParameter("@localBatchId", localBatchId));
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
        }

        // 根据全局批次ID获取本地批次号
        public static int? GetLocalBatchIdByBatchId(int batchId)
        {
            var sql = "SELECT LocalBatchID FROM wt.Batch WHERE BatchID=@batchId";
            var result = DBHelper.ExecScalar(sql, new SqlParameter("@batchId", batchId));
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
        }

        public DataTable GetWitches(int? islandId=null, int? batchId=null, string? nameLike=null)
        {
            var sql = @"SELECT 
                            w.WitchID,
                            w.PrisonerNo,
                            w.PersonalNo,
                            w.Name,
                            w.Gender,
                            w.BirthDate,
                            DATEDIFF(YEAR, w.BirthDate, GETDATE()) AS Age,
                            w.Height,
                            w.Weight,
                            w.BloodType,
                            w.Magic,
                            w.[Status],
                            w.HighestEducation,
                            w.Birthplace,
                            w.Phone,
                            w.Email,
                            w.Skills,
                            w.Hobbies,
                            w.Dreams,
                            w.Trauma,
                            w.IslandID,
                            i.Name AS IslandName,
                            w.BatchID,
                            b.LocalBatchID,
                            w.AvatarPath,
                            w.DescriptionPublic
                        FROM wt.Witch w
                        LEFT JOIN wt.Island i ON w.IslandID = i.IslandID
                        LEFT JOIN wt.Batch b ON w.BatchID = b.BatchID
                        WHERE 1=1";
            var ps = new System.Collections.Generic.List<SqlParameter>();
            if (islandId.HasValue) { sql += " AND w.IslandID=@is"; ps.Add(new SqlParameter("@is", islandId.Value)); }
            if (batchId.HasValue)  { sql += " AND w.BatchID=@ba";  ps.Add(new SqlParameter("@ba", batchId.Value)); }
            if (!string.IsNullOrWhiteSpace(nameLike))
            { sql += " AND w.Name LIKE @nm"; ps.Add(new SqlParameter("@nm", "%"+nameLike.Trim()+"%")); }
            sql += " ORDER BY w.PrisonerNo";
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
        /// 删除魔女（仅限状态为"待分配"的魔女）
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <returns>是否删除成功</returns>
        public bool DeleteWitch(int witchId)
        {
            const string sql = @"
                DELETE FROM wt.Witch 
                WHERE WitchID = @WitchID 
                AND Status = N'待分配'";
            
            int rowsAffected = DBHelper.ExecNonQuery(sql, new SqlParameter("@WitchID", witchId));
            return rowsAffected > 0;
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
        /// 添加魔女的完整详细档案（41个字段，包含时间戳）
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
            try
            {
                using var conn = DBHelper.GetConn();
                using var cmd = new SqlCommand("wt.sp_AddWitchComplete", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // 只传递必填参数和有值的可选参数
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = name;
                cmd.Parameters.Add("@Magic", SqlDbType.NVarChar, 100).Value = magic;
                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status;
                
                if (prisonerNo != null)
                    cmd.Parameters.Add("@PrisonerNo", SqlDbType.NVarChar, 20).Value = prisonerNo;
                if (personalNo != null)
                    cmd.Parameters.Add("@PersonalNo", SqlDbType.NVarChar, 20).Value = personalNo;
                if (gender != null)
                    cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = gender;
                if (birthDate.HasValue)
                    cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = birthDate.Value;
                if (nationality != null)
                    cmd.Parameters.Add("@Ethnicity", SqlDbType.NVarChar, 50).Value = nationality;
                if (birthplace != null)
                    cmd.Parameters.Add("@Birthplace", SqlDbType.NVarChar, 100).Value = birthplace;
                if (formerName != null)
                    cmd.Parameters.Add("@FormerName", SqlDbType.NVarChar, 100).Value = formerName;
                if (height.HasValue)
                    cmd.Parameters.Add("@Height", SqlDbType.Decimal).Value = height.Value;
                if (weight.HasValue)
                    cmd.Parameters.Add("@Weight", SqlDbType.Decimal).Value = weight.Value;
                if (bloodType != null)
                    cmd.Parameters.Add("@BloodType", SqlDbType.NVarChar, 10).Value = bloodType;
                if (address != null)
                    cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = address;
                if (phone != null)
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value = phone;
                if (email != null)
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                if (lineAccount != null)
                    cmd.Parameters.Add("@LineAccount", SqlDbType.NVarChar, 100).Value = lineAccount;
                if (highestEducation != null)
                    cmd.Parameters.Add("@HighestEducation", SqlDbType.NVarChar, 100).Value = highestEducation;
                if (educationHistory != null)
                    cmd.Parameters.Add("@EducationHistory", SqlDbType.NVarChar, -1).Value = educationHistory;
                if (workHistory != null)
                    cmd.Parameters.Add("@WorkHistory", SqlDbType.NVarChar, -1).Value = workHistory;
                if (familyStructure != null)
                    cmd.Parameters.Add("@FamilyStructure", SqlDbType.NVarChar, 200).Value = familyStructure;
                if (father != null)
                    cmd.Parameters.Add("@Father", SqlDbType.NVarChar, 200).Value = father;
                if (mother != null)
                    cmd.Parameters.Add("@Mother", SqlDbType.NVarChar, 200).Value = mother;
                if (otherFamily1 != null)
                    cmd.Parameters.Add("@OtherFamily1", SqlDbType.NVarChar, 200).Value = otherFamily1;
                if (otherFamily2 != null)
                    cmd.Parameters.Add("@OtherFamily2", SqlDbType.NVarChar, 200).Value = otherFamily2;
                if (otherFamily3 != null)
                    cmd.Parameters.Add("@OtherFamily3", SqlDbType.NVarChar, 200).Value = otherFamily3;
                if (skills != null)
                    cmd.Parameters.Add("@Skills", SqlDbType.NVarChar, 500).Value = skills;
                if (hobbies != null)
                    cmd.Parameters.Add("@Hobbies", SqlDbType.NVarChar, 500).Value = hobbies;
                if (ideal != null)
                    cmd.Parameters.Add("@Dreams", SqlDbType.NVarChar, 500).Value = ideal;
                if (dislike != null)
                    cmd.Parameters.Add("@Dislikes", SqlDbType.NVarChar, 500).Value = dislike;
                if (trauma != null)
                    cmd.Parameters.Add("@Trauma", SqlDbType.NVarChar, -1).Value = trauma;
                if (witchMethod != null)
                    cmd.Parameters.Add("@WitchTransformMethod", SqlDbType.NVarChar, 500).Value = witchMethod;
                if (remarks != null)
                    cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar, -1).Value = remarks;
                if (publicDescription != null)
                    cmd.Parameters.Add("@DescriptionPublic", SqlDbType.NVarChar, -1).Value = publicDescription;
                if (avatarPath != null)
                    cmd.Parameters.Add("@AvatarPath", SqlDbType.NVarChar, 255).Value = avatarPath;
                if (islandId.HasValue)
                    cmd.Parameters.Add("@IslandID", SqlDbType.Int).Value = islandId.Value;
                if (batchId.HasValue)
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = batchId.Value;
                if (captureTime.HasValue)
                    cmd.Parameters.Add("@CaptureTime", SqlDbType.DateTime2).Value = captureTime.Value;
                if (departureTime.HasValue)
                    cmd.Parameters.Add("@DepartureTime", SqlDbType.DateTime2).Value = departureTime.Value;
                if (arrivalTime.HasValue)
                    cmd.Parameters.Add("@ArrivalTime", SqlDbType.DateTime2).Value = arrivalTime.Value;
                if (deathTime.HasValue)
                    cmd.Parameters.Add("@DeathTime", SqlDbType.DateTime2).Value = deathTime.Value;
                
                // 添加输出参数
                var outputParam = cmd.Parameters.Add("@NewWitchID", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;

                // 执行存储过程
                cmd.ExecuteNonQuery();

                // 从输出参数获取 WitchID
                if (outputParam.Value != DBNull.Value)
                {
                    return Convert.ToInt32(outputParam.Value);
                }

                throw new Exception("添加魔女失败：未返回 WitchID");
            }
            catch (Exception ex)
            {
                throw new Exception($"添加魔女失败：{ex.Message}");
            }
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
        /// 获取本岛批次及人数统计（用于批次分配）
        /// </summary>
        /// <param name="islandId">岛屿ID</param>
        /// <returns>包含 LocalBatchID, DisplayText, CurrentCount 的数据表</returns>
        public DataTable GetLocalBatchesWithCount(int islandId)
        {
            const string sql = @"
                SELECT 
                    b.LocalBatchID,
                    b.BatchID,
                    ISNULL(COUNT(w.WitchID), 0) AS CurrentCount,
                    CASE 
                        WHEN ISNULL(COUNT(w.WitchID), 0) >= 13 
                        THEN '批次 ' + CAST(b.LocalBatchID AS NVARCHAR) + ' (' + CAST(COUNT(w.WitchID) AS NVARCHAR) + '/13 - 已满)'
                        ELSE '批次 ' + CAST(b.LocalBatchID AS NVARCHAR) + ' (' + CAST(ISNULL(COUNT(w.WitchID), 0) AS NVARCHAR) + '/13)'
                    END AS DisplayText
                FROM wt.Batch b
                LEFT JOIN wt.Witch w ON b.BatchID = w.BatchID
                WHERE b.IslandID = @islandId
                GROUP BY b.LocalBatchID, b.BatchID
                ORDER BY b.LocalBatchID";
            
            return DBHelper.ExecDataTable(sql, new SqlParameter("@islandId", islandId));
        }

        /// <summary>
        /// 更新魔女的本岛批次
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <param name="localBatchId">本岛批次ID（1-10），null表示待分配</param>
        /// <param name="islandId">岛屿ID（用于验证权限和获取全局BatchID）</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateWitchLocalBatch(int witchId, int? localBatchId, int islandId)
        {
            try
            {
                // 如果是待分配（localBatchId为null），直接设置BatchID为null
                if (!localBatchId.HasValue)
                {
                    const string sqlClear = @"
                        UPDATE wt.Witch 
                        SET BatchID = NULL 
                        WHERE WitchID = @witchId AND IslandID = @islandId";
                    
                    int rows = DBHelper.ExecNonQuery(sqlClear,
                        new SqlParameter("@witchId", witchId),
                        new SqlParameter("@islandId", islandId));
                    
                    return rows > 0;
                }

                // 获取全局BatchID
                int? globalBatchId = GetBatchIdByLocal(islandId, localBatchId.Value);
                if (!globalBatchId.HasValue)
                {
                    throw new Exception($"找不到岛屿 {islandId} 的本地批次 {localBatchId.Value}");
                }

                // 检查批次容量
                var (currentCount, maxCapacity) = GetBatchCapacity(globalBatchId.Value);
                if (currentCount >= maxCapacity)
                {
                    throw new Exception($"批次 {localBatchId.Value} 已满（{currentCount}/{maxCapacity}人）");
                }

                // 更新魔女的BatchID
                const string sql = @"
                    UPDATE wt.Witch 
                    SET BatchID = @batchId 
                    WHERE WitchID = @witchId AND IslandID = @islandId";
                
                int affectedRows = DBHelper.ExecNonQuery(sql,
                    new SqlParameter("@batchId", globalBatchId.Value),
                    new SqlParameter("@witchId", witchId),
                    new SqlParameter("@islandId", islandId));
                
                return affectedRows > 0;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 更新魔女状态（简化版，不记录执行结果）
        /// </summary>
        /// <param name="witchId">魔女ID</param>
        /// <param name="newStatus">新状态</param>
        /// <returns>是否更新成功</returns>
        public bool UpdateWitchStatusSimple(int witchId, string newStatus)
        {
            const string sql = @"
                UPDATE wt.Witch 
                SET [Status] = @status 
                WHERE WitchID = @witchId";
            
            int rows = DBHelper.ExecNonQuery(sql,
                new SqlParameter("@status", newStatus),
                new SqlParameter("@witchId", witchId));
            
            return rows > 0;
        }

        /// <summary>
        /// 更新魔女的完整详细档案（41个字段）
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
            try
            {
                using var conn = DBHelper.GetConn();
                using var cmd = new SqlCommand("wt.sp_UpdateWitchComplete", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // 只传递必填参数和有值的可选参数
                cmd.Parameters.Add("@WitchID", SqlDbType.Int).Value = witchId;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = name;
                cmd.Parameters.Add("@Magic", SqlDbType.NVarChar, 100).Value = magic;
                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 20).Value = status;
                
                if (prisonerNo != null)
                    cmd.Parameters.Add("@PrisonerNo", SqlDbType.NVarChar, 20).Value = prisonerNo;
                if (personalNo != null)
                    cmd.Parameters.Add("@PersonalNo", SqlDbType.NVarChar, 20).Value = personalNo;
                if (gender != null)
                    cmd.Parameters.Add("@Gender", SqlDbType.NVarChar, 10).Value = gender;
                if (birthDate.HasValue)
                    cmd.Parameters.Add("@BirthDate", SqlDbType.Date).Value = birthDate.Value;
                if (nationality != null)
                    cmd.Parameters.Add("@Ethnicity", SqlDbType.NVarChar, 50).Value = nationality;
                if (birthplace != null)
                    cmd.Parameters.Add("@Birthplace", SqlDbType.NVarChar, 100).Value = birthplace;
                if (formerName != null)
                    cmd.Parameters.Add("@FormerName", SqlDbType.NVarChar, 100).Value = formerName;
                if (height.HasValue)
                    cmd.Parameters.Add("@Height", SqlDbType.Decimal).Value = height.Value;
                if (weight.HasValue)
                    cmd.Parameters.Add("@Weight", SqlDbType.Decimal).Value = weight.Value;
                if (bloodType != null)
                    cmd.Parameters.Add("@BloodType", SqlDbType.NVarChar, 10).Value = bloodType;
                if (address != null)
                    cmd.Parameters.Add("@Address", SqlDbType.NVarChar, 500).Value = address;
                if (phone != null)
                    cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 50).Value = phone;
                if (email != null)
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                if (lineAccount != null)
                    cmd.Parameters.Add("@LineAccount", SqlDbType.NVarChar, 100).Value = lineAccount;
                if (highestEducation != null)
                    cmd.Parameters.Add("@HighestEducation", SqlDbType.NVarChar, 100).Value = highestEducation;
                if (educationHistory != null)
                    cmd.Parameters.Add("@EducationHistory", SqlDbType.NVarChar, -1).Value = educationHistory;
                if (workHistory != null)
                    cmd.Parameters.Add("@WorkHistory", SqlDbType.NVarChar, -1).Value = workHistory;
                if (familyStructure != null)
                    cmd.Parameters.Add("@FamilyStructure", SqlDbType.NVarChar, 200).Value = familyStructure;
                if (father != null)
                    cmd.Parameters.Add("@Father", SqlDbType.NVarChar, 200).Value = father;
                if (mother != null)
                    cmd.Parameters.Add("@Mother", SqlDbType.NVarChar, 200).Value = mother;
                if (otherFamily1 != null)
                    cmd.Parameters.Add("@OtherFamily1", SqlDbType.NVarChar, 200).Value = otherFamily1;
                if (otherFamily2 != null)
                    cmd.Parameters.Add("@OtherFamily2", SqlDbType.NVarChar, 200).Value = otherFamily2;
                if (otherFamily3 != null)
                    cmd.Parameters.Add("@OtherFamily3", SqlDbType.NVarChar, 200).Value = otherFamily3;
                if (skills != null)
                    cmd.Parameters.Add("@Skills", SqlDbType.NVarChar, 500).Value = skills;
                if (hobbies != null)
                    cmd.Parameters.Add("@Hobbies", SqlDbType.NVarChar, 500).Value = hobbies;
                if (ideal != null)
                    cmd.Parameters.Add("@Dreams", SqlDbType.NVarChar, 500).Value = ideal;
                if (dislike != null)
                    cmd.Parameters.Add("@Dislikes", SqlDbType.NVarChar, 500).Value = dislike;
                if (trauma != null)
                    cmd.Parameters.Add("@Trauma", SqlDbType.NVarChar, -1).Value = trauma;
                if (witchMethod != null)
                    cmd.Parameters.Add("@WitchTransformMethod", SqlDbType.NVarChar, 500).Value = witchMethod;
                if (remarks != null)
                    cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar, -1).Value = remarks;
                if (publicDescription != null)
                    cmd.Parameters.Add("@DescriptionPublic", SqlDbType.NVarChar, -1).Value = publicDescription;
                if (avatarPath != null)
                    cmd.Parameters.Add("@AvatarPath", SqlDbType.NVarChar, 255).Value = avatarPath;
                if (islandId.HasValue)
                    cmd.Parameters.Add("@IslandID", SqlDbType.Int).Value = islandId.Value;
                if (batchId.HasValue)
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = batchId.Value;
                if (captureTime.HasValue)
                    cmd.Parameters.Add("@CaptureTime", SqlDbType.DateTime2).Value = captureTime.Value;
                if (departureTime.HasValue)
                    cmd.Parameters.Add("@DepartureTime", SqlDbType.DateTime2).Value = departureTime.Value;
                if (arrivalTime.HasValue)
                    cmd.Parameters.Add("@ArrivalTime", SqlDbType.DateTime2).Value = arrivalTime.Value;
                if (deathTime.HasValue)
                    cmd.Parameters.Add("@DeathTime", SqlDbType.DateTime2).Value = deathTime.Value;

                // 执行存储过程
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"更新魔女失败：{ex.Message}");
            }
        }
    }
}
