using System;
using System.Drawing;
using System.Windows.Forms;
using WitchTrialSystem.DAL;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 编辑魔女公开描述的窗口
    /// </summary>
    public partial class WitchEditDescriptionForm : Form
    {
        private readonly int _witchId;
        private readonly string _witchName;
        private readonly string _prisonerNo;
        private readonly WitchDAL _dal;

        private Label _lblInfo;
        private Label _lblDesc;
        private TextBox _txtDescription;
        private Label _lblCount;
        private Button _btnSave;
        private Button _btnCancel;

        public WitchEditDescriptionForm(int witchId, string witchName, string prisonerNo, string currentDescription)
        {
            _witchId = witchId;
            _witchName = witchName;
            _prisonerNo = prisonerNo;
            _dal = new WitchDAL();

            InitializeComponent();
            BLL.IconHelper.SetFormIcon(this);  // 设置应用程序图标
            LoadData(currentDescription);
        }

        private void InitializeComponent()
        {
            this.Text = $"编辑公开描述 - {_witchName} ({_prisonerNo})";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // 信息标签
            _lblInfo = new Label
            {
                Text = $"魔女：{_witchName} ({_prisonerNo})",
                Location = new Point(20, 20),
                Size = new Size(550, 25),
                Font = new Font("微软雅黑", 10, FontStyle.Bold)
            };

            // 描述标签
            _lblDesc = new Label
            {
                Text = "公开描述：",
                Location = new Point(20, 60),
                Size = new Size(100, 25),
                Font = new Font("微软雅黑", 9)
            };

            // 描述文本框
            _txtDescription = new TextBox
            {
                Location = new Point(20, 90),
                Size = new Size(550, 300),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("微软雅黑", 9),
                WordWrap = true
            };

            // 字数统计
            _lblCount = new Label
            {
                Location = new Point(20, 400),
                Size = new Size(200, 20),
                Font = new Font("微软雅黑", 8),
                ForeColor = Color.Gray
            };

            _txtDescription.TextChanged += (s, e) =>
            {
                _lblCount.Text = $"字数：{_txtDescription.Text.Length}";
            };

            // 保存按钮
            _btnSave = new Button
            {
                Text = "保存修改",
                Location = new Point(400, 430),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSave.FlatAppearance.BorderSize = 0;
            _btnSave.Click += BtnSave_Click;

            // 取消按钮
            _btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(490, 430),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnCancel.FlatAppearance.BorderSize = 0;
            _btnCancel.Click += (s, e) => this.Close();

            // 添加控件
            this.Controls.AddRange(new Control[] {
                _lblInfo, _lblDesc, _txtDescription, _lblCount, _btnSave, _btnCancel
            });
        }

        private void LoadData(string currentDescription)
        {
            _txtDescription.Text = currentDescription ?? "";
            _lblCount.Text = $"字数：{_txtDescription.Text.Length}";
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                string newDescription = _txtDescription.Text.Trim();

                // 调用 DAL 更新
                _dal.UpdateDescription(_witchId, newDescription);

                MessageBox.Show("公开描述已更新！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"更新失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
