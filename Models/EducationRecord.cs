using System.Text.Json.Serialization;

namespace WitchTrialSystem.Models
{
    /// <summary>
    /// 教育经历记录
    /// </summary>
    public class EducationRecord
    {
        /// <summary>
        /// 学校名称
        /// </summary>
        [JsonPropertyName("school")]
        public string School { get; set; } = string.Empty;

        /// <summary>
        /// 学历
        /// </summary>
        [JsonPropertyName("degree")]
        public string Degree { get; set; } = string.Empty;

        /// <summary>
        /// 状态（毕业/在读/未入学）
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 特殊说明
        /// </summary>
        [JsonPropertyName("specialNote")]
        public string SpecialNote { get; set; } = string.Empty;
    }
}
