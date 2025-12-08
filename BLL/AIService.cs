using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WitchTrialSystem.BLL
{
    /// <summary>
    /// AI服务 - 豆包大模型集成
    /// </summary>
    public class AIService
    {
        private readonly string _apiKey;
        private readonly string _modelId;
        private readonly HttpClient _httpClient;
        private List<ChatMessage> _conversationHistory;
        private string _knowledgeBase;

        public AIService(string apiKey, string modelId)
        {
            _apiKey = apiKey;
            _modelId = modelId;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
            _conversationHistory = new List<ChatMessage>();
            
            // 加载知识库
            LoadKnowledgeBase();
        }

        /// <summary>
        /// 加载项目文档作为知识库
        /// </summary>
        private void LoadKnowledgeBase()
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("# 魔女审判系统 - 项目知识库");
                sb.AppendLine();

                // 要加载的文档列表
                var docFiles = new[]
                {
                    "README.md",
                    "项目架构说明.md",
                    "数据库结构文档.md",
                    "系统界面跳转与权限层级说明.md",
                    "四层权限体系说明.md",
                    "CHANGELOG.md"
                };

                int loadedCount = 0;
                foreach (var file in docFiles)
                {
                    if (File.Exists(file))
                    {
                        sb.AppendLine($"## 文档: {file}");
                        sb.AppendLine();
                        sb.AppendLine(File.ReadAllText(file, Encoding.UTF8));
                        sb.AppendLine();
                        sb.AppendLine("---");
                        sb.AppendLine();
                        loadedCount++;
                    }
                }

                _knowledgeBase = sb.ToString();
                Console.WriteLine($"知识库加载完成，共加载 {loadedCount} 个文档");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载知识库失败: {ex.Message}");
                _knowledgeBase = "知识库加载失败";
            }
        }

        /// <summary>
        /// 发送消息到豆包AI
        /// </summary>
        public async Task<string> SendMessageAsync(string userMessage)
        {
            try
            {
                // 添加用户消息到历史
                _conversationHistory.Add(new ChatMessage
                {
                    Role = "user",
                    Content = userMessage
                });

                // 构建请求消息列表
                var messages = new List<ChatMessage>();

                // 系统提示词（包含知识库）
                messages.Add(new ChatMessage
                {
                    Role = "system",
                    Content = $@"你是魔女审判系统的智能助手。你的任务是根据以下项目文档回答用户的问题。

{_knowledgeBase}

请注意：
1. 优先使用上述文档中的信息回答问题
2. 如果文档中没有相关信息，可以基于常识回答，但要说明这不是文档中的内容
3. 回答要准确、简洁、友好
4. 可以使用表格、列表等格式让回答更清晰
5. 涉及技术细节时要准确引用文档内容"
                });

                // 添加对话历史（最近5轮）
                var recentHistory = _conversationHistory.TakeLast(10).ToList();
                messages.AddRange(recentHistory);

                // 构建请求体
                var requestBody = new
                {
                    model = _modelId,
                    messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToArray(),
                    temperature = 0.7,
                    max_tokens = 2000
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // 设置请求头
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                // 发送请求（豆包API端点）
                var response = await _httpClient.PostAsync(
                    "https://ark.cn-beijing.volces.com/api/v3/chat/completions",
                    content
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"API请求失败: {response.StatusCode}, {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                
                // 调试：输出原始响应
                Console.WriteLine("API响应: " + responseContent);
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<DouBaoResponse>(responseContent, options);

                var aiMessage = result?.Choices?[0]?.Message?.Content ?? "抱歉，我没有收到有效的回复";

                // 添加AI回复到历史
                _conversationHistory.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = aiMessage
                });

                return aiMessage;
            }
            catch (Exception ex)
            {
                return $"❌ 发生错误: {ex.Message}";
            }
        }

        /// <summary>
        /// 清空对话历史
        /// </summary>
        public void ClearHistory()
        {
            _conversationHistory.Clear();
        }

        /// <summary>
        /// 获取知识库状态
        /// </summary>
        public string GetKnowledgeBaseStatus()
        {
            if (string.IsNullOrEmpty(_knowledgeBase) || _knowledgeBase == "知识库加载失败")
            {
                return "❌ 知识库未加载";
            }

            var docCount = _knowledgeBase.Split("## 文档:").Length - 1;
            return $"✅ 已加载 {docCount} 个文档";
        }
    }

    /// <summary>
    /// 聊天消息
    /// </summary>
    public class ChatMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
    }

    /// <summary>
    /// 豆包API响应
    /// </summary>
    public class DouBaoResponse
    {
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        public Message Message { get; set; }
    }

    public class Message
    {
        public string Content { get; set; }
    }
}
