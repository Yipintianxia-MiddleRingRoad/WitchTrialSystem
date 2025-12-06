using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WitchTrialSystem.DAL;
using WitchTrialSystem.Models;
using Newtonsoft.Json;
using Microsoft.Data.SqlClient;

namespace WitchTrialSystem.UI
{
    public partial class WitchAddForm : Form
    {
        private readonly int? _witchId;  // 如果是编辑模式，存储魔女ID
        private readonly bool _isEditMode;  // 是否为编辑模式
        
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

        // 新增模式构造函数
        public WitchAddForm()
        {
            _witchId = null;
            _isEditMode = false;
            InitializeComponent();
            InitializeCustomComponents();
            LoadIslands();
        }

        // 编辑模式构造函数
        public WitchAddForm(int witchId)
        {
            _witchId = witchId;
            _isEditMode = true;
            InitializeComponent();
            InitializeCustomComponents();
            LoadIslands();
            LoadWitchData();  // 加载现有数据
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "编辑魔女信息 - 国家层" : "添加魔女 - 国家层";
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
            btnSave.Text = "保存";
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
            
            // 囚犯编号（必填）
            Label lblPrisonerNo = new Label { Text = "囚犯编号*:", Location = new Point(400, y), Width = labelWidth };
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
            dtpBirthDate.Value = new DateTime(2000, 1, 1);
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
            
            // 说明文字
            Label lblNote = new Label();
            lblNote.Text = "注意：\n" +
                          "1. 如果不选择岛屿和批次，魔女将被标记为\"待分配\"状态，不会创建用户账号。\n" +
                          "2. 头像图片请放在 Images\\{囚犯编号}.png\n" +
                          "3. 姓名图片请放在 Images\\characters\\{囚犯编号}.png";
            lblNote.Location = new Point(20, y);
            lblNote.Size = new Size(700, 60);
            lblNote.ForeColor = Color.DarkOrange;
            tab.Controls.Add(lblNote);
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

        private void CmbIsland_SelectedIndexChanged(object sender, EventArgs e)
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
                try
                {
                    // 添加空选项
                    cmbBatch.Items.Add("");
                    
                    // 根据岛屿加载批次（使用本地批次号）
                    var batches = GetLocalBatchesByIsland(islandId);
                    
                    // 调试信息
                    System.Diagnostics.Debug.WriteLine($"岛屿{islandId}的批次列表：{string.Join(", ", batches)}");
                    
                    foreach (var localBatchId in batches)
                    {
                        cmbBatch.Items.Add($"批次{localBatchId}");
                    }
                    
                    // 默认选择空选项
                    if (cmbBatch.Items.Count > 0)
                    {
                        cmbBatch.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载批次失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CmbBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblBatchInfo.Text = "";
            
            if (string.IsNullOrEmpty(cmbBatch.Text) || string.IsNullOrEmpty(cmbIsland.Text))
            {
                return;
            }
            
            // 检查批次容量
            int islandId = cmbIsland.SelectedIndex;
            string batchText = cmbBatch.Text.Replace("批次", "");
            if (int.TryParse(batchText, out int localBatchId))
            {
                // 获取全局批次ID
                int? globalBatchId = GetGlobalBatchId(islandId, localBatchId);
                if (globalBatchId.HasValue)
                {
                    var (currentCount, maxCapacity) = WitchDAL.GetBatchCapacity(islandId, globalBatchId.Value);
                    lblBatchInfo.Text = $"当前人数: {currentCount}/{maxCapacity}";
                    
                    if (currentCount >= maxCapacity)
                    {
                        lblBatchInfo.ForeColor = Color.Red;
                        lblBatchInfo.Text += " (已满，无法添加)";
                        btnSave.Enabled = false;
                    }
                    else
                    {
                        lblBatchInfo.ForeColor = Color.Blue;
                        btnSave.Enabled = true;
                    }
                }
            }
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
            
            // 验证囚犯编号（必填）
            if (string.IsNullOrWhiteSpace(txtPrisonerNo.Text))
            {
                errorProvider.SetError(txtPrisonerNo, "囚犯编号不能为空");
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
            
            // 验证岛屿-批次关系
            bool hasIsland = !string.IsNullOrEmpty(cmbIsland.Text) && cmbIsland.SelectedIndex > 0;
            bool hasBatch = !string.IsNullOrEmpty(cmbBatch.Text);
            
            if (hasIsland && !hasBatch)
            {
                errorProvider.SetError(cmbBatch, "选择了岛屿必须选择批次");
                isValid = false;
            }
            
            if (!hasIsland && hasBatch)
            {
                errorProvider.SetError(cmbIsland, "选择了批次必须选择岛屿");
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
                    
                    // 只有当批次不为空且不是空选项时才处理
                    if (!string.IsNullOrEmpty(cmbBatch.Text) && cmbBatch.Text.Trim() != "")
                    {
                        string batchText = cmbBatch.Text.Replace("批次", "").Trim();
                        if (int.TryParse(batchText, out int localBatchId))
                        {
                            // 将本地批次号转换为全局批次ID
                            System.Diagnostics.Debug.WriteLine($"转换：岛屿{islandId}, 本地批次{localBatchId}");
                            var globalBatchId = GetGlobalBatchId(islandId.Value, localBatchId);
                            System.Diagnostics.Debug.WriteLine($"结果：全局批次ID = {globalBatchId}");
                            
                            if (globalBatchId.HasValue)
                            {
                                batchId = globalBatchId.Value;
                            }
                            else
                            {
                                MessageBox.Show($"错误：找不到岛屿{islandId}的本地批次{localBatchId}对应的全局批次ID。\n\n请检查数据库中是否存在该批次记录。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                
                // 如果没有选择岛屿和批次，自动设置状态为"待分配"
                string status = cmbStatus.Text;
                if (!islandId.HasValue && !batchId.HasValue)
                {
                    status = "待分配";
                }
                
                // 根据模式调用不同的方法
                if (_isEditMode && _witchId.HasValue)
                {
                    // 编辑模式：更新现有魔女
                    WitchDAL.UpdateWitchComplete(
                        witchId: _witchId.Value,
                    name: txtName.Text.Trim(),
                    prisonerNo: string.IsNullOrWhiteSpace(txtPrisonerNo.Text) ? null : txtPrisonerNo.Text.Trim(),
                    personalNo: string.IsNullOrWhiteSpace(txtPersonalNo.Text) ? null : txtPersonalNo.Text.Trim(),
                    gender: string.IsNullOrWhiteSpace(cmbGender.Text) ? null : cmbGender.Text,
                    birthDate: dtpBirthDate.Value,
                    nationality: string.IsNullOrWhiteSpace(txtNationality.Text) ? null : txtNationality.Text.Trim(),
                    birthplace: string.IsNullOrWhiteSpace(txtBirthplace.Text) ? null : txtBirthplace.Text.Trim(),
                    formerName: string.IsNullOrWhiteSpace(txtFormerName.Text) ? null : txtFormerName.Text.Trim(),
                    height: numHeight.Value > 0 ? (decimal?)numHeight.Value : null,
                    weight: numWeight.Value > 0 ? (decimal?)numWeight.Value : null,
                    bloodType: string.IsNullOrWhiteSpace(cmbBloodType.Text) ? null : cmbBloodType.Text,
                    address: string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                    phone: string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                    email: string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                    lineAccount: string.IsNullOrWhiteSpace(txtLine.Text) ? null : txtLine.Text.Trim(),
                    highestEducation: string.IsNullOrWhiteSpace(txtHighestEducation.Text) ? null : txtHighestEducation.Text.Trim(),
                    educationHistory: educationJson,
                    workHistory: workJson,
                    familyStructure: string.IsNullOrWhiteSpace(txtFamilyStructure.Text) ? null : txtFamilyStructure.Text.Trim(),
                    father: string.IsNullOrWhiteSpace(txtFather.Text) ? null : txtFather.Text.Trim(),
                    mother: string.IsNullOrWhiteSpace(txtMother.Text) ? null : txtMother.Text.Trim(),
                    otherFamily1: string.IsNullOrWhiteSpace(txtOtherFamily1.Text) ? null : txtOtherFamily1.Text.Trim(),
                    otherFamily2: string.IsNullOrWhiteSpace(txtOtherFamily2.Text) ? null : txtOtherFamily2.Text.Trim(),
                    otherFamily3: string.IsNullOrWhiteSpace(txtOtherFamily3.Text) ? null : txtOtherFamily3.Text.Trim(),
                    skills: string.IsNullOrWhiteSpace(txtSkills.Text) ? null : txtSkills.Text.Trim(),
                    hobbies: string.IsNullOrWhiteSpace(txtHobbies.Text) ? null : txtHobbies.Text.Trim(),
                    ideal: string.IsNullOrWhiteSpace(txtIdeal.Text) ? null : txtIdeal.Text.Trim(),
                    dislike: string.IsNullOrWhiteSpace(txtDislike.Text) ? null : txtDislike.Text.Trim(),
                    trauma: string.IsNullOrWhiteSpace(txtTrauma.Text) ? null : txtTrauma.Text.Trim(),
                    magic: txtMagic.Text.Trim(),
                    status: status,
                    witchMethod: string.IsNullOrWhiteSpace(txtWitchMethod.Text) ? null : txtWitchMethod.Text.Trim(),
                    remarks: string.IsNullOrWhiteSpace(txtRemarks.Text) ? null : txtRemarks.Text.Trim(),
                    publicDescription: string.IsNullOrWhiteSpace(txtPublicDescription.Text) ? null : txtPublicDescription.Text.Trim(),
                    islandId: islandId,
                    batchId: batchId,
                    avatarPath: GenerateAvatarPath(txtPrisonerNo.Text),
                    captureTime: chkCaptureTime.Checked ? (DateTime?)dtpCaptureTime.Value : null,
                    departureTime: chkDepartureTime.Checked ? (DateTime?)dtpDepartureTime.Value : null,
                    arrivalTime: chkArrivalTime.Checked ? (DateTime?)dtpArrivalTime.Value : null,
                    deathTime: chkDeathTime.Checked ? (DateTime?)dtpDeathTime.Value : null
                    );
                    
                    MessageBox.Show("魔女信息更新成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // 新增模式：添加新魔女
                    int newWitchId = WitchDAL.AddWitchComplete(
                        name: txtName.Text.Trim(),
                        prisonerNo: string.IsNullOrWhiteSpace(txtPrisonerNo.Text) ? null : txtPrisonerNo.Text.Trim(),
                        personalNo: string.IsNullOrWhiteSpace(txtPersonalNo.Text) ? null : txtPersonalNo.Text.Trim(),
                        gender: string.IsNullOrWhiteSpace(cmbGender.Text) ? null : cmbGender.Text,
                        birthDate: dtpBirthDate.Value,
                        nationality: string.IsNullOrWhiteSpace(txtNationality.Text) ? null : txtNationality.Text.Trim(),
                        birthplace: string.IsNullOrWhiteSpace(txtBirthplace.Text) ? null : txtBirthplace.Text.Trim(),
                        formerName: string.IsNullOrWhiteSpace(txtFormerName.Text) ? null : txtFormerName.Text.Trim(),
                        height: numHeight.Value > 0 ? (decimal?)numHeight.Value : null,
                        weight: numWeight.Value > 0 ? (decimal?)numWeight.Value : null,
                        bloodType: string.IsNullOrWhiteSpace(cmbBloodType.Text) ? null : cmbBloodType.Text,
                        address: string.IsNullOrWhiteSpace(txtAddress.Text) ? null : txtAddress.Text.Trim(),
                        phone: string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim(),
                        email: string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        lineAccount: string.IsNullOrWhiteSpace(txtLine.Text) ? null : txtLine.Text.Trim(),
                        highestEducation: string.IsNullOrWhiteSpace(txtHighestEducation.Text) ? null : txtHighestEducation.Text.Trim(),
                        educationHistory: educationJson,
                        workHistory: workJson,
                        familyStructure: string.IsNullOrWhiteSpace(txtFamilyStructure.Text) ? null : txtFamilyStructure.Text.Trim(),
                        father: string.IsNullOrWhiteSpace(txtFather.Text) ? null : txtFather.Text.Trim(),
                        mother: string.IsNullOrWhiteSpace(txtMother.Text) ? null : txtMother.Text.Trim(),
                        otherFamily1: string.IsNullOrWhiteSpace(txtOtherFamily1.Text) ? null : txtOtherFamily1.Text.Trim(),
                        otherFamily2: string.IsNullOrWhiteSpace(txtOtherFamily2.Text) ? null : txtOtherFamily2.Text.Trim(),
                        otherFamily3: string.IsNullOrWhiteSpace(txtOtherFamily3.Text) ? null : txtOtherFamily3.Text.Trim(),
                        skills: string.IsNullOrWhiteSpace(txtSkills.Text) ? null : txtSkills.Text.Trim(),
                        hobbies: string.IsNullOrWhiteSpace(txtHobbies.Text) ? null : txtHobbies.Text.Trim(),
                        ideal: string.IsNullOrWhiteSpace(txtIdeal.Text) ? null : txtIdeal.Text.Trim(),
                        dislike: string.IsNullOrWhiteSpace(txtDislike.Text) ? null : txtDislike.Text.Trim(),
                        trauma: string.IsNullOrWhiteSpace(txtTrauma.Text) ? null : txtTrauma.Text.Trim(),
                        magic: txtMagic.Text.Trim(),
                        status: status,
                        witchMethod: string.IsNullOrWhiteSpace(txtWitchMethod.Text) ? null : txtWitchMethod.Text.Trim(),
                        remarks: string.IsNullOrWhiteSpace(txtRemarks.Text) ? null : txtRemarks.Text.Trim(),
                        publicDescription: string.IsNullOrWhiteSpace(txtPublicDescription.Text) ? null : txtPublicDescription.Text.Trim(),
                        islandId: islandId,
                        batchId: batchId,
                        avatarPath: GenerateAvatarPath(txtPrisonerNo.Text),
                        captureTime: chkCaptureTime.Checked ? (DateTime?)dtpCaptureTime.Value : null,
                        departureTime: chkDepartureTime.Checked ? (DateTime?)dtpDepartureTime.Value : null,
                        arrivalTime: chkArrivalTime.Checked ? (DateTime?)dtpArrivalTime.Value : null,
                        deathTime: chkDeathTime.Checked ? (DateTime?)dtpDeathTime.Value : null
                    );
                    
                    MessageBox.Show($"魔女添加成功！WitchID: {newWitchId}", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 根据囚犯编号生成头像路径
        /// </summary>
        private string? GenerateAvatarPath(string? prisonerNo)
        {
            if (string.IsNullOrWhiteSpace(prisonerNo))
                return null;
            
            return $"Images\\{prisonerNo.Trim()}.png";
        }

        /// <summary>
        /// 获取指定岛屿的本地批次列表
        /// </summary>
        private List<int> GetLocalBatchesByIsland(int islandId)
        {
            try
            {
                var dt = DBHelper.ExecDataTable(
                    "SELECT LocalBatchID FROM wt.Batch WHERE IslandID = @islandId ORDER BY LocalBatchID",
                    new SqlParameter("@islandId", islandId));
                
                var batches = new List<int>();
                foreach (DataRow row in dt.Rows)
                {
                    batches.Add(Convert.ToInt32(row["LocalBatchID"]));
                }
                return batches;
            }
            catch
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// 根据岛屿ID和本地批次号获取全局批次ID
        /// </summary>
        private int? GetGlobalBatchId(int islandId, int localBatchId)
        {
            try
            {
                var result = DBHelper.ExecScalar(
                    "SELECT BatchID FROM wt.Batch WHERE IslandID = @islandId AND LocalBatchID = @localBatchId",
                    new SqlParameter("@islandId", islandId),
                    new SqlParameter("@localBatchId", localBatchId));
                
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 根据岛屿ID和全局批次ID获取本地批次号
        /// </summary>
        private int? GetLocalBatchIdFromGlobal(int islandId, int globalBatchId)
        {
            try
            {
                var result = DBHelper.ExecScalar(
                    "SELECT LocalBatchID FROM wt.Batch WHERE IslandID = @islandId AND BatchID = @batchId",
                    new SqlParameter("@islandId", islandId),
                    new SqlParameter("@batchId", globalBatchId));
                
                return result != null ? Convert.ToInt32(result) : (int?)null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 加载现有魔女数据（编辑模式）
        /// </summary>
        private void LoadWitchData()
        {
            if (!_witchId.HasValue) return;

            try
            {
                var dal = new WitchDAL();
                var dt = dal.GetWitchDetail(_witchId.Value);
                
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("未找到魔女信息", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                var row = dt.Rows[0];

                // 基本信息
                txtName.Text = GetString(row, "Name");
                txtPrisonerNo.Text = GetString(row, "PrisonerNo");
                txtPersonalNo.Text = GetString(row, "PersonalNo");
                if (!string.IsNullOrEmpty(GetString(row, "Gender")))
                    cmbGender.Text = GetString(row, "Gender");
                if (row["BirthDate"] != DBNull.Value)
                    dtpBirthDate.Value = Convert.ToDateTime(row["BirthDate"]);
                txtNationality.Text = GetString(row, "Ethnicity");
                txtBirthplace.Text = GetString(row, "Birthplace");
                txtFormerName.Text = GetString(row, "FormerName");

                // 身体特征
                if (row["Height"] != DBNull.Value)
                    numHeight.Value = Convert.ToDecimal(row["Height"]);
                if (row["Weight"] != DBNull.Value)
                    numWeight.Value = Convert.ToDecimal(row["Weight"]);
                if (!string.IsNullOrEmpty(GetString(row, "BloodType")))
                    cmbBloodType.Text = GetString(row, "BloodType");

                // 联系方式
                txtAddress.Text = GetString(row, "Address");
                txtPhone.Text = GetString(row, "Phone");
                txtEmail.Text = GetString(row, "Email");
                txtLine.Text = GetString(row, "LineAccount");

                // 教育背景
                txtHighestEducation.Text = GetString(row, "HighestEducation");
                string eduJson = GetString(row, "EducationHistory");
                if (!string.IsNullOrEmpty(eduJson))
                {
                    try
                    {
                        educationRecords = JsonConvert.DeserializeObject<List<EducationRecord>>(eduJson) ?? new List<EducationRecord>();
                        RefreshEducationGrid();
                    }
                    catch { }
                }

                // 工作经历
                string workJson = GetString(row, "WorkHistory");
                if (!string.IsNullOrEmpty(workJson))
                {
                    try
                    {
                        workRecords = JsonConvert.DeserializeObject<List<WorkRecord>>(workJson) ?? new List<WorkRecord>();
                        RefreshWorkGrid();
                    }
                    catch { }
                }

                // 家庭关系
                txtFamilyStructure.Text = GetString(row, "FamilyStructure");
                txtFather.Text = GetString(row, "Father");
                txtMother.Text = GetString(row, "Mother");
                txtOtherFamily1.Text = GetString(row, "OtherFamily1");
                txtOtherFamily2.Text = GetString(row, "OtherFamily2");
                txtOtherFamily3.Text = GetString(row, "OtherFamily3");

                // 个性特征
                txtSkills.Text = GetString(row, "Skills");
                txtHobbies.Text = GetString(row, "Hobbies");
                txtIdeal.Text = GetString(row, "Dreams");
                txtDislike.Text = GetString(row, "Dislikes");
                txtTrauma.Text = GetString(row, "Trauma");

                // 魔女信息
                txtMagic.Text = GetString(row, "Magic");
                if (!string.IsNullOrEmpty(GetString(row, "Status")))
                    cmbStatus.Text = GetString(row, "Status");
                txtWitchMethod.Text = GetString(row, "WitchTransformMethod");
                txtRemarks.Text = GetString(row, "Remarks");
                txtPublicDescription.Text = GetString(row, "DescriptionPublic");

                // 分配信息
                if (row["IslandID"] != DBNull.Value)
                {
                    int islandId = Convert.ToInt32(row["IslandID"]);
                    cmbIsland.SelectedIndex = islandId;  // 1=岛屿1, 2=岛屿2
                    
                    // 加载批次后，设置选中的批次
                    if (row["BatchID"] != DBNull.Value)
                    {
                        int globalBatchId = Convert.ToInt32(row["BatchID"]);
                        // 获取本地批次号
                        var localBatchId = GetLocalBatchIdFromGlobal(islandId, globalBatchId);
                        if (localBatchId.HasValue)
                        {
                            cmbBatch.Text = $"批次{localBatchId.Value}";
                        }
                        else
                        {
                            // 如果找不到对应的本地批次，说明数据不一致，清空批次选择
                            cmbBatch.SelectedIndex = 0; // 选择空选项
                        }
                    }
                    else
                    {
                        // 批次为NULL，选择空选项
                        cmbBatch.SelectedIndex = 0;
                    }
                }
                else
                {
                    // 岛屿为NULL，清空选择
                    cmbIsland.SelectedIndex = 0;
                }
                txtAvatarPath.Text = GetString(row, "AvatarPath");

                // 时间戳
                if (row["CaptureTime"] != DBNull.Value)
                {
                    chkCaptureTime.Checked = true;
                    dtpCaptureTime.Value = Convert.ToDateTime(row["CaptureTime"]);
                }
                if (row["DepartureTime"] != DBNull.Value)
                {
                    chkDepartureTime.Checked = true;
                    dtpDepartureTime.Value = Convert.ToDateTime(row["DepartureTime"]);
                }
                if (row["ArrivalTime"] != DBNull.Value)
                {
                    chkArrivalTime.Checked = true;
                    dtpArrivalTime.Value = Convert.ToDateTime(row["ArrivalTime"]);
                }
                if (row["DeathTime"] != DBNull.Value)
                {
                    chkDeathTime.Checked = true;
                    dtpDeathTime.Value = Convert.ToDateTime(row["DeathTime"]);
                }

                // 更新窗口标题
                this.Text = $"编辑魔女信息 - {GetString(row, "Name")}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载数据失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        /// <summary>
        /// 安全获取字符串值
        /// </summary>
        private string GetString(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value)
                return string.Empty;
            return row[columnName]?.ToString() ?? string.Empty;
        }
    }
}
