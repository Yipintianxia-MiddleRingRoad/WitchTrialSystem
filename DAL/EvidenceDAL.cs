using System.Data;

namespace WitchTrialSystem.DAL
{
    /// <summary>
    /// 证物数据访问层：负责从数据库读取证物基本信息
    /// </summary>
    public class EvidenceDAL
    {
        /// <summary>
        /// 获取所有证物数据（证物序号、名称、描述等）
        /// </summary>
        public DataTable GetEvidenceItems()
        {
            const string sql = @"
SELECT EvidenceID,
       EvidenceNo,
       Name,
       Description
FROM wt.Evidence
ORDER BY TRY_CAST(EvidenceNo AS INT), EvidenceNo";

            return DBHelper.ExecDataTable(sql);
        }
    }
}

