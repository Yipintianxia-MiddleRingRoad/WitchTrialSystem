using System.Windows.Forms;

namespace WitchTrialSystem.UI
{
    /// <summary>
    /// 图鉴·记录界面
    /// 功能：展示记录信息
    /// </summary>
    public class RecordsForm : BasePokedexForm
    {
        /// <summary>
        /// 构造函数：初始化记录界面
        /// </summary>
        public RecordsForm(string username) : base(username)
        {
            Text = "图鉴 · 记录";
            UpdateTitle();  // 添加用户信息到标题
        }

        protected override string GetBackgroundImageName()
        {
            return "records_bg.png";  // 记录背景图
        }

        protected override void DisableCurrentPageButton()
        {
            _btnRecords.Enabled = false;
            _btnRecords.Cursor = Cursors.Default;
        }
    }
}
