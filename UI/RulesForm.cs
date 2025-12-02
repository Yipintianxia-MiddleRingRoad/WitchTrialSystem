using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·规定界面
    /// 功能：展示规定信息
    /// </summary>
    public class RulesForm : BasePokedexForm
    {
        /// <summary>
        /// 构造函数：初始化规定界面
        /// </summary>
        public RulesForm(string username) : base(username)
        {
            Text = "图鉴 · 规定";
            UpdateTitle();  // 添加用户信息到标题
        }

        protected override string GetBackgroundImageName()
        {
            return "rules_bg.png";  // 规定背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnRules.Enabled = false;
            _btnRules.Cursor = Cursors.Default;
        }
    }
}
