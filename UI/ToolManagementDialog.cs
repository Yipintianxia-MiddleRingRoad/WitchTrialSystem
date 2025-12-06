using System;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 刑具管理对话框
    /// 功能：添加、更换、移除刑具
    /// </summary>
    public partial class ToolManagementDialog : Form
    {
        private readonly ExecutionPlatformModel _platform;
        private readonly ToolOperation _operation;
        
        // UI控件
        private readonly TextBox _txtToolName;
        private readonly TextBox _txtToolType;
        private readonly TextBox _txtToolDescription;
        private readonly Button _btnOk;
        private readonly Button _btnCancel;
        
        /// <summary>
        /// 刑具名称
        /// </summary>
        public string ToolName => _txtToolName.Text.Trim();
        
        /// <summary>
        /// 刑具类型
        /// </summary>
        public string ToolType => _txtToolType.Text.Trim();
        
        /// <summary>
        /// 刑具描述
        /// </summary>
        public string ToolDescription => _txtToolDescription.Text.Trim();
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="platform">处刑台模型</param>
        /// <param name="operation">操作类型</param>
        public ToolManagementDialog(ExecutionPlatformModel platform, ToolOperation operation)
        {
            _platform = platform;
            _operation = operation;
            
            InitializeComponent();
            
            Text = operation == ToolOperation.Add ? "添加刑具" : "更换刑具";
            Size = new Size(450, 400);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // 初始化控件
            _txtToolName = new TextBox { Width = 300 };
            _txtToolType = new TextBox { Width = 300 };
            _txtToolDescription = new TextBox
            {
                Width = 300,
                Height = 80,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            _btnOk = new Button { Text = "确定", DialogResult = DialogResult.OK, Width = 80 };
            _btnCancel = new Button { Text = "取消", DialogResult = DialogResult.Cancel, Width = 80 };
            
            AcceptButton = _btnOk;
            CancelButton = _btnCancel;
            
            // 如果是更换操作，填充现有数据
            if (operation == ToolOperation.Update && platform.HasTool)
            {
                _txtToolName.Text = platform.ToolName ?? "";
                _txtToolType.Text = platform.ToolType ?? "";
                _txtToolDescription.Text = platform.ToolDescription ?? "";
            }
            
            // 布局
            BuildLayout();
            
            // 事件绑定
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
                Text = _operation == ToolOperation.Add ? "添加刑具" : "更换刑具",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblTitle);
            y += 40;
            
            // 处刑台信息
            var lblPlatform = new Label
            {
                Text = $"处刑台编号：{_platform.PlatformNumber}  位置：{_platform.LocationDescription}",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblPlatform);
            y += 35;
            
            // 刑具名称
            var lblName = new Label
            {
                Text = "刑具名称：*",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblName);
            y += 25;
            
            _txtToolName.Location = new Point(10, y);
            mainPanel.Controls.Add(_txtToolName);
            y += 35;
            
            // 刑具类型
            var lblType = new Label
            {
                Text = "刑具类型：*",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblType);
            y += 25;
            
            _txtToolType.Location = new Point(10, y);
            mainPanel.Controls.Add(_txtToolType);
            y += 35;
            
            // 刑具描述
            var lblDesc = new Label
            {
                Text = "刑具描述：",
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblDesc);
            y += 25;
            
            _txtToolDescription.Location = new Point(10, y);
            mainPanel.Controls.Add(_txtToolDescription);
            y += 90;
            
            // 提示
            var lblHint = new Label
            {
                Text = "* 为必填项",
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(10, y)
            };
            mainPanel.Controls.Add(lblHint);
            
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
            // 验证必填项
            if (string.IsNullOrWhiteSpace(_txtToolName.Text))
            {
                MessageBox.Show("请输入刑具名称。", "验证失败", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtToolName.Focus();
                DialogResult = DialogResult.None;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(_txtToolType.Text))
            {
                MessageBox.Show("请输入刑具类型。", "验证失败", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtToolType.Focus();
                DialogResult = DialogResult.None;
                return;
            }
        }
    }
}
