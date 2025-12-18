using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// 录音服务：管理录音文件的存储、查询和删除
    /// 文件按账号所属编号（658-721）存储到 UI/recorder/{编号}/ 下
    /// </summary>
    public class RecordingService
    {
        /// <summary>
        /// 获取用户对应的录音文件夹编号
        /// 如果用户名是数字且在658-721范围内，直接使用；否则尝试从数据库获取UserID
        /// </summary>
        private int GetUserFolderNumber(string username)
        {
            // 尝试解析用户名为数字
            if (int.TryParse(username, out int number) && number >= 658 && number <= 721)
            {
                return number;
            }

            // 如果用户名不是编号，尝试从数据库获取UserID
            try
            {
                var user = new UserDAL().GetByUsername(username);
                if (user.HasValue)
                {
                    // 使用UserID作为文件夹编号，如果UserID在范围内则使用，否则使用UserID本身
                    int userId = user.Value.UserID;
                    if (userId >= 658 && userId <= 721)
                    {
                        return userId;
                    }
                    // 如果UserID不在范围内，仍然使用UserID（确保每个用户有独立文件夹）
                    return userId;
                }
            }
            catch
            {
                // 如果查询失败，使用用户名哈希值作为后备方案
            }

            // 后备方案：使用用户名的哈希值生成一个在合理范围内的编号
            int hash = Math.Abs(username.GetHashCode());
            return 658 + (hash % 64); // 生成658-721范围内的值
        }

        /// <summary>
        /// 获取用户录音文件夹路径
        /// </summary>
        private string GetUserRecordingFolder(string username)
        {
            int folderNumber = GetUserFolderNumber(username);
            string baseDir = AppContext.BaseDirectory;
            return Path.Combine(baseDir, "UI", "recorder", folderNumber.ToString());
        }

        /// <summary>
        /// 构建新的录音文件路径
        /// </summary>
        public string BuildNewFilePath(string username)
        {
            string folder = GetUserRecordingFolder(username);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(folder, $"recording_{timestamp}.wav");
        }

        /// <summary>
        /// 录音信息模型
        /// </summary>
        public class RecordingInfo
        {
            public string FilePath { get; set; } = "";
            public string FileName { get; set; } = "";
            public TimeSpan Duration { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        /// <summary>
        /// 列出用户的所有录音文件
        /// </summary>
        public IEnumerable<RecordingInfo> ListRecordings(string username)
        {
            string folder = GetUserRecordingFolder(username);
            
            // 确保文件夹存在
            if (!Directory.Exists(folder))
            {
                return Enumerable.Empty<RecordingInfo>();
            }

            var recordings = new List<RecordingInfo>();
            
            try
            {
                var files = Directory.GetFiles(folder, "*.wav", SearchOption.TopDirectoryOnly);
                
                foreach (var filePath in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(filePath);
                        if (!fileInfo.Exists) continue;

                        // 获取文件名（不含路径）
                        string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                        
                        // 尝试从文件名中提取时间戳，如果失败则使用文件创建时间
                        DateTime createdAt = fileInfo.CreationTime;
                        if (fileName.StartsWith("recording_") && fileName.Length >= 19)
                        {
                            string timeStr = fileName.Substring(10, 15); // "yyyyMMdd_HHmmss"
                            if (DateTime.TryParseExact(timeStr, "yyyyMMdd_HHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime parsed))
                            {
                                createdAt = parsed;
                            }
                        }

                        // 获取音频时长（简化版：使用文件大小估算，实际应该读取WAV文件头）
                        // 这里使用一个简单的估算：假设44.1kHz单声道16位，约10KB/秒
                        TimeSpan duration = EstimateDuration(fileInfo.Length);

                        recordings.Add(new RecordingInfo
                        {
                            FilePath = filePath,
                            FileName = fileInfo.Name,
                            Duration = duration,
                            CreatedAt = createdAt
                        });
                    }
                    catch
                    {
                        // 忽略无法读取的文件
                        continue;
                    }
                }

                // 按创建时间降序排列（最新的在前）
                return recordings.OrderByDescending(r => r.CreatedAt);
            }
            catch
            {
                return Enumerable.Empty<RecordingInfo>();
            }
        }

        /// <summary>
        /// 估算音频文件时长（简化版）
        /// 假设44.1kHz单声道16位WAV格式，约10KB/秒
        /// </summary>
        private TimeSpan EstimateDuration(long fileSizeBytes)
        {
            // WAV文件头约44字节，实际音频数据 = 文件大小 - 44
            long audioDataSize = Math.Max(0, fileSizeBytes - 44);
            // 44.1kHz * 1声道 * 2字节(16位) = 88200字节/秒
            double seconds = audioDataSize / 88200.0;
            return TimeSpan.FromSeconds(Math.Max(0, seconds));
        }

        /// <summary>
        /// 删除录音文件
        /// </summary>
        public void DeleteRecording(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException("录音文件不存在", filePath);
            }

            try
            {
                File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"删除录音文件失败：{ex.Message}", ex);
            }
        }

        /// <summary>
        /// 格式化时长显示
        /// </summary>
        public static string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours >= 1)
            {
                return $"{(int)duration.TotalHours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}";
            }
            return $"{duration.Minutes:D2}:{duration.Seconds:D2}";
        }
    }
}

