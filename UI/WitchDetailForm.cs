using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.BLL;

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
            
            _btnExportPdf.Text = "📑 导出PDF";
            _btnExportPdf.Size = new Size(120, 35);
            _btnExportPdf.ForeColor = Color.White;
            _btnExportPdf.BackColor = Color.FromArgb(41, 128, 185);
            _btnExportPdf.FlatStyle = FlatStyle.Flat;
            _btnExportPdf.FlatAppearance.BorderSize = 0;
            _btnExportPdf.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnExportPdf.Click += BtnExportPdf_Click;
            toolbar.Controls.Add(_btnExportPdf);

            _btnExportWord.Text = "📄 导出Word";
            _btnExportWord.Size = new Size(120, 35);
            _btnExportWord.ForeColor = Color.White;
            _btnExportWord.BackColor = Color.FromArgb(39, 174, 96);
            _btnExportWord.FlatStyle = FlatStyle.Flat;
            _btnExportWord.FlatAppearance.BorderSize = 0;
            _btnExportWord.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            _btnExportWord.Click += BtnExportWord_Click;
            toolbar.Controls.Add(_btnExportWord);

            _btnPrint.Text = "🖨️ 打印";
            _btnPrint.Size = new Size(100, 35);
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
                _btnExportPdf.Location = new Point(rightX - 120, 12);
                rightX -= 130;
                _btnExportWord.Location = new Point(rightX - 120, 12);
                rightX -= 130;
                _btnPrint.Location = new Point(rightX - 100, 12);
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
            
            // 内容面板
            _contentPanel.Location = new Point(0, 0);
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
            int y = 0;
            int contentWidth = 714;  // A4宽度(794) - 左右边距(80) = 714px

            // ========== 档案标题 ==========
            var title = new Label
            {
                Text = "魔女审判系统 · 个人档案",
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 40,
                Font = new Font("黑体", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            _contentPanel.Controls.Add(title);
            y += 50;

            // 分隔线
            AddSeparator(_contentPanel, ref y, contentWidth);

            // ========== 第一部分：头像和核心信息（横向布局）==========
            var topSection = new Panel
            {
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 200,
                BackColor = Color.White
            };

            // 左侧：头像
            _avatar.Size = new Size(160, 160);
            _avatar.Location = new Point(20, 20);
            _avatar.SizeMode = PictureBoxSizeMode.Zoom;
            _avatar.BorderStyle = BorderStyle.FixedSingle;
            LoadAvatar();
            topSection.Controls.Add(_avatar);

            // 右侧：核心信息（两列布局）
            int infoX = 200;
            int infoY = 20;
            int col1Width = 250;
            int col2Width = 250;

            // 第一列
            AddInfoLine(topSection, ref infoY, infoX, "囚人番号", GetString("PrisonerNo"), true);
            AddInfoLine(topSection, ref infoY, infoX, "个人番号", GetString("PersonalNo"));
            AddInfoLine(topSection, ref infoY, infoX, "姓名", GetString("Name"), true);
            AddInfoLine(topSection, ref infoY, infoX, "曾用名", GetString("FormerName"));
            AddInfoLine(topSection, ref infoY, infoX, "性别", GetString("Gender"));

            // 第二列
            infoY = 20;
            infoX = 200 + col1Width;
            AddInfoLine(topSection, ref infoY, infoX, "出生日期", GetDate("BirthDate"));
            AddInfoLine(topSection, ref infoY, infoX, "年龄", GetString("Age") + " 岁");
            AddInfoLine(topSection, ref infoY, infoX, "民族", GetString("Ethnicity"));
            AddInfoLine(topSection, ref infoY, infoX, "籍贯", GetString("Birthplace"));
            AddInfoLine(topSection, ref infoY, infoX, "状态", GetString("Status"), true);

            _contentPanel.Controls.Add(topSection);
            y += 210;

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
            y += physicalPanel.Height + 15;

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
            y += contactPanel.Height + 15;

            // ========== 第四部分：教育背景 ==========
            AddSectionTitle(_contentPanel, ref y, "教育背景", contentWidth);
            AddTextBlock(_contentPanel, ref y, "最高学历", GetString("HighestEducation"), contentWidth);
            AddTextBlock(_contentPanel, ref y, "教育经历", ParseEducationHistory(), contentWidth);

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
            y += 20;
            var footer = new Label
            {
                Text = $"档案编号：{GetString("PrisonerNo")} | 生成日期：{DateTime.Now:yyyy-MM-dd HH:mm}",
                Location = new Point(0, y),
                Width = contentWidth,
                Height = 20,
                Font = new Font("微软雅黑", 8),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.Gray
            };
            _contentPanel.Controls.Add(footer);
            y += 30;

            // 设置内容面板高度
            _contentPanel.Height = y;
            _a4Panel.Height = Math.Max(1123, y + 80);  // 至少一页A4
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
                // 简单解析JSON（实际项目中应使用JSON库）
                json = json.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                var items = json.Split(new[] { "}," }, StringSplitOptions.RemoveEmptyEntries);
                var result = "";
                foreach (var item in items)
                {
                    result += item.Replace("\"", "").Replace(",", "\n  ") + "\n\n";
                }
                return result.Trim();
            }
            catch
            {
                return json;
            }
        }

        private string ParseWorkHistory()
        {
            string json = GetString("WorkHistory");
            if (string.IsNullOrEmpty(json) || json == "[]") return "";

            try
            {
                json = json.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "");
                var items = json.Split(new[] { "}," }, StringSplitOptions.RemoveEmptyEntries);
                var result = "";
                foreach (var item in items)
                {
                    result += item.Replace("\"", "").Replace(",", "\n  ") + "\n\n";
                }
                return result.Trim();
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
                Height = 30,
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                ForeColor = color ?? Color.FromArgb(52, 73, 94),
                BackColor = Color.FromArgb(240, 240, 240),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(10, 0, 0, 0)
            };
            parent.Controls.Add(lbl);
            y += 35;
        }

        private void AddInfoLine(Panel parent, ref int y, int x, string label, string value, bool bold = false)
        {
            if (string.IsNullOrEmpty(value) || value == "无") return;

            var lbl = new Label
            {
                Text = $"{label}：{value}",
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("微软雅黑", 9, bold ? FontStyle.Bold : FontStyle.Regular)
            };
            parent.Controls.Add(lbl);
            y += 25;
        }

        private Panel CreateInfoGrid((string label, string value)[] items, int columns)
        {
            var panel = new Panel
            {
                Width = 714,  // 固定宽度
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            int itemWidth = (714 - 30) / columns;
            int x = 10, y = 10;
            int col = 0;

            foreach (var (label, value) in items)
            {
                if (string.IsNullOrEmpty(value) || value == "无") continue;

                var lbl = new Label
                {
                    Text = $"{label}：{value}",
                    Location = new Point(x, y),
                    Width = itemWidth - 10,
                    Height = 25,
                    Font = new Font("微软雅黑", 9)
                };
                panel.Controls.Add(lbl);

                col++;
                if (col >= columns)
                {
                    col = 0;
                    x = 10;
                    y += 30;
                }
                else
                {
                    x += itemWidth;
                }
            }

            panel.Height = y + 40;
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
                Height = 20,
                Font = new Font("微软雅黑", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 100, 100)
            };
            parent.Controls.Add(lblLabel);
            y += 22;

            var lblValue = new Label
            {
                Text = value,
                Location = new Point(10, y),
                Width = width - 20,
                AutoSize = false,
                Height = 0,  // 先设为0
                Font = new Font("微软雅黑", 9),
                BackColor = bgColor ?? Color.FromArgb(250, 250, 250),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // 计算实际需要的高度
            using (var g = lblValue.CreateGraphics())
            {
                var size = g.MeasureString(value, lblValue.Font, lblValue.Width - 20);
                lblValue.Height = (int)size.Height + 20;
            }
            
            parent.Controls.Add(lblValue);
            y += lblValue.Height + 10;
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
                    MessageBox.Show("打印任务已发送！", "打印", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打印失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (e.Graphics == null) return;

            // 绘制A4内容到打印页面
            var bitmap = new Bitmap(_a4Panel.Width, _a4Panel.Height);
            _a4Panel.DrawToBitmap(bitmap, new Rectangle(0, 0, _a4Panel.Width, _a4Panel.Height));
            
            // 缩放到打印页面
            e.Graphics.DrawImage(bitmap, e.MarginBounds);
        }

        private void BtnExportWord_Click(object? sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "HTML文件 (*.html)|*.html",
                    FileName = $"魔女档案_{GetString("Name")}_{GetString("PrisonerNo")}.html",
                    Title = "导出为HTML（可用Word打开）"
                };

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    ExportToHtml(saveDialog.FileName);
                    MessageBox.Show($"导出成功！\n文件：{saveDialog.FileName}\n\n可以用Word打开此HTML文件。", 
                        "导出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // 询问是否打开文件
                    if (MessageBox.Show("是否立即打开文件？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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

        private void BtnExportPdf_Click(object? sender, EventArgs e)
        {
            MessageBox.Show("PDF导出功能需要安装第三方库（如iTextSharp）。\n\n当前建议：\n1. 使用'导出Word'功能导出HTML\n2. 用Word打开后另存为PDF\n\n或者使用'打印'功能，选择'Microsoft Print to PDF'", 
                "PDF导出", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
