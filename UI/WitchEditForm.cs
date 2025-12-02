using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;
using Newtonsoft.Json;
using System.Data;

namespace WitchTrialSystem.UI
{
    public partial class WitchEditForm : Form
    {
        private readonly int _witchId;
        private TabControl tabControl;
        private Button btnSave;
        private Button btnCancel;
        private ErrorProvider errorProvider;
        
        // 基本信息
        private TextBox txtName;
        private TextBox txtPrisonerNo;
        private TextBox txtPersonalNo;
        private ComboBox cmbGender;
        private DateTimePicker dtpBirthDate;
        private TextBox txtNationality;
        private TextBox txtBirthplace;
        private TextBox txtFormerName;
        
        // 身体特征
        private NumericUpDown numHeight;
        private NumericUpDown numWeight;
        private ComboBox cmbBloodType;
        
        // 联系方式
        private TextBox txtAddress;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtLine;
        
        // 教育背景
        private TextBox txtHighestEducation;
        private DataGridView dgvEducation;
        private Button btnAddEducation;
        private Button btnEditEducation;
        private Button btnDeleteEducation;
        private List<EducationRecord> educationRecords = new List<EducationRecord>();
        
        // 工作经历
        private DataGridView dgvWork;
        private Button btnAddWork;
        private Button btnEditWork;
        private Button btnDeleteWork;
        private List<WorkRecord> workRecords = new List<WorkRecord>();
        
        // 家庭关系
        private TextBox txtFamilyStructure;
        private TextBox txtFather;
        private TextBox txtMother;
        private TextBox txtOtherFamily1;
        private TextBox txtOtherFamily2;
        private TextBox txtOtherFamily3;
        
        // 个性特征
        private TextBox txtSkills;
        private TextBox txtHobbies;
        private TextBox txtIdeal;
        private TextBox txtDislike;
        private TextBox txtTrauma;
        
        // 魔女信息
        private TextBox txtMagic;
        private ComboBox cmbStatus;
        private TextBox txtWitchMethod;
        private TextBox txtRemarks;
        private TextBox txtPublicDescription;
        
        // 分配信息
        private ComboBox cmbIsland;
        private ComboBox cmbBatch;
        private TextBox txtAvatarPath;
        private Button btnBrowseAvatar;
        private Label lblBatchInfo;
        
        // 时间戳
        private DateTimePicker dtpCaptureTime;
        private DateTimePicker dtpDepartureTime;
        private DateTimePicker dtpArrivalTime;
        private DateTimePicker dtpDeathTime;
        private CheckBox chkCaptureTime;
        private CheckBox chkDepartureTime;
        private CheckBox chkArrivalTime;
        private CheckBox chkDeathTime;

        public WitchEditForm(int witchId)
        {
            _witchId = witchId;
            InitializeComponent();
            InitializeCustomComponents();
            LoadWitchData();
        }

        private void InitializeComponent()
        {
            this.Text = $"编辑魔女信息 - ID: {_witchId}";
            this.Size = new Size(920, 740);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
        }

        private void InitializeCustomComponents()
        {
            // 创建 TabControl
            tabControl = new TabControl();
            tabControl.Location = new Point(10, 10);
            tabControl.Size = new Size(880, 630);
            this.Controls.Add(tabControl);
            
            // 创建各个标签页
            CreateBasicInfoTab();
            CreatePhysicalTab();
            CreateContactTab();
            CreateEducationTab();
            CreateWorkTab();
            CreateFamilyTab();
            CreatePersonalityTab();
            CreateWitchInfoTab();
            CreateAssignmentTab();
            CreateTimestampTab();
            
            // 创建保存和取消按钮
            btnSave = new Button();
            btnSave.Text = "保存修改";
            btnSave.Location = new Point(690, 650);
            btnSave.Size = new Size(90, 35);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            btnCancel = new Button();
            btnCancel.Text = "取消";
            btnCancel.Location = new Point(790, 650);
            btnCancel.Size = new Size(90, 35);
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
            
            this.CancelButton = btnCancel;
        }

        private void CreateBasicInfoTab()
        {
            TabPage tab = new TabPage("基本信息");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 200;
            int spacing = 35;
            
            // 姓名（必填）
            Label lblName = new Label { Text = "姓名*:", Location = new Point(20, y), Width = labelWidth };
            txtName = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblName);
            tab.Controls.Add(txtName);
            
            // 囚犯编号
            Label lblPrisonerNo = new Label { Text = "囚犯编号:", Location = new Point(400, y), Width = labelWidth };
            txtPrisonerNo = new TextBox { Location = new Point(510, y), Width = controlWidth };
            tab.Controls.Add(lblPrisonerNo);
            tab.Controls.Add(txtPrisonerNo);
            
            y += spacing;
            
            // 个人番号
            Label lblPersonalNo = new Label { Text = "个人番号:", Location = new Point(20, y), Width = labelWidth };
            txtPersonalNo = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblPersonalNo);
            tab.Controls.Add(txtPersonalNo);
            
            // 性别
            Label lblGender = new Label { Text = "性别:", Location = new Point(400, y), Width = labelWidth };
            cmbGender = new ComboBox { Location = new Point(510, y), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbGender.Items.AddRange(new object[] { "女", "男", "其他" });
            tab.Controls.Add(lblGender);
            tab.Controls.Add(cmbGender);
            
            y += spacing;
            
            // 出生日期
            Label lblBirthDate = new Label { Text = "出生日期:", Location = new Point(20, y), Width = labelWidth };
            dtpBirthDate = new DateTimePicker { Location = new Point(130, y), Width = controlWidth, Format = DateTimePickerFormat.Short };
            tab.Controls.Add(lblBirthDate);
            tab.Controls.Add(dtpBirthDate);
            
            // 民族
            Label lblNationality = new Label { Text = "民族:", Location = new Point(400, y), Width = labelWidth };
            txtNationality = new TextBox { Location = new Point(510, y), Width = controlWidth };
            tab.Controls.Add(lblNationality);
            tab.Controls.Add(txtNationality);
            
            y += spacing;
            
            // 籍贯
            Label lblBirthplace = new Label { Text = "籍贯:", Location = new Point(20, y), Width = labelWidth };
            txtBirthplace = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblBirthplace);
            tab.Controls.Add(txtBirthplace);
            
            // 曾用名
            Label lblFormerName = new Label { Text = "曾用名:", Location = new Point(400, y), Width = labelWidth };
            txtFormerName = new TextBox { Location = new Point(510, y), Width = controlWidth };
            tab.Controls.Add(lblFormerName);
            tab.Controls.Add(txtFormerName);
        }

        private void CreatePhysicalTab()
        {
            TabPage tab = new TabPage("身体特征");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 200;
            int spacing = 35;
            
            // 身高
            Label lblHeight = new Label { Text = "身高(cm):", Location = new Point(20, y), Width = labelWidth };
            numHeight = new NumericUpDown { Location = new Point(130, y), Width = controlWidth, DecimalPlaces = 1, Minimum = 0, Maximum = 300 };
            tab.Controls.Add(lblHeight);
            tab.Controls.Add(numHeight);
            
            y += spacing;
            
            // 体重
            Label lblWeight = new Label { Text = "体重(kg):", Location = new Point(20, y), Width = labelWidth };
            numWeight = new NumericUpDown { Location = new Point(130, y), Width = controlWidth, DecimalPlaces = 1, Minimum = 0, Maximum = 300 };
            tab.Controls.Add(lblWeight);
            tab.Controls.Add(numWeight);
            
            y += spacing;
            
            // 血型
            Label lblBloodType = new Label { Text = "血型:", Location = new Point(20, y), Width = labelWidth };
            cmbBloodType = new ComboBox { Location = new Point(130, y), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBloodType.Items.AddRange(new object[] { "A", "B", "AB", "O", "未知" });
            tab.Controls.Add(lblBloodType);
            tab.Controls.Add(cmbBloodType);
        }

        private void CreateContactTab()
        {
            TabPage tab = new TabPage("联系方式");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 600;
            int spacing = 35;
            
            // 地址
            Label lblAddress = new Label { Text = "地址:", Location = new Point(20, y), Width = labelWidth };
            txtAddress = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblAddress);
            tab.Controls.Add(txtAddress);
            
            y += spacing;
            
            // 电话
            Label lblPhone = new Label { Text = "电话:", Location = new Point(20, y), Width = labelWidth };
            txtPhone = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblPhone);
            tab.Controls.Add(txtPhone);
            
            y += spacing;
            
            // 邮箱
            Label lblEmail = new Label { Text = "邮箱:", Location = new Point(20, y), Width = labelWidth };
            txtEmail = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblEmail);
            tab.Controls.Add(txtEmail);
            
            y += spacing;
            
            // LINE账号
            Label lblLine = new Label { Text = "LINE账号:", Location = new Point(20, y), Width = labelWidth };
            txtLine = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblLine);
            tab.Controls.Add(txtLine);
        }

        private void CreateEducationTab()
        {
            TabPage tab = new TabPage("教育背景");
            tabControl.TabPages.Add(tab);
            
            // 最高学历
            Label lblHighestEducation = new Label { Text = "最高学历:", Location = new Point(20, 20), Width = 100 };
            txtHighestEducation = new TextBox { Location = new Point(130, 20), Width = 200 };
            tab.Controls.Add(lblHighestEducation);
            tab.Controls.Add(txtHighestEducation);
            
            // 教育经历列表
            Label lblEducationList = new Label { Text = "教育经历:", Location = new Point(20, 60), Width = 100 };
            tab.Controls.Add(lblEducationList);
            
            dgvEducation = new DataGridView();
            dgvEducation.Location = new Point(20, 85);
            dgvEducation.Size = new Size(820, 400);
            dgvEducation.AllowUserToAddRows = false;
            dgvEducation.AllowUserToDeleteRows = false;
            dgvEducation.ReadOnly = true;
            dgvEducation.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvEducation.MultiSelect = false;
            dgvEducation.AutoGenerateColumns = false;
            
            dgvEducation.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "学校", DataPropertyName = "School", Width = 200 });
            dgvEducation.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "学历", DataPropertyName = "Degree", Width = 150 });
            dgvEducation.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "状态", DataPropertyName = "Status", Width = 150 });
            dgvEducation.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "特殊说明", DataPropertyName = "SpecialNote", Width = 300 });
            
            tab.Controls.Add(dgvEducation);
            
            // 按钮
            btnAddEducation = new Button { Text = "添加", Location = new Point(20, 500), Size = new Size(80, 30) };
            btnAddEducation.Click += BtnAddEducation_Click;
            tab.Controls.Add(btnAddEducation);
            
            btnEditEducation = new Button { Text = "编辑", Location = new Point(110, 500), Size = new Size(80, 30) };
            btnEditEducation.Click += BtnEditEducation_Click;
            tab.Controls.Add(btnEditEducation);
            
            btnDeleteEducation = new Button { Text = "删除", Location = new Point(200, 500), Size = new Size(80, 30) };
            btnDeleteEducation.Click += BtnDeleteEducation_Click;
            tab.Controls.Add(btnDeleteEducation);
        }

        private void CreateWorkTab()
        {
            TabPage tab = new TabPage("工作经历");
            tabControl.TabPages.Add(tab);
            
            // 工作经历列表
            dgvWork = new DataGridView();
            dgvWork.Location = new Point(20, 20);
            dgvWork.Size = new Size(820, 460);
            dgvWork.AllowUserToAddRows = false;
            dgvWork.AllowUserToDeleteRows = false;
            dgvWork.ReadOnly = true;
            dgvWork.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWork.MultiSelect = false;
            dgvWork.AutoGenerateColumns = false;
            
            dgvWork.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "时间段", DataPropertyName = "Period", Width = 150 });
            dgvWork.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "公司", DataPropertyName = "Company", Width = 200 });
            dgvWork.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "职位", DataPropertyName = "Position", Width = 150 });
            dgvWork.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "薪资", DataPropertyName = "Salary", Width = 100 });
            dgvWork.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "离职原因", DataPropertyName = "ResignReason", Width = 200 });
            
            tab.Controls.Add(dgvWork);
            
            // 按钮
            btnAddWork = new Button { Text = "添加", Location = new Point(20, 500), Size = new Size(80, 30) };
            btnAddWork.Click += BtnAddWork_Click;
            tab.Controls.Add(btnAddWork);
            
            btnEditWork = new Button { Text = "编辑", Location = new Point(110, 500), Size = new Size(80, 30) };
            btnEditWork.Click += BtnEditWork_Click;
            tab.Controls.Add(btnEditWork);
            
            btnDeleteWork = new Button { Text = "删除", Location = new Point(200, 500), Size = new Size(80, 30) };
            btnDeleteWork.Click += BtnDeleteWork_Click;
            tab.Controls.Add(btnDeleteWork);
        }

        private void CreateFamilyTab()
        {
            TabPage tab = new TabPage("家庭关系");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 120;
            int controlWidth = 600;
            int spacing = 35;
            
            // 家庭结构
            Label lblFamilyStructure = new Label { Text = "家庭结构:", Location = new Point(20, y), Width = labelWidth };
            txtFamilyStructure = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblFamilyStructure);
            tab.Controls.Add(txtFamilyStructure);
            
            y += spacing;
            
            // 父亲
            Label lblFather = new Label { Text = "父亲:", Location = new Point(20, y), Width = labelWidth };
            txtFather = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblFather);
            tab.Controls.Add(txtFather);
            
            y += spacing;
            
            // 母亲
            Label lblMother = new Label { Text = "母亲:", Location = new Point(20, y), Width = labelWidth };
            txtMother = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblMother);
            tab.Controls.Add(txtMother);
            
            y += spacing;
            
            // 其他家庭成员1
            Label lblOtherFamily1 = new Label { Text = "其他家庭成员1:", Location = new Point(20, y), Width = labelWidth };
            txtOtherFamily1 = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblOtherFamily1);
            tab.Controls.Add(txtOtherFamily1);
            
            y += spacing;
            
            // 其他家庭成员2
            Label lblOtherFamily2 = new Label { Text = "其他家庭成员2:", Location = new Point(20, y), Width = labelWidth };
            txtOtherFamily2 = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblOtherFamily2);
            tab.Controls.Add(txtOtherFamily2);
            
            y += spacing;
            
            // 其他家庭成员3
            Label lblOtherFamily3 = new Label { Text = "其他家庭成员3:", Location = new Point(20, y), Width = labelWidth };
            txtOtherFamily3 = new TextBox { Location = new Point(150, y), Width = controlWidth };
            tab.Controls.Add(lblOtherFamily3);
            tab.Controls.Add(txtOtherFamily3);
        }

        private void CreatePersonalityTab()
        {
            TabPage tab = new TabPage("个性特征");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 600;
            int spacing = 35;
            
            // 技能特长
            Label lblSkills = new Label { Text = "技能特长:", Location = new Point(20, y), Width = labelWidth };
            txtSkills = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblSkills);
            tab.Controls.Add(txtSkills);
            
            y += spacing;
            
            // 兴趣爱好
            Label lblHobbies = new Label { Text = "兴趣爱好:", Location = new Point(20, y), Width = labelWidth };
            txtHobbies = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblHobbies);
            tab.Controls.Add(txtHobbies);
            
            y += spacing;
            
            // 理想
            Label lblIdeal = new Label { Text = "理想:", Location = new Point(20, y), Width = labelWidth };
            txtIdeal = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblIdeal);
            tab.Controls.Add(txtIdeal);
            
            y += spacing;
            
            // 讨厌的事物
            Label lblDislike = new Label { Text = "讨厌的事物:", Location = new Point(20, y), Width = labelWidth };
            txtDislike = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblDislike);
            tab.Controls.Add(txtDislike);
            
            y += spacing;
            
            // 心理创伤
            Label lblTrauma = new Label { Text = "心理创伤:", Location = new Point(20, y), Width = labelWidth };
            txtTrauma = new TextBox { Location = new Point(130, y), Width = controlWidth, Multiline = true, Height = 100 };
            tab.Controls.Add(lblTrauma);
            tab.Controls.Add(txtTrauma);
        }

        private void CreateWitchInfoTab()
        {
            TabPage tab = new TabPage("魔女信息");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 600;
            int spacing = 35;
            
            // 魔法（必填）
            Label lblMagic = new Label { Text = "魔法*:", Location = new Point(20, y), Width = labelWidth };
            txtMagic = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblMagic);
            tab.Controls.Add(txtMagic);
            
            y += spacing;
            
            // 状态（必填）
            Label lblStatus = new Label { Text = "状态*:", Location = new Point(20, y), Width = labelWidth };
            cmbStatus = new ComboBox { Location = new Point(130, y), Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new object[] { "待分配", "分配至岛屿", "审判中", "死亡(正常)", "死亡(魔女化)", "其它" });
            tab.Controls.Add(lblStatus);
            tab.Controls.Add(cmbStatus);
            
            y += spacing;
            
            // 魔女化办法
            Label lblWitchMethod = new Label { Text = "魔女化办法:", Location = new Point(20, y), Width = labelWidth };
            txtWitchMethod = new TextBox { Location = new Point(130, y), Width = controlWidth };
            tab.Controls.Add(lblWitchMethod);
            tab.Controls.Add(txtWitchMethod);
            
            y += spacing;
            
            // 备注
            Label lblRemarks = new Label { Text = "备注:", Location = new Point(20, y), Width = labelWidth };
            txtRemarks = new TextBox { Location = new Point(130, y), Width = controlWidth, Multiline = true, Height = 100 };
            tab.Controls.Add(lblRemarks);
            tab.Controls.Add(txtRemarks);
            
            y += 110;
            
            // 公开描述
            Label lblPublicDescription = new Label { Text = "公开描述:", Location = new Point(20, y), Width = labelWidth };
            txtPublicDescription = new TextBox { Location = new Point(130, y), Width = controlWidth, Multiline = true, Height = 150 };
            tab.Controls.Add(lblPublicDescription);
            tab.Controls.Add(txtPublicDescription);
        }

        private void CreateAssignmentTab()
        {
            TabPage tab = new TabPage("分配信息");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 100;
            int controlWidth = 200;
            int spacing = 35;
            
            // 岛屿
            Label lblIsland = new Label { Text = "岛屿:", Location = new Point(20, y), Width = labelWidth };
            cmbIsland = new ComboBox { Location = new Point(130, y), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbIsland.SelectedIndexChanged += CmbIsland_SelectedIndexChanged;
            tab.Controls.Add(lblIsland);
            tab.Controls.Add(cmbIsland);
            
            y += spacing;
            
            // 批次
            Label lblBatch = new Label { Text = "批次:", Location = new Point(20, y), Width = labelWidth };
            cmbBatch = new ComboBox { Location = new Point(130, y), Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBatch.SelectedIndexChanged += CmbBatch_SelectedIndexChanged;
            tab.Controls.Add(lblBatch);
            tab.Controls.Add(cmbBatch);
            
            y += spacing;
            
            // 批次信息
            lblBatchInfo = new Label { Text = "", Location = new Point(130, y), Width = 400, ForeColor = Color.Blue };
            tab.Controls.Add(lblBatchInfo);
            
            y += spacing;
            
            // 头像路径
            Label lblAvatarPath = new Label { Text = "头像路径:", Location = new Point(20, y), Width = labelWidth };
            txtAvatarPath = new TextBox { Location = new Point(130, y), Width = 400 };
            tab.Controls.Add(lblAvatarPath);
            tab.Controls.Add(txtAvatarPath);
            
            btnBrowseAvatar = new Button { Text = "浏览...", Location = new Point(540, y - 2), Size = new Size(80, 25) };
            btnBrowseAvatar.Click += BtnBrowseAvatar_Click;
            tab.Controls.Add(btnBrowseAvatar);
        }

        private void CreateTimestampTab()
        {
            TabPage tab = new TabPage("时间记录");
            tabControl.TabPages.Add(tab);
            
            int y = 20;
            int labelWidth = 120;
            int controlWidth = 200;
            int spacing = 45;
            
            // 被抓捕时间
            chkCaptureTime = new CheckBox { Text = "被抓捕时间:", Location = new Point(20, y), Width = labelWidth };
            chkCaptureTime.CheckedChanged += (s, e) => dtpCaptureTime.Enabled = chkCaptureTime.Checked;
            dtpCaptureTime = new DateTimePicker { Location = new Point(150, y), Width = controlWidth, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Enabled = false };
            tab.Controls.Add(chkCaptureTime);
            tab.Controls.Add(dtpCaptureTime);
            
            y += spacing;
            
            // 离开囚牢时间
            chkDepartureTime = new CheckBox { Text = "离开囚牢时间:", Location = new Point(20, y), Width = labelWidth };
            chkDepartureTime.CheckedChanged += (s, e) => dtpDepartureTime.Enabled = chkDepartureTime.Checked;
            dtpDepartureTime = new DateTimePicker { Location = new Point(150, y), Width = controlWidth, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Enabled = false };
            tab.Controls.Add(chkDepartureTime);
            tab.Controls.Add(dtpDepartureTime);
            
            y += spacing;
            
            // 抵达魔女岛时间
            chkArrivalTime = new CheckBox { Text = "抵达魔女岛时间:", Location = new Point(20, y), Width = labelWidth };
            chkArrivalTime.CheckedChanged += (s, e) => dtpArrivalTime.Enabled = chkArrivalTime.Checked;
            dtpArrivalTime = new DateTimePicker { Location = new Point(150, y), Width = controlWidth, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Enabled = false };
            tab.Controls.Add(chkArrivalTime);
            tab.Controls.Add(dtpArrivalTime);
            
            y += spacing;
            
            // 死亡时间
            chkDeathTime = new CheckBox { Text = "死亡时间:", Location = new Point(20, y), Width = labelWidth };
            chkDeathTime.CheckedChanged += (s, e) => dtpDeathTime.Enabled = chkDeathTime.Checked;
            dtpDeathTime = new DateTimePicker { Location = new Point(150, y), Width = controlWidth, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Enabled = false };
            tab.Controls.Add(chkDeathTime);
            tab.Controls.Add(dtpDeathTime);
            
            y += spacing + 10;
            
            // 说明文字
            Label lblNote = new Label();
            lblNote.Text = "提示：勾选复选框后才会保存对应的时间。\n所有时间字段都是可选的，可以后续补充。";
            lblNote.Location = new Point(20, y);
            lblNote.Size = new Size(600, 40);
            lblNote.ForeColor = Color.Gray;
            tab.Controls.Add(lblNote);
        }

        /// <summary>
        /// 加载魔女数据
        /// </summary>
        private void LoadWitchData()
        {
            try
            {
                // 从数据库加载魔女信息
                var dal = new WitchDAL();
                var dt = dal.GetWitchDetail(_witchId);
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("未找到指定的魔女信息。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }
                
                var row = dt.Rows[0];
                
                // 填充基本信息
                txtName.Text = row["Name"]?.ToString() ?? "";
                txtPrisonerNo.Text = row["PrisonerNo"]?.ToString() ?? "";
                txtPersonalNo.Text = row["PersonalNo"]?.ToString() ?? "";
                cmbGender.Text = row["Gender"]?.ToString() ?? "";
                if (row["BirthDate"] != DBNull.Value)
                    dtpBirthDate.Value = Convert.ToDateTime(row["BirthDate"]);
                txtNationality.Text = row["Ethnicity"]?.ToString() ?? "";
                txtBirthplace.Text = row["Birthplace"]?.ToString() ?? "";
                txtFormerName.Text = row["FormerName"]?.ToString() ?? "";
                
                // 填充身体特征
                if (row["Height"] != DBNull.Value)
                    numHeight.Value = Convert.ToDecimal(row["Height"]);
                if (row["Weight"] != DBNull.Value)
                    numWeight.Value = Convert.ToDecimal(row["Weight"]);
                cmbBloodType.Text = row["BloodType"]?.ToString() ?? "";
                
                // 填充联系方式
                txtAddress.Text = row["Address"]?.ToString() ?? "";
                txtPhone.Text = row["Phone"]?.ToString() ?? "";
                txtEmail.Text = row["Email"]?.ToString() ?? "";
                txtLine.Text = row["LineAccount"]?.ToString() ?? "";
                
                // 填充教育背景
                txtHighestEducation.Text = row["HighestEducation"]?.ToString() ?? "";
                LoadEducationHistory(row["EducationHistory"]?.ToString());
                
                // 填充工作经历
                LoadWorkHistory(row["WorkHistory"]?.ToString());
                
                // 填充家庭关系
                txtFamilyStructure.Text = row["FamilyStructure"]?.ToString() ?? "";
                txtFather.Text = row["Father"]?.ToString() ?? "";
                txtMother.Text = row["Mother"]?.ToString() ?? "";
                txtOtherFamily1.Text = row["OtherFamily1"]?.ToString() ?? "";
                txtOtherFamily2.Text = row["OtherFamily2"]?.ToString() ?? "";
                txtOtherFamily3.Text = row["OtherFamily3"]?.ToString() ?? "";
                
                // 填充个性特征
                txtSkills.Text = row["Skills"]?.ToString() ?? "";
                txtHobbies.Text = row["Hobbies"]?.ToString() ?? "";
                txtIdeal.Text = row["Dreams"]?.ToString() ?? "";
                txtDislike.Text = row["Dislikes"]?.ToString() ?? "";
                txtTrauma.Text = row["Trauma"]?.ToString() ?? "";
                
                // 填充魔女信息
                txtMagic.Text = row["Magic"]?.ToString() ?? "";
                cmbStatus.Text = row["Status"]?.ToString() ?? "";
                txtWitchMethod.Text = row["WitchTransformMethod"]?.ToString() ?? "";
                txtRemarks.Text = row["Remarks"]?.ToString() ?? "";
                txtPublicDescription.Text = row["DescriptionPublic"]?.ToString() ?? "";
                
                // 填充分配信息
                LoadIslands();
                if (row["IslandID"] != DBNull.Value)
                {
                    int islandId = Convert.ToInt32(row["IslandID"]);
                    cmbIsland.SelectedIndex = islandId;
                    LoadBatches();
                    if (row["BatchID"] != DBNull.Value)
                    {
                        int batchId = Convert.ToInt32(row["BatchID"]);
                        cmbBatch.Text = $"批次{batchId}";
                    }
                }
                txtAvatarPath.Text = row["AvatarPath"]?.ToString() ?? "";
                
                // 填充时间戳
                LoadTimestamps(row);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载魔女数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
        
        /// <summary>
        /// 加载教育经历
        /// </summary>
        private void LoadEducationHistory(string? educationJson)
        {
            educationRecords.Clear();
            if (!string.IsNullOrWhiteSpace(educationJson))
            {
                try
                {
                    var records = JsonConvert.DeserializeObject<List<EducationRecord>>(educationJson);
                    if (records != null)
                    {
                        educationRecords.AddRange(records);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"解析教育经历数据失败：{ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            RefreshEducationGrid();
        }
        
        /// <summary>
        /// 加载工作经历
        /// </summary>
        private void LoadWorkHistory(string? workJson)
        {
            workRecords.Clear();
            if (!string.IsNullOrWhiteSpace(workJson))
            {
                try
                {
                    var records = JsonConvert.DeserializeObject<List<WorkRecord>>(workJson);
                    if (records != null)
                    {
                        workRecords.AddRange(records);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"解析工作经历数据失败：{ex.Message}", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            RefreshWorkGrid();
        }
        
        /// <summary>
        /// 加载时间戳
        /// </summary>
        private void LoadTimestamps(DataRow row)
        {
            try
            {
                // 被抓捕时间
                if (row.Table.Columns.Contains("CaptureTime") && row["CaptureTime"] != DBNull.Value)
                {
                    chkCaptureTime.Checked = true;
                    dtpCaptureTime.Value = Convert.ToDateTime(row["CaptureTime"]);
                }
                
                // 离开囚牢时间
                if (row.Table.Columns.Contains("DepartureTime") && row["DepartureTime"] != DBNull.Value)
                {
                    chkDepartureTime.Checked = true;
                    dtpDepartureTime.Value = Convert.ToDateTime(row["DepartureTime"]);
                }
                
                // 抵达魔女岛时间
                if (row.Table.Columns.Contains("ArrivalTime") && row["ArrivalTime"] != DBNull.Value)
                {
                    chkArrivalTime.Checked = true;
                    dtpArrivalTime.Value = Convert.ToDateTime(row["ArrivalTime"]);
                }
                
                // 死亡时间
                if (row.Table.Columns.Contains("DeathTime") && row["DeathTime"] != DBNull.Value)
                {
                    chkDeathTime.Checked = true;
                    dtpDeathTime.Value = Convert.ToDateTime(row["DeathTime"]);
                }
            }
            catch (Exception ex)
            {
                // 时间戳字段可能不存在，忽略错误
                System.Diagnostics.Debug.WriteLine($"加载时间戳失败：{ex.Message}");
            }
        }

        // 教育经历事件处理
        private void BtnAddEducation_Click(object sender, EventArgs e)
        {
            using (var dialog = new EducationEditDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    educationRecords.Add(dialog.Record);
                    RefreshEducationGrid();
                }
            }
        }

        private void BtnEditEducation_Click(object sender, EventArgs e)
        {
            if (dgvEducation.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要编辑的教育经历。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            int index = dgvEducation.SelectedRows[0].Index;
            using (var dialog = new EducationEditDialog(educationRecords[index]))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    educationRecords[index] = dialog.Record;
                    RefreshEducationGrid();
                }
            }
        }

        private void BtnDeleteEducation_Click(object sender, EventArgs e)
        {
            if (dgvEducation.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的教育经历。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (MessageBox.Show("确定要删除选中的教育经历吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int index = dgvEducation.SelectedRows[0].Index;
                educationRecords.RemoveAt(index);
                RefreshEducationGrid();
            }
        }

        private void RefreshEducationGrid()
        {
            dgvEducation.DataSource = null;
            dgvEducation.DataSource = educationRecords.ToList();
        }

        // 工作经历事件处理
        private void BtnAddWork_Click(object sender, EventArgs e)
        {
            using (var dialog = new WorkEditDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    workRecords.Add(dialog.Record);
                    RefreshWorkGrid();
                }
            }
        }

        private void BtnEditWork_Click(object sender, EventArgs e)
        {
            if (dgvWork.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要编辑的工作经历。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            int index = dgvWork.SelectedRows[0].Index;
            using (var dialog = new WorkEditDialog(workRecords[index]))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    workRecords[index] = dialog.Record;
                    RefreshWorkGrid();
                }
            }
        }

        private void BtnDeleteWork_Click(object sender, EventArgs e)
        {
            if (dgvWork.SelectedRows.Count == 0)
            {
                MessageBox.Show("请先选择要删除的工作经历。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            if (MessageBox.Show("确定要删除选中的工作经历吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int index = dgvWork.SelectedRows[0].Index;
                workRecords.RemoveAt(index);
                RefreshWorkGrid();
            }
        }

        private void RefreshWorkGrid()
        {
            dgvWork.DataSource = null;
            dgvWork.DataSource = workRecords.ToList();
        }

        // 分配信息事件处理
        private void LoadIslands()
        {
            cmbIsland.Items.Clear();
            cmbIsland.Items.Add(""); // 空选项表示不分配
            cmbIsland.Items.Add("岛屿1");
            cmbIsland.Items.Add("岛屿2");
        }

        private void LoadBatches()
        {
            cmbBatch.Items.Clear();
            lblBatchInfo.Text = "";
            
            if (string.IsNullOrEmpty(cmbIsland.Text))
            {
                return;
            }
            
            // 加载对应岛屿的批次
            int islandId = cmbIsland.SelectedIndex; // 0=空, 1=岛屿1, 2=岛屿2
            
            if (islandId > 0)
            {
                // 根据岛屿加载批次（简化版，实际应该从数据库查询）
                for (int i = 1; i <= 10; i++)
                {
                    cmbBatch.Items.Add($"批次{i}");
                }
            }
        }

        private void CmbIsland_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadBatches();
        }

        private void CmbBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblBatchInfo.Text = "";
            
            if (string.IsNullOrEmpty(cmbBatch.Text) || string.IsNullOrEmpty(cmbIsland.Text))
            {
                return;
            }
            
            // 显示批次信息（简化版）
            lblBatchInfo.Text = "批次信息已选择";
            lblBatchInfo.ForeColor = Color.Blue;
        }

        private void BtnBrowseAvatar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "图片文件|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                dialog.Title = "选择头像图片";
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtAvatarPath.Text = dialog.FileName;
                }
            }
        }

        // 数据验证
        private bool ValidateData()
        {
            errorProvider.Clear();
            bool isValid = true;
            
            // 验证姓名（必填）
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                errorProvider.SetError(txtName, "姓名不能为空");
                isValid = false;
            }
            
            // 验证魔法（必填）
            if (string.IsNullOrWhiteSpace(txtMagic.Text))
            {
                errorProvider.SetError(txtMagic, "魔法不能为空");
                isValid = false;
            }
            
            // 验证状态（必填）
            if (cmbStatus.SelectedIndex < 0)
            {
                errorProvider.SetError(cmbStatus, "请选择状态");
                isValid = false;
            }
            
            // 验证出生日期
            if (dtpBirthDate.Value > DateTime.Now)
            {
                errorProvider.SetError(dtpBirthDate, "出生日期不能晚于当前日期");
                isValid = false;
            }
            
            // 验证邮箱格式
            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                try
                {
                    var addr = new System.Net.Mail.MailAddress(txtEmail.Text);
                    if (addr.Address != txtEmail.Text)
                    {
                        errorProvider.SetError(txtEmail, "邮箱格式不正确");
                        isValid = false;
                    }
                }
                catch
                {
                    errorProvider.SetError(txtEmail, "邮箱格式不正确");
                    isValid = false;
                }
            }
            
            return isValid;
        }

        // 保存按钮点击事件
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateData())
            {
                MessageBox.Show("请修正输入错误后再保存。", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // 序列化教育和工作经历
                string educationJson = educationRecords.Count > 0 ? JsonConvert.SerializeObject(educationRecords) : null;
                string workJson = workRecords.Count > 0 ? JsonConvert.SerializeObject(workRecords) : null;
                
                // 处理岛屿和批次
                int? islandId = null;
                int? batchId = null;
                
                if (!string.IsNullOrEmpty(cmbIsland.Text) && cmbIsland.SelectedIndex > 0)
                {
                    islandId = cmbIsland.SelectedIndex;
                    
                    if (!string.IsNullOrEmpty(cmbBatch.Text))
                    {
                        string batchText = cmbBatch.Text.Replace("批次", "");
                        if (int.TryParse(batchText, out int batch))
                        {
                            batchId = batch;
                        }
                    }
                }
                
                // 如果没有选择岛屿和批次，自动设置状态为"待分配"
                string status = cmbStatus.Text;
                if (!islandId.HasValue && !batchId.HasValue)
                {
                    status = "待分配";
                }
                
                // 调用 DAL 更新（按照 DAL 中的参数顺序）
                WitchDAL.UpdateWitchComplete(
                    _witchId,
                    txtName.Text.Trim(),
                    txtMagic.Text.Trim(),
                    status,
                    string.IsNullOrWhiteSpace(txtPrisonerNo.Text) ? null : txtPrisonerNo.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtPersonalNo.Text) ? null : txtPersonalNo.Text.Trim(),
                    string.IsNullOrWhiteSpace(cmbGender.Text) ? null : cmbGender.Text,
                    (DateTime?)dtpBirthDate.Value,
                    string.IsNullOrWhiteSpace(txtNationality.Text) ? null : txtNationality.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtBirthplace.Text) ? null : txtBirthplace.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtFormerName.Text) ? null : txtFormerName.Text.Trim(),
                    numHeight.Value > 0 ? (decimal?)numHeight.Value : null,
                    numWeight.Value > 0 ? (decimal?)numWeight.Value : null,
                    string.IsNullOrWhiteSpace(cmbBloodType.Text) ? null : cmbBloodType.Text,
                    string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtLine.Text) ? null : txtLine.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtHighestEducation.Text) ? null : txtHighestEducation.Text.Trim(),
                    educationJson,
                    workJson,
                    string.IsNullOrWhiteSpace(txtFamilyStructure.Text) ? null : txtFamilyStructure.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtFather.Text) ? null : txtFather.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtMother.Text) ? null : txtMother.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtOtherFamily1.Text) ? null : txtOtherFamily1.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtOtherFamily2.Text) ? null : txtOtherFamily2.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtOtherFamily3.Text) ? null : txtOtherFamily3.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtSkills.Text) ? null : txtSkills.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtHobbies.Text) ? null : txtHobbies.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtIdeal.Text) ? null : txtIdeal.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtDislike.Text) ? null : txtDislike.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtTrauma.Text) ? null : txtTrauma.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtWitchMethod.Text) ? null : txtWitchMethod.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtRemarks.Text) ? null : txtRemarks.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtPublicDescription.Text) ? null : txtPublicDescription.Text.Trim(),
                    islandId,
                    batchId,
                    string.IsNullOrWhiteSpace(txtAvatarPath.Text) ? null : txtAvatarPath.Text.Trim(),
                    chkCaptureTime.Checked ? (DateTime?)dtpCaptureTime.Value : null,
                    chkDepartureTime.Checked ? (DateTime?)dtpDepartureTime.Value : null,
                    chkArrivalTime.Checked ? (DateTime?)dtpArrivalTime.Value : null,
                    chkDeathTime.Checked ? (DateTime?)dtpDeathTime.Value : null
                );
                
                MessageBox.Show($"魔女信息更新成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
