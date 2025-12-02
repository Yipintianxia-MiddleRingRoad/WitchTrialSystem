using System;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 工作经历编辑对话框
    /// </summary>
    public class WorkEditDialog : Form
    {
        private TextBox _txtPeriod;
        private TextBox _txtCompany;
        private TextBox _txtPosition;
        private TextBox _txtSalary;
        private TextBox _txtResignReason;
        private Button _btnOK;
        private Button _btnCancel;

        public WorkRecord Record { get; private set; }

        public WorkEditDialog(WorkRecord? record = null)
        {
            Record = record ?? new WorkRecord();
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "编辑工作经历";
            this.Size = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 时间段
            var lblPeriod = new Label
            {
                Text = "时间段：",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtPeriod = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9),
                PlaceholderText = "例如：2020/04-2022/03"
            };

            // 公司
            var lblCompany = new Label
            {
                Text = "公司名称：",
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtCompany = new TextBox
            {
                Location = new Point(130, 60),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9)
            };

            // 职位
            var lblPosition = new Label
            {
                Text = "职位：",
                Location = new Point(20, 100),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtPosition = new TextBox
            {
                Location = new Point(130, 100),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9)
            };

            // 薪资
            var lblSalary = new Label
            {
                Text = "薪资：",
                Location = new Point(20, 140),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtSalary = new TextBox
            {
                Location = new Point(130, 140),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9),
                PlaceholderText = "例如：月薪 25 万日元"
            };

            // 离职原因
            var lblResignReason = new Label
            {
                Text = "离职原因：",
                Location = new Point(20, 180),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtResignReason = new TextBox
            {
                Location = new Point(130, 180),
                Size = new Size(340, 80),
                Font = new Font("微软雅黑", 9),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // 确定按钮
            _btnOK = new Button
            {
                Text = "确定",
                Location = new Point(300, 290),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            _btnOK.FlatAppearance.BorderSize = 0;
            _btnOK.Click += BtnOK_Click;

            // 取消按钮
            _btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(390, 290),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            _btnCancel.FlatAppearance.BorderSize = 0;

            // 添加控件
            this.Controls.AddRange(new Control[] {
                lblPeriod, _txtPeriod,
                lblCompany, _txtCompany,
                lblPosition, _txtPosition,
                lblSalary, _txtSalary,
                lblResignReason, _txtResignReason,
                _btnOK, _btnCancel
            });

            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void LoadData()
        {
            _txtPeriod.Text = Record.Period;
            _txtCompany.Text = Record.Company;
            _txtPosition.Text = Record.Position;
            _txtSalary.Text = Record.Salary;
            _txtResignReason.Text = Record.ResignReason;
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(_txtCompany.Text))
            {
                MessageBox.Show("请输入公司名称", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtCompany.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtPosition.Text))
            {
                MessageBox.Show("请输入职位", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtPosition.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            // 保存数据
            Record.Period = _txtPeriod.Text.Trim();
            Record.Company = _txtCompany.Text.Trim();
            Record.Position = _txtPosition.Text.Trim();
            Record.Salary = _txtSalary.Text.Trim();
            Record.ResignReason = _txtResignReason.Text.Trim();
        }
    }
}
