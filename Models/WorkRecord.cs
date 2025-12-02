using System.Text.Json.Serialization;

namespace WitchTrialSystem.Models
{
    /// <summary>
    /// 工作经历记录
    /// </summary>
    public class WorkRecord
    {
        /// <summary>
        /// 时间段
        /// </summary>
        [JsonPropertyName("period")]
        public string Period { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        [JsonPropertyName("company")]
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// 职位
        /// </summary>
        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// 薪资
        /// </summary>
        [JsonPropertyName("salary")]
        public string Salary { get; set; } = string.Empty;

        /// <summary>
        /// 离职原因
        /// </summary>
        [JsonPropertyName("resignReason")]
        public string ResignReason { get; set; } = string.Empty;
    }
}
