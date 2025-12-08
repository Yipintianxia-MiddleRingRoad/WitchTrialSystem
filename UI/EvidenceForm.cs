using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using System.Drawing.Text;
using System.Text.Json;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·证物界面：展示证物照片、基本信息与素材缩略图
    /// </summary>
    public class EvidenceForm : BasePokedexForm
    {
        #region 数据字段

        private const bool DebugLog = true;   // 调试开关，true 时输出查找路径信息

        private readonly EvidenceDAL _evidenceDal = new();
        private DataTable _dt = new();
        private int _current = -1;
        private bool _thumbnailsInitialized = false;  // 标记缩略图是否已初始化
        private int _thumbnailPage = 0;  // 缩略图当前页码
        private List<string> _allEvidenceImages = new();  // 所有证物图片列表
        
        // 自定义字体
        private PrivateFontCollection _fontCollection = new();
        private FontFamily? _customFontFamily;
        private readonly List<Font> _createdFonts = new();  // 保存所有创建的字体，用于释放

        #endregion

        #region UI 控件

        private readonly PictureBox _mainImage = new()
        {
            SizeMode = PictureBoxSizeMode.Zoom,
            Width = 280,  // 缩小主图尺寸
            Height = 280,  // 缩小主图尺寸
            BackColor = Color.Transparent,
            BorderStyle = BorderStyle.None
        };

        // 证物名称：使用两个Label实现第一个字大、其余字小
        private readonly Label _lblNameFirst = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 26, FontStyle.Bold),  // 第一个字：26号粗体
            ForeColor = Color.FromArgb(39, 33, 31),
            BackColor = Color.Transparent
        };

        private readonly Label _lblNameRest = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 18, FontStyle.Bold),  // 其余字：18号粗体
            ForeColor = Color.FromArgb(39, 33, 31),
            BackColor = Color.Transparent
        };

        private readonly Label _lblNumber = new()
        {
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Regular),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.White,
            BackColor = Color.FromArgb(150, 30, 30, 30),
            Padding = new Padding(6, 2, 6, 2)
        };

        private readonly Label _descTitle = new()
        {
            Text = "证物描述",
            AutoSize = true,
            Font = new Font("Segoe UI", 24, FontStyle.Bold),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.FromArgb(50, 50, 50),
            BackColor = Color.Transparent
        };

        private readonly Label _descContent = new()
        {
            AutoSize = true,  // 允许自动调整大小以适应内容
            Font = new Font("Segoe UI", 14, FontStyle.Regular),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.FromArgb(47, 35, 34),
            BackColor = Color.Transparent,
            MaximumSize = new Size(450, 0)  // 限制最大宽度为450，自动换行，高度不限制
        };

        private readonly Panel _descPanel = new()
        {
            BackColor = Color.Transparent,
            AutoScroll = true,
            HorizontalScroll = { Enabled = false, Visible = false },  // 禁用横向滚动
            VerticalScroll = { Enabled = true, Visible = true }  // 只允许纵向滚动
        };

        private readonly FlowLayoutPanel _materialBar = new()
        {
            Height = 100,  // 与 PokedexForm 一致
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoScroll = false,  // 禁用滚动，使用翻页按钮
            Padding = new Padding(40, 8, 40, 8),  // 与 PokedexForm 一致
            BackColor = Color.Transparent,  // 与 PokedexForm 一致
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right  // 使用 Anchor 而不是 Dock，可以更灵活控制位置
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

        private readonly Label _materialTitle = new()
        {
            Text = "证物素材",
            AutoSize = true,
            Font = new Font("Segoe UI", 13, FontStyle.Bold),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.White,
            BackColor = Color.Transparent
        };

        private readonly Label _emptyLabel = new()
        {
            Text = "暂无证物数据",
            AutoSize = true,
            Font = new Font("Segoe UI", 14, FontStyle.Italic),  // 将在 LoadCustomFont 后更新
            ForeColor = Color.DarkRed,
            BackColor = Color.Transparent
        };

        private PictureBox? _selectedThumb;


        // 设置文件路径
        private static string GetSettingsFilePath()
        {
            var appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WitchTrialSystem"
            );
            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }
            return Path.Combine(appDataPath, "EvidenceFormSettings.json");
        }

        // 设置类
        private class EvidenceFormSettings
        {
            public int MainImageLeft { get; set; }
            public int MainImageTop { get; set; }
        }

        #endregion

        /// <summary>
        /// 构造函数：初始化证物界面
        /// </summary>
        public EvidenceForm(string username) : base(username)
        {
            Text = "图鉴 · 证物";
            UpdateTitle();  // 添加用户信息到标题
            LoadCustomFont();  // 加载自定义字体
            InitializeLayout();
            Load += EvidenceForm_Load;
        }

        protected override string GetBackgroundImageName()
        {
            return "evidence_bg.png";  // 证物背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnEvidence.Enabled = false;
            _btnEvidence.Cursor = Cursors.Default;
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
                        LogDebug($"✅ 成功加载字体: {_customFontFamily.Name}");
                        
                        // 应用字体到所有控件
                        ApplyCustomFonts();
                    }
                    else
                    {
                        LogDebug("❌ 字体集合为空");
                    }
                }
                else
                {
                    LogDebug($"⚠️ 字体文件不存在: {fontPath}");
                }
            }
            catch (Exception ex)
            {
                LogDebug($"❌ 加载字体失败: {ex.Message}");
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
            var nameFirstFont = CreateCustomFont(26, FontStyle.Bold);
            if (nameFirstFont != null) _lblNameFirst.Font = nameFirstFont;
            
            var nameRestFont = CreateCustomFont(18, FontStyle.Bold);
            if (nameRestFont != null) _lblNameRest.Font = nameRestFont;

            var numberFont = CreateCustomFont(14, FontStyle.Regular);
            if (numberFont != null) _lblNumber.Font = numberFont;

            var descTitleFont = CreateCustomFont(24, FontStyle.Bold);
            if (descTitleFont != null) _descTitle.Font = descTitleFont;

            var descContentFont = CreateCustomFont(14, FontStyle.Regular);
            if (descContentFont != null) _descContent.Font = descContentFont;

            // 按钮使用图片，不需要设置字体

            var materialTitleFont = CreateCustomFont(13, FontStyle.Bold);
            if (materialTitleFont != null) _materialTitle.Font = materialTitleFont;

            var emptyLabelFont = CreateCustomFont(14, FontStyle.Italic);
            if (emptyLabelFont != null) _emptyLabel.Font = emptyLabelFont;
        }

        #endregion

        #region 初始化布局

        private void InitializeLayout()
        {
            // 左侧主图（向右下角移动一点）
            // 尝试加载保存的位置
            var savedSettings = LoadSettings();
            if (savedSettings != null && savedSettings.MainImageLeft > 0 && savedSettings.MainImageTop > 0)
            {
                // 验证保存的位置是否在有效范围内
                int validLeft = Math.Max(0, Math.Min(savedSettings.MainImageLeft, ClientSize.Width - _mainImage.Width));
                int validTop = Math.Max(0, Math.Min(savedSettings.MainImageTop, ClientSize.Height - _mainImage.Height));
                _mainImage.Left = validLeft;
                _mainImage.Top = validTop;
            }
            else
            {
                // 使用默认位置
                _mainImage.Left = 180;  // 向右平移30像素
                _mainImage.Top = 105;  // 向下移动20像素
            }
            _bg.Controls.Add(_mainImage);

            // 描述区域（收窄，最右边大幅收窄）- 向上移动82像素
            int rightMargin = 200;  // 最右边大幅收窄，留出200像素边距
            _descPanel.Left = ClientSize.Width / 2 + 35;  // 右半部分起始位置
            // 将描述区域上移82像素
            _descPanel.Top = Math.Max(ClientSize.Height / 3 - 82, 100);
            _descPanel.Width = ClientSize.Width - _descPanel.Left - rightMargin;  // 收窄宽度，最右边大幅收窄
            // 重新计算高度，稍微上收底部空间
            int availableHeight = ClientSize.Height - _descPanel.Top - 140;  // 比之前多保留 60 像素底部空间
            _descPanel.Height = Math.Max(200, availableHeight);  // 至少 200 高度
            // 不再显示“证物描述”标题，直接让描述文本顶到上方
            _descContent.Left = 0;
            _descContent.Top = 0;
            // 动态设置描述文本的最大宽度（适配收窄后的面板）
            _descContent.MaximumSize = new Size(_descPanel.Width - 20, 0);  // 留出左右边距
            _descPanel.Controls.Add(_descContent);
            _bg.Controls.Add(_descPanel);

            // 只显示证物名称（使用两个Label：第一个字大，其余字小）
            // 第一个字：向右移动10像素，向下移动27像素
            _lblNameFirst.Left = ClientSize.Width / 2 + 35 + 10;
            _lblNameFirst.Top = Math.Max(_descPanel.Top - 50 - 60 + 27, 0);
            _bg.Controls.Add(_lblNameFirst);
            
            // 第二个Label紧跟在第一个后面，底端对齐（位置会在ShowEvidenceAt中动态调整）
            // 后添加，确保显示在大字上层
            _bg.Controls.Add(_lblNameRest);
            _lblNameRest.BringToFront();  // 确保小字显示在最上层

            // 底部素材条（手动设置位置，让它更靠下）- 向上移动30像素
            // 删除了"证物素材"标题后，直接贴底显示
            _materialBar.Left = 0;
            _materialBar.Width = ClientSize.Width;
            _materialBar.Top = ClientSize.Height - _materialBar.Height - 10 - 30;  // 向上移动30像素
            _bg.Controls.Add(_materialBar);

            // 翻页按钮
            // 按钮中心线与缩略图列表中心线对齐
            int materialBarCenterY = _materialBar.Top + _materialBar.Height / 2;
            int buttonTopPosition = materialBarCenterY - _btnPrevPage.Height / 2;
            
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
            _btnPrevPage.Click += (s, e) => NavigateThumbnails(-1);
            _bg.Controls.Add(_btnPrevPage);
            _btnPrevPage.BringToFront();  // 确保按钮在最上层

            _btnNextPage.Left = ClientSize.Width - _btnNextPage.Width;  // 页面最右端
            _btnNextPage.Top = buttonTopPosition;
            _btnNextPage.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _btnNextPage.FlatAppearance.BorderSize = 0;
            _btnNextPage.FlatAppearance.MouseOverBackColor = Color.Transparent;
            _btnNextPage.FlatAppearance.MouseDownBackColor = Color.Transparent;
            _btnNextPage.Click += (s, e) => NavigateThumbnails(1);
            _bg.Controls.Add(_btnNextPage);
            _btnNextPage.BringToFront();  // 确保按钮在最上层
        }


        #endregion

        #region 数据加载与展示

        private void EvidenceForm_Load(object? sender, EventArgs e)
        {
            try
            {
                _dt = _evidenceDal.GetEvidenceItems();
                LogDebug($"证物总数：{_dt.Rows.Count}");
                
                // 调试：输出所有列名
                LogDebug($"数据表列：{string.Join(", ", _dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName))}");
                
                // 调试：输出第一条数据的所有字段值
                if (_dt.Rows.Count > 0)
                {
                    var firstRow = _dt.Rows[0];
                    LogDebug("第一条证物数据：");
                    foreach (DataColumn col in _dt.Columns)
                    {
                        var value = firstRow[col.ColumnName];
                        LogDebug($"  {col.ColumnName} = {value ?? "(null)"}");
                    }
                }
                
                if (_dt.Rows.Count == 0)
                {
                    ShowEmptyState();
                    return;
                }
                ShowEvidenceAt(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载证物数据失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogDebug($"加载失败异常：{ex}");
            }
        }

        private void ShowEmptyState()
        {
            _emptyLabel.Left = _mainImage.Left;
            _emptyLabel.Top = _mainImage.Top + _mainImage.Height / 2 - _emptyLabel.Height / 2;
            _bg.Controls.Add(_emptyLabel);
        }

        private void ShowEvidenceAt(int index)
        {
            if (index < 0 || index >= _dt.Rows.Count) return;
            _current = index;
            var row = _dt.Rows[index];

            var evidenceName = Convert.ToString(row["Name"]) ?? "未命名证物";
            
            // 使用两个Label设置证物名称：第一个字26号，其余字18号
            if (string.IsNullOrEmpty(evidenceName))
            {
                _lblNameFirst.Text = "";
                _lblNameRest.Text = "";
            }
            else if (evidenceName.Length == 1)
            {
                _lblNameFirst.Text = evidenceName;
                _lblNameRest.Text = "";
            }
            else
            {
                _lblNameFirst.Text = evidenceName[0].ToString();
                _lblNameRest.Text = evidenceName.Substring(1);
                
                // 紧贴第一个字（向左移动20像素），底端对齐
                _lblNameRest.Left = _lblNameFirst.Left + _lblNameFirst.Width - 20;
                // 底端对齐：大字底部 = 小字底部
                // 大字Top + 大字Height = 小字Top + 小字Height
                // 小字Top = 大字Top + 大字Height - 小字Height
                _lblNameRest.Top = _lblNameFirst.Top + _lblNameFirst.Height - _lblNameRest.Height;
            }
            
            _descContent.Text = Convert.ToString(row["Description"]) ?? "暂无描述。";

            var photoPath = ResolvePrimaryImagePath(row);
            LogDebug($"显示证物：{evidenceName}，主图={photoPath ?? "(null)"}");
            SetMainImage(photoPath);

            // 只在第一次加载时初始化缩略图，之后只更新高亮状态
            if (!_thumbnailsInitialized)
            {
                BuildMaterialThumbnails(row, photoPath);
                _thumbnailsInitialized = true;
                // 首次加载后，确保当前证物图片所在的页面被显示
                UpdateThumbnailHighlight(photoPath);
            }
            else
            {
                UpdateThumbnailHighlight(photoPath);
            }
        }

        #endregion

        #region 素材缩略图

        private void BuildMaterialThumbnails(DataRow row, string? mainImagePath)
        {
            // 收集所有证物图片（只在第一次调用时）
            if (_allEvidenceImages.Count == 0)
            {
                List<string> normalImages = new();
                List<string> witchImages = new();  // 包含"魔女"的图片

                foreach (DataRow evidenceRow in _dt.Rows)
                {
                    var imagePath = ResolvePrimaryImagePath(evidenceRow);
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                    {
                        if (!_allEvidenceImages.Contains(imagePath, StringComparer.OrdinalIgnoreCase))
                        {
                            // 检查文件名是否包含"魔女"
                            var fileName = Path.GetFileNameWithoutExtension(imagePath);
                            if (fileName.Contains("魔女", StringComparison.OrdinalIgnoreCase))
                            {
                                witchImages.Add(imagePath);
                            }
                            else
                            {
                                normalImages.Add(imagePath);
                            }
                        }
                    }
                }
                
                // 先排序普通图片
                normalImages.Sort(StringComparer.OrdinalIgnoreCase);
                // 再排序包含"魔女"的图片
                witchImages.Sort(StringComparer.OrdinalIgnoreCase);
                
                // 先添加普通图片，再添加"魔女"图片（放到后面）
                _allEvidenceImages.AddRange(normalImages);
                _allEvidenceImages.AddRange(witchImages);
            }

            // 显示当前页的缩略图（11张）
            RefreshThumbnails(mainImagePath);
        }

        /// <summary>
        /// 刷新缩略图显示（显示当前页的11张）
        /// </summary>
        private void RefreshThumbnails(string? mainImagePath)
        {
            _materialBar.Controls.Clear();
            _selectedThumb = null;

            if (_allEvidenceImages.Count == 0)
            {
                LogDebug("素材：0 张（不显示素材栏）");
                UpdatePageButtons();
                return;
            }

            // 计算缩略图大小（与 PokedexForm 一致）
            int thumbnailCount = 11;  // 固定显示11张
            int slotWidth = Math.Max(72, (ClientSize.Width - 80) / thumbnailCount - 8);
            int slotHeight = 72;

            // 计算当前页的起始索引
            int startIndex = _thumbnailPage * thumbnailCount;
            int endIndex = Math.Min(startIndex + thumbnailCount, _allEvidenceImages.Count);

            LogDebug($"素材栏：显示第 {_thumbnailPage + 1} 页，图片 {startIndex + 1}-{endIndex} / 共 {_allEvidenceImages.Count} 张");

            // 显示当前页的缩略图
            for (int i = startIndex; i < endIndex; i++)
            {
                var path = _allEvidenceImages[i];
                var thumb = new PictureBox
                {
                    Width = slotWidth,
                    Height = slotHeight,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Margin = new Padding(1, 14, 11, 6),  // 与 PokedexForm 一致
                    Cursor = Cursors.Hand,
                    BorderStyle = BorderStyle.None,  // 与 PokedexForm 一致
                    BackColor = Color.Transparent,  // 与 PokedexForm 一致
                    Tag = path
                };
                thumb.Image = LoadThumbnail(path);
                thumb.Click += (_, __) => OnThumbnailClick(thumb);
                _materialBar.Controls.Add(thumb);

                // 高亮当前证物对应的图片
                if (!string.IsNullOrEmpty(mainImagePath) &&
                    string.Equals(Path.GetFullPath(path), Path.GetFullPath(mainImagePath), StringComparison.OrdinalIgnoreCase))
                {
                    HighlightThumbnail(thumb);
                }
            }

            // 更新翻页按钮状态
            UpdatePageButtons();
        }

        /// <summary>
        /// 翻页导航
        /// </summary>
        private void NavigateThumbnails(int direction)
        {
            int thumbnailCount = 11;
            int maxPage = (_allEvidenceImages.Count + thumbnailCount - 1) / thumbnailCount - 1;
            
            int newPage = _thumbnailPage + direction;
            if (newPage < 0) newPage = 0;
            if (newPage > maxPage) newPage = maxPage;

            if (newPage != _thumbnailPage)
            {
                _thumbnailPage = newPage;
                var currentPhotoPath = ResolvePrimaryImagePath(_dt.Rows[_current]);
                RefreshThumbnails(currentPhotoPath);
            }
        }

        /// <summary>
        /// 更新翻页按钮的启用状态
        /// </summary>
        private void UpdatePageButtons()
        {
            int thumbnailCount = 11;
            int maxPage = (_allEvidenceImages.Count + thumbnailCount - 1) / thumbnailCount - 1;
            
            _btnPrevPage.Enabled = _thumbnailPage > 0;
            _btnNextPage.Enabled = _thumbnailPage < maxPage;
            
            // 使用图片按钮，不需要调整背景颜色
        }

        /// <summary>
        /// 更新缩略图高亮状态（如果当前图片不在当前页，切换到对应页面）
        /// </summary>
        private void UpdateThumbnailHighlight(string? mainImagePath)
        {
            if (string.IsNullOrEmpty(mainImagePath)) return;

            // 查找当前图片在哪一页
            int imageIndex = _allEvidenceImages.FindIndex(p => 
                string.Equals(Path.GetFullPath(p), Path.GetFullPath(mainImagePath), StringComparison.OrdinalIgnoreCase));

            if (imageIndex >= 0)
            {
                int thumbnailCount = 11;
                int targetPage = imageIndex / thumbnailCount;

                // 如果不在当前页，切换到对应页面
                if (targetPage != _thumbnailPage)
                {
                    _thumbnailPage = targetPage;
                    RefreshThumbnails(mainImagePath);
                }
                else
                {
                    // 在当前页，只更新高亮
                    // 清除所有高亮
                    if (_selectedThumb != null && !_selectedThumb.IsDisposed)
                    {
                        _selectedThumb.BackColor = Color.Transparent;
                        _selectedThumb.Padding = new Padding(0);
                    }
                    _selectedThumb = null;

                    // 找到并高亮对应的缩略图
                    foreach (Control control in _materialBar.Controls)
                    {
                        if (control is PictureBox thumb && thumb.Tag is string path)
                        {
                            if (string.Equals(Path.GetFullPath(path), Path.GetFullPath(mainImagePath), StringComparison.OrdinalIgnoreCase))
                            {
                                HighlightThumbnail(thumb);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void OnThumbnailClick(PictureBox thumb)
        {
            if (thumb.Tag is not string path) return;
            
            // 从图片路径中提取序号（例如 Clue_005_000.png -> 000 -> 0）
            var fileName = Path.GetFileNameWithoutExtension(path);
            var match = System.Text.RegularExpressions.Regex.Match(fileName, @"Clue_005_(\d+)");
            
            if (match.Success && int.TryParse(match.Groups[1].Value, out int imageIndex))
            {
                // 根据图片序号找到对应的证物（EvidenceNo 应该匹配）
                // 查找 EvidenceNo 等于 imageIndex 的证物
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    var row = _dt.Rows[i];
                    var evidenceNo = Convert.ToString(row["EvidenceNo"]) ?? string.Empty;
                    
                    // 尝试解析 EvidenceNo
                    if (int.TryParse(evidenceNo, out int evidenceNoInt) && evidenceNoInt == imageIndex)
                    {
                        // 找到对应的证物，更新显示（ShowEvidenceAt 内部会处理高亮）
                        ShowEvidenceAt(i);
                        return;
                    }
                }
                
                // 如果没找到精确匹配，尝试字符串匹配
                var evidenceNoStr = imageIndex.ToString();
                for (int i = 0; i < _dt.Rows.Count; i++)
                {
                    var row = _dt.Rows[i];
                    var evidenceNo = Convert.ToString(row["EvidenceNo"]) ?? string.Empty;
                    if (evidenceNo.Trim() == evidenceNoStr)
                    {
                        // 找到对应的证物，更新显示（ShowEvidenceAt 内部会处理高亮）
                        ShowEvidenceAt(i);
                        return;
                    }
                }
            }
            
            // 如果无法匹配到证物，至少切换图片
            SetMainImage(path);
            UpdateThumbnailHighlight(path);
        }

        private void HighlightThumbnail(PictureBox thumb)
        {
            // 不再显示高亮效果，只记录选中的缩略图
            if (_selectedThumb != null && !_selectedThumb.IsDisposed)
            {
                _selectedThumb.BackColor = Color.Transparent;
                _selectedThumb.Padding = new Padding(0);
            }
            _selectedThumb = thumb;
            // 移除黄色背景高亮
        }

        #endregion

        #region 辅助方法

        private IEnumerable<string> ResolveMaterialImagePaths(DataRow row)
        {
            var results = new List<string>();

            // 方案一：数据库列 MaterialImages（以 ; 分隔）
            if (_dt.Columns.Contains("MaterialImages"))
            {
                var raw = Convert.ToString(row["MaterialImages"]);
                if (!string.IsNullOrWhiteSpace(raw))
                {
                    LogDebug($"方案一：MaterialImages = {raw}");
                    var parts = raw.Split(new[] { ';', '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in parts)
                    {
                        var path = ResolveExistingFilePath(item.Trim());
                        if (File.Exists(path))
                        {
                            results.Add(path);
                            LogDebug($"  找到素材：{path}");
                        }
                        else
                        {
                            LogDebug($"  未找到：{item.Trim()} → {path}");
                        }
                    }
                }
            }

            // 方案二：MaterialFolder 指向包含素材的目录
            if (results.Count == 0 && _dt.Columns.Contains("MaterialFolder"))
            {
                var folder = Convert.ToString(row["MaterialFolder"]);
                if (!string.IsNullOrWhiteSpace(folder))
                {
                    LogDebug($"方案二：MaterialFolder = {folder}");
                    var resolvedFolder = ResolveExistingDirectory(folder);
                    if (!string.IsNullOrWhiteSpace(resolvedFolder) && Directory.Exists(resolvedFolder))
                    {
                        var files = Directory.GetFiles(resolvedFolder, "*.png");
                        results.AddRange(files);
                        LogDebug($"  在目录中找到 {files.Length} 个文件：{resolvedFolder}");
                    }
                    else
                    {
                        LogDebug($"  目录不存在：{folder} → {resolvedFolder ?? "(null)"}");
                    }
                }
            }

            // 方案三：通过证物序号匹配默认命名（Clue_005_YYY.png，005是固定的）
            // 每个证物只对应一张图片，根据 EvidenceNo 决定
            if (results.Count == 0)
            {
                var no = Convert.ToString(row["EvidenceNo"]) ?? string.Empty;
                LogDebug($"方案三：EvidenceNo = {no}");
                
                var evidenceDir = ResolveExistingDirectory(Path.Combine("Images", "Evidence"));
                LogDebug($"  查找目录：Images/Evidence → {evidenceDir ?? "(null)"}");
                
                if (!string.IsNullOrWhiteSpace(evidenceDir) && Directory.Exists(evidenceDir))
                {
                    // 根据 EvidenceNo 确定图片序号（000, 001, 002...）
                    // 如果 EvidenceNo 是数字，直接使用；否则尝试解析
                    string imageSuffix = "000";
                    if (int.TryParse(no, out int evidenceIndex))
                    {
                        // EvidenceNo 是数字，格式化为三位数（0 -> 000, 1 -> 001）
                        imageSuffix = evidenceIndex.ToString("D3");
                    }
                    else
                    {
                        // 如果不是数字，尝试从字符串中提取数字，或使用默认值
                        var match = System.Text.RegularExpressions.Regex.Match(no, @"\d+");
                        if (match.Success && int.TryParse(match.Value, out int extracted))
                        {
                            imageSuffix = extracted.ToString("D3");
                        }
                    }
                    
                    // Clue_005_ 是固定前缀，根据 EvidenceNo 选择对应的单张图片
                    var specificImage = Path.Combine(evidenceDir, $"Clue_005_{imageSuffix}.png");
                    if (File.Exists(specificImage))
                    {
                        results.Add(specificImage);
                        LogDebug($"  找到对应图片：Clue_005_{imageSuffix}.png");
                    }
                    else
                    {
                        // 找不到对应图片，不显示任何图片
                        LogDebug($"  未找到对应图片：Clue_005_{imageSuffix}.png（不显示）");
                    }
                }
                else
                {
                    LogDebug($"  证物图片目录不存在：{evidenceDir ?? "(null)"}");
                }
            }

            var finalResults = results.Where(File.Exists).Distinct().ToList();
            // 确保结果按文件名排序
            finalResults.Sort(StringComparer.OrdinalIgnoreCase);
            LogDebug($"素材解析完成，共找到 {finalResults.Count} 个有效文件");
            return finalResults;
        }

        private string? ResolvePrimaryImagePath(DataRow row)
        {
            // 优先使用 PhotoPath，其次 ImagePath（兼容你刚添加的列名）
            if (_dt.Columns.Contains("PhotoPath"))
            {
                var photo = Convert.ToString(row["PhotoPath"]);
                if (!string.IsNullOrWhiteSpace(photo))
                {
                    var resolved = ResolveExistingFilePath(photo);
                    if (!string.IsNullOrWhiteSpace(resolved) && File.Exists(resolved))
                    {
                        LogDebug($"PhotoPath 命中：{resolved}");
                        return resolved;
                    }
                    else
                    {
                        LogDebug($"PhotoPath 未找到文件：原始值={photo}, 解析后={resolved}");
                    }
                }
            }
            if (_dt.Columns.Contains("ImagePath"))
            {
                var photo = Convert.ToString(row["ImagePath"]);
                if (!string.IsNullOrWhiteSpace(photo))
                {
                    var resolved = ResolveExistingFilePath(photo);
                    if (!string.IsNullOrWhiteSpace(resolved) && File.Exists(resolved))
                    {
                        LogDebug($"ImagePath 命中：{resolved}");
                        return resolved;
                    }
                    else
                    {
                        LogDebug($"ImagePath 未找到文件：原始值={photo}, 解析后={resolved}");
                    }
                }
            }

            // 如果数据库字段都没有，尝试通过素材路径或序号匹配
            LogDebug("尝试通过素材路径或序号匹配查找主图...");
            var allMaterials = ResolveMaterialImagePaths(row).ToList();
            
            if (allMaterials.Count > 0)
            {
                // 每个证物只对应一张图片，直接使用第一张（也是唯一一张）
                LogDebug($"找到主图：{allMaterials[0]}");
                return allMaterials[0];
            }
            else
            {
                LogDebug("未找到任何图片（包括素材路径和序号匹配）");
                return null;
            }
        }

        private static string NormalizeEvidenceNo(string rawNo)
        {
            if (string.IsNullOrWhiteSpace(rawNo))
                return "000";

            rawNo = rawNo.Trim();

            // 情况 1：纯数字（例如 1 / 5 / 12 / 658）→ 补齐三位，便于与 005 这种命名匹配
            if (rawNo.All(char.IsDigit) && rawNo.Length <= 3 && int.TryParse(rawNo, out int num))
            {
                return num.ToString("D3");
            }

            // 情况 2：包含下划线或其他字符（例如 005_000、005-01），直接原样返回
            // 这样就可以和 Clue_005_000.png 这一类名字直接对应
            return rawNo;
        }

        private static string ResolveExistingFilePath(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return string.Empty;

            var normalized = path.Replace('/', Path.DirectorySeparatorChar);

            // 1) 绝对路径
            if (Path.IsPathRooted(normalized) && File.Exists(normalized))
                return normalized;

            // 2) 输出目录（bin/...）
            var inBin = Path.Combine(AppContext.BaseDirectory, normalized);
            if (File.Exists(inBin))
                return inBin;

            // 3) 项目根目录（bin/Debug/net9.0-windows/../../..）
            var projectRoot = GetProjectRoot();
            if (!string.IsNullOrEmpty(projectRoot))
            {
                var inRoot = Path.Combine(projectRoot, normalized);
                if (File.Exists(inRoot))
                    return inRoot;
            }

            return inBin; // 返回一个合理的尝试路径，便于后续判空
        }

        private static string? ResolveExistingDirectory(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;

            var normalized = path.Replace('/', Path.DirectorySeparatorChar);

            if (Path.IsPathRooted(normalized) && Directory.Exists(normalized))
                return normalized;

            var inBin = Path.Combine(AppContext.BaseDirectory, normalized);
            if (Directory.Exists(inBin))
                return inBin;

            var projectRoot = GetProjectRoot();
            if (!string.IsNullOrEmpty(projectRoot))
            {
                var inRoot = Path.Combine(projectRoot, normalized);
                if (Directory.Exists(inRoot))
                    return inRoot;
            }

            return null;
        }

        private static string? GetProjectRoot()
        {
            try
            {
                // bin/Debug/net9.0-windows → up three levels
                var dir = AppContext.BaseDirectory;
                return Directory.GetParent(dir)?.Parent?.Parent?.Parent?.FullName;
            }
            catch
            {
                return null;
            }
        }

        private void SetMainImage(string? path)
        {
            var previous = _mainImage.Image;
            _mainImage.Image = null;
            previous?.Dispose();

            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                _mainImage.Image = LoadBitmap(path);
                LogDebug($"主图加载成功：{path}");
            }
            else
            {
                // 找不到对应图片，不显示占位图，保持空白
                _mainImage.Image = null;
                LogDebug($"主图未找到：{path ?? "(null)"}（不显示占位图）");
            }
        }

        private static Image? LoadThumbnail(string path)
        {
            try
            {
                return LoadBitmap(path);
            }
            catch
            {
                return null;
            }
        }

        private static Bitmap LoadBitmap(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var temp = Image.FromStream(fs);
            return new Bitmap(temp);
        }

        /// <summary>
        /// 设置证物名称标签（删除"证物序号："前缀，直接显示名称）
        /// 第一个字保持原大小，其余字缩小
        /// </summary>
        private void SetEvidenceNumberLabel(string evidenceName)
        {
            if (string.IsNullOrEmpty(evidenceName))
            {
                _lblNumber.Text = "";
                return;
            }

            // 直接显示证物名称（不带"证物序号："前缀）
            _lblNumber.Text = evidenceName;
            
            // 调整字体：整体使用12号字体（比原来的14号略小）
            // 注：Label控件无法实现单个字符不同大小，所以统一缩小
            var smallerFont = CreateCustomFont(12, FontStyle.Regular);
            if (smallerFont != null)
            {
                _lblNumber.Font = smallerFont;
            }
            else
            {
                _lblNumber.Font = new Font("Segoe UI", 12, FontStyle.Regular);
            }
        }

        private void LogDebug(string message)
        {
            if (!DebugLog) return;
            try { Console.WriteLine("[EvidenceForm] " + message); } catch { /* ignore */ }
        }

        /// <summary>
        /// 保存主图位置设置
        /// </summary>
        private void SaveSettings()
        {
            try
            {
                var settings = new EvidenceFormSettings
                {
                    MainImageLeft = _mainImage.Left,
                    MainImageTop = _mainImage.Top
                };

                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                var filePath = GetSettingsFilePath();
                File.WriteAllText(filePath, json);
                LogDebug($"设置已保存: 主图(Left={settings.MainImageLeft}, Top={settings.MainImageTop})");
            }
            catch (Exception ex)
            {
                LogDebug($"保存设置失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 加载主图位置设置
        /// </summary>
        private EvidenceFormSettings? LoadSettings()
        {
            try
            {
                var filePath = GetSettingsFilePath();
                if (!File.Exists(filePath))
                {
                    LogDebug("设置文件不存在，使用默认位置");
                    return null;
                }

                var json = File.ReadAllText(filePath);
                var settings = JsonSerializer.Deserialize<EvidenceFormSettings>(json);
                
                if (settings != null)
                {
                    LogDebug($"设置已加载: 主图(Left={settings.MainImageLeft}, Top={settings.MainImageTop})");
                }
                
                return settings;
            }
            catch (Exception ex)
            {
                LogDebug($"加载设置失败: {ex.Message}，使用默认位置");
                return null;
            }
        }


        #endregion
    }
}
