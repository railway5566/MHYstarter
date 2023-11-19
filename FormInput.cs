using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiHoYoStarter
{
    public partial class FormInput : Form
    {
        private string gameNameEN;

        public FormInput(string gameNameEN)
        {
            InitializeComponent();
            this.gameNameEN = gameNameEN;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAcctName.Text))
            {
                MessageBox.Show("請輸入帳號備註", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                MiHoYoAccount acct = null;
                if (gameNameEN == "Genshin")
                {
                    acct = new GenshinAccount();
                }
                else if (gameNameEN == "GenshinOversea")
                {
                    acct = new GenshinOverseaAccount();
                }
                else if (gameNameEN == "GenshinCloud")
                {
                    acct = new GenshinCloudAccount();
                }
                else if (gameNameEN == "StarRail")
                {
                    acct = new StarRailAccount();
                }
                else if (gameNameEN == "StarRailOversea")
                {
                    acct = new StarRailOverseaAccount();
                }
                else if (gameNameEN == "HonkaiImpact3")
                {
                    acct = new HonkaiImpact3Account();
                }
                else
                {
                    MessageBox.Show("未知的遊戲帳號類型", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                acct.ReadFromRegistry();
                acct.Name = txtAcctName.Text;
                acct.WriteToDisk();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}