using System;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 处刑台移动对话框
    /// 功能：确认移动操作，支持自定义时间
    /// </summary>
    public partial class PlatformMoveDialog : Form
    {
        private readonly ExecutionPlatformModel _platform;
        private readonly string _targetLocation;
        private readonly bool _isToTrialHall;
        
        // UI控件
        private readonly RadioButton _rbCurrentTime;
        private readonly RadioButton _rbCustomTime;
        private readonly DateTimePicker _dtpCustomTime;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;
        
        /// <summary>
        /// 自定义时间（如果选择了自定义时间）
        /// </summary>
        public DateTime? CustomTime { get; private set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="platform">处刑台模型</param>
        /// <param name="targetLocation">目标位置描述</param>
        /// <param name="isToTrialHall">是否移动到审判庭</param>
        public PlatformMoveDialog(ExecutionPlatformModel platform, string targetLocation, bool isToTrialHall)
        {
            _platform = platform;
            _targetLocation = targetLocation;
            _isToTrialHall = isToTrialHall;
            
            InitializeComponent();
            
            Text = isToTrialHall ? "移动到审判庭" : "返回原位";
            Size = new Size(450, 350);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // 初始化控件
            _rbCurrentTime = new RadioButton { Text = "使用当前时间", Checked = true, AutoSize = true };
            _rbCustomTime = new RadioButton { Text = "自定义时间", AutoSize = true };
            _dtpCustomTime = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm:ss",
                ShowUpDown = false,
                Width = 200,
                Enabled = false
            };
            _btnOk = new Button { Text = "确定", DialogResult = DialogResult.OK, Width = 80 };
            _btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 80 };
            
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
            
            // 布局
            BuildLayout();
            
            // 事件绑定
            _rbCurrentTime.CheckedChanged += (s, e) => _dtpCustomTime.Enabled = !_rbCurrentTime.Checked;
            _rbCustomTime.CheckedChanged += (s, e) => _dtpCustomTime.Enabled = _rbCustomTime.Checked;
            _btnOk.Click += BtnOk_Click;
        }
        
        /// <summary>
        /// 构建布局
        /// </summary>
        private void BuildLayout()
        {
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };
            
            int y = 10;
            
            // 标题
            var lblTitle = new Label
            {
                Text = _isToTrialHall ? "移动到审判庭" : "返回原位",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblTitle);
            y += 40;
            
            // 处刑台信息
            var lblPlatform = new Label
            {
                Text = $"处刑台编号：{_platform.PlatformNumber}",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblPlatform);
            y += 25;
            
            var lblCurrent = new Label
            {
                Text = $"当前位置：{_platform.LocationDescription}",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblCurrent);
            y += 25;
            
            var lblTarget = new Label
            {
                Text = $"目标位置：{_targetLocation}",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblTarget);
            y += 25;
            
            var lblTool = new Label
            {
                Text = $"刑具：{(_platform.HasTool ? _platform.ToolName : "无")}",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblTool);
            y += 35;
            
            // 分隔线
            var separator = new Panel
            {
                Height = 1,
                BackColor = Color.Gray,
                Location = new Point(10, y),
                Width = 400
            };
            mainPanel.Controls.Add(separator);
            y += 15;
            
            // 时间选择
            var lblTimeTitle = new Label
            {
                Text = "移动时间：",
                Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblTimeTitle);
            y += 30;
            
            _rbCurrentTime.Location = new Point(20, y);
            mainPanel.Controls.Add(_rbCurrentTime);
            y += 30;
            
            _rbCustomTime.Location = new Point(20, y);
            mainPanel.Controls.Add(_rbCustomTime);
            
            _dtpCustomTime.Location = new Point(150, y - 3);
            mainPanel.Controls.Add(_dtpCustomTime);
            y += 40;
            
            // 按钮
            var btnPanel = new Panel
            {
                Height = 40,
                Dock = DockStyle.Bottom
            };
            
            _btnCancel.Location = new Point(320, 5);
            _btnOk.Location = new Point(230, 5);
            
            btnPanel.Controls.Add(_btnCancel);
            btnPanel.Controls.Add(_btnOk);
            
            Controls.Add(mainPanel);
            Controls.Add(btnPanel);
        }
        
        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (_rbCustomTime.Checked)
            {
                CustomTime = _dtpCustomTime.Value;
                
                // 验证时间不能是未来时间
                if (CustomTime > DateTime.Now)
                {
                    MessageBox.Show("自定义时间不能是未来时间。", "时间错误", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DialogResult = DialogResult.None;
                    return;
                }
            }
            else
            {
                CustomTime = null; // 使用当前时间
            }
        }
    }
}
