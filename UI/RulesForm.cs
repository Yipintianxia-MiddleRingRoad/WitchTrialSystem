using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·规定界面
    /// 功能：展示规定信息
    /// 左下角：五个热键按钮（规定Ⅰ到Ⅴ）
    /// 中部左侧：显示规定名称（罗马数字+名称）
    /// 中部右侧：显示规定内容（从文件读取）
    /// </summary>
    public class RulesForm : BasePokedexForm
    {
        #region 数据字段

        private readonly string[] _ruleFiles = {
            "规定1-生活规定.md",
            "规定2-警卫巡逻.md",
            "规定3-放风时间的规定.md",
            "规定4-魔女审判.md",
            "规定5-时间表.md"
        };

        private readonly string[] _ruleNames = {
            "生活规定",
            "警卫巡逻",
            "放风时间的规定",
            "魔女审判",
            "时间表"
        };

        private int _currentRuleIndex = 0;

        // 自定义字体
        private PrivateFontCollection _fontCollection = new();
        private FontFamily? _customFontFamily;
        private readonly List<Font> _createdFonts = new();  // 保存所有创建的字体，用于释放

        #endregion

        #region UI 控件

        // 左下角：规定热键按钮和文本标签
        private readonly Button[] _ruleButtons = new Button[5];  // 透明热键按钮
        private readonly Label[] _ruleLabels = new Label[5];     // 文本标签

        // 中部左侧：规定名称显示
        private readonly Panel _leftPanel = new() { BackColor = Color.Transparent };
        private readonly Label _romanNumeralLabel = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),  // 罗马数字稍小一点
            ForeColor = Color.FromArgb(0x37, 0x2E, 0x2E),  // #372E2E
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.TopCenter
            // 移除MaximumSize，允许标签根据内容自由调整大小
        };
        private readonly Label _ruleNameLabel = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),
            ForeColor = Color.FromArgb(0xC5, 0xB3, 0xAE),  // 与罗马数字相同的颜色
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.TopCenter
            // 移除MaximumSize，允许标签根据内容自由调整大小
        };

        // 中部右侧：规定内容显示
        private readonly Panel _rightPanel = new() { BackColor = Color.Transparent };
        private readonly TransparentScrollPanel _contentPanel = new()
        {
            BackColor = Color.Transparent,
            AutoScroll = true,
            HorizontalScroll = { Enabled = false, Visible = false },
            VerticalScroll = { Enabled = true, Visible = true }
        };
        private readonly Label _contentLabel = new()
        {
            AutoSize = true,
            ForeColor = Color.FromArgb(0xC6, 0xB3, 0xAC),  // 统一为中左规定的颜色
            BackColor = Color.Transparent
        };

        #endregion

        /// <summary>
        /// 构造函数：初始化规定界面
        /// </summary>
        public RulesForm(string username) : base(username)
        {
            Text = "图鉴 · 规定";
            BLL.IconHelper.SetFormIcon(this);  // 设置窗体图标
            UpdateTitle();  // 添加用户信息到标题
            LoadCustomFont();  // 加载自定义字体
            InitializeLayout();
            Load += RulesForm_Load;
        }

        /// <summary>
        /// 释放字体资源
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // 释放所有创建的字体
                foreach (var font in _createdFonts)
                {
                    font?.Dispose();
                }
                _createdFonts.Clear();

                // 释放字体集合
                _fontCollection?.Dispose();
            }
            base.Dispose(disposing);
        }

        protected override string GetBackgroundImageName()
        {
            return "rules_bg.png";  // 规定背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnRules.Enabled = false;
            _btnRules.Cursor = Cursors.Default;
        }

        #region 字体加载

        /// <summary>
        /// 加载自定义字体
        /// </summary>
        private void LoadCustomFont()
        {
            try
            {
                string fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "方正小标宋简.ttf");

                if (File.Exists(fontPath))
                {
                    _fontCollection.AddFontFile(fontPath);
                    if (_fontCollection.Families.Length > 0)
                    {
                        _customFontFamily = _fontCollection.Families[0];

                        // 应用字体到所有控件
                        ApplyCustomFonts();
                    }
                }
            }
            catch
            {
                // 如果加载失败，使用默认字体
            }
        }

        /// <summary>
        /// 创建自定义字体（根据大小和样式）
        /// </summary>
        private Font? CreateCustomFont(float size, FontStyle style = FontStyle.Regular)
        {
            if (_customFontFamily != null)
            {
                var font = new Font(_customFontFamily, size, style);
                _createdFonts.Add(font);  // 记录创建的字体，用于后续释放
                return font;
            }
            return null;
        }

        /// <summary>
        /// 应用自定义字体到所有控件
        /// </summary>
        private void ApplyCustomFonts()
        {
            if (_customFontFamily == null) return;

            // 更新所有控件的字体（大幅调小以确保完整显示）
            var romanNumeralFont = CreateCustomFont(20, FontStyle.Bold);  // 罗马数字字体大幅调小
            if (romanNumeralFont != null) _romanNumeralLabel.Font = romanNumeralFont;

            var ruleNameFont = CreateCustomFont(24, FontStyle.Bold);  // 规定名称字体大幅调小
            if (ruleNameFont != null) _ruleNameLabel.Font = ruleNameFont;

            var contentFont = CreateCustomFont(12, FontStyle.Regular);  // 中右文字内容字体小一点点
            if (contentFont != null) _contentLabel.Font = contentFont;

            var labelFont = CreateCustomFont(24, FontStyle.Bold);  // 与中左部分文字相同的大小和样式
            if (labelFont != null)
            {
                foreach (var label in _ruleLabels)
                {
                    if (label != null) label.Font = labelFont;
                }
            }

            var buttonFont = CreateCustomFont(20, FontStyle.Bold);
            if (buttonFont != null)
            {
                foreach (var button in _ruleButtons)
                {
                    if (button != null) button.Font = buttonFont;
                }
            }
        }

        #endregion

        #region 初始化布局

        private void InitializeLayout()
        {
            // 中部左侧面板：规定名称显示
            _bg.Controls.Add(_leftPanel);
            _leftPanel.Controls.Add(_romanNumeralLabel);
            _leftPanel.Controls.Add(_ruleNameLabel);

            // 中部右侧面板：规定内容显示
            _bg.Controls.Add(_rightPanel);
            _rightPanel.Controls.Add(_contentPanel);
            _contentPanel.Controls.Add(_contentLabel);

            // 延迟到布局完成后设置所有控件的位置和大小
            _bg.Layout += (s, e) => PositionPanels();

            // 左下角：创建五个规定热键按钮
            InitializeRuleButtons();
        }

        private void PositionPanels()
        {
            // 中部左侧面板：规定名称显示（大幅增加尺寸确保文字完全显示）
            _leftPanel.Width = (int)(ClientSize.Width * 0.6);  // 增加到60%的窗口宽度
            _leftPanel.Height = 300;  // 大幅增加高度到200像素
            _leftPanel.Left = 30;  // 再往左移一点点
            // 将规定名称区域放在页面中部略偏上：按窗口高度的 45% 位置居中
            int desiredCenterY = (int)(ClientSize.Height * 0.35);
            _leftPanel.Top = Math.Max(20, desiredCenterY - _leftPanel.Height / 2);

            // 规定名称标签居中
            CenterRuleLabels();

            // 中部右侧面板：规定内容显示（适应左侧面板的大幅变化）
            _rightPanel.Width = (int)(ClientSize.Width * 0.35) - 50;  // 相应缩小到35%的窗口宽度
            _rightPanel.Height = ClientSize.Height - 300;  // 底部往上提
            _rightPanel.Left = (int)(ClientSize.Width * 0.6) + 20;  // 紧跟左侧面板
            _rightPanel.Top = 80;

            // 内容滚动容器（直接放在右侧面板顶部）
            _contentPanel.Left = 0;
            _contentPanel.Top = 0;
            _contentPanel.Width = _rightPanel.Width - 50;
            _contentPanel.Height = _rightPanel.Height;

            _contentLabel.Left = 10;  // 增加左边距，避免文字被挡住
            _contentLabel.Top = 0;
            // 字体将在 ApplyCustomFonts 中设置
            _contentLabel.MaximumSize = new Size(_contentPanel.ClientSize.Width - 30, 0);  // 调整最大宽度，留出左右边距
        }

        /// <summary>
        /// 初始化规定热键按钮和文本标签
        /// </summary>
        private void InitializeRuleButtons()
        {
            string[] romanNumerals = { "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ" };

            for (int i = 0; i < 5; i++)
            {
                // 创建文本标签（显示"规定"在上，罗马数字在下）
                _ruleLabels[i] = new Label
                {
                    Text = $"规定\n{romanNumerals[i]}",
                    AutoSize = true,
                    ForeColor = Color.FromArgb(0xC6, 0xB3, 0xAC),  // 与中左部分文字相同颜色
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Tag = i
                };

                // 创建透明热键按钮
                _ruleButtons[i] = new Button
                {
                    Text = "",
                    Width = 90,  // 增加宽度
                    Height = 50, // 增加高度以适应两行文字
                    BackColor = Color.Transparent,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                _ruleButtons[i].FlatAppearance.BorderSize = 0; // 无边框，完全不可见
                _ruleButtons[i].FlatAppearance.MouseOverBackColor = Color.Transparent; // 鼠标悬停也不显示
                _ruleButtons[i].Click += RuleButton_Click;
                _ruleLabels[i].Click += RuleButton_Click; // 标签也可以点击

                _bg.Controls.Add(_ruleButtons[i]);  // 先添加按钮
                _bg.Controls.Add(_ruleLabels[i]);  // 后添加标签，确保标签在最上层
                _ruleLabels[i].BringToFront();     // 确保标签在最上层
            }

            // 延迟设置按钮位置，直到窗体完全加载
            _bg.Layout += (s, e) => PositionRuleButtons();

            // 默认选中第一个规定
            HighlightRuleButton(0);
        }

        private void PositionRuleButtons()
        {
            for (int i = 0; i < 5; i++)
            {
                if (_ruleLabels[i] != null && _ruleButtons[i] != null)
                {
                    // 定位：左下角，水平排列
                    int baseLeft = 80 + i * 117;  // 稍微调整间距
                    int baseTop = ClientSize.Height - 110;  // 稍微往上调整位置

                    // 文本标签和透明热键按钮完全重合
                    _ruleLabels[i].Left = baseLeft;
                    _ruleLabels[i].Top = baseTop;

                    // 透明热键按钮与文本标签完全重合
                    _ruleButtons[i].Left = baseLeft;
                    _ruleButtons[i].Top = baseTop;
                    _ruleButtons[i].Width = _ruleLabels[i].Width;   // 宽度与标签相同
                    _ruleButtons[i].Height = _ruleLabels[i].Height; // 高度与标签相同
                }
            }
        }

        #endregion

        #region 事件处理

        private void RulesForm_Load(object? sender, EventArgs e)
        {
            try
            {
                // 显示第一个规定
                ShowRule(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载规定失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 规定按钮点击事件
        /// </summary>
        private void RuleButton_Click(object? sender, EventArgs e)
        {
            int index = -1;

            if (sender is Button button && button.Tag is int buttonIndex)
            {
                index = buttonIndex;
            }
            else if (sender is Label label && label.Tag is int labelIndex)
            {
                index = labelIndex;
            }

            if (index >= 0)
            {
                ShowRule(index);
            }
        }

        #endregion

        #region 规定显示逻辑

        /// <summary>
        /// 显示指定索引的规定
        /// </summary>
        private void ShowRule(int index)
        {
            if (index < 0 || index >= _ruleFiles.Length) return;

            _currentRuleIndex = index;

            // 更新左侧规定名称
            string romanNumeral = GetRomanNumeral(index + 1);
            _romanNumeralLabel.Text = romanNumeral;
            _ruleNameLabel.Text = _ruleNames[index];
            CenterRuleLabels();

            // 读取并显示规定内容
            string content = LoadRuleContent(index);
            _contentLabel.Text = content;

            // 重新计算内容宽度
            _contentLabel.MaximumSize = new Size(_contentPanel.ClientSize.Width - 40, 0);

            // 高亮当前按钮
            HighlightRuleButton(index);
        }

        /// <summary>
        /// 居中规定名称标签（罗马数字和中文名称）
        /// </summary>
        private void CenterRuleLabels()
        {
            if (_romanNumeralLabel != null && _ruleNameLabel != null && _leftPanel != null)
            {
                // 罗马数字：重新定位，确保在面板内完整显示
                int romanLeft = Math.Max(10, (_leftPanel.Width - _romanNumeralLabel.Width) / 2 - 90);  // 向左移动20像素
                romanLeft = Math.Min(romanLeft, _leftPanel.Width - _romanNumeralLabel.Width - 10);  // 确保不超出右边界
                _romanNumeralLabel.Left = romanLeft;
                _romanNumeralLabel.Top = 150;  // 向下移动50像素

                // 中文名称：重新定位，确保在面板内完整显示
                int ruleLeft = Math.Max(10, (_leftPanel.Width - _ruleNameLabel.Width) / 2 - 90);  // 向左移动20像素
                ruleLeft = Math.Min(ruleLeft, _leftPanel.Width - _ruleNameLabel.Width - 10);  // 确保不超出右边界
                _ruleNameLabel.Left = ruleLeft;
                _ruleNameLabel.Top = 120 + 130;  // 向下移动50像素

                // 确保规定名称显示在最上层
                _ruleNameLabel.BringToFront();
            }
        }

        /// <summary>
        /// 高亮规定按钮
        /// </summary>
        private void HighlightRuleButton(int index)
        {
            for (int i = 0; i < _ruleLabels.Length; i++)
            {
                if (_ruleLabels[i] != null && _ruleButtons[i] != null)
                {
                    if (i == index)
                    {
                        // 选中状态：只改变标签颜色，按钮完全不可见
                        _ruleLabels[i].ForeColor = Color.FromArgb(0xC6, 0xB3, 0xAC);  // 高亮状态
                    }
                    else
                    {
                        // 未选中状态：恢复默认颜色
                        _ruleLabels[i].ForeColor = Color.FromArgb(0x9D, 0x92, 0x91);  // 默认颜色
                    }
                }
            }
        }

        /// <summary>
        /// 获取罗马数字
        /// </summary>
        private string GetRomanNumeral(int number)
        {
            return number switch
            {
                1 => "Ⅰ",
                2 => "Ⅱ",
                3 => "Ⅲ",
                4 => "Ⅳ",
                5 => "Ⅴ",
                _ => number.ToString()
            };
        }

        /// <summary>
        /// 加载规定内容
        /// </summary>
        private string LoadRuleContent(int index)
        {
            if (index < 0 || index >= _ruleFiles.Length) return "无效的规定索引";

            string fileName = _ruleFiles[index];
            string filePath = Path.Combine(AppContext.BaseDirectory, "Images", "Rules", fileName);

            // 读取.md文件
            if (File.Exists(filePath))
            {
                try
                {
                    return File.ReadAllText(filePath, Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    return $"读取文件失败：{ex.Message}";
                }
            }

            return $"文件未找到：{fileName}";
        }

        #endregion
    }
}
