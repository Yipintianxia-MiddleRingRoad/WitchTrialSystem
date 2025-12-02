using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·证物界面
    /// 功能：展示证物信息
    /// </summary>
    public class EvidenceForm : BasePokedexForm
    {
        /// <summary>
        /// 构造函数：初始化证物界面
        /// </summary>
        public EvidenceForm(string username) : base(username)
        {
            Text = "图鉴 · 证物";
            UpdateTitle();  // 添加用户信息到标题
        }

        protected override string GetBackgroundImageName()
        {
            return "evidence_bg.png";  // 证物背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnEvidence.Enabled = false;
            _btnEvidence.Cursor = Cursors.Default;
        }
    }
}
