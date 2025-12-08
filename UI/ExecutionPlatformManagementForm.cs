using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.BLL;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 处刑台管理主界面
    /// 功能：显示地下室49个位置和审判庭，支持处刑台移动和刑具管理
    /// </summary>
    public partial class ExecutionPlatformManagementForm : Form
    {
        #region 字段定义
        
        private readonly string _username;
        private readonly string _roleName;
        private readonly int _islandId;
        private readonly ExecutionPlatformService _platformService;
        private readonly MovementLogService _logService;
        
        // UI控件
        private readonly ComboBox _cbIsland;
        private readonly Button _btnRefresh;
        private readonly Label _lblStatus;
        private readonly Panel _panelBasement;  // 地下室区域
        private readonly Panel _panelTrialHall; // 审判庭区域
        private readonly Button[] _positionButtons; // 49个位置按钮
        private Panel _trialHallPanel;     // 审判庭显示面板（非readonly，因为在方法中赋值）
        
        #endregion

        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="roleName">角色名</param>
        /// <param name="islandId">岛屿ID</param>
        public ExecutionPlatformManagementForm(string username, string roleName, int islandId)
        {
            _username = username;
            _roleName = roleName;
            _islandId = islandId;
            _platformService = new ExecutionPlatformService();
            _logService = new MovementLogService();
            
            InitializeComponent();
            
            Text = "处刑台管理系统";
            Size = new Size(1200, 800);
            StartPosition = FormStartPosition.CenterScreen;
            
            // 设置图标
            IconHelper.SetFormIcon(this);
            
            // 初始化控件
            _cbIsland = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };
            _btnRefresh = new Button { Text = "刷新", Width = 80 };
            _lblStatus = new Label { AutoSize = true, ForeColor = Color.DimGray };
            _positionButtons = new Button[49];
            
            // 顶部工具栏
            var toolbar = CreateToolbar();
            toolbar.Dock = DockStyle.Top;
            Controls.Add(toolbar);
            
            // 主内容区域
            var mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            
            // 地下室区域（左侧）
            _panelBasement = CreateBasementPanel();
            _panelBasement.Dock = DockStyle.Fill;
            mainPanel.Controls.Add(_panelBasement);
            
            // 审判庭区域（右侧）
            _panelTrialHall = CreateTrialHallPanel();
            _panelTrialHall.Dock = DockStyle.Right;
            _panelTrialHall.Width = 300;
            mainPanel.Controls.Add(_panelTrialHall);
            
            Controls.Add(mainPanel);
            
            // 事件绑定
            Load += Form_Load;
            _btnRefresh.Click += (s, e) => LoadData();
            _cbIsland.SelectedIndexChanged += (s, e) => LoadData();
        }
        
        #endregion

        #region UI创建方法
        
        /// <summary>
        /// 创建顶部工具栏
        /// </summary>
        private Panel CreateToolbar()
        {
            var panel = new Panel
            {
                Height = 60,  // 增加高度
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(10)
            };
            
            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = true  // 添加滚动条
            };
            
            // 岛屿选择（仅Admin可见）
            if (_roleName == "Admin")
            {
                flow.Controls.Add(new Label { Text = "岛屿：", AutoSize = true, Padding = new Padding(0, 8, 5, 0) });
                flow.Controls.Add(_cbIsland);
            }
            
            _btnRefresh.Height = 35;  // 增加按钮高度
            flow.Controls.Add(_btnRefresh);
            
            _lblStatus.Padding = new Padding(10, 10, 10, 0);
            flow.Controls.Add(_lblStatus);
            
            // 查看移动记录按钮
            var btnViewLog = new Button 
            { 
                Text = "查看移动记录", 
                Width = 130,  // 增加宽度
                Height = 35,  // 增加高度
                Margin = new Padding(10, 0, 0, 0) 
            };
            btnViewLog.Click += (s, e) => OpenMovementLogView();
            flow.Controls.Add(btnViewLog);
            
            panel.Controls.Add(flow);
            return panel;
        }
        
        /// <summary>
        /// 创建地下室布局面板
        /// </summary>
        private Panel CreateBasementPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                AutoScroll = true
            };
            
            var groupBox = new GroupBox
            {
                Text = "地下室（-1F）- 49个位置",
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            
            // 使用TableLayoutPanel布局7x7网格
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7,
                RowCount = 7,
                AutoScroll = true,
                Padding = new Padding(5)
            };
            
            // 设置列宽和行高
            for (int i = 0; i < 7; i++)
            {
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 14.28f));
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 14.28f));
            }
            
            // 创建49个位置按钮
            for (int i = 0; i < 49; i++)
            {
                int position = i + 1;
                var btn = new Button
                {
                    Text = $"位置 {position}\n空",
                    Dock = DockStyle.Fill,
                    Margin = new Padding(3),
                    BackColor = Color.LightGray,
                    ForeColor = Color.Black,
                    Font = new Font("Microsoft YaHei", 9),
                    Tag = position
                };
                
                // 右键菜单
                btn.MouseDown += PositionButton_MouseDown;
                
                _positionButtons[i] = btn;
                table.Controls.Add(btn, i % 7, i / 7);
            }
            
            groupBox.Controls.Add(table);
            panel.Controls.Add(groupBox);
            
            return panel;
        }
        
        /// <summary>
        /// 创建审判庭面板
        /// </summary>
        private Panel CreateTrialHallPanel()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            
            var groupBox = new GroupBox
            {
                Text = "🏛️ 审判庭（1F）- 位置50",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),  // 加粗字体
                ForeColor = Color.DarkRed  // 深红色标题
            };
            
            _trialHallPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(255, 250, 205),  // 浅黄色背景
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var label = new Label
            {
                Text = "空闲",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),  // 更大的字体
                ForeColor = Color.Gray,
                Tag = 50
            };
            
            // 右键菜单
            label.MouseDown += TrialHallPanel_MouseDown;
            
            _trialHallPanel.Controls.Add(label);
            groupBox.Controls.Add(_trialHallPanel);
            panel.Controls.Add(groupBox);
            
            return panel;
        }
        
        #endregion

        #region 数据加载方法
        
        /// <summary>
        /// 窗体加载事件
        /// </summary>
        private void Form_Load(object? sender, EventArgs e)
        {
            try
            {
                // 加载岛屿列表（仅Admin）
                if (_roleName == "Admin")
                {
                    LoadIslands();
                }
                
                // 加载数据
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// 加载岛屿列表
        /// </summary>
        private void LoadIslands()
        {
            // TODO: 从数据库加载岛屿列表
            // 暂时使用固定数据
            var dt = new DataTable();
            dt.Columns.Add("IslandID", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "梅露露岛");
            dt.Rows.Add(2, "乌蒂娜岛");
            
            _cbIsland.DisplayMember = "Name";
            _cbIsland.ValueMember = "IslandID";
            _cbIsland.DataSource = dt;
            
            // 选中当前岛屿
            _cbIsland.SelectedValue = _islandId;
        }
        
        /// <summary>
        /// 加载处刑台数据
        /// </summary>
        private void LoadData()
        {
            try
            {
                int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                    ? Convert.ToInt32(_cbIsland.SelectedValue)
                    : _islandId;
                
                // 获取所有处刑台
                var platforms = _platformService.GetPlatformsByIsland(currentIslandId);
                
                // 重置所有位置按钮
                for (int i = 0; i < 49; i++)
                {
                    _positionButtons[i].Text = $"位置 {i + 1}\n空";
                    _positionButtons[i].BackColor = Color.LightGray;
                    _positionButtons[i].Tag = i + 1;
                }
                
                // 重置审判庭
                var trialLabel = _trialHallPanel.Controls[0] as Label;
                if (trialLabel != null)
                {
                    trialLabel.Text = "空闲";
                    trialLabel.Tag = 50;
                    _trialHallPanel.BackColor = Color.LightYellow;
                }
                
                // 更新处刑台位置
                foreach (var platform in platforms)
                {
                    if (platform.CurrentPosition == 50)
                    {
                        // 在审判庭
                        if (trialLabel != null)
                        {
                            trialLabel.Text = $"处刑台 {platform.PlatformNumber}\n" +
                                            $"{(platform.HasTool ? platform.ToolName : "无刑具")}\n" +
                                            $"状态：{platform.Status}";
                            trialLabel.Tag = platform;
                            _trialHallPanel.BackColor = Color.LightCoral;
                        }
                    }
                    else if (platform.CurrentPosition >= 1 && platform.CurrentPosition <= 49)
                    {
                        // 在地下室
                        var btn = _positionButtons[platform.CurrentPosition - 1];
                        btn.Text = $"位置 {platform.CurrentPosition}\n" +
                                  $"处刑台 {platform.PlatformNumber}\n" +
                                  $"{(platform.HasTool ? platform.ToolName : "无刑具")}";
                        btn.BackColor = platform.IsAtHome ? Color.LightGreen : Color.LightBlue;
                        btn.Tag = platform;
                    }
                }
                
                _lblStatus.Text = $"已加载 {platforms.Count} 个处刑台";
                _lblStatus.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"加载失败：{ex.Message}";
                _lblStatus.ForeColor = Color.Red;
            }
        }
        
        #endregion

        #region 事件处理方法
        
        /// <summary>
        /// 位置按钮鼠标按下事件（处理右键菜单）
        /// </summary>
        private void PositionButton_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            
            var btn = sender as Button;
            if (btn == null || btn.Tag == null) return;
            
            // 如果Tag是int，说明是空位置
            if (btn.Tag is int)
            {
                return; // 空位置不显示菜单
            }
            
            // 如果Tag是ExecutionPlatformModel，显示操作菜单
            if (btn.Tag is ExecutionPlatformModel platform)
            {
                ShowPlatformContextMenu(btn, platform);
            }
        }
        
        /// <summary>
        /// 审判庭面板鼠标按下事件
        /// </summary>
        private void TrialHallPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            
            var label = sender as Label;
            if (label == null || label.Tag == null) return;
            
            // 如果Tag是ExecutionPlatformModel，显示操作菜单
            if (label.Tag is ExecutionPlatformModel platform)
            {
                ShowPlatformContextMenu(label, platform);
            }
        }
        
        /// <summary>
        /// 显示处刑台右键菜单
        /// </summary>
        private void ShowPlatformContextMenu(Control control, ExecutionPlatformModel platform)
        {
            var menu = new ContextMenuStrip();
            
            // Admin只能查看，不能操作
            if (_roleName != "Admin")
            {
                // 移动到审判庭（仅当在地下室且审判庭为空时）
                if (platform.CurrentPosition != 50)
                {
                    int currentIslandId = _islandId;
                    bool trialHallOccupied = _platformService.IsTrialHallOccupied(currentIslandId);
                    
                    var moveToTrialItem = new ToolStripMenuItem("移动到审判庭");
                    moveToTrialItem.Enabled = !trialHallOccupied;
                    moveToTrialItem.Click += (s, e) => MoveToTrialHall(platform);
                    menu.Items.Add(moveToTrialItem);
                }
                
                // 返回原位（仅当在审判庭时）
                if (platform.CurrentPosition == 50)
                {
                    var returnHomeItem = new ToolStripMenuItem("返回原位");
                    returnHomeItem.Click += (s, e) => ReturnToHome(platform);
                    menu.Items.Add(returnHomeItem);
                }
                
                menu.Items.Add(new ToolStripSeparator());
                
                // 刑具管理（仅Meruru角色，即监管者才能管理刑具）
                if (_roleName == "Meruru")
                {
                    if (!platform.HasTool)
                    {
                        var addToolItem = new ToolStripMenuItem("添加刑具");
                        addToolItem.Click += (s, e) => AddTool(platform);
                        menu.Items.Add(addToolItem);
                    }
                    else
                    {
                        var updateToolItem = new ToolStripMenuItem("更换刑具");
                        updateToolItem.Click += (s, e) => UpdateTool(platform);
                        menu.Items.Add(updateToolItem);
                        
                        var removeToolItem = new ToolStripMenuItem("移除刑具");
                        removeToolItem.Click += (s, e) => RemoveTool(platform);
                        menu.Items.Add(removeToolItem);
                    }
                    
                    menu.Items.Add(new ToolStripSeparator());
                }
            }
            
            // 查看详情（所有角色都可以）
            var viewDetailItem = new ToolStripMenuItem("查看详情");
            viewDetailItem.Click += (s, e) => ViewPlatformDetail(platform);
            menu.Items.Add(viewDetailItem);
            
            menu.Show(control, control.PointToClient(Cursor.Position));
        }
        
        #endregion

        #region 业务操作方法
        
        /// <summary>
        /// 移动到审判庭
        /// </summary>
        private void MoveToTrialHall(ExecutionPlatformModel platform)
        {
            // 打开移动对话框
            using var dialog = new PlatformMoveDialog(platform, "审判庭（位置50）", true);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                        ? Convert.ToInt32(_cbIsland.SelectedValue)
                        : _islandId;
                    
                    var (success, message) = _platformService.MoveToTrialHall(
                        platform.PlatformID, 
                        currentIslandId, 
                        dialog.CustomTime
                    );
                    
                    if (success)
                    {
                        MessageBox.Show("移动成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show($"移动失败：{message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// 返回原位
        /// </summary>
        private void ReturnToHome(ExecutionPlatformModel platform)
        {
            // 打开移动对话框
            using var dialog = new PlatformMoveDialog(platform, $"原位（位置{platform.HomePosition}）", false);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                        ? Convert.ToInt32(_cbIsland.SelectedValue)
                        : _islandId;
                    
                    var (success, message) = _platformService.ReturnToHome(
                        platform.PlatformID, 
                        currentIslandId, 
                        dialog.CustomTime
                    );
                    
                    if (success)
                    {
                        MessageBox.Show("返回成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show($"返回失败：{message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// 添加刑具
        /// </summary>
        private void AddTool(ExecutionPlatformModel platform)
        {
            using var dialog = new ToolManagementDialog(platform, ToolOperation.Add);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                        ? Convert.ToInt32(_cbIsland.SelectedValue)
                        : _islandId;
                    
                    var (success, message) = _platformService.AddTool(
                        platform.PlatformID,
                        dialog.ToolName,
                        dialog.ToolType,
                        dialog.ToolDescription,
                        currentIslandId
                    );
                    
                    if (success)
                    {
                        MessageBox.Show("添加刑具成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show($"添加失败：{message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// 更换刑具
        /// </summary>
        private void UpdateTool(ExecutionPlatformModel platform)
        {
            using var dialog = new ToolManagementDialog(platform, ToolOperation.Update);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                        ? Convert.ToInt32(_cbIsland.SelectedValue)
                        : _islandId;
                    
                    var (success, message) = _platformService.UpdateTool(
                        platform.PlatformID,
                        dialog.ToolName,
                        dialog.ToolType,
                        dialog.ToolDescription,
                        currentIslandId
                    );
                    
                    if (success)
                    {
                        MessageBox.Show("更换刑具成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show($"更换失败：{message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// 移除刑具
        /// </summary>
        private void RemoveTool(ExecutionPlatformModel platform)
        {
            var result = MessageBox.Show(
                $"确定要移除处刑台 {platform.PlatformNumber} 上的刑具\"{platform.ToolName}\"吗？",
                "确认移除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                        ? Convert.ToInt32(_cbIsland.SelectedValue)
                        : _islandId;
                    
                    var (success, message) = _platformService.RemoveTool(platform.PlatformID, currentIslandId);
                    
                    if (success)
                    {
                        MessageBox.Show("移除刑具成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show($"移除失败：{message}", "失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"操作失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        /// <summary>
        /// 查看处刑台详情
        /// </summary>
        private void ViewPlatformDetail(ExecutionPlatformModel platform)
        {
            var detail = $"处刑台编号：{platform.PlatformNumber}\n" +
                        $"原位位置：{platform.HomePosition}\n" +
                        $"当前位置：{platform.CurrentPosition} ({platform.LocationDescription})\n" +
                        $"状态：{platform.Status}\n" +
                        $"刑具名称：{platform.ToolName ?? "无"}\n" +
                        $"刑具类型：{platform.ToolType ?? "无"}\n" +
                        $"刑具描述：{platform.ToolDescription ?? "无"}\n" +
                        $"创建时间：{platform.CreatedAt:yyyy-MM-dd HH:mm:ss}\n" +
                        $"更新时间：{platform.UpdatedAt:yyyy-MM-dd HH:mm:ss}";
            
            MessageBox.Show(detail, "处刑台详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        /// <summary>
        /// 打开移动记录查看窗口
        /// </summary>
        private void OpenMovementLogView()
        {
            int currentIslandId = _roleName == "Admin" && _cbIsland.SelectedValue != null
                ? Convert.ToInt32(_cbIsland.SelectedValue)
                : _islandId;
            
            using var logForm = new MovementLogViewForm(_username, _roleName, currentIslandId);
            logForm.ShowDialog(this);
        }
        
        #endregion
    }
    
    /// <summary>
    /// 刑具操作类型
    /// </summary>
    public enum ToolOperation
    {
        Add,
        Update,
        Remove
    }
}
