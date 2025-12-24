using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 魔女详细信息窗口 - A4档案版式
    /// 显示单个魔女的完整档案信息，支持打印和导出
    /// </summary>
    public class WitchDetailForm : Form
    {
        private readonly int _witchId;
        private readonly WitchDAL _dal = new();
        private DataRow? _witchData;

        // UI控件
        private readonly Panel _a4Panel = new();  // A4纸张容器
        private readonly Panel _contentPanel = new();  // 内容面板
        private readonly PictureBox _avatar = new();
        private readonly Label _lblTitle = new();
        private readonly Button _btnClose = new();
        private readonly Button _btnPrint = new();
        private readonly Button _btnExportWord = new();
        private readonly Button _btnExportPdf = new();

        public WitchDetailForm(int witchId)
        {
            _witchId = witchId;
            InitializeForm();
            LoadData();
            BuildUI();
        }

        private void InitializeForm()
        {
            Text = "魔女详细档案 - A4版式";
            WindowState = FormWindowState.Maximized;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            MinimizeBox = true;
            BackColor = Color.FromArgb(200, 200, 200);  // 灰色背景，衬托白色A4纸
            
            // 设置应用程序图标
            IconHelper.SetFormIcon(this);
        }

        private void LoadData()
        {
            var dt = _dal.GetWitchDetail(_witchId);
            if (dt.Rows.Count > 0)
            {
                _witchData = dt.Rows[0];
                Text = $"魔女详细档案 - {GetString("Name")}";
            }
        }

        private void BuildUI()
        {
            if (_witchData == null)
            {
                var lblError = new Label
                {
                    Text = "未找到魔女信息",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("微软雅黑", 14)
                };
                Controls.Add(lblError);
                return;
            }

            // 工具栏
            var toolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(52, 73, 94),
                Padding = new Padding(20, 10, 20, 10)
            };

            // 标题（左侧）
            _lblTitle.Text = $"魔女档案 - {GetString("Name")} ({GetString("PrisonerNo")})";
            _lblTitle.Font = new Font("微软雅黑", 14, FontStyle.Bold);
            _lblTitle.ForeColor = Color.White;
            _lblTitle.AutoSize = true;
            _lblTitle.Location = new Point(20, 18);
            toolbar.Controls.Add(_lblTitle);

            // 按钮组（右侧，从右往左排列）
            _btnClose.Text = "✖ 关闭";
            _btnClose.Size = new Size(90, 35);
            _btnClose.ForeColor = Color.White;
            _btnClose.BackColor = Color.FromArgb(192, 57, 43);
            _btnClose.FlatStyle = FlatStyle.Flat;
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnClose.Click += (s, e) => Close();
            toolbar.Controls.Add(_btnClose);

            _btnExportWord.Text = "📄 导出HTML";
            _btnExportWord.Size = new Size(160, 35);
            _btnExportWord.Font = new Font("微软雅黑", 10);
            _btnExportWord.ForeColor = Color.White;
            _btnExportWord.BackColor = Color.FromArgb(39, 174, 96);
            _btnExportWord.FlatStyle = FlatStyle.Flat;
            _btnExportWord.FlatAppearance.BorderSize = 0;
            _btnExportWord.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnExportWord.Click += BtnExportWord_Click;
            toolbar.Controls.Add(_btnExportWord);

            _btnPrint.Text = "🖨️ 打印/导出PDF";
            _btnPrint.Size = new Size(180, 35);
            _btnPrint.Font = new Font("微软雅黑", 10);
            _btnPrint.ForeColor = Color.White;
            _btnPrint.BackColor = Color.FromArgb(52, 152, 219);
            _btnPrint.FlatStyle = FlatStyle.Flat;
            _btnPrint.FlatAppearance.BorderSize = 0;
            _btnPrint.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnPrint.Click += BtnPrint_Click;
            toolbar.Controls.Add(_btnPrint);

            // 窗口大小改变时重新定位按钮
            toolbar.Resize += (s, e) =>
            {
                int rightX = toolbar.Width - 20;
                _btnClose.Location = new Point(rightX - 90, 12);
                rightX -= 100;
                _btnExportWord.Location = new Point(rightX - 160, 12);
                rightX -= 170;
                _btnPrint.Location = new Point(rightX - 180, 12);
            };

            Controls.Add(toolbar);

            // 滚动容器
            var scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(200, 200, 200)
            };

            // A4纸张容器 (210mm × 297mm = 794px × 1123px at 96 DPI)
            _a4Panel.Width = 794;
            _a4Panel.Height = 1123;
            _a4Panel.BackColor = Color.White;
            _a4Panel.Padding = new Padding(40, 40, 40, 40);  // A4边距
            
            // 内容面板（向下移动以避免被工具栏遮挡）
            _contentPanel.Location = new Point(40, 40);  // 从(0,0)改为(40,40)，使用A4边距
            _contentPanel.Width = 794 - 80;  // A4宽度减去左右边距
            _contentPanel.BackColor = Color.White;
            _contentPanel.AutoScroll = false;
            
            BuildA4Content();
            
            _a4Panel.Controls.Add(_contentPanel);
            scrollPanel.Controls.Add(_a4Panel);
            
            // 窗口大小改变时重新居中A4纸张
            scrollPanel.Resize += (s, e) =>
            {
                _a4Panel.Location = new Point(
                    Math.Max(20, (scrollPanel.Width - _a4Panel.Width) / 2),
                    30
                );
            };
            
            Controls.Add(scrollPanel);
        }

        private void BuildA4Content()
        {
            int y = 0;  // 从0开始，因为内容面板已经有边距了
            int contentWidth = 714;  // A4宽度(794) - 左右边距(80) = 714px

            // ========== 档案标题（压缩版）==========
            var title = new Label
            {
                Text = "魔女审判系统 · 个人档案",
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 32,  // 从40压缩到32
                Font = new Font("黑体", 16, FontStyle.Bold),  // 从18压缩到16
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            _contentPanel.Controls.Add(title);
            y += 38;  // 从50压缩到38

            // 分隔线
            AddSeparator(_contentPanel, ref y, contentWidth);

            // ========== 第一部分：头像和核心信息（横向布局，压缩版）==========
            var topSection = new Panel
            {
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 150,  // 从200压缩到150
                BackColor = Color.White
            };

            // 左侧：头像（压缩）
            _avatar.Size = new Size(130, 130);  // 从160压缩到130
            _avatar.Location = new Point(10, 10);  // 调整位置
            _avatar.SizeMode = PictureBoxSizeMode.Zoom;
            _avatar.BorderStyle = BorderStyle.FixedSingle;
            LoadAvatar();
            topSection.Controls.Add(_avatar);

            // 右侧：核心信息（两列布局，压缩）
            int infoX = 155;  // 从200调整到155
            int infoY = 10;  // 从20调整到10
            int col1Width = 280;  // 从250调整到280
            int col2Width = 280;

            // 第一列
            AddInfoLine(topSection, ref infoY, infoX, "囚人番号", GetString("PrisonerNo"), true);
            AddInfoLine(topSection, ref infoY, infoX, "个人番号", GetString("PersonalNo"));
            AddInfoLine(topSection, ref infoY, infoX, "姓名", GetString("Name"), true);
            AddInfoLine(topSection, ref infoY, infoX, "曾用名", GetString("FormerName"));
            AddInfoLine(topSection, ref infoY, infoX, "性别", GetString("Gender"));

            // 第二列
            infoY = 10;  // 从20调整到10
            infoX = 155 + col1Width;
            AddInfoLine(topSection, ref infoY, infoX, "出生日期", GetDate("BirthDate"));
            AddInfoLine(topSection, ref infoY, infoX, "年龄", GetString("Age") + " 岁");
            AddInfoLine(topSection, ref infoY, infoX, "民族", GetString("Ethnicity"));
            AddInfoLine(topSection, ref infoY, infoX, "籍贯", GetString("Birthplace"));
            AddInfoLine(topSection, ref infoY, infoX, "状态", GetString("Status"), true);

            _contentPanel.Controls.Add(topSection);
            y += 160;  // 从210压缩到160

            // ========== 第二部分：身体特征和魔法能力 ==========
            AddSectionTitle(_contentPanel, ref y, "身体特征与能力", contentWidth);
            
            var physicalPanel = CreateInfoGrid(new[]
            {
                ("身高", GetDecimal("Height") + " cm"),
                ("体重", GetDecimal("Weight") + " kg"),
                ("血型", GetString("BloodType")),
                ("魔法", GetString("Magic")),
                ("处刑结果", GetString("ExecutionResult"))
            }, 3);  // 3列布局
            physicalPanel.Location = new Point(0, y);
            physicalPanel.Width = contentWidth;
            _contentPanel.Controls.Add(physicalPanel);
            y += physicalPanel.Height + 10;  // 从15压缩到10

            // ========== 第三部分：联系方式 ==========
            AddSectionTitle(_contentPanel, ref y, "联系方式", contentWidth);
            
            var contactPanel = CreateInfoGrid(new[]
            {
                ("地址", GetString("Address")),
                ("电话", GetString("Phone")),
                ("邮箱", GetString("Email")),
                ("LINE账号", GetString("LineAccount"))
            }, 2);  // 2列布局
            contactPanel.Location = new Point(0, y);
            contactPanel.Width = contentWidth;
            _contentPanel.Controls.Add(contactPanel);
            y += contactPanel.Height + 10;  // 从15压缩到10

            // ========== 第四部分：教育背景 ==========
            AddSectionTitle(_contentPanel, ref y, "教育背景", contentWidth);
            AddTextBlock(_contentPanel, ref y, "最高学历", GetString("HighestEducation"), contentWidth);
            AddEducationHistoryTwoColumns(_contentPanel, ref y, contentWidth);

            // ========== 第五部分：家庭关系 ==========
            AddSectionTitle(_contentPanel, ref y, "家庭关系", contentWidth);
            AddTextBlock(_contentPanel, ref y, "家庭结构", GetString("FamilyStructure"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "父亲", GetString("Father"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "母亲", GetString("Mother"), contentWidth);
            
            var other1 = GetString("OtherFamily1");
            if (!string.IsNullOrEmpty(other1) && other1 != "无")
                AddTextBlock(_contentPanel, ref y, "其他成员", other1, contentWidth);

            // ========== 第六部分：个性特征 ==========
            AddSectionTitle(_contentPanel, ref y, "个性特征", contentWidth);
            AddTextBlock(_contentPanel, ref y, "技能/特长", GetString("Skills"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "兴趣爱好", GetString("Hobbies"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "理想", GetString("Dreams"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "讨厌的事物", GetString("Dislikes"), contentWidth);

            // ========== 第七部分：心理创伤（特殊样式）==========
            AddSectionTitle(_contentPanel, ref y, "心理创伤", contentWidth, Color.FromArgb(180, 0, 0));
            AddTextBlock(_contentPanel, ref y, "创伤描述", GetString("Trauma"), contentWidth, Color.FromArgb(255, 240, 240));

            // ========== 第八部分：魔女相关 ==========
            AddSectionTitle(_contentPanel, ref y, "魔女相关信息", contentWidth);
            AddTextBlock(_contentPanel, ref y, "魔女化办法", GetString("WitchTransformMethod"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "公开描述", GetString("DescriptionPublic"), contentWidth);

            // ========== 页脚 ==========
            y += 15;  // 从20压缩到15
            var footer = new Label
            {
                Text = $"档案编号：{GetString("PrisonerNo")} | 生成日期：{DateTime.Now:yyyy-MM-dd HH:mm}",
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 18,  // 从20压缩到18
                Font = new Font("微软雅黑", 7.5f),  // 从8压缩到7.5
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            _contentPanel.Controls.Add(footer);
            y += 25;  // 从30压缩到25

            // 设置内容面板高度
            _contentPanel.Height = y;
            
            // 尝试将内容压缩到一页A4（1123px），如果超出则允许扩展
            int targetHeight = 1123 - 80;  // A4高度减去上下边距
            if (y > targetHeight)
            {
                // 内容超出一页，允许扩展
                _a4Panel.Height = y + 80;
            }
            else
            {
                // 内容在一页内，固定为A4高度
                _a4Panel.Height = 1123;
            }
        }



        private void LoadAvatar()
        {
            try
            {
                string? avatarPath = GetString("AvatarPath");
                string placeholder = Path.Combine(AppContext.BaseDirectory, "Images", "_placeholder.png");

                string? resolved = null;
                if (!string.IsNullOrWhiteSpace(avatarPath))
                {
                    resolved = Path.IsPathRooted(avatarPath)
                        ? avatarPath
                        : Path.Combine(AppContext.BaseDirectory, avatarPath);
                }

                if (!string.IsNullOrEmpty(resolved) && File.Exists(resolved))
                {
                    _avatar.Image = Image.FromFile(resolved);
                }
                else if (File.Exists(placeholder))
                {
                    _avatar.Image = Image.FromFile(placeholder);
                }
            }
            catch
            {
                // 忽略图片加载异常
            }
        }

        private string ParseEducationHistory()
        {
            string json = GetString("EducationHistory");
            if (string.IsNullOrEmpty(json) || json == "[]") return "无";

            try
            {
                // 首先尝试使用System.Text.Json解析
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    AllowTrailingCommas = true,
                    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip
                };
                var records = System.Text.Json.JsonSerializer.Deserialize<List<EducationRecord>>(json, options);
                if (records == null || records.Count == 0) return "无";

                var result = new System.Text.StringBuilder();
                int index = 1;
                
                foreach (var record in records)
                {
                    result.AppendLine($"【{index}】");
                    if (!string.IsNullOrEmpty(record.School))
                        result.AppendLine($"  学校：{record.School}");
                    if (!string.IsNullOrEmpty(record.Degree))
                        result.AppendLine($"  学位：{record.Degree}");
                    if (!string.IsNullOrEmpty(record.Status))
                        result.AppendLine($"  状态：{record.Status}");
                    if (!string.IsNullOrEmpty(record.SpecialNote))
                        result.AppendLine($"  备注：{record.SpecialNote}");
                    result.AppendLine();
                    index++;
                }
                
                return result.ToString().Trim();
            }
            catch (System.Text.Json.JsonException jsonEx)
            {
                // System.Text.Json解析失败，尝试使用Newtonsoft.Json
                try
                {
                    var records = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EducationRecord>>(json);
                    if (records == null || records.Count == 0) return "无";

                    var result = new System.Text.StringBuilder();
                    int index = 1;
                    
                    foreach (var record in records)
                    {
                        result.AppendLine($"【{index}】");
                        if (!string.IsNullOrEmpty(record.School))
                            result.AppendLine($"  学校：{record.School}");
                        if (!string.IsNullOrEmpty(record.Degree))
                            result.AppendLine($"  学位：{record.Degree}");
                        if (!string.IsNullOrEmpty(record.Status))
                            result.AppendLine($"  状态：{record.Status}");
                        if (!string.IsNullOrEmpty(record.SpecialNote))
                            result.AppendLine($"  备注：{record.SpecialNote}");
                        result.AppendLine();
                        index++;
                    }
                    
                    return result.ToString().Trim();
                }
                catch (Exception newtonsoftEx)
                {
                    // 两种解析器都失败，返回详细错误信息
                    // 尝试清理JSON中的特殊字符
                    string cleanedJson = json.Replace("'", "'").Replace("'", "'").Replace(""", "\"").Replace(""", "\"");
                    
                    return $"[教育经历解析失败]\n" +
                           $"System.Text.Json错误：{jsonEx.Message}\n" +
                           $"Newtonsoft.Json错误：{newtonsoftEx.Message}\n\n" +
                           $"JSON长度：{json.Length} 字符\n" +
                           $"前100字符：{(json.Length > 100 ? json.Substring(0, 100) : json)}\n\n" +
                           $"建议：请在Admin界面重新编辑并保存此魔女的教育经历";
                }
            }
            catch (Exception ex)
            {
                // 其他异常
                return $"[教育经历解析失败：{ex.GetType().Name}]\n" +
                       $"错误信息：{ex.Message}\n\n" +
                       $"JSON长度：{json.Length} 字符\n" +
                       $"建议：请在Admin界面重新编辑并保存此魔女的教育经历";
            }
        }

        private string ParseWorkHistory()
        {
            string json = GetString("WorkHistory");
            if (string.IsNullOrEmpty(json) || json == "[]") return "无";

            try
            {
                // 字段名中英文对照
                var fieldNames = new Dictionary<string, string>
                {
                    { "Company", "公司" },
                    { "Position", "职位" },
                    { "StartDate", "开始时间" },
                    { "EndDate", "结束时间" },
                    { "Department", "部门" },
                    { "Responsibilities", "职责" },
                    { "Achievements", "成就" },
                    { "Salary", "薪资" },
                    { "ReasonForLeaving", "离职原因" }
                };

                // 简单解析JSON
                json = json.Replace("[", "").Replace("]", "");
                var items = json.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                var result = new System.Text.StringBuilder();
                
                int index = 1;
                foreach (var item in items)
                {
                    var cleanItem = item.Replace("{", "").Replace("}", "").Replace("\"", "");
                    var fields = cleanItem.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    result.AppendLine($"【{index}】");
                    
                    foreach (var field in fields)
                    {
                        var parts = field.Split(new[] { ':' }, 2);
                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();
                            
                            // 翻译字段名
                            var displayName = fieldNames.ContainsKey(key) ? fieldNames[key] : key;
                            result.AppendLine($"  {displayName}：{value}");
                        }
                    }
                    result.AppendLine();
                    index++;
                }
                
                return result.ToString().Trim();
            }
            catch
            {
                return json;
            }
        }

        // ========== A4版式辅助方法 ==========
        
        private void AddSeparator(Panel parent, ref int y, int width)
        {
            var line = new Panel
            {
                Location = new Point(0, y),
                Width = width,
                Height = 2,
                BackColor = Color.FromArgb(52, 73, 94)
            };
            parent.Controls.Add(line);
            y += 10;
        }

        private void AddSectionTitle(Panel parent, ref int y, string title, int width, Color? color = null)
        {
            var lbl = new Label
            {
                Text = title,
                Location = new Point(0, y),
                Width = width,
                Height = 24,  // 从30压缩到24
                Font = new Font("微软雅黑", 10, FontStyle.Bold),  // 从11压缩到10
                ForeColor = color ?? Color.FromArgb(52, 73, 94),
                BackColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 0, 0)  // 从10压缩到8
            };
            parent.Controls.Add(lbl);
            y += 28;  // 从35压缩到28
        }

        private void AddInfoLine(Panel parent, ref int y, int x, string label, string value, bool bold = false)
        {
            if (string.IsNullOrEmpty(value) || value == "无") return;

            var lbl = new Label
            {
                Text = $"{label}：{value}",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("微软雅黑", 8.5f, bold ? FontStyle.Bold : FontStyle.Regular)  // 从9压缩到8.5
            };
            parent.Controls.Add(lbl);
            y += 22;  // 从25压缩到22
        }

        private Panel CreateInfoGrid((string label, string value)[] items, int columns)
        {
            var panel = new Panel
            {
                Width = 714,  // 固定宽度
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)  // 从15压缩到10
            };

            int itemWidth = (714 - 20) / columns;  // 从30调整到20
            int x = 8, y = 8;  // 从10调整到8
            int col = 0;

            foreach (var (label, value) in items)
            {
                if (string.IsNullOrEmpty(value) || value == "无") continue;

                var lbl = new Label
                {
                    Text = $"{label}：{value}",
                    Location = new Point(x, y),
                    Width = itemWidth - 8,  // 从10调整到8
                    Height = 20,  // 从25压缩到20
                    Font = new Font("微软雅黑", 8.5f)  // 从9压缩到8.5
                };
                panel.Controls.Add(lbl);

                col++;
                if (col >= columns)
                {
                    col = 0;
                    x = 8;
                    y += 24;  // 从30压缩到24
                }
                else
                {
                    x += itemWidth;
                }
            }

            panel.Height = y + 30;  // 从40压缩到30
            return panel;
        }

        private void AddTextBlock(Panel parent, ref int y, string label, string value, int width, Color? bgColor = null)
        {
            if (string.IsNullOrEmpty(value) || value == "无") return;

            var lblLabel = new Label
            {
                Text = label + "：",
                Location = new Point(0, y),
                Width = width,
                Height = 18,  // 从20压缩到18
                Font = new Font("微软雅黑", 8.5f, FontStyle.Bold),  // 从9压缩到8.5
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            parent.Controls.Add(lblLabel);
            y += 20;  // 从22压缩到20

            var lblValue = new Label
            {
                Text = value,
                Location = new Point(8, y),  // 从10调整到8
                Width = width - 16,  // 从20调整到16
                AutoSize = false,
                Height = 0,  // 先设为0
                Font = new Font("微软雅黑", 8.5f),  // 从9压缩到8.5
                BackColor = bgColor ?? Color.FromArgb(250, 250, 250),
                Padding = new Padding(8),  // 从10压缩到8
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // 计算实际需要的高度
            using (var g = lblValue.CreateGraphics())
            {
                var size = g.MeasureString(value, lblValue.Font, lblValue.Width - 16);
                lblValue.Height = Math.Max(24, (int)size.Height + 16);  // 最小高度24，从20调整到16
            }
            
            parent.Controls.Add(lblValue);
            y += lblValue.Height + 8;  // 从10压缩到8
        }

        /// <summary>
        /// 添加教育经历（两列布局）
        /// </summary>
        private void AddEducationHistoryTwoColumns(Panel parent, ref int y, int width)
        {
            string json = GetString("EducationHistory");
            if (string.IsNullOrEmpty(json) || json == "[]") return;

            try
            {
                // 使用System.Text.Json正确解析，不区分大小写
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var records = System.Text.Json.JsonSerializer.Deserialize<List<EducationRecord>>(json, options);
                if (records == null || records.Count == 0) return;
                
                // 标题
                var lblTitle = new Label
                {
                    Text = "教育经历：",
                    Location = new Point(0, y),
                    Width = width,
                    Height = 18,
                    Font = new Font("微软雅黑", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 100, 100)
                };
                parent.Controls.Add(lblTitle);
                y += 20;

                int index = 1;
                foreach (var record in records)
                {
                    // 创建字段字典
                    var fieldDict = new Dictionary<string, string>();
                    if (!string.IsNullOrEmpty(record.School)) fieldDict["学校"] = record.School;
                    if (!string.IsNullOrEmpty(record.Degree)) fieldDict["学位"] = record.Degree;
                    if (!string.IsNullOrEmpty(record.Status)) fieldDict["状态"] = record.Status;
                    if (!string.IsNullOrEmpty(record.SpecialNote)) fieldDict["备注"] = record.SpecialNote;

                    // 创建面板容器
                    var panel = new Panel
                    {
                        Location = new Point(8, y),
                        Width = width - 16,
                        BackColor = Color.FromArgb(250, 250, 250),
                        BorderStyle = BorderStyle.FixedSingle,
                        Padding = new Padding(8)
                    };

                    int panelY = 8;
                    int columnWidth = (panel.Width - 24) / 2;  // 两列，中间留间隔

                    // 标题【1】【2】
                    var lblIndex = new Label
                    {
                        Text = $"【{index}】",
                        Location = new Point(8, panelY),
                        Width = panel.Width - 16,
                        Height = 20,
                        Font = new Font("微软雅黑", 9f, FontStyle.Bold),
                        ForeColor = Color.FromArgb(50, 50, 50)
                    };
                    panel.Controls.Add(lblIndex);
                    panelY += 22;

                    // 分成两列显示
                    int leftY = panelY;
                    int rightY = panelY;
                    int fieldIndex = 0;

                    foreach (var kvp in fieldDict)
                    {
                        bool isLeftColumn = (fieldIndex % 2 == 0);
                        int x = isLeftColumn ? 8 : (columnWidth + 16);
                        int currentY = isLeftColumn ? leftY : rightY;

                        var lblField = new Label
                        {
                            Text = $"{kvp.Key}：{kvp.Value}",
                            Location = new Point(x, currentY),
                            Width = columnWidth,
                            AutoSize = false,
                            Font = new Font("微软雅黑", 8.5f),
                            ForeColor = Color.FromArgb(60, 60, 60)
                        };

                        // 计算高度
                        using (var g = lblField.CreateGraphics())
                        {
                            var size = g.MeasureString(lblField.Text, lblField.Font, columnWidth);
                            lblField.Height = Math.Max(18, (int)size.Height + 4);
                        }

                        panel.Controls.Add(lblField);

                        if (isLeftColumn)
                            leftY += lblField.Height + 4;
                        else
                            rightY += lblField.Height + 4;

                        fieldIndex++;
                    }

                    // 设置面板高度
                    panel.Height = Math.Max(leftY, rightY) + 8;
                    parent.Controls.Add(panel);
                    y += panel.Height + 8;
                    index++;
                }
            }
            catch (Exception ex)
            {
                // 如果解析失败，使用原来的方法
                AddTextBlock(parent, ref y, "教育经历", ParseEducationHistory(), width);
            }
        }

        // ========== 导出功能 ==========
        
        private void BtnPrint_Click(object? sender, EventArgs e)
        {
            try
            {
                var printDialog = new PrintDialog();
                var printDocument = new PrintDocument();
                printDocument.PrintPage += PrintDocument_PrintPage;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.PrinterSettings = printDialog.PrinterSettings;
                    printDocument.Print();
                    
                    // 检查是否选择了PDF打印机
                    string printerName = printDialog.PrinterSettings.PrinterName.ToLower();
                    if (printerName.Contains("pdf"))
                    {
                        MessageBox.Show("PDF导出任务已发送！\n\n请在打印对话框中选择保存位置。", 
                            "导出PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("打印任务已发送！\n\n提示：如需导出PDF，请在打印对话框中选择 'Microsoft Print to PDF' 打印机。", 
                            "打印", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            // 绘制A4内容到打印页面
            var bitmap = new Bitmap(_a4Panel.Width, _a4Panel.Height);
            _a4Panel.DrawToBitmap(bitmap, new Rectangle(0, 0, _a4Panel.Width, _a4Panel.Height));
            
            // 计算缩放比例，保持宽高比
            float scaleWidth = (float)e.MarginBounds.Width / _a4Panel.Width;
            float scaleHeight = (float)e.MarginBounds.Height / _a4Panel.Height;
            float scale = Math.Min(scaleWidth, scaleHeight);  // 使用较小的缩放比例以保持宽高比
            
            int scaledWidth = (int)(_a4Panel.Width * scale);
            int scaledHeight = (int)(_a4Panel.Height * scale);
            
            // 居中绘制
            int x = e.MarginBounds.Left + (e.MarginBounds.Width - scaledWidth) / 2;
            int y = e.MarginBounds.Top;
            
            e.Graphics.DrawImage(bitmap, x, y, scaledWidth, scaledHeight);
        }

        private void BtnExportWord_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "HTML文件 (*.html)|*.html",
                    FileName = $"魔女档案_{GetString("Name")}_{GetString("PrisonerNo")}.html",
                    Title = "导出为HTML"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToHtml(saveDialog.FileName);
                    MessageBox.Show($"HTML文件导出成功！\n\n文件位置：{saveDialog.FileName}\n\n提示：可以用浏览器打开查看，也可以用Word打开编辑。", 
                        "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // 询问是否打开文件
                    if (MessageBox.Show("是否立即用浏览器打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = saveDialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportToHtml(string filePath)
        {
            var html = new StringBuilder();
            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html><head>");
            html.AppendLine("<meta charset='utf-8'>");
            html.AppendLine($"<title>魔女档案 - {GetString("Name")}</title>");
            html.AppendLine("<style>");
            html.AppendLine(@"
                body { font-family: '微软雅黑', Arial; margin: 0; padding: 20px; background: #f0f0f0; }
                .a4 { width: 210mm; min-height: 297mm; background: white; margin: 0 auto; padding: 20mm; box-shadow: 0 0 10px rgba(0,0,0,0.1); }
                h1 { text-align: center; color: #34495e; border-bottom: 2px solid #34495e; padding-bottom: 10px; }
                .section { margin: 20px 0; }
                .section-title { background: #f0f0f0; padding: 8px 10px; font-weight: bold; color: #34495e; margin: 15px 0 10px 0; }
                .info-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; background: #fafafa; padding: 15px; border: 1px solid #ddd; }
                .info-item { padding: 5px; }
                .text-block { background: #fafafa; padding: 10px; border: 1px solid #ddd; margin: 5px 0; }
                .trauma { background: #fff0f0; }
                .label { font-weight: bold; color: #666; }
                .avatar { float: left; margin-right: 20px; border: 1px solid #ddd; }
                .top-info { overflow: auto; margin-bottom: 20px; }
                .footer { text-align: center; color: #999; font-size: 12px; margin-top: 30px; border-top: 1px solid #ddd; padding-top: 10px; }
            ");
            html.AppendLine("</style></head><body><div class='a4'>");

            // 标题
            html.AppendLine($"<h1>魔女审判系统 · 个人档案</h1>");

            // 头像和核心信息
            html.AppendLine("<div class='top-info'>");
            
            // 头像（如果有）
            string avatarPath = GetString("AvatarPath");
            if (!string.IsNullOrEmpty(avatarPath))
            {
                string fullPath = Path.IsPathRooted(avatarPath) ? avatarPath : Path.Combine(AppContext.BaseDirectory, avatarPath);
                if (File.Exists(fullPath))
                {
                    // 转换为base64
                    byte[] imageBytes = File.ReadAllBytes(fullPath);
                    string base64 = Convert.ToBase64String(imageBytes);
                    html.AppendLine($"<img src='data:image/png;base64,{base64}' class='avatar' width='160' height='160' />");
                }
            }

            html.AppendLine("<div class='info-grid'>");
            AddHtmlInfo(html, "囚人番号", GetString("PrisonerNo"));
            AddHtmlInfo(html, "个人番号", GetString("PersonalNo"));
            AddHtmlInfo(html, "姓名", GetString("Name"));
            AddHtmlInfo(html, "曾用名", GetString("FormerName"));
            AddHtmlInfo(html, "性别", GetString("Gender"));
            AddHtmlInfo(html, "出生日期", GetDate("BirthDate"));
            AddHtmlInfo(html, "年龄", GetString("Age") + " 岁");
            AddHtmlInfo(html, "民族", GetString("Ethnicity"));
            AddHtmlInfo(html, "籍贯", GetString("Birthplace"));
            AddHtmlInfo(html, "状态", GetString("Status"));
            html.AppendLine("</div></div>");

            // 身体特征
            html.AppendLine("<div class='section-title'>身体特征与能力</div>");
            html.AppendLine("<div class='info-grid'>");
            AddHtmlInfo(html, "身高", GetDecimal("Height") + " cm");
            AddHtmlInfo(html, "体重", GetDecimal("Weight") + " kg");
            AddHtmlInfo(html, "血型", GetString("BloodType"));
            AddHtmlInfo(html, "魔法", GetString("Magic"));
            AddHtmlInfo(html, "处刑结果", GetString("ExecutionResult"));
            html.AppendLine("</div>");

            // 联系方式
            html.AppendLine("<div class='section-title'>联系方式</div>");
            AddHtmlTextBlock(html, "地址", GetString("Address"));
            AddHtmlTextBlock(html, "电话", GetString("Phone"));
            AddHtmlTextBlock(html, "邮箱", GetString("Email"));
            AddHtmlTextBlock(html, "LINE账号", GetString("LineAccount"));

            // 教育背景
            html.AppendLine("<div class='section-title'>教育背景</div>");
            AddHtmlTextBlock(html, "最高学历", GetString("HighestEducation"));
            AddHtmlTextBlock(html, "教育经历", ParseEducationHistory());

            // 家庭关系
            html.AppendLine("<div class='section-title'>家庭关系</div>");
            AddHtmlTextBlock(html, "家庭结构", GetString("FamilyStructure"));
            AddHtmlTextBlock(html, "父亲", GetString("Father"));
            AddHtmlTextBlock(html, "母亲", GetString("Mother"));

            // 个性特征
            html.AppendLine("<div class='section-title'>个性特征</div>");
            AddHtmlTextBlock(html, "技能/特长", GetString("Skills"));
            AddHtmlTextBlock(html, "兴趣爱好", GetString("Hobbies"));
            AddHtmlTextBlock(html, "理想", GetString("Dreams"));
            AddHtmlTextBlock(html, "讨厌的事物", GetString("Dislikes"));

            // 心理创伤
            html.AppendLine("<div class='section-title' style='color: #b40000;'>心理创伤</div>");
            AddHtmlTextBlock(html, "创伤描述", GetString("Trauma"), "trauma");

            // 魔女相关
            html.AppendLine("<div class='section-title'>魔女相关信息</div>");
            AddHtmlTextBlock(html, "魔女化办法", GetString("WitchTransformMethod"));
            AddHtmlTextBlock(html, "公开描述", GetString("DescriptionPublic"));

            // 页脚
            html.AppendLine($"<div class='footer'>档案编号：{GetString("PrisonerNo")} | 生成日期：{DateTime.Now:yyyy-MM-dd HH:mm}</div>");

            html.AppendLine("</div></body></html>");

            File.WriteAllText(filePath, html.ToString(), Encoding.UTF8);
        }

        private void AddHtmlInfo(StringBuilder html, string label, string value)
        {
            if (string.IsNullOrEmpty(value) || value == "无") return;
            html.AppendLine($"<div class='info-item'><span class='label'>{label}：</span>{value}</div>");
        }

        private void AddHtmlTextBlock(StringBuilder html, string label, string value, string cssClass = "")
        {
            if (string.IsNullOrEmpty(value) || value == "无") return;
            html.AppendLine($"<div class='text-block {cssClass}'><span class='label'>{label}：</span><br/>{value.Replace("\n", "<br/>")}</div>");
        }

        // 辅助方法
        private string GetString(string columnName)
        {
            if (_witchData == null) return "";
            var value = _witchData[columnName];
            if (value == null || value == DBNull.Value) return "";
            return value.ToString() ?? "";
        }

        private string GetDecimal(string columnName)
        {
            if (_witchData == null) return "";
            var value = _witchData[columnName];
            if (value == null || value == DBNull.Value) return "";
            if (value is decimal d) return d.ToString("0.##");
            return value.ToString() ?? "";
        }

        private string GetDate(string columnName)
        {
            if (_witchData == null) return "";
            var value = _witchData[columnName];
            if (value == null || value == DBNull.Value) return "";
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd");
            return value.ToString() ?? "";
        }
    }
}
