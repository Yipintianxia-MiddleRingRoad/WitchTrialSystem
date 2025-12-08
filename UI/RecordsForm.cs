using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 透明滚动条面板（滚动条透明但可见）
    /// </summary>
    public class TransparentScrollPanel : Panel
    {
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            // 移除滚动条主题，使其更透明
            if (IsHandleCreated)
            {
                SetWindowTheme(Handle, "", "");
            }
        }

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }

    /// <summary>
    /// 图鉴·记录界面
    /// 功能：展示记录信息
    /// 上方：记录内容（从数据库读取）
    /// 底部：记录标题列表（支持翻页）
    /// </summary>
    public class RecordsForm : BasePokedexForm
    {
        #region Windows API

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        #endregion

        #region 数据字段

        private readonly RecordDAL _recordDal = new();
        private DataTable _dt = new();
        private int _currentIndex = -1;
        private int _recordPage = 0;  // 记录列表当前页码

        // 自定义字体
        private PrivateFontCollection _fontCollection = new();
        private FontFamily? _customFontFamily;
        private readonly List<Font> _createdFonts = new();  // 保存所有创建的字体，用于释放
        private Font? _recordItemFont;  // 缓存的记录项字体

        #endregion

        #region UI 控件

        // 左侧：当前记录名称
        private readonly Panel _leftPanel = new() { BackColor = Color.Transparent };
        private readonly Label _currentRecordName = new()
        {
            AutoSize = false,  // 改为固定宽度，避免文字被截断
            Width = 500,  // 宽度扩大2倍：250 * 2 = 500
            Height = 50,  // 单行显示，高度50px足够
            TextAlign = ContentAlignment.MiddleCenter,  // 居中对齐
            Font = new Font("Segoe UI", 20, FontStyle.Bold),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.FromArgb(47, 35, 34),  // 统一颜色
            BackColor = Color.Transparent
        };

        // 右侧：记录内容
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
            ForeColor = Color.FromArgb(47, 35, 34),  // 统一颜色
            BackColor = Color.Transparent
        };

        // 底部：记录列表（参考 EvidenceForm 的缩略图实现）
        private readonly FlowLayoutPanel _recordBar = new()
        {
            Height = 100,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,  // 禁用滚动，使用翻页按钮
            Padding = new Padding(40, 8, 40, 8),
            BackColor = Color.Transparent,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        // 翻页按钮
        private readonly Button _btnPrevPage = new()
        {
            Text = "",  // 清空文字，使用图片
            Width = 40,
            Height = 100,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
            BackgroundImageLayout = ImageLayout.Stretch
        };

        private readonly Button _btnNextPage = new()
        {
            Text = "",  // 清空文字，使用图片
            Width = 40,
            Height = 100,
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
            BackgroundImageLayout = ImageLayout.Stretch
        };

        private readonly Label _recordTitle = new()
        {
            Text = "记录列表",
            AutoSize = true,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };

        private Label? _selectedRecord;

        #endregion

        /// <summary>
        /// 构造函数：初始化记录界面
        /// </summary>
        public RecordsForm(string username) : base(username)
        {
            Text = "图鉴 · 记录";
            UpdateTitle();  // 添加用户信息到标题
            LoadCustomFont();  // 加载自定义字体
            InitializeLayout();
            Load += RecordsForm_Load;
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
            return "records_bg.png";  // 记录背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnRecords.Enabled = false;
            _btnRecords.Cursor = Cursors.Default;
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

            // 更新所有控件的字体
            var currentRecordNameFont = CreateCustomFont(22, FontStyle.Bold);
            if (currentRecordNameFont != null) _currentRecordName.Font = currentRecordNameFont;

            var contentLabelFont = CreateCustomFont(15, FontStyle.Regular);
            if (contentLabelFont != null) _contentLabel.Font = contentLabelFont;

            var buttonFont = CreateCustomFont(20, FontStyle.Bold);
            if (buttonFont != null)
            {
                _btnPrevPage.Font = buttonFont;
                _btnNextPage.Font = buttonFont;
            }

            var recordTitleFont = CreateCustomFont(26, FontStyle.Bold);
            if (recordTitleFont != null) _recordTitle.Font = recordTitleFont;

            // 缓存记录项字体，供动态创建的 Label 使用
            _recordItemFont = CreateCustomFont(11, FontStyle.Regular);
        }

        #endregion

        #region 初始化布局

        private void InitializeLayout()
        {
            // 左侧面板：当前记录名称
            _leftPanel.Width = ClientSize.Width / 2 - 50;  // 左半侧，留出间距
            _leftPanel.Height = 100;  // 只显示名称，不需要太大
            _leftPanel.Left = 30;  // 再往左移一点点：从 40 改为 30（左移10像素）
            // 将记录名称区域放在页面中部略偏上：按窗口高度的 45% 位置居中
            int desiredCenterY = (int)(ClientSize.Height * 0.35);
            _leftPanel.Top = Math.Max(20, desiredCenterY - _leftPanel.Height / 2);
            _bg.Controls.Add(_leftPanel);

            // 左侧当前记录名称（水平居中，垂直靠下）
            _leftPanel.Controls.Add(_currentRecordName);
            // 等待控件布局完成后再计算位置
            _leftPanel.Layout += (s, e) =>
            {
                _currentRecordName.Left = (_leftPanel.Width - _currentRecordName.Width) / 2;
                // 向下移动10像素：从15改为25
                _currentRecordName.Top = 25;
            };
            // 初始设置（会在 Layout 事件中更新）
            _currentRecordName.Left = 0;
            _currentRecordName.Top = 0;

            // 右侧面板：记录内容（缩小宽度）
            _rightPanel.Width = ClientSize.Width / 2 - 150;  // 缩小：从 -50 改为 -150
            _rightPanel.Height = ClientSize.Height - 300;  // 底部往上提：从 -200 改为 -250（增加50像素）
            _rightPanel.Left = ClientSize.Width / 2 + 20;  // 从中间开始
            _rightPanel.Top = 80;
            _bg.Controls.Add(_rightPanel);

            // 内容滚动容器（直接放在右侧面板顶部）
            _rightPanel.Controls.Add(_contentPanel);
            _contentPanel.Left = 0;
            _contentPanel.Top = 0;
            _contentPanel.Width = _rightPanel.Width - 50;
            _contentPanel.Height = _rightPanel.Height;
            _contentPanel.Controls.Add(_contentLabel);

            _contentLabel.Left = 10;  // 增加左边距，避免文字被挡住
            _contentLabel.Top = 0;
            // 字体将在 ApplyCustomFonts 中设置
            _contentLabel.MaximumSize = new Size(_contentPanel.ClientSize.Width - 30, 0);  // 调整最大宽度，留出左右边距

            // 标题面板（底部）
            var recordHeaderPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.Transparent
            };
            
            _recordTitle.Left = 40;
            _recordTitle.Top = 32;
            
            recordHeaderPanel.Controls.Add(_recordTitle);
            _bg.Controls.Add(recordHeaderPanel);

            // 底部记录列表栏（参考 EvidenceForm 的实现）
            _recordBar.Left = 0;
            _recordBar.Width = ClientSize.Width;
            _recordBar.Top = ClientSize.Height - 42 - _recordBar.Height - 10;  // 标题面板高度42 + 额外间距10
            _bg.Controls.Add(_recordBar);

            // 翻页按钮（在标题面板之后添加，确保在最上层）
            // 按钮中心线与记录列表中心线对齐
            int recordBarCenterY = _recordBar.Top + _recordBar.Height / 2;
            int buttonTopPosition = recordBarCenterY - _btnPrevPage.Height / 2;
            
            // 加载按钮图片
            try
            {
                string leftButtonPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "button_left.png");
                string rightButtonPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "button_right.png");
                
                if (File.Exists(leftButtonPath))
                {
                    _btnPrevPage.BackgroundImage = Image.FromFile(leftButtonPath);
                }
                
                if (File.Exists(rightButtonPath))
                {
                    _btnNextPage.BackgroundImage = Image.FromFile(rightButtonPath);
                }
            }
            catch
            {
                // 如果加载失败，保持透明背景
            }
            
            _btnPrevPage.Left = 0;  // 页面最左端
            _btnPrevPage.Top = buttonTopPosition;
            _btnPrevPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            _btnPrevPage.FlatAppearance.BorderSize = 0;
            _btnPrevPage.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _btnPrevPage.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _btnPrevPage.Click += (s, e) => NavigateRecords(-1);
            _bg.Controls.Add(_btnPrevPage);
            _btnPrevPage.BringToFront();  // 确保按钮在最上层

            _btnNextPage.Left = ClientSize.Width - _btnNextPage.Width;  // 页面最右端
            _btnNextPage.Top = buttonTopPosition;
            _btnNextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnNextPage.FlatAppearance.BorderSize = 0;
            _btnNextPage.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _btnNextPage.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _btnNextPage.Click += (s, e) => NavigateRecords(1);
            _bg.Controls.Add(_btnNextPage);
            _btnNextPage.BringToFront();  // 确保按钮在最上层
        }

        #endregion

        #region 数据加载与展示

        private void RecordsForm_Load(object? sender, EventArgs e)
        {
            try
            {
                // 设置滚动条主题，使其更透明
                SetScrollBarTheme();
                
                _dt = _recordDal.GetRecords();
                
                if (_dt.Rows.Count == 0)
                {
                    MessageBox.Show("暂无记录数据", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                BuildRecordList();
                ShowRecordAt(0);
                
                // 确保左侧标题居中显示
                CenterCurrentRecordName();
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载记录失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 将左侧当前记录名称居中显示（水平居中，垂直靠下）
        /// </summary>
        private void CenterCurrentRecordName()
        {
            if (_currentRecordName != null && _leftPanel != null)
            {
                _currentRecordName.Left = (_leftPanel.Width - _currentRecordName.Width) / 2;
                // 向下移动10像素：从10改为20
                _currentRecordName.Top = 20;
            }
        }

        /// <summary>
        /// 设置滚动条主题，使其透明
        /// </summary>
        private void SetScrollBarTheme()
        {
            try
            {
                // 设置内容面板的滚动条为透明
                void SetContentPanelScrollBarTransparent()
                {
                    if (_contentPanel.IsHandleCreated)
                    {
                        // 移除主题，使滚动条更透明
                        SetWindowTheme(_contentPanel.Handle, "", "");
                    }
                }

                if (_contentPanel.IsHandleCreated)
                {
                    SetContentPanelScrollBarTransparent();
                }
                else
                {
                    _contentPanel.HandleCreated += (s, e) =>
                    {
                        SetContentPanelScrollBarTransparent();
                    };
                }

                // 设置记录列表栏的滚动条（如果有）
                if (_recordBar.IsHandleCreated)
                {
                    SetWindowTheme(_recordBar.Handle, "", "");
                }
                else
                {
                    _recordBar.HandleCreated += (s, e) =>
                    {
                        SetWindowTheme(_recordBar.Handle, "", "");
                    };
                }

                // 确保在窗体显示后再次设置滚动条主题（提高成功率）
                Shown += (s, e) =>
                {
                    try
                    {
                        if (_contentPanel.IsHandleCreated)
                        {
                            SetContentPanelScrollBarTransparent();
                        }
                        // 延迟一点再设置，确保控件完全初始化
                        System.Threading.Thread.Sleep(50);
                        if (_contentPanel.IsHandleCreated)
                        {
                            SetContentPanelScrollBarTransparent();
                        }
                    }
                    catch { }
                };
            }
            catch
            {
                // 如果设置失败，使用默认滚动条
            }
        }

        /// <summary>
        /// 显示指定索引的记录
        /// </summary>
        private void ShowRecordAt(int index)
        {
            if (index < 0 || index >= _dt.Rows.Count) return;

            _currentIndex = index;
            var row = _dt.Rows[index];

            string title = Convert.ToString(row["Title"]) ?? "无标题";

            // 更新左侧当前记录名称
            _currentRecordName.Text = title;
            // 重新居中显示
            CenterCurrentRecordName();

            // 从 Content 列读取文件路径，然后读取文件内容
            string filePath = Convert.ToString(row["Content"]) ?? string.Empty;
            string content = LoadFileContent(filePath);

            _contentLabel.Text = content;

            // 重新计算内容宽度
            _contentLabel.MaximumSize = new Size(_contentPanel.ClientSize.Width - 20, 0);

            // 更新底部列表的高亮
            UpdateRecordListHighlight();
        }

        /// <summary>
        /// 根据文件路径加载文件内容
        /// </summary>
        private string LoadFileContent(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return "无内容";
            }

            try
            {
                // 尝试作为绝对路径
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath, Encoding.UTF8);
                }

                // 尝试从程序目录查找
                string fullPath = Path.Combine(AppContext.BaseDirectory, filePath);
                if (File.Exists(fullPath))
                {
                    return File.ReadAllText(fullPath, Encoding.UTF8);
                }

                // 尝试从项目根目录查找
                var projectRoot = GetProjectRoot();
                if (!string.IsNullOrEmpty(projectRoot))
                {
                    fullPath = Path.Combine(projectRoot, filePath);
                    if (File.Exists(fullPath))
                    {
                        return File.ReadAllText(fullPath, Encoding.UTF8);
                    }
                }

                return $"文件未找到：{filePath}";
            }
            catch (Exception ex)
            {
                return $"读取文件失败：{ex.Message}";
            }
        }

        /// <summary>
        /// 获取项目根目录
        /// </summary>
        private static string? GetProjectRoot()
        {
            try
            {
                var dir = AppContext.BaseDirectory;
                return Directory.GetParent(dir)?.Parent?.Parent?.Parent?.FullName;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 刷新记录列表显示（显示当前页的记录）
        /// </summary>
        private void RefreshRecordBar()
        {
            _recordBar.Controls.Clear();
            _selectedRecord = null;

            if (_dt.Rows.Count == 0)
            {
                UpdatePageButtons();
                return;
            }

            // 计算记录项大小
            int recordCount = 9;  // 固定显示9个记录项
            int itemWidth = Math.Max(120, (ClientSize.Width - 80) / recordCount - 8);
            int itemHeight = 72;

            // 计算当前页的起始索引
            int startIndex = _recordPage * recordCount;
            int endIndex = Math.Min(startIndex + recordCount, _dt.Rows.Count);

            // 显示当前页的记录
            for (int i = startIndex; i < endIndex; i++)
            {
                var row = _dt.Rows[i];
                string title = Convert.ToString(row["Title"]) ?? "无标题";

                var label = new Label
                {
                    Text = title,
                    AutoSize = false,
                    Width = itemWidth,
                    Height = itemHeight,
                    Font = _recordItemFont ?? new Font("Segoe UI", 11, FontStyle.Regular),
                    ForeColor = Color.FromArgb(196, 177, 169),
                    BackColor = Color.Transparent,
                    Padding = new Padding(8, 4, 8, 4),
                    Margin = new Padding(1, 21, 11, 6),  // 向下移动7像素：从14改为21
                    TextAlign = ContentAlignment.MiddleCenter,
                    Cursor = Cursors.Hand,
                    Tag = i
                };

                label.Click += (_, __) => ShowRecordAt((int)label.Tag);
                _recordBar.Controls.Add(label);

                // 高亮当前记录
                if (i == _currentIndex)
                {
                    HighlightRecord(label);
                }
            }

            // 更新翻页按钮状态
            UpdatePageButtons();
        }

        /// <summary>
        /// 翻页导航
        /// </summary>
        private void NavigateRecords(int direction)
        {
            int recordCount = 9;
            int maxPage = (_dt.Rows.Count + recordCount - 1) / recordCount - 1;
            
            int newPage = _recordPage + direction;
            if (newPage < 0) newPage = 0;
            if (newPage > maxPage) newPage = maxPage;

            if (newPage != _recordPage)
            {
                _recordPage = newPage;
                RefreshRecordBar();
            }
        }

        /// <summary>
        /// 更新翻页按钮的启用状态
        /// </summary>
        private void UpdatePageButtons()
        {
            int recordCount = 9;
            int maxPage = (_dt.Rows.Count + recordCount - 1) / recordCount - 1;
            
            _btnPrevPage.Enabled = _recordPage > 0;
            _btnNextPage.Enabled = _recordPage < maxPage;
            
            // 调整按钮透明度以显示状态
            _btnPrevPage.BackColor = _btnPrevPage.Enabled 
                ? Color.FromArgb(150, 0, 0, 0) 
                : Color.FromArgb(50, 0, 0, 0);
            _btnNextPage.BackColor = _btnNextPage.Enabled 
                ? Color.FromArgb(150, 0, 0, 0) 
                : Color.FromArgb(50, 0, 0, 0);
        }

        /// <summary>
        /// 更新记录列表的高亮状态（如果当前记录不在当前页，切换到对应页面）
        /// </summary>
        private void UpdateRecordListHighlight()
        {
            if (_currentIndex < 0 || _currentIndex >= _dt.Rows.Count) return;

            // 查找当前记录在哪一页
            int recordCount = 9;
            int targetPage = _currentIndex / recordCount;

            // 如果不在当前页，切换到对应页面
            if (targetPage != _recordPage)
            {
                _recordPage = targetPage;
                RefreshRecordBar();
            }
            else
            {
                // 在当前页，只更新高亮
                // 清除所有高亮
                if (_selectedRecord != null && !_selectedRecord.IsDisposed)
                {
                    _selectedRecord.BackColor = Color.Transparent;
                    _selectedRecord.Padding = new Padding(8, 4, 8, 4);
                }
                _selectedRecord = null;

                // 找到并高亮对应的记录
                foreach (Control control in _recordBar.Controls)
                {
                    if (control is Label label && label.Tag is int index)
                    {
                        if (index == _currentIndex)
                        {
                            HighlightRecord(label);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 高亮记录项
        /// </summary>
        private void HighlightRecord(Label label)
        {
            if (_selectedRecord != null && !_selectedRecord.IsDisposed)
            {
                _selectedRecord.BackColor = Color.Transparent;
                _selectedRecord.Padding = new Padding(8, 4, 8, 4);
            }
            _selectedRecord = label;
            _selectedRecord.BackColor = Color.Transparent;
            _selectedRecord.Padding = new Padding(8, 4, 8, 4);
        }

        /// <summary>
        /// 构建记录列表（初始化时调用）
        /// </summary>
        private void BuildRecordList()
        {
            RefreshRecordBar();
        }

        #endregion
    }
}