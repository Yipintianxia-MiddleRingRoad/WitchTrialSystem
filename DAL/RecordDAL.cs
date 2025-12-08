using System.Data;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 记录数据访问层：负责从数据库读取记录信息
    /// </summary>
    public class RecordDAL
    {
        /// <summary>
        /// 获取所有记录数据（RecordID、Title、Content文件路径）
        /// </summary>
        public DataTable GetRecords()
        {
            const string sql = @"
SELECT RecordID,
       Title,
       Content
FROM wt.Record
ORDER BY RecordID";

            return DBHelper.ExecDataTable(sql);
        }

        /// <summary>
        /// 根据记录ID获取记录内容
        /// </summary>
        public DataTable GetRecordById(int recordId)
        {
            const string sql = @"
SELECT RecordID,
       Title,
       Content
FROM wt.Record
WHERE RecordID = @RecordID";

            return DBHelper.ExecDataTable(sql, 
                new Microsoft.Data.SqlClient.SqlParameter("@RecordID", recordId));
        }
    }
}

