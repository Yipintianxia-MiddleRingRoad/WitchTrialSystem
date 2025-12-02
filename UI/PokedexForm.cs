using System;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·人物界面
    /// 功能：展示魔女详细信息（头像、姓名、囚犯编号、魔法、描述）
    /// 特色：支持自定义字体、姓名图片、缩略图导航
    /// </summary>
    public class PokedexForm : Form
    {
        #region 核心字段（数据访问+基础配置）
        private readonly string _username;
        private readonly UserProfileDAL _profileDal = new();
        private readonly WitchDAL _dal = new();
        private readonly PermissionDAL _permissionDal = new();
        private DataTable _dt = new();
        private int _current = 0;
        
        // 自定义字体
        private PrivateFontCollection _fontCollection = new();
        private Font? _customFont;
        #endregion

        #region 姓名映射配置（已废弃，现在直接使用囚犯编号）
        /// <summary>
        /// 姓名映射字典（已废弃）
        /// 现在姓名图片直接使用囚犯编号命名，不再需要映射表
        /// 保留此字典仅为了向后兼容，实际代码中不再使用
        /// </summary>
        [Obsolete("现在直接使用囚犯编号查找图片，此映射表已不再使用")]
        private static readonly Dictionary<string, string> NameMapping = new()
        {
            { "樱羽艾玛", "ema" }, { "二阶堂希罗", "hiro" }, { "夏目安安", "anan" },
            { "城崎诺亚", "noah" }, { "莲见蕾娅", "leia" }, { "佐伯米莉亚", "meria" },
            { "宝生玛格", "margo" }, { "黑部奈叶香", "nanoka" }, { "紫藤爱丽莎", "alisa" },
            { "橘雪莉", "sherry" }, { "远野汉娜", "hanna" }, { "泽渡可可", "coco" },
            { "冰上梅露露", "meruru" }
        };
        #endregion

        #region UI控件字段（按功能分区）
        // 背景容器
        private readonly Panel _bg = new() { Dock = DockStyle.Fill, BackgroundImageLayout = ImageLayout.Stretch };

        // 左侧：大头像+姓名
        private readonly PictureBox _bigAvatar = new() 
        { 
            SizeMode = PictureBoxSizeMode.Zoom, 
            Width = 350, 
            Height = 350, 
            BackColor = Color.Transparent 
        };
        
        // 独立的自定义绘制 Panel 用于姓名控件
        private readonly CustomDrawPanel _namePanel = new() 
        { 
            BackColor = Color.Transparent, 
            Width = 400, 
            Height = 150 
        };
        
        private readonly Label _nameLabel = new() 
        { 
            AutoSize = true, 
            ForeColor = Color.White, 
            BackColor = Color.FromArgb(140, 0, 0, 0),
            Font = new Font("Segoe UI", 28, FontStyle.Bold),
            Padding = new Padding(6, 2, 6, 2)
        };
        private readonly PictureBox _nameImage = new() 
        { 
            SizeMode = PictureBoxSizeMode.Zoom, 
            BackColor = Color.Transparent, 
            BorderStyle = BorderStyle.None, 
            Visible = false 
        };

        // 右侧：描述容器+内容控件
        private readonly Panel _rightPanel = new() { BackColor = Color.Transparent };
        private readonly Label _descTitle = new() 
        { 
            Text = "Description", 
            AutoSize = true, 
            Font = new Font("Segoe UI", 18, FontStyle.Bold), 
            ForeColor = Color.FromArgb(60, 60, 60), 
            BackColor = Color.Transparent 
        };
        private readonly Label _prisonerNoLabel = new() { Text = "囚犯编号：", AutoSize = true, BackColor = Color.Transparent };
        private readonly Label _magicLabel = new() { Text = "魔法：", AutoSize = true, BackColor = Color.Transparent };
        private readonly Label _lblPrisonerNo = new() { AutoSize = true, BackColor = Color.Transparent };
        private readonly Label _lblMagic = new() { AutoSize = true, BackColor = Color.Transparent };

        // 描述滚动容器
        private readonly Panel _descScrollHost = new() { BackColor = Color.Transparent, AutoScroll = true };
        private readonly Label _descContent = new() 
        { 
            AutoSize = true, 
            ForeColor = Color.FromArgb(50, 50, 50), 
            BackColor = Color.Transparent 
        };

        // 底部：缩略图条
        private readonly FlowLayoutPanel _thumbBar = new() 
        { 
            Dock = DockStyle.Bottom, 
            Height = 140, 
            FlowDirection = FlowDirection.LeftToRight, 
            WrapContents = false, 
            Padding = new Padding(40, 8, 40, 8), 
            BackColor = Color.Transparent 
        };

        // 右上角：退出按钮
        private readonly Panel _logoutButton = new() 
        { 
            Size = new Size(42, 42), 
            BackColor = Color.Transparent, 
            Cursor = Cursors.Hand, 
            Anchor = AnchorStyles.Top | AnchorStyles.Right 
        };

        // 右侧导航按钮（在缩略图上方）
        private readonly Panel _btnEvidence = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnCharacters = new() { Size = new Size(100, 50), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnMap = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnRules = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        private readonly Panel _btnRecords = new() { Size = new Size(100, 55), BackColor = Color.Transparent, Cursor = Cursors.Hand };
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数：初始化图鉴·人物界面
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public PokedexForm(string username)
        {
            _username = username;
            LoadCustomFont();           // 1. 加载自定义字体
            InitializeFormSettings();   // 2. 初始化窗体设置
            LoadBackgroundImage();      // 3. 加载背景图
            InitializeControlsLayout(); // 4. 初始化控件布局
            SetupNavigationButtons();   // 5. 设置导航按钮
            BindEvents();               // 6. 绑定事件
        }
        
        #endregion

        #region 初始化方法
        /// <summary>
        /// 加载自定义字体
        /// </summary>
        private void LoadCustomFont()
        {
            try
            {
                string fontPath = Path.Combine(AppContext.BaseDirectory, "Fonts", "方正小标宋简.ttf");
                
                // 调试信息
                string debugInfo = $"程序目录: {AppContext.BaseDirectory}\n字体路径: {fontPath}\n文件存在: {File.Exists(fontPath)}";
                Console.WriteLine(debugInfo);
                
                if (File.Exists(fontPath))
                {
                    _fontCollection.AddFontFile(fontPath);
                    if (_fontCollection.Families.Length > 0)
                    {
                        _customFont = new Font(_fontCollection.Families[0], 12, FontStyle.Regular);
                        Console.WriteLine($"✅ 成功加载字体: {_fontCollection.Families[0].Name}");
                        // MessageBox.Show($"字体加载成功！\n字体名称: {_fontCollection.Families[0].Name}", "字体加载");
                    }
                    else
                    {
                        Console.WriteLine("❌ 字体集合为空");
                        MessageBox.Show($"字体文件存在但无法加载！\n{fontPath}", "字体加载失败");
                    }
                }
                else
                {
                    Console.WriteLine($"⚠️ 字体文件不存在: {fontPath}");
                    MessageBox.Show($"字体文件不存在！\n{fontPath}\n\n请确保字体文件在 Fonts 文件夹中", "字体加载失败");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ 加载字体失败: {ex.Message}");
                MessageBox.Show($"加载字体时出错：\n{ex.Message}", "字体加载错误");
            }
        }

        /// <summary>
        /// 初始化窗体基础设置
        /// </summary>
        private void InitializeFormSettings()
        {
            Text = $"图鉴 · 人物 (当前用户：{_username})";
            StartPosition = FormStartPosition.CenterScreen;
            Width = 1280;
            Height = 760;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            DoubleBuffered = true;
            KeyPreview = true;
        }

        /// <summary>
        /// 加载背景图
        /// </summary>
        private void LoadBackgroundImage()
        {
            string bgPath = Path.Combine(AppContext.BaseDirectory, "Images", "ui", "pokedex_bg.png");
            if (File.Exists(bgPath))
                _bg.BackgroundImage = Image.FromFile(bgPath);
            Controls.Add(_bg);
        }

        /// <summary>
        /// 初始化所有控件布局
        /// </summary>
        private void InitializeControlsLayout()
        {
            // 左侧大头像
            _bg.Controls.Add(_bigAvatar);
            _bigAvatar.Left = 125;  // 120 + 5
            _bigAvatar.Top = 120;
            _bigAvatar.SendToBack();

            // 创建独立的姓名 Panel
            _bg.Controls.Add(_namePanel);
            _namePanel.Left = _bigAvatar.Left - 80;  // Panel 左边界（45）
            _namePanel.Top = _bigAvatar.Top + _bigAvatar.Height - 150;  // Panel 顶部
            _namePanel.BringToFront();

            // 姓名控件添加到 Panel 中
            _namePanel.Controls.Add(_nameLabel);
            _namePanel.Controls.Add(_nameImage);

            // 右侧面板（占屏幕右半部分）
            _rightPanel.Width = ClientSize.Width / 2;
            _rightPanel.Height = 520;
            _rightPanel.Left = ClientSize.Width / 2 + 35;  // 向右移动35像素（15+20）
            _rightPanel.Top = _bigAvatar.Top;
            _bg.Controls.Add(_rightPanel);

            // 右侧标题
            _rightPanel.Controls.Add(_descTitle);
            _descTitle.Left = 0;
            _descTitle.Top = 0;

            // 囚犯编号
            _rightPanel.Controls.Add(_prisonerNoLabel);
            _prisonerNoLabel.Left = 0;
            _prisonerNoLabel.Top = 48;
            if (_customFont != null) _prisonerNoLabel.Font = _customFont;  // 应用自定义字体
            
            _rightPanel.Controls.Add(_lblPrisonerNo);
            _lblPrisonerNo.Left = _prisonerNoLabel.Right + 6;
            _lblPrisonerNo.Top = 48;
            _lblPrisonerNo.Width = _rightPanel.Width - _lblPrisonerNo.Left - 20;
            if (_customFont != null) _lblPrisonerNo.Font = _customFont;  // 应用自定义字体

            // 魔法类型
            _rightPanel.Controls.Add(_magicLabel);
            _magicLabel.Left = 0;
            _magicLabel.Top = 76;
            if (_customFont != null) _magicLabel.Font = _customFont;  // 应用自定义字体
            
            _rightPanel.Controls.Add(_lblMagic);
            _lblMagic.Left = 80;
            _lblMagic.Top = 76;
            if (_customFont != null) _lblMagic.Font = _customFont;  // 应用自定义字体

            // 描述滚动容器
            _rightPanel.Controls.Add(_descScrollHost);
            _descScrollHost.Left = 0;
            _descScrollHost.Top = 110;
            _descScrollHost.Width = _rightPanel.Width;
            _descScrollHost.Height = _rightPanel.Height - 120;
            _descScrollHost.Controls.Add(_descContent);
            
            // 应用自定义字体到描述内容
            if (_customFont != null) _descContent.Font = _customFont;

            // 描述文本缩进（9个汉字宽度）
            int charWidth = TextRenderer.MeasureText("汉", _descContent.Font).Width;
            _descContent.MaximumSize = new Size(_descScrollHost.ClientSize.Width - 10 - charWidth * 9, 0);

            // 底部缩略图条
            _bg.Controls.Add(_thumbBar);
        }

        /// <summary>
        /// 设置右侧导航按钮
        /// </summary>
        private void SetupNavigationButtons()
        {
            // 按钮位置（根据精确的像素范围）
            int rightX = ClientSize.Width - 110;

            // 按钮Y坐标（范围：140-195, 210-260, 275-330, 345-400, 415-470）
            _btnEvidence.Location = new Point(rightX, 140);
            _btnCharacters.Location = new Point(rightX, 210);
            _btnMap.Location = new Point(rightX, 275);
            _btnRules.Location = new Point(rightX, 345);
            _btnRecords.Location = new Point(rightX, 415);

            // 添加到背景
            _bg.Controls.Add(_btnEvidence);
            _bg.Controls.Add(_btnCharacters);
            _bg.Controls.Add(_btnMap);
            _bg.Controls.Add(_btnRules);
            _bg.Controls.Add(_btnRecords);

            // 禁用当前页面的按钮（人物）
            _btnCharacters.Enabled = false;
            _btnCharacters.Cursor = Cursors.Default;

            // 绑定点击事件
            _btnEvidence.Click += (s, e) => NavigateTo(new EvidenceForm(_username));
            _btnMap.Click += (s, e) => NavigateTo(new MapForm(_username));
            _btnRules.Click += (s, e) => NavigateTo(new RulesForm(_username));
            _btnRecords.Click += (s, e) => NavigateTo(new RecordsForm(_username));

            // 确保按钮在最上层
            _btnEvidence.BringToFront();
            _btnCharacters.BringToFront();
            _btnMap.BringToFront();
            _btnRules.BringToFront();
            _btnRecords.BringToFront();
        }

        /// <summary>
        /// 导航到其他页面
        /// </summary>
        private void NavigateTo(Form newForm)
        {
            newForm.FormClosed += (s, e) => this.Close();
            this.Hide();
            newForm.Show();
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        private void BindEvents()
        {
            _thumbBar.BringToFront();

            // 退出按钮
            _logoutButton.Left = ClientSize.Width - 22 - _logoutButton.Width;
            _logoutButton.Top = 18;
            _bg.Controls.Add(_logoutButton);
            new ToolTip().SetToolTip(_logoutButton, "退出登录");

            Load += OnFormLoad;
            _logoutButton.Click += (_, __) => DoLogout();
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) DoLogout(); };
        }
        #endregion

        #region 事件处理方法
        
        /// <summary>
        /// 窗体加载事件：加载魔女数据并显示第一个角色
        /// </summary>
        private void OnFormLoad(object? sender, EventArgs e)
        {
            try
            {
                // 获取当前用户的岛/批信息
                var userProfile = _profileDal.GetProfile(_username);
                int? islandId = userProfile.Rows.Count > 0 ? userProfile.Rows[0].Field<int?>("IslandID") : null;
                int? batchId = userProfile.Rows.Count > 0 ? userProfile.Rows[0].Field<int?>("BatchID") : null;

                // 使用PermissionDAL获取有权限的魔女数据（确保权限正确）
                var rawData = _permissionDal.GetWitchesByPermission(_username, null);
                
                // 如果是普通魔女角色，数据已经按权限筛选了
                // 如果是管理员、Meruru、Warden，需要进一步按岛屿和批次筛选
                if (islandId.HasValue)
                {
                    var view = new DataView(rawData);
                    var filter = $"IslandID = {islandId.Value}";
                    if (batchId.HasValue)
                        filter += $" AND BatchID = {batchId.Value}";
                    view.RowFilter = filter;
                    rawData = view.ToTable();
                }
                
                // 去重处理（按囚犯编号）
                var distinctData = rawData.AsEnumerable()
                    .GroupBy(row => Convert.ToString(row["PrisonerNo"]))
                    .Select(group => group.First()) // 每个PrisonerNo只取第一条记录
                    .OrderBy(row => int.TryParse(Convert.ToString(row["PrisonerNo"]), out int no) ? no : 9999);
                _dt = distinctData.CopyToDataTable();

                // 构建缩略图+默认显示第一个角色
                BuildThumbnails();
                if (_dt.Rows.Count > 0)
                    ShowCharacterAt(0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载图鉴失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 构建底部缩略图列表
        /// </summary>
        private void BuildThumbnails()
        {
            _thumbBar.Controls.Clear();
            int characterCount = Math.Min(13, _dt.Rows.Count);
            int slotWidth = Math.Max(72, (ClientSize.Width - 80) / 13 - 8);
            int slotHeight = 72;

            for (int i = 0; i < characterCount; i++)
            {
                DataRow row = _dt.Rows[i];
                PictureBox thumb = new()
                {
                    Width = slotWidth,
                    Height = slotHeight,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Cursor = Cursors.Hand,
                    Margin = new Padding(1, 14, 11, 6),
                    BorderStyle = BorderStyle.None,
                    BackColor = Color.Transparent,
                    Tag = i
                };

                // 加载缩略图（优先数据库路径，其次占位图）
                thumb.Image = LoadImageFromPath(Convert.ToString(row["AvatarPath"]));
                thumb.Click += (_, __) => ShowCharacterAt((int)thumb.Tag);
                _thumbBar.Controls.Add(thumb);
            }
        }

        /// <summary>
        /// 显示指定索引的角色详情
        /// </summary>
        private void ShowCharacterAt(int index)
        {
            if (index < 0 || index >= _dt.Rows.Count) return;
            _current = index;
            DataRow row = _dt.Rows[_current];

            // 显示大头像
            _bigAvatar.Image = LoadImageFromPath(Convert.ToString(row["AvatarPath"]));

            // 显示姓名（优先图片，其次文字）
            string characterName = Convert.ToString(row["Name"]) ?? "";
            string prisonerNo = Convert.ToString(row["PrisonerNo"]) ?? "";
            ShowCharacterName(characterName, prisonerNo);

            // 显示右侧信息
            _lblPrisonerNo.Text = Convert.ToString(row["PrisonerNo"]) ?? "—";
            _lblMagic.Text = Convert.ToString(row["Magic"]) ?? "—";
            _descContent.Text = Convert.ToString(row["DescriptionPublic"]) ?? "";

            // 重新计算描述文本宽度（适配容器）
            int charWidth = TextRenderer.MeasureText("汉", _descContent.Font).Width;
            _descContent.MaximumSize = new Size(_descScrollHost.ClientSize.Width - 10 - charWidth * 9, 0);
        }

        /// <summary>
        /// 退出到手机界面
        /// </summary>
        private void DoLogout()
        {
            DialogResult result = MessageBox.Show("确定要返回手机界面吗？", "返回手机", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                new PhoneForm(_username).Show();
                Close();
            }
        }
        #endregion

        #region 辅助方法
        
        /// <summary>
        /// 加载图片：处理路径解析，如果图片不存在则使用占位图
        /// </summary>
        /// <param name="imagePath">图片路径（可以是相对路径或绝对路径）</param>
        /// <returns>加载的图片对象</returns>
        private Image LoadImageFromPath(string? imagePath)
        {
            string defaultPlaceholder = Path.Combine(AppContext.BaseDirectory, "Images", "_placeholder.png");

            if (string.IsNullOrWhiteSpace(imagePath))
                return File.Exists(defaultPlaceholder) ? Image.FromFile(defaultPlaceholder) : new Bitmap(1, 1);

            string fullPath = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(AppContext.BaseDirectory, imagePath);
            return File.Exists(fullPath) ? Image.FromFile(fullPath) : 
                   File.Exists(defaultPlaceholder) ? Image.FromFile(defaultPlaceholder) : new Bitmap(1, 1);
        }

        /// <summary>
        /// 显示角色姓名（图片/文字切换）
        /// 优先使用囚犯编号查找图片，如果没有则显示文字
        /// </summary>
        private void ShowCharacterName(string characterName, string prisonerNo)
        {
            // 如果没有囚犯编号，直接显示文字
            if (string.IsNullOrWhiteSpace(prisonerNo))
            {
                _namePanel.OverlayImage = null;
                _namePanel.Invalidate();
                _nameLabel.Text = characterName;
                _nameLabel.Visible = true;
                _nameImage.Visible = false;
                return;
            }

            string imageFolder = Path.Combine(AppContext.BaseDirectory, "Images", "characters");
            // 优先使用囚犯编号查找图片（适用于批次1和批次2）
            string nameImagePath = Path.Combine(imageFolder, $"{prisonerNo}.png");

            // 调试输出
            Console.WriteLine($"角色名: {characterName}");
            Console.WriteLine($"囚犯编号: {prisonerNo}");
            Console.WriteLine($"姓名图片路径: {nameImagePath}");
            Console.WriteLine($"文件存在: {File.Exists(nameImagePath)}");

            try
            {
                if (File.Exists(nameImagePath))
                {
                    // 使用自定义绘制方式显示姓名图片
                    var nameImg = Image.FromFile(nameImagePath);
                    
                    // 按比例调整图片尺寸
                    int targetWidth = 360;
                    int targetHeight = (int)(nameImg.Height * targetWidth / (double)nameImg.Width);
                    var resizedNameImg = new Bitmap(targetWidth, targetHeight);
                    using (var g = Graphics.FromImage(resizedNameImg))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(nameImg, 0, 0, targetWidth, targetHeight);
                    }
                    nameImg.Dispose();

                    // 设置自定义 Panel 的绘制属性
                    _namePanel.BackgroundSourceImage = _bigAvatar.Image;  // 大头像作为背景
                    _namePanel.OverlayImage = resizedNameImg;             // 姓名PNG作为覆盖层
                    _namePanel.OverlayPosition = new Point(10, 187 - targetHeight);  // 姓名PNG的位置
                    _namePanel.Invalidate();  // 触发重绘

                    // 隐藏原来的控件
                    _nameImage.Visible = false;
                    _nameLabel.Visible = false;
                }
                else
                {
                    // 没有姓名图片，清空自定义绘制
                    _namePanel.OverlayImage = null;
                    _namePanel.Invalidate();

                    // 显示文字姓名
                    _nameLabel.Text = characterName;
                    Size textSize = TextRenderer.MeasureText(_nameLabel.Text, _nameLabel.Font);
                    _nameLabel.Left = 80;
                    _nameLabel.Top = 140 - textSize.Height;
                    _nameLabel.Visible = true;
                    _nameImage.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"加载姓名图片失败: {ex.Message}");
                _nameImage.Visible = false;
                _nameLabel.Visible = true;
            }
        }

        /// <summary>
        /// 获取角色对应的文件名（已废弃，现在直接使用囚犯编号）
        /// 保留此方法以保持代码兼容性，但不再使用
        /// </summary>
        [Obsolete("现在直接使用囚犯编号查找图片，此方法已不再使用")]
        private string GetFileNameForCharacter(string characterName)
        {
            // 优先使用数据库中的英文名
            if (_dt.Columns.Contains("EnglishName"))
            {
                string? englishName = Convert.ToString(_dt.Rows[_current]["EnglishName"]);
                if (!string.IsNullOrWhiteSpace(englishName))
                    return englishName.ToLower();
            }

            // 其次使用映射表
            if (NameMapping.TryGetValue(characterName, out string mappedName))
                return mappedName.ToLower();

            // 最后使用中文名小写
            return characterName.ToLower();
        }
        #endregion
    }

    /// <summary>
    /// 自定义绘制的 Panel，用于正确显示透明PNG
    /// </summary>
    public class CustomDrawPanel : Panel
    {
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Image? BackgroundSourceImage { get; set; }  // 大头像图片
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Image? OverlayImage { get; set; }           // 姓名PNG图片
        
        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Point OverlayPosition { get; set; }         // 姓名PNG的位置

        public CustomDrawPanel()
        {
            SetStyle(ControlStyles.UserPaint | 
                     ControlStyles.AllPaintingInWmPaint | 
                     ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // 1. 先绘制大头像的对应区域（作为背景）
            if (BackgroundSourceImage != null)
            {
                // Panel 在屏幕上的位置：Left=40, Top=320, Size=400x150
                // 大头像 PictureBox 的位置：Left=120, Top=120, Size=350x350
                // Panel 相对于大头像 PictureBox 的偏移：(-80, 200)
                
                // 由于大头像使用 Zoom 模式，需要计算实际显示的图片区域
                var img = BackgroundSourceImage;
                var boxSize = new Size(350, 350);
                
                // 计算 Zoom 后的实际显示尺寸和位置
                float imgRatio = (float)img.Width / img.Height;
                float boxRatio = (float)boxSize.Width / boxSize.Height;
                
                int displayWidth, displayHeight, offsetX, offsetY;
                if (imgRatio > boxRatio)
                {
                    // 图片更宽，以宽度为准
                    displayWidth = boxSize.Width;
                    displayHeight = (int)(boxSize.Width / imgRatio);
                    offsetX = 0;
                    offsetY = (boxSize.Height - displayHeight) / 2;
                }
                else
                {
                    // 图片更高，以高度为准
                    displayHeight = boxSize.Height;
                    displayWidth = (int)(boxSize.Height * imgRatio);
                    offsetX = (boxSize.Width - displayWidth) / 2;
                    offsetY = 0;
                }
                
                // Panel 相对于实际显示图片的偏移
                int panelOffsetX = -80 - offsetX;
                int panelOffsetY = 200 - offsetY;
                
                // 计算源图片中对应的区域
                float scaleX = (float)img.Width / displayWidth;
                float scaleY = (float)img.Height / displayHeight;
                
                var srcRect = new RectangleF(
                    panelOffsetX * scaleX,
                    panelOffsetY * scaleY,
                    Width * scaleX,
                    Height * scaleY
                );
                
                var destRect = new Rectangle(0, 0, Width, Height);
                
                g.DrawImage(BackgroundSourceImage, destRect, srcRect, GraphicsUnit.Pixel);
            }

            // 2. 再绘制姓名PNG（透明部分会显示下面的大头像）
            if (OverlayImage != null)
            {
                g.DrawImage(OverlayImage, OverlayPosition);
            }
        }
    }
}
