using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.BLL;
using WitchTrialSystem.UI;

namespace WitchTrialSystem
{
    /// <summary>
    /// 管理面板（Form1_Admin）
    /// 功能：管理员专用主界面
    /// 包含：用户信息卡片、魔女列表、数据管理功能
    /// </summary>
    public partial class Form1_Admin : Form
    {
        #region 字段定义
        
        // 核心字段
        private readonly string _username;
        private readonly UserProfileDAL _profileDal = new();
        private readonly WitchDAL _dal = new();
        private readonly PermissionDAL _permissionDal = new();

        private int _userId;
        private string _roleName = "";
        private bool _canManage = false;

        // 用户卡片 UI

        private readonly PictureBox _avatar = new() { Width = 86, Height = 86, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle };
        private readonly Label _lblUser  = new() { AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        private readonly Label _lblRole  = new() { AutoSize = true, ForeColor = Color.DimGray };
        private readonly Label _lblCn    = new() { AutoSize = true };
        private readonly Label _lblNo    = new() { AutoSize = true };
        private readonly Label _lblMagic = new() { AutoSize = true, MaximumSize = new Size(520, 0) };

        // 列表与工具栏
        private readonly ComboBox _cbIsland = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160, Height = 35 };
        private readonly ComboBox _cbBatch  = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120, Height = 35 };
        private readonly TextBox  _tbSearch = new() { PlaceholderText = "按名字搜索", Width = 220, Height = 35 };
        private readonly Button   _btnRefresh = new() { Text = "刷新", Width = 80, Height = 35 };
        private readonly Button   _btnAdd     = new() { Text = "新增魔女", Width = 100, Height = 35 };
        private readonly Button   _btnAddCountry = new() { Text = "国家层添加", Width = 110, Height = 35 };
        private readonly Label    _status     = new() { AutoSize = true, ForeColor = Color.DimGray, Padding = new Padding(8,2,8,2) };
        private readonly DataGridView _grid   = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false };

        // 用户操作按钮
        private readonly Button _btnChangePwd = new() { Text = "修改密码", AutoSize = true };
        private readonly Button _btnLogout    = new() { Text = "退出登录", AutoSize = true };
        private readonly Button _btnStatus     = new() { Text = "更改状态", Width = 100, Height = 35 };
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数：初始化管理员面板
        /// </summary>
        /// <param name="username">当前登录的用户名</param>
        public Form1_Admin(string username)
        {
            _username = username;

            InitializeComponent();
            Text = "魔女审判 · 管理员面板";
            WindowState = FormWindowState.Maximized; // 全屏显示
            StartPosition = FormStartPosition.CenterScreen;
            
            // 固定角色为 Admin
            _roleName = "Admin";
            _canManage = true;
            
            // 设置应用程序图标
            IconHelper.SetFormIcon(this);

            // —— 顶部：用户卡片 —— //
            var card = BuildUserCard();
            card.Dock = DockStyle.Top;

        // —— 第二行：工具条 —— //
        var bar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 80,                       // 进一步增加高度，确保按钮完整显示
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,              // ✅ 单行，不换行
            AutoScroll = true,                 // ✅ 超出时出现横向滚动条
            Padding = new Padding(15, 15, 15, 15),
            BackColor = Color.WhiteSmoke
        };

        _tbSearch.Width = 220;                 // 可按需要再调窄/变宽

        bar.Controls.Add(new Label{Text="岛屿", AutoSize=true, Padding=new Padding(0,8,6,0)});
        bar.Controls.Add(_cbIsland);
        bar.Controls.Add(new Label{Text="批次", AutoSize=true, Padding=new Padding(12,8,6,0)});
        bar.Controls.Add(_cbBatch);
        bar.Controls.Add(_tbSearch);
        bar.Controls.Add(_btnRefresh);
        bar.Controls.Add(_btnAddCountry);
        bar.Controls.Add(_status);



            // —— 第三行：数据网格 —— //
            _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Controls.Add(_grid);
            Controls.Add(bar);
            Controls.Add(card);

            // 事件
            Load += Form1_Load;
            _btnRefresh.Click += (_,__) => LoadGrid();
            _tbSearch.KeyDown += (s,e)=>{ if(e.KeyCode==Keys.Enter){ e.SuppressKeyPress=true; LoadGrid(); } };
            _cbIsland.SelectedIndexChanged += (_,__) => { LoadBatches(); LoadGrid(); };
            _cbBatch.SelectedIndexChanged  += (_,__) => { LoadGrid(); };
            _btnAddCountry.Click += (_,__) => OnAddWitchCountry();
            _btnChangePwd.Click += (_, __) => OnChangePassword();
            _btnLogout.Click    += (_, __) => OnLogout();
            _grid.CellDoubleClick += Grid_CellDoubleClick;  // 双击查看详情
            _grid.CellMouseDown += Grid_CellMouseDown;  // 右键菜单

        }

        private Panel BuildUserCard()
        {
            // 右侧信息区：纵向排列，自动根据内容增高
            var right = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Padding = new Padding(12, 6, 6, 6),
                MaximumSize = new Size(760, 0) // 防止一行太长导致控件被挤出
            };
            right.Controls.Add(_lblUser);
            right.Controls.Add(_lblRole);
            right.Controls.Add(_lblCn);

            // 按钮加入信息区（一定要在这里 Add）
            _btnChangePwd.Margin = new Padding(0, 6, 8, 0);
            _btnLogout.Margin    = new Padding(0, 6, 0, 0);
            right.Controls.Add(_btnChangePwd);
            right.Controls.Add(_btnLogout);

            // 卡片改为“自动尺寸”，不再固定 Height，避免内容被截断
            var card = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(10)
            };

            // 左侧头像
            _avatar.Width = 250; _avatar.Height = 250;
            _avatar.SizeMode = PictureBoxSizeMode.Zoom;
            _avatar.BorderStyle = BorderStyle.FixedSingle;

            // 手动布局：头像在左，信息在右
            card.Controls.Add(_avatar);
            _avatar.Left = 10; _avatar.Top = 10;

            card.Controls.Add(right);
            right.Left = _avatar.Right + 10;
            right.Top  = 8;

            // 底部分隔线（用 DockBottom，随 AutoSize 卡片一起增长）
            var line = new Panel { Dock = DockStyle.Bottom, Height = 1, BackColor = Color.Gainsboro };
            card.Controls.Add(line);

            return card;
        }
        
        #endregion

        #region 事件处理
        
        /// <summary>
        /// 窗体加载事件：加载用户信息和数据列表
        /// </summary>
        private void Form1_Load(object? sender, EventArgs e)
        {
            try
            {
                // 1) 加载用户卡片
                LoadUserCard();

                // 2) 加载列表
                var dbName = DBHelper.ExecScalar("SELECT DB_NAME()")?.ToString() ?? "(unknown)";
                _status.Text = "已连接：" + dbName;
                LoadIslands(); LoadBatches(); LoadGrid();
            }
            catch (Exception ex)
            {
                _status.Text = "数据库连接失败：" + ex.Message;
                _status.ForeColor = Color.OrangeRed;
            }
        }
        
        #endregion

        #region 数据加载方法
        
        /// <summary>
        /// 加载用户卡片信息
        /// </summary>
        private void LoadUserCard()
        {
            var dt = _profileDal.GetProfile(_username);
            if (dt.Rows.Count == 0)
            {
                _lblUser.Text = $"账号：{_username}";
                _lblRole.Text = $"角色：未知";
                _lblCn.Text   = "中文名：—";
                return;
            }

            var r = dt.Rows[0];
            string role = r["RoleName"] as string ?? "Unknown";
            string cn   = r["CnName"]   as string ?? "—";
            string? avatar = r["AvatarPath"] as string;
            
            _userId   = Convert.ToInt32(r["UserID"]);
            _roleName = Convert.ToString(r["RoleName"]) ?? "";
            _canManage = _roleName == "Admin" || _roleName == "Warden" || _roleName == "Meruru";

            _lblUser.Text  = $"账号：{_username}";
            _lblRole.Text  = $"角色：{role}";
            _lblCn.Text    = $"中文名：{(cn ?? "—")}";

            _btnAdd.Enabled    = _canManage;   // 只有管理员/典狱长/梅露露能新增
            _btnStatus.Enabled = _canManage;   // 同上：能改状态


            // 头像：支持绝对/相对路径（相对路径以程序目录为基准），缺图用占位
            _avatar.Image = null;
            try
            {
                // 常规头像加载逻辑
                object avatarObj = r["AvatarPath"];
                string avatarPath = (avatarObj == null || avatarObj == DBNull.Value) ? null : avatarObj.ToString();

                // 占位图相对路径：Images/_placeholder.png
                string placeholder = Path.Combine(AppContext.BaseDirectory, "Images", "_placeholder.png");

                string resolved = null;
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

        /// <summary>
        /// 加载岛屿列表（根据用户权限）
        /// </summary>
        private void LoadIslands()
        {
            var dt = _permissionDal.GetIslandsByPermission(_username);
            
            // Admin 角色添加"全部"选项
            if (_roleName == "Admin")
            {
                var allDt = new DataTable();
                allDt.Columns.Add("IslandID", typeof(int));
                allDt.Columns.Add("Name", typeof(string));
                allDt.Rows.Add(0, "全部");
                
                foreach (DataRow row in dt.Rows)
                {
                    allDt.Rows.Add(row["IslandID"], row["Name"]);
                }
                
                _cbIsland.DisplayMember = "Name";
                _cbIsland.ValueMember = "IslandID";
                _cbIsland.DataSource = allDt;
            }
            else
            {
                _cbIsland.DisplayMember = "Name";
                _cbIsland.ValueMember = "IslandID";
                _cbIsland.DataSource = dt;
            }
        }
        
        /// <summary>
        /// 加载批次列表（根据选中的岛屿）
        /// </summary>
        private DataTable CreateBatchTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("BatchID", typeof(string));
            dt.Columns.Add("DisplayText", typeof(string));
            dt.Rows.Add("0", "全部");
            return dt;
        }

        private void LoadBatches()
        {
            try
            {
                if (_cbIsland.SelectedValue == null) return;
                
                int islandId;
                // 处理可能的不同类型转换情况
                if (_cbIsland.SelectedValue is int)
                {
                    islandId = (int)_cbIsland.SelectedValue;
                }
                else if (_cbIsland.SelectedValue is DataRowView rowView)
                {
                    islandId = Convert.ToInt32(rowView["IslandID"]);
                }
                else
                {
                    return;
                }

                // 创建包含"全部"选项的数据表
                var dt = new DataTable();
                dt.Columns.Add("BatchID", typeof(int));
                dt.Columns.Add("DisplayText", typeof(string));
                dt.Rows.Add(0, "全部");
                
                // 获取数据库中的批次数据
                var batches = _dal.GetBatches(islandId);
                
                // 直接使用实际的BatchID
                foreach (DataRow row in batches.Rows)
                {
                    int batchId = Convert.ToInt32(row["BatchID"]);
                    dt.Rows.Add(batchId, batchId.ToString());
                }
                
                // 更新数据源
                _cbBatch.DisplayMember = "DisplayText";
                _cbBatch.ValueMember = "BatchID";
                _cbBatch.DataSource = dt;
                
                // 默认选中"全部"
                if (_cbBatch.Items.Count > 0)
                {
                    _cbBatch.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                _status.Text = "加载批次失败: " + ex.Message;
                _status.ForeColor = Color.Red;
            }
        }
        
        /// <summary>
        /// 加载魔女数据网格（根据用户权限）
        /// </summary>
        private void LoadGrid()
        {
            try
            {
                if (_cbIsland.SelectedValue == null) return;
                
                int islandId;
                // 处理可能的不同类型转换情况
                if (_cbIsland.SelectedValue is int)
                {
                    islandId = (int)_cbIsland.SelectedValue;
                }
                else if (_cbIsland.SelectedValue is DataRowView rowView)
                {
                    islandId = Convert.ToInt32(rowView["IslandID"]);
                }
                else
                {
                    return;
                }
            
                // 处理批次筛选
                int? batchId = null;
                if (_cbBatch.SelectedValue != null && Convert.ToInt32(_cbBatch.SelectedValue) != 0)
                {
                    batchId = Convert.ToInt32(_cbBatch.SelectedValue);
                }
                
                var nameLike = string.IsNullOrWhiteSpace(_tbSearch.Text) ? null : _tbSearch.Text.Trim();

                // 直接使用WitchDAL获取数据，不经过PermissionDAL的预筛选
                DataTable allWitches;
                if (_roleName == "Admin")
                {
                    // 管理员：如果选择"全部"（islandId=0），则不传递岛屿ID
                    if (islandId == 0)
                    {
                        allWitches = _dal.GetWitches(null, batchId, nameLike);
                    }
                    else
                    {
                        allWitches = _dal.GetWitches(islandId, batchId, nameLike);
                    }
                }
                else if (_roleName == "Meruru" || _roleName == "Warden")
                {
                    // Meruru和Warden：获取指定岛屿的数据
                    allWitches = _dal.GetWitches(islandId, batchId, nameLike);
                }
                else
                {
                    // 其他角色：通过PermissionDAL获取（已经预筛选）
                    allWitches = _permissionDal.GetWitchesByPermission(_username, nameLike);
                    
                    // 如果还需要筛选批次，使用DataView
                    if (batchId.HasValue)
                    {
                        var view = new DataView(allWitches);
                        view.RowFilter = $"BatchID = {batchId.Value}";
                        allWitches = view.ToTable();
                    }
                }
                
                // 应用数据
                _grid.DataSource = allWitches;
                
                // 配置列显示
                var dt = allWitches;
                var cols = _grid.Columns;
                int displayIndex = 0;
                
                // 显示记录数
                _status.Text = $"共 {dt.Rows.Count} 条";
                
                // 核心识别信息
                var c = cols["PrisonerNo"];    
                if (c != null) { c.HeaderText = "囚人番号"; c.Width = 80; c.DisplayIndex = displayIndex++; }
                
                c = cols["PersonalNo"];    
                if (c != null) { c.HeaderText = "个人番号"; c.Width = 120; c.DisplayIndex = displayIndex++; }
                
                c = cols["Name"];          
                if (c != null) { c.HeaderText = "姓名"; c.Width = 100; c.DisplayIndex = displayIndex++; }
                
                // 基本信息
                c = cols["Gender"];        
                if (c != null) { c.HeaderText = "性别"; c.Width = 50; c.DisplayIndex = displayIndex++; }
                
                c = cols["Age"];           
                if (c != null) { c.HeaderText = "年龄"; c.Width = 50; c.DisplayIndex = displayIndex++; }
                
                c = cols["Status"];        
                if (c != null) { c.HeaderText = "状态"; c.Width = 100; c.DisplayIndex = displayIndex++; }
                
                // 身体特征
                c = cols["Height"];        
                if (c != null) { c.HeaderText = "身高"; c.Width = 60; c.DisplayIndex = displayIndex++; }
                
                c = cols["Weight"];        
                if (c != null) { c.HeaderText = "体重"; c.Width = 60; c.DisplayIndex = displayIndex++; }
                
                c = cols["BloodType"];     
                if (c != null) { c.HeaderText = "血型"; c.Width = 50; c.DisplayIndex = displayIndex++; }
                
                // 能力信息
                c = cols["Magic"];         
                if (c != null) { c.HeaderText = "魔法"; c.Width = 100; c.DisplayIndex = displayIndex++; }
                
                // 教育与背景
                c = cols["HighestEducation"];
                if (c != null) { c.HeaderText = "最高学历"; c.Width = 100; c.DisplayIndex = displayIndex++; }
                
                c = cols["Birthplace"];    
                if (c != null) { c.HeaderText = "籍贯"; c.Width = 80; c.DisplayIndex = displayIndex++; }
                
                // 联系方式
                c = cols["Phone"];         
                if (c != null) { c.HeaderText = "电话"; c.Width = 110; c.DisplayIndex = displayIndex++; }
                
                c = cols["Email"];         
                if (c != null) { c.HeaderText = "邮箱"; c.Width = 180; c.DisplayIndex = displayIndex++; }
                
                // 个性特征
                c = cols["Skills"];        
                if (c != null) { c.HeaderText = "技能特长"; c.Width = 150; c.DisplayIndex = displayIndex++; }
                
                c = cols["Hobbies"];       
                if (c != null) { c.HeaderText = "兴趣爱好"; c.Width = 150; c.DisplayIndex = displayIndex++; }
                
                c = cols["Dreams"];        
                if (c != null) { c.HeaderText = "理想"; c.Width = 150; c.DisplayIndex = displayIndex++; }
                
                c = cols["Trauma"];        
                if (c != null) { c.HeaderText = "心理创伤"; c.Width = 200; c.DisplayIndex = displayIndex++; }
                
                // 系统字段
                c = cols["IslandID"];      
                if (c != null) { c.HeaderText = "岛"; c.Width = 40; c.DisplayIndex = displayIndex++; }
                
                c = cols["BatchID"];       
                if (c != null) { c.HeaderText = "批次"; c.Width = 50; c.DisplayIndex = displayIndex++; }
                
                c = cols["WitchID"];   
                if (c != null) { c.HeaderText = "ID"; c.Width = 50; c.DisplayIndex = displayIndex++; }
                
                // 隐藏字段
                c = cols["AvatarPath"];
                if (c != null) { c.Visible = false; }
                
                c = cols["BirthDate"];
                if (c != null) { c.Visible = false; } // 已经显示年龄，不需要显示出生日期
                
                // 描述列占满剩余空间
                c = cols["DescriptionPublic"];
                if (c != null) 
                { 
                    c.HeaderText = "公开描述"; 
                    c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    c.DisplayIndex = displayIndex++;
                }

                _status.Text = $"共 {dt.Rows.Count} 条";
            }
            catch (Exception ex)
            {
                _status.Text = "加载魔女数据失败: " + ex.Message;
                _status.ForeColor = Color.Red;
            }
        }
        
        #endregion

        #region 业务操作方法
        


        /// <summary>
        /// 国家层添加魔女（完整档案）
        /// </summary>
        private void OnAddWitchCountry()
        {
            using (var form = new WitchTrialSystem.UI.WitchAddForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    // 保存成功，刷新数据网格
                    LoadGrid();
                }
            }
        }
        
        /// <summary>
        /// 修改密码
        /// </summary>
        private void OnChangePassword()
        {
            // 简易对话框：输入 旧密码 / 新密码 / 确认密码
            using var f = new Form { Width = 360, Height = 220, Text = "修改密码", StartPosition = FormStartPosition.CenterParent };
            var l1 = new Label { Left = 12, Top = 16, Text = "旧密码", AutoSize = true };
            var l2 = new Label { Left = 12, Top = 56, Text = "新密码", AutoSize = true };
            var l3 = new Label { Left = 12, Top = 96, Text = "确认新密码", AutoSize = true };
            var t1 = new TextBox { Left = 100, Top = 12, Width = 220, UseSystemPasswordChar = true };
            var t2 = new TextBox { Left = 100, Top = 52, Width = 220, UseSystemPasswordChar = true };
            var t3 = new TextBox { Left = 100, Top = 92, Width = 220, UseSystemPasswordChar = true };
            var ok = new Button  { Left = 160, Top = 130, Width = 70, Text = "确定", DialogResult = DialogResult.OK };
            var ca = new Button  { Left = 250, Top = 130, Width = 70, Text = "取消", DialogResult = DialogResult.Cancel };
            f.Controls.AddRange(new Control[] { l1, l2, l3, t1, t2, t3, ok, ca });
            f.AcceptButton = ok; f.CancelButton = ca;

            if (f.ShowDialog(this) != DialogResult.OK) return;

            var oldPwd = t1.Text; var newPwd = t2.Text; var confirm = t3.Text;
            if (string.IsNullOrEmpty(oldPwd) || string.IsNullOrEmpty(newPwd))
            { MessageBox.Show("密码不能为空。"); return; }
            if (newPwd != confirm)
            { MessageBox.Show("两次输入的新密码不一致。"); return; }

            try
            {
                var bll = new UserBLL();
                var okChange = bll.ChangePassword(_username, oldPwd, newPwd);
                if (!okChange)
                {
                    MessageBox.Show("修改失败：旧密码不正确或账号不存在。");
                    return;
                }
                MessageBox.Show("密码已更新。请使用新密码重新登录。");
                OnLogout();
            }
            catch (Exception ex)
            {
                MessageBox.Show("修改密码出错：" + ex.Message);
            }
        }
        
        /// <summary>
        /// 退出登录
        /// </summary>
        private void OnLogout()
        {
            var result = MessageBox.Show("确定要退出登录吗？", "退出登录",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                var login = new LoginForm();
                login.Show();
                this.Close();
            }
        }



        /// <summary>
        /// 双击单元格查看魔女详情
        /// </summary>
        private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            // 忽略列头点击
            if (e.RowIndex < 0) return;

            try
            {
                // 获取选中行的WitchID
                var drv = _grid.Rows[e.RowIndex].DataBoundItem as System.Data.DataRowView;
                if (drv == null || !drv.Row.Table.Columns.Contains("WitchID"))
                {
                    MessageBox.Show("无法获取魔女ID。");
                    return;
                }

                int witchId = Convert.ToInt32(drv["WitchID"]);

                // 打开详情窗口
                using var detailForm = new WitchDetailForm(witchId);
                detailForm.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开详情窗口失败：{ex.Message}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 右键菜单 - 编辑魔女信息
        /// </summary>
        private void Grid_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            // 只处理右键点击，且不是列头
            if (e.Button != MouseButtons.Right || e.RowIndex < 0) return;

            // 选中当前行
            _grid.ClearSelection();
            _grid.Rows[e.RowIndex].Selected = true;
            _grid.CurrentCell = _grid.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];

            // 获取选中行的数据
            var drv = _grid.Rows[e.RowIndex].DataBoundItem as System.Data.DataRowView;
            if (drv == null || !drv.Row.Table.Columns.Contains("WitchID"))
            {
                return;
            }

            int witchId = Convert.ToInt32(drv["WitchID"]);
            string witchName = Convert.ToString(drv["Name"]) ?? "未知";

            // 创建右键菜单
            var contextMenu = new ContextMenuStrip();
            
            // Admin 可以编辑所有信息
            var editMenuItem = new ToolStripMenuItem($"编辑魔女信息 - {witchName}");
            editMenuItem.Click += (s, args) => OnEditWitchInfo(witchId);
            contextMenu.Items.Add(editMenuItem);
            
            // 显示菜单
            contextMenu.Show(_grid, _grid.PointToClient(Cursor.Position));
        }

        /// <summary>
        /// 编辑魔女完整信息
        /// </summary>
        private void OnEditWitchInfo(int witchId)
        {
            try
            {
                // 使用专门的编辑窗口
                var form = new WitchTrialSystem.UI.WitchEditForm(witchId);
                if (form.ShowDialog(this) == DialogResult.OK)
                {
                    // 保存成功，刷新数据网格
                    LoadGrid();
                }
                form.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"编辑失败：{ex.Message}\n\n详细信息：{ex.StackTrace}", "错误", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        #endregion

        #region 辅助方法
        
        /// <summary>
        /// 简单输入对话框
        /// </summary>
        private static string? Prompt(string text, string title)
        {
            using var f=new Form{ Width=380, Height=140, Text=title, StartPosition=FormStartPosition.CenterParent };
            var lbl=new Label{ Left=10, Top=10, Width=340, Text=text };
            var tb =new TextBox{ Left=10, Top=40, Width=340 };
            var ok =new Button{ Text="确定", Left=190, Width=75, Top=70, DialogResult=DialogResult.OK };
            var cancel=new Button{ Text="取消", Left=275, Width=75, Top=70, DialogResult=DialogResult.Cancel };
            f.Controls.AddRange(new Control[]{lbl,tb,ok,cancel}); f.AcceptButton=ok; f.CancelButton=cancel;
            return f.ShowDialog()==DialogResult.OK ? tb.Text : null;
        }
        
        #endregion
    }
}

