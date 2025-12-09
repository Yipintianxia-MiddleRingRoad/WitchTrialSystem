using System;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.Models;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 教育经历编辑对话框
    /// </summary>
    public class EducationEditDialog : Form
    {
        private TextBox _txtSchool;
        private TextBox _txtDegree;
        private ComboBox _cbStatus;
        private TextBox _txtSpecialNote;
        private Button _btnOK;
        private Button _btnCancel;

        public EducationRecord Record { get; private set; }

        public EducationEditDialog(EducationRecord? record = null)
        {
            Record = record ?? new EducationRecord();
            InitializeComponent();
            BLL.IconHelper.SetFormIcon(this);  // 设置应用程序图标
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "编辑教育经历";
            this.Size = new Size(500, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 学校
            var lblSchool = new Label
            {
                Text = "学校名称：",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtSchool = new TextBox
            {
                Location = new Point(130, 20),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9)
            };

            // 学历
            var lblDegree = new Label
            {
                Text = "学历：",
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtDegree = new TextBox
            {
                Location = new Point(130, 60),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9),
                PlaceholderText = "例如：中学校、高等学校、大学"
            };

            // 状态
            var lblStatus = new Label
            {
                Text = "状态：",
                Location = new Point(20, 100),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _cbStatus = new ComboBox
            {
                Location = new Point(130, 100),
                Size = new Size(340, 25),
                Font = new Font("微软雅黑", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cbStatus.Items.AddRange(new object[] { "毕业", "在读", "未入学" });

            // 特殊说明
            var lblSpecialNote = new Label
            {
                Text = "特殊说明：",
                Location = new Point(20, 140),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            _txtSpecialNote = new TextBox
            {
                Location = new Point(130, 140),
                Size = new Size(340, 100),
                Font = new Font("微软雅黑", 9),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            // 确定按钮
            _btnOK = new Button
            {
                Text = "确定",
                Location = new Point(300, 260),
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
                Location = new Point(390, 260),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            _btnCancel.FlatAppearance.BorderSize = 0;

            // 添加控件
            this.Controls.AddRange(new Control[] {
                lblSchool, _txtSchool,
                lblDegree, _txtDegree,
                lblStatus, _cbStatus,
                lblSpecialNote, _txtSpecialNote,
                _btnOK, _btnCancel
            });

            this.AcceptButton = _btnOK;
            this.CancelButton = _btnCancel;
        }

        private void LoadData()
        {
            _txtSchool.Text = Record.School;
            _txtDegree.Text = Record.Degree;
            _txtSpecialNote.Text = Record.SpecialNote;

            if (!string.IsNullOrEmpty(Record.Status))
            {
                int index = _cbStatus.Items.IndexOf(Record.Status);
                if (index >= 0)
                    _cbStatus.SelectedIndex = index;
            }
        }

        private void BtnOK_Click(object? sender, EventArgs e)
        {
            // 验证必填字段
            if (string.IsNullOrWhiteSpace(_txtSchool.Text))
            {
                MessageBox.Show("请输入学校名称", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtSchool.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (string.IsNullOrWhiteSpace(_txtDegree.Text))
            {
                MessageBox.Show("请输入学历", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _txtDegree.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            if (_cbStatus.SelectedIndex < 0)
            {
                MessageBox.Show("请选择状态", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _cbStatus.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            // 保存数据
            Record.School = _txtSchool.Text.Trim();
            Record.Degree = _txtDegree.Text.Trim();
            Record.Status = _cbStatus.SelectedItem?.ToString() ?? "";
            Record.SpecialNote = _txtSpecialNote.Text.Trim();
        }
    }
}
