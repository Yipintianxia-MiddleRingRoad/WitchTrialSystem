using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using WitchTrialSystem.BLL;

namespace WitchTrialSystem.UI
{
    public partial class AIChatForm : Form
    {
        private AIService _aiService;
        private Panel chatPanel;
        private TextBox inputBox;
        private Button sendButton;
        private Button clearButton;
        private Label statusLabel;
        private bool _isProcessing = false;

        public AIChatForm(string apiKey, string modelId)
        {
            _aiService = new AIService(apiKey, modelId);
            InitializeComponent();
            BLL.IconHelper.SetFormIcon(this);  // 设置应用程序图标
            
            // 显示欢迎消息
            AddAIMessage("你好！我是魔女审判系统的智能助手。\n\n我已经学习了项目的所有文档，可以回答关于系统架构、数据库结构、权限体系等问题。\n\n有什么可以帮助你的吗？");
        }

        private void InitializeComponent()
        {
            this.Text = "智慧大模型 - 项目助手";
            this.Size = new Size(1400, 900);  // 更大的默认尺寸
            this.MinimumSize = new Size(1000, 700);  // 设置最小尺寸
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 245);

            // 使用TableLayoutPanel实现自适应布局
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(15)
            };
            
            // 设置行高：聊天区域自动填充，底部固定高度
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // 聊天区域
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F)); // 输入区域
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // 按钮区域
            
            this.Controls.Add(mainLayout);

            // 聊天显示区域
            chatPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            mainLayout.Controls.Add(chatPanel, 0, 0);

            // 输入区域容器
            var inputPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };
            mainLayout.Controls.Add(inputPanel, 0, 1);

            // 输入框
            inputBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                Font = new Font("微软雅黑", 11),
                PlaceholderText = "输入你的问题... (Ctrl+Enter 发送)",
                ScrollBars = ScrollBars.Vertical
            };
            inputBox.KeyDown += InputBox_KeyDown;
            inputPanel.Controls.Add(inputBox);

            // 发送按钮（右侧）
            sendButton = new Button
            {
                Dock = DockStyle.Right,
                Width = 100,
                Text = "发送",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            sendButton.Click += SendButton_Click;
            inputPanel.Controls.Add(sendButton);

            // 底部按钮区域
            var bottomPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };
            mainLayout.Controls.Add(bottomPanel, 0, 2);

            // 清空按钮
            clearButton = new Button
            {
                Width = 100,
                Height = 35,
                Text = "清空对话",
                Font = new Font("微软雅黑", 9),
                BackColor = Color.FromArgb(220, 220, 220),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            clearButton.Click += ClearButton_Click;
            bottomPanel.Controls.Add(clearButton);

            // 状态标签
            statusLabel = new Label
            {
                Text = _aiService.GetKnowledgeBaseStatus(),
                Font = new Font("微软雅黑", 9),
                ForeColor = Color.Gray,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = true,
                Padding = new Padding(15, 8, 0, 0)
            };
            bottomPanel.Controls.Add(statusLabel);
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+Enter 发送
            if (e.Control && e.KeyCode == Keys.Enter)
            {
                SendButton_Click(sender, e);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            if (_isProcessing) return;

            var userMessage = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(userMessage))
            {
                MessageBox.Show("请输入问题", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _isProcessing = true;
            sendButton.Enabled = false;
            inputBox.Enabled = false;

            // 显示用户消息
            AddUserMessage(userMessage);
            inputBox.Clear();

            // 显示加载提示
            var loadingLabel = AddLoadingMessage();

            try
            {
                // 调用AI服务
                var aiResponse = await _aiService.SendMessageAsync(userMessage);

                // 移除加载提示
                chatPanel.Controls.Remove(loadingLabel);

                // 显示AI回复
                AddAIMessage(aiResponse);
            }
            catch (Exception ex)
            {
                chatPanel.Controls.Remove(loadingLabel);
                AddAIMessage($"❌ 发生错误: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                sendButton.Enabled = true;
                inputBox.Enabled = true;
                inputBox.Focus();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要清空所有对话记录吗？",
                "确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                chatPanel.Controls.Clear();
                _aiService.ClearHistory();
                AddAIMessage("对话已清空。有什么可以帮助你的吗？");
            }
        }

        private void AddUserMessage(string message)
        {
            var messagePanel = CreateMessagePanel(message, true);
            chatPanel.Controls.Add(messagePanel);
            ScrollToBottom();
        }

        private void AddAIMessage(string message)
        {
            var messagePanel = CreateMessagePanel(message, false);
            chatPanel.Controls.Add(messagePanel);
            ScrollToBottom();
        }

        private Label AddLoadingMessage()
        {
            var loadingLabel = new Label
            {
                Text = "🤖 AI正在思考中...",
                AutoSize = true,
                Font = new Font("微软雅黑", 9),
                ForeColor = Color.Gray,
                Location = new Point(10, GetNextMessageY()),
                Padding = new Padding(10)
            };
            chatPanel.Controls.Add(loadingLabel);
            ScrollToBottom();
            return loadingLabel;
        }

        private Panel CreateMessagePanel(string message, bool isUser)
        {
            var panel = new Panel
            {
                AutoSize = true,
                MaximumSize = new Size(600, 0),
                Location = new Point(isUser ? 150 : 10, GetNextMessageY()),
                Padding = new Padding(10)
            };

            var label = new Label
            {
                Text = (isUser ? "👤 用户: " : "🤖 AI: ") + message,
                AutoSize = true,
                MaximumSize = new Size(580, 0),
                Font = new Font("微软雅黑", 10),
                BackColor = isUser ? Color.FromArgb(220, 240, 255) : Color.FromArgb(240, 255, 240),
                ForeColor = Color.Black,
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };

            panel.Controls.Add(label);
            panel.Height = label.Height + 20;

            return panel;
        }

        private int GetNextMessageY()
        {
            if (chatPanel.Controls.Count == 0)
                return 10;

            var lastControl = chatPanel.Controls[chatPanel.Controls.Count - 1];
            return lastControl.Bottom + 10;
        }

        private void ScrollToBottom()
        {
            chatPanel.AutoScrollPosition = new Point(0, chatPanel.AutoScrollMinSize.Height);
        }
    }
}
