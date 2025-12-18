using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 简易聊天页面：
    /// 左侧：联系人列表
    /// 右上：与当前联系人的对话记录
    /// 右下：输入框 + 发送按钮
    /// </summary>
    public class ChatForm : Form
    {
        private readonly string _username;

        // 左侧联系人
        private readonly ListBox _lstContacts = new()
        {
            Dock = DockStyle.Left,
            Width = 220,
            BorderStyle = BorderStyle.None
        };

        // 右侧对话显示
        private readonly RichTextBox _txtConversation = new()
        {
            ReadOnly = true,
            BorderStyle = BorderStyle.None,
            BackColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            Dock = DockStyle.Fill
        };

        // 底部输入区
        private readonly TextBox _txtInput = new()
        {
            Multiline = false,
            BorderStyle = BorderStyle.FixedSingle
        };

        private readonly Button _btnSend = new()
        {
            Text = "发送",
            Width = 90,
            Height = 32,
            BackColor = Color.FromArgb(70, 130, 180),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        // 清空当前对话记录按钮
        private readonly Button _btnClearHistory = new()
        {
            Text = "清空记录",
            Width = 90,
            Height = 26,
            BackColor = Color.FromArgb(60, 60, 60),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };

        // 简单内存会话结构：联系人 -> 消息列表
        private class ChatMessage
        {
            public string Sender { get; set; } = "";
            public string Text { get; set; } = "";
            public DateTime Time { get; set; }
        }

        private readonly Dictionary<string, List<ChatMessage>> _conversations = new();
        private string? _currentContact;
        // 显示名 -> 用户名
        private readonly Dictionary<string, string> _contactUsernames = new();
        // 有“未读”消息高亮的联系人（使用显示名标识）
        private readonly HashSet<string> _unreadContacts = new();

        public ChatForm(string username)
        {
            _username = username;
            InitializeForm();
            SetupLayout();
            LoadContactsFromDatabase();
            HookEvents();

            if (_lstContacts.Items.Count > 0)
            {
                _lstContacts.SelectedIndex = 0;
            }
        }

        private void InitializeForm()
        {
            Text = $"聊天（当前用户：{_username}）";
            StartPosition = FormStartPosition.Manual;
            Width = 900;
            Height = 600;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(25, 25, 25);
            DoubleBuffered = true;

            // 初始位置：在屏幕中间基础上整体向右偏移一段，避免被其他界面左侧遮挡
            var wa = Screen.PrimaryScreen?.WorkingArea ?? new Rectangle(0, 0, 1920, 1080);
            int centerX = wa.Left + (wa.Width - Width) / 2;
            int centerY = wa.Top + (wa.Height - Height) / 2;
            Left = centerX + 150; // 向右移 150 像素
            Top = centerY;

            KeyPreview = true;
            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    Close();
            };
        }

        private void SetupLayout()
        {
            // 使用 SplitContainer 将窗体分成左右两个子区域：
            // 左侧：联系人列表；右侧：对话内容 + 输入区
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                IsSplitterFixed = true, // 锁定分割条，保持大致 1:2 比例
                BackColor = Color.FromArgb(25, 25, 25)
            };

            // 窗体/容器尺寸变化时，让左右宽度尽量保持“左 1/3、右 2/3”（并限制在允许范围内，避免异常）
            split.SizeChanged += (_, __) =>
            {
                // 宽度太小时不调整，避免计算越界
                if (split.Width <= split.Panel1MinSize + split.Panel2MinSize + split.SplitterWidth)
                    return;

                int target = split.Width / 3; // 左侧约占三分之一
                int min = split.Panel1MinSize;
                int max = split.Width - split.Panel2MinSize - split.SplitterWidth;
                if (max < min)
                    return;

                target = Math.Max(min, Math.Min(max, target));
                split.SplitterDistance = target;
            };

            // ================= 左侧：联系人“子窗体” =================
            var leftPanel = split.Panel1;
            leftPanel.BackColor = Color.FromArgb(35, 35, 35);
            leftPanel.Padding = new Padding(8);

            var lblContacts = new Label
            {
                Text = "联系人",
                Dock = DockStyle.Top,
                Height = 26,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            _lstContacts.BackColor = Color.FromArgb(45, 45, 45);
            _lstContacts.ForeColor = Color.White;
            _lstContacts.Dock = DockStyle.Fill;
            _lstContacts.BorderStyle = BorderStyle.None;
            _lstContacts.DrawMode = DrawMode.OwnerDrawFixed;
            _lstContacts.ItemHeight = 20;

            leftPanel.Controls.Add(_lstContacts);
            leftPanel.Controls.Add(lblContacts);

            // ================= 右侧：对话“子窗体” =================
            var rightPanel = split.Panel2;
            rightPanel.BackColor = Color.FromArgb(30, 30, 30);
            rightPanel.Padding = new Padding(8);

            // 对话标题 + 清空按钮 顶部区域
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.FromArgb(30, 30, 30)
            };

            var lblTitle = new Label
            {
                Text = "对话",
                Dock = DockStyle.Left,
                Width = 120,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            _btnClearHistory.Dock = DockStyle.Right;
            _btnClearHistory.FlatAppearance.BorderSize = 0;

            topPanel.Controls.Add(_btnClearHistory);
            topPanel.Controls.Add(lblTitle);

            // 底部输入区域
            var bottomPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 48,
                BackColor = Color.FromArgb(35, 35, 35)
            };

            _txtInput.Multiline = false;
            _txtInput.BorderStyle = BorderStyle.FixedSingle;
            _txtInput.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            _txtInput.Location = new Point(8, 10);
            _txtInput.Width = bottomPanel.Width - _btnSend.Width - 24;
            _txtInput.Height = 28;

            _btnSend.Text = "发送";
            _btnSend.Width = 90;
            _btnSend.Height = 32;
            _btnSend.BackColor = Color.FromArgb(70, 130, 180);
            _btnSend.ForeColor = Color.White;
            _btnSend.FlatStyle = FlatStyle.Flat;
            _btnSend.FlatAppearance.BorderSize = 0;
            _btnSend.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            _btnSend.Location = new Point(bottomPanel.Width - _btnSend.Width - 8, 8);

            bottomPanel.Controls.Add(_txtInput);
            bottomPanel.Controls.Add(_btnSend);
            bottomPanel.Resize += (_, __) =>
            {
                _txtInput.Width = bottomPanel.Width - _btnSend.Width - 24;
                _btnSend.Location = new Point(bottomPanel.Width - _btnSend.Width - 8, 8);
            };

            // 对话内容区域填充右侧子窗体
            _txtConversation.ReadOnly = true;
            _txtConversation.BorderStyle = BorderStyle.None;
            _txtConversation.BackColor = Color.FromArgb(30, 30, 30);
            _txtConversation.ForeColor = Color.White;
            _txtConversation.Dock = DockStyle.Fill;

            // Dock 顺序很重要：先 Bottom，再 Fill，最后 Top，避免聊天内容被标题栏遮挡
            rightPanel.Controls.Add(bottomPanel);
            rightPanel.Controls.Add(_txtConversation);
            rightPanel.Controls.Add(topPanel);

            // 最后把 SplitContainer 加到主窗体
            Controls.Add(split);
        }

        private void HookEvents()
        {
            _lstContacts.SelectedIndexChanged += (_, __) => ChangeContact();
            _btnSend.Click += (_, __) => SendMessage();
            _btnClearHistory.Click += (_, __) => ClearCurrentConversation();
            _lstContacts.DrawItem += LstContacts_DrawItem;
            _txtInput.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    SendMessage();
                }
            };
        }

        /// <summary>
        /// 从数据库加载联系人：当前登录账号所属的「同岛、同批次」的魔女
        /// </summary>
        private void LoadContactsFromDatabase()
        {
            try
            {
                // 1. 获取当前用户的 IslandID 和 BatchID
                const string sqlUser = @"
SELECT TOP 1 IslandID, BatchID
FROM wt.[User]
WHERE Username = @Username";

                var userDt = DBHelper.ExecDataTable(sqlUser,
                    new SqlParameter("@Username", _username));

                if (userDt.Rows.Count == 0)
                {
                    MessageBox.Show("未能获取当前用户的岛屿和批次信息，无法加载联系人。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var row = userDt.Rows[0];
                if (row.IsNull("IslandID") || row.IsNull("BatchID"))
                {
                    MessageBox.Show("当前用户未设置岛屿或批次，无法加载联系人。", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int islandId = Convert.ToInt32(row["IslandID"]);
                int batchId = Convert.ToInt32(row["BatchID"]);

                // 2. 查询同岛同批次的魔女（排除自己），按囚犯编号排序
                const string sqlContacts = @"
SELECT DISTINCT u.Username,
       ISNULL(w.PrisonerNo, u.Username)   AS PrisonerNo,
       ISNULL(w.Name, u.Username)         AS WitchName
FROM wt.[User] u
LEFT JOIN wt.UserWitch uw ON uw.UserID = u.UserID
LEFT JOIN wt.Witch w ON w.WitchID = uw.WitchID
WHERE u.RoleID = 4
  AND u.IslandID = @IslandID
  AND u.BatchID = @BatchID
  AND u.Username <> @Username
ORDER BY PrisonerNo, WitchName";

                var contactsDt = DBHelper.ExecDataTable(sqlContacts,
                    new SqlParameter("@IslandID", islandId),
                    new SqlParameter("@BatchID", batchId),
                    new SqlParameter("@Username", _username));

                _lstContacts.Items.Clear();
                _conversations.Clear();
                _contactUsernames.Clear();

                foreach (DataRow r in contactsDt.Rows)
                {
                    string username = Convert.ToString(r["Username"]) ?? "";
                    string prisonerNo = Convert.ToString(r["PrisonerNo"]) ?? "";
                    string witchName = Convert.ToString(r["WitchName"]) ?? username;

                    string display = string.IsNullOrWhiteSpace(prisonerNo)
                        ? witchName
                        : $"{prisonerNo} - {witchName}";

                    _lstContacts.Items.Add(display);
                    _conversations[display] = new List<ChatMessage>();
                    _contactUsernames[display] = username;

                    // 根据本地文件推断是否存在“对方最近发来的消息”，用于高亮联系人
                    UpdateUnreadFlagFromFile(display);
                }

                if (_lstContacts.Items.Count == 0)
                {
                    _txtConversation.Text = "当前岛屿、本批次中没有可聊天的其他魔女。";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载联系人失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ChangeContact()
        {
            if (_lstContacts.SelectedItem is not string contact)
                return;

            _currentContact = contact;
            // 打开对话时，认为“未读”已查看，取消高亮
            if (_unreadContacts.Remove(contact))
            {
                _lstContacts.Invalidate();
            }
            EnsureConversationLoaded(contact);
            RenderConversation();
        }

        private void RenderConversation()
        {
            _txtConversation.Clear();
            if (_currentContact == null) return;

            if (!_conversations.TryGetValue(_currentContact, out var list))
                return;

            foreach (var msg in list.OrderBy(m => m.Time))
            {
                AppendMessageToView(msg);
            }
            _txtConversation.SelectionStart = _txtConversation.TextLength;
            _txtConversation.ScrollToCaret();
        }

        private void AppendMessageToView(ChatMessage msg)
        {
            var isMe = string.Equals(msg.Sender, _username, StringComparison.OrdinalIgnoreCase);
            var timeStr = msg.Time.ToString("HH:mm");
            var header = isMe ? $"我 ({timeStr})" : $"{msg.Sender} ({timeStr})";

            // 第一行先输出“我/对方 + 时间”标题，再输出消息正文
            _txtConversation.SelectionColor = isMe ? Color.DeepSkyBlue : Color.Gold;
            _txtConversation.AppendText(header + ": ");

            _txtConversation.SelectionColor = Color.White;
            _txtConversation.AppendText(msg.Text + Environment.NewLine + Environment.NewLine);
        }

        private void SendMessage()
        {
            if (_currentContact == null)
            {
                MessageBox.Show("请先在左侧选择一个聊天对象。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var text = _txtInput.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (!_conversations.TryGetValue(_currentContact, out var list))
            {
                list = new List<ChatMessage>();
                _conversations[_currentContact] = list;
            }

            // 我的消息
            var myMsg = new ChatMessage
            {
                Sender = _username,
                Text = text,
                Time = DateTime.Now
            };
            list.Add(myMsg);
            AppendMessageToView(myMsg);
            SaveMessageToFile(_currentContact, myMsg);
            _txtInput.Clear();

            _txtConversation.SelectionStart = _txtConversation.TextLength;
            _txtConversation.ScrollToCaret();
        }

        #region 本地文件存储相关

        /// <summary>
        /// 某个“拥有者用户”与“对端用户”的会话文件路径：
        /// 形如：Data\ChatLogs\{ownerUsername}\{peerUsername}.txt
        /// </summary>
        private string GetConversationFilePath(string ownerUsername, string peerUsername)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dataDir = Path.Combine(baseDir, "Data", "ChatLogs", ownerUsername);
            Directory.CreateDirectory(dataDir);

            string safePeer = peerUsername;
            // 去掉路径中不允许的字符
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                safePeer = safePeer.Replace(c, '_');
            }

            return Path.Combine(dataDir, safePeer + ".txt");
        }

        /// <summary>
        /// 当前用户 + 联系人显示名 -> 本地聊天记录文件路径
        /// 形如：Data\ChatLogs\{当前用户名}\{对方用户名}.txt
        /// </summary>
        private string GetConversationFilePath(string contactDisplayName)
        {
            string contactUsername = TryGetContactUsername(contactDisplayName) ?? contactDisplayName;
            return GetConversationFilePath(_username, contactUsername);
        }

        private string? TryGetContactUsername(string displayName)
        {
            if (_contactUsernames.TryGetValue(displayName, out var u) && !string.IsNullOrWhiteSpace(u))
                return u;
            return null;
        }

        /// <summary>
        /// 将单条消息追加写入本地文件
        /// </summary>
        private void SaveMessageToFile(string contactDisplayName, ChatMessage msg)
        {
            try
            {
                string path = GetConversationFilePath(contactDisplayName);
                // 使用 ISO 8601 格式时间，字段之间用制表符分隔
                string safeText = msg.Text
                    .Replace("\r", "\\r")
                    .Replace("\n", "\\n");
                string line = $"{msg.Time.ToString("o", CultureInfo.InvariantCulture)}\t{msg.Sender}\t{safeText}";

                // 1）写入“我”这边看到的记录文件：Data\ChatLogs\{_username}\{对方用户名}.txt
                File.AppendAllText(path, line + Environment.NewLine);

                // 2）同时镜像一份到“对方用户”的目录，便于对方登录后也能看到历史记录
                string? contactUsername = TryGetContactUsername(contactDisplayName);
                if (!string.IsNullOrWhiteSpace(contactUsername))
                {
                    string mirrorPath = GetConversationFilePath(contactUsername, _username);
                    File.AppendAllText(mirrorPath, line + Environment.NewLine);
                }
            }
            catch
            {
                // 文件写入失败不阻塞聊天，仅忽略
            }
        }

        /// <summary>
        /// 保证内存中存在该联系人的会话列表，如果没有则从本地文件加载
        /// </summary>
        private void EnsureConversationLoaded(string contactDisplayName)
        {
            if (_conversations.TryGetValue(contactDisplayName, out var list) && list.Count > 0)
                return;

            var loaded = LoadConversationFromFile(contactDisplayName);
            _conversations[contactDisplayName] = loaded;
        }

        /// <summary>
        /// 从本地文件读取会话记录
        /// </summary>
        private List<ChatMessage> LoadConversationFromFile(string contactDisplayName)
        {
            var result = new List<ChatMessage>();
            try
            {
                string path = GetConversationFilePath(contactDisplayName);
                if (!File.Exists(path))
                    return result;

                foreach (var line in File.ReadAllLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split('\t');
                    if (parts.Length < 3)
                        continue;

                    if (!DateTime.TryParseExact(parts[0], "o", CultureInfo.InvariantCulture,
                            DateTimeStyles.RoundtripKind, out var time))
                        time = DateTime.Now;

                    string sender = parts[1];
                    string text = parts[2]
                        .Replace("\\r", "\r")
                        .Replace("\\n", "\n");

                    result.Add(new ChatMessage
                    {
                        Sender = sender,
                        Text = text,
                        Time = time
                    });
                }
            }
            catch
            {
                // 读取失败同样忽略，返回空列表
            }

            return result;
        }

        /// <summary>
        /// 根据本地文件的最后一条消息，判断该联系人是否有“对方发来的未读消息”
        /// </summary>
        private void UpdateUnreadFlagFromFile(string contactDisplayName)
        {
            try
            {
                string path = GetConversationFilePath(contactDisplayName);
                if (!File.Exists(path))
                    return;

                // 只看最后一行，避免大文件全部读取
                string? lastLine = File.ReadLines(path).LastOrDefault(l => !string.IsNullOrWhiteSpace(l));
                if (string.IsNullOrWhiteSpace(lastLine))
                    return;

                var parts = lastLine.Split('\t');
                if (parts.Length < 3)
                    return;

                string sender = parts[1];
                if (!string.Equals(sender, _username, StringComparison.OrdinalIgnoreCase))
                {
                    _unreadContacts.Add(contactDisplayName);
                }
            }
            catch
            {
                // 忽略异常
            }
        }

        /// <summary>
        /// 清空当前联系人的聊天记录（仅删除本地当前用户这边的文件和内存）
        /// </summary>
        private void ClearCurrentConversation()
        {
            if (_currentContact == null)
                return;

            var confirm = MessageBox.Show(
                "确定要清空当前对话记录吗？\n此操作仅影响你这边看到的记录，不会删除对方的记录。",
                "确认删除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                if (_conversations.TryGetValue(_currentContact, out var list))
                {
                    list.Clear();
                }

                string path = GetConversationFilePath(_currentContact);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                _txtConversation.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"删除聊天记录失败：{ex.Message}", "错误",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        /// <summary>
        /// 左侧联系人自绘：有未读消息的联系人用高亮颜色显示
        /// </summary>
        private void LstContacts_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _lstContacts.Items.Count)
                return;

            string text = _lstContacts.Items[e.Index]?.ToString() ?? string.Empty;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color backColor = selected ? Color.FromArgb(70, 130, 180) : Color.FromArgb(45, 45, 45);
            Color foreColor;

            if (selected)
            {
                foreColor = Color.White;
            }
            else if (_unreadContacts.Contains(text))
            {
                // 有未读消息的联系人用金色高亮
                foreColor = Color.Gold;
            }
            else
            {
                foreColor = Color.White;
            }

            using (var backBrush = new SolidBrush(backColor))
            using (var foreBrush = new SolidBrush(foreColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                var format = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                e.Graphics.DrawString(text, _lstContacts.Font, foreBrush, e.Bounds, format);
            }

            e.DrawFocusRectangle();
        }
    }
}


