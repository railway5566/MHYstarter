using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiHoYoStarter
{
    public class GameFormControl
    {
        private string userDataPath;
        public string GameNameCN { get; set; }

        /// <summary>
        /// 遊戲簡稱
        /// </summary>
        public string GameNameShortCN { get; set; }

        public string GameNameEN { get; set; }

        public string ProcessName { get; set; }

        private FormMain formMain;

        private TextBox txtPath;
        private TextBox txtStartParam;
        private ListView lvwAcct;
        private Button btnChoosePath;
        private Button btnAdd;
        private Button btnSwitch;
        private Button btnDelete;
        private Button btnStart;

        private CheckBox chkAutoStart;

        public List<ToolStripMenuItem> AcctMenuItemList { get; set; }

        public GameFormControl(string nameCN, string nameShortCN, string nameEN, string processName)
        {
            GameNameCN = nameCN;
            GameNameShortCN = nameShortCN;
            GameNameEN = nameEN;
            ProcessName = processName;
            AcctMenuItemList = new List<ToolStripMenuItem>();
        }

        public void InitControl(FormMain formMain, TabPage tabPage, string pathSetting)
        {
            this.formMain = formMain;
            // 使用者資料路徑
            userDataPath = Path.Combine(Application.StartupPath, "UserData", GameNameEN);
            if (!Directory.Exists(userDataPath))
            {
                Directory.CreateDirectory(userDataPath);
            }

            // 初始化控制項
            FindControl(tabPage, $"txt{GameNameEN}Path", ref txtPath);
            FindControl(tabPage, $"txt{GameNameEN}StartParam", ref txtStartParam);
            FindControl(tabPage, $"lvw{GameNameEN}Acct", ref lvwAcct);
            FindControl(tabPage, $"btn{GameNameEN}ChoosePath", ref btnChoosePath);
            FindControl(tabPage, $"btn{GameNameEN}Add", ref btnAdd);
            FindControl(tabPage, $"btn{GameNameEN}Switch", ref btnSwitch);
            FindControl(tabPage, $"btn{GameNameEN}Delete", ref btnDelete);
            FindControl(tabPage, $"btn{GameNameEN}Start", ref btnStart);
            FindControl(tabPage, $"chk{GameNameEN}AutoStart", ref chkAutoStart);



            // 預設路徑
            if (string.IsNullOrEmpty(pathSetting))
            {
                string installPath = FindInstallPathFromRegistry(GameNameCN);
                if (installPath != null)
                {
                    string path = null;
                    switch (GameNameEN)
                    {
                        case "Genshin":
                            path = Path.Combine(installPath, "Genshin Impact Game", "YuanShen.exe"); // 只支持国服
                            break;
                        case "GenshinOversea":
                            path = Path.Combine(installPath, "Genshin Impact Game", "GenshinImpact.exe");
                            break;
                        case "GenshinCloud":
                            path = Path.Combine(installPath, "Genshin Impact Cloud Game.exe");
                            break;
                        case "StarRail":
                            path = Path.Combine(installPath, "Game", "StarRail.exe");
                            break;
                        case "StarRailOversea":
                            path = Path.Combine(installPath, "Game", "StarRail.exe");
                            break;
                        case "HonkaiImpact3":
                            path = Path.Combine(installPath, "Game", "BH3.exe");
                            break;
                    }

                    if (path != null && File.Exists(path))
                    {
                        txtPath.Text = path;
                    }
                }
            }
            else
            {
                txtPath.Text = pathSetting;
            }

            // 綁定事件
            btnChoosePath.Click += btnChoosePathClick;
            btnAdd.Click += btnAddClick;
            btnSwitch.Click += btnSwitchClick;
            btnDelete.Click += btnDeleteClick;
            if (btnStart != null)
            {
                btnStart.Click += btnhStartClick;
            }

            lvwAcct.MouseDoubleClick += lvwAcct_MouseDoubleClick;

            RefreshList();
        }

        private void FindControl<T>(TabPage tabPage, string name, ref T result) where T : Control
        {
            var controls = tabPage.Controls.Find(name, true);
            if (controls.Length > 0)
            {
                result = (T)controls[0];
            }
        }

        private void btnChoosePathClick(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false; //是否可以选择多个文件
            dialog.Title = "請選擇遊戲啟動程序（注意不是遊戲啟動器 launcher.exe！）";
            //选择某种类型的文件
            switch (GameNameEN)
            {
                case "Genshin":
                    dialog.Filter = "原神|YuanShen.exe|可执行文件(*.exe)|*.exe";
                    break;
                case "GenshinOversea":
                    dialog.Filter = "原神（国际服）|GenshinImpact.exe|可执行文件(*.exe)|*.exe";
                    break;
                case "GenshinCloud":
                    dialog.Filter = "云原神|Genshin Impact Cloud Game.exe|可执行文件(*.exe)|*.exe";
                    break;
                case "StarRail":
                    dialog.Filter = "崩坏：星穹铁道|StarRail.exe|可执行文件(*.exe)|*.exe";
                    break;
                case "StarRailOversea":
                    dialog.Filter = "崩坏：星穹铁道（国际服）|StarRail.exe|可执行文件(*.exe)|*.exe";
                    break;
                case "HonkaiImpact3":
                    dialog.Filter = "崩坏3|BH3.exe|可执行文件(*.exe)|*.exe";
                    break;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (dialog.FileName.EndsWith("launcher.exe"))
                {
                    string msg = $@"請選擇【{GameNameCN}】的遊戲本體執行檔（注意不是啟動器！！！）
以下是遊戲本體執行檔的路徑：
原神：\Genshin Impact\Genshin Impact Game\YuanShen.exe
原神（國際服飾）：\Genshin Impact\Genshin Impact Game\GenshinImpact.exe
雲原神：\Genshin Impact Cloud Game\Genshin Impact Cloud Game.exe
崩壞：星穹鐵道：\Star Rail\Game\StarRail.exe
崩潰3：\Honkai Impact 3rd\Game\BH3.exe";
                    MessageBox.Show(msg, "提示");
                    return;
                }

                txtPath.Text = dialog.FileName;
            }
        }

        private void btnAddClick(object sender, EventArgs e)
        {
            FormInput form = new FormInput(GameNameEN);
            form.ShowDialog();
            RefreshList();
        }

        private void btnSwitchClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show($"請先選擇【{GameNameCN}】遊戲啟動程式路徑，才能進行帳號切換", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!txtPath.Text.ToLower().EndsWith("exe"))
            {
                MessageBox.Show($"請先選擇正確遊戲啟動程式路徑（注意不是目錄，是exe執行檔）", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (lvwAcct.SelectedItems.Count == 0)
            {
                MessageBox.Show("請選擇要切換的帳號", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = lvwAcct.SelectedItems[0]?.Text;
            Switch(name);
        }

        private void btnDeleteClick(object sender, EventArgs e)
        {
            if (lvwAcct.SelectedItems.Count == 0)
            {
                MessageBox.Show("請選擇要切換的帳號", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = lvwAcct.SelectedItems[0]?.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("請選擇要切換的帳號", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MiHoYoAccount.DeleteFromDisk(userDataPath, name);
            RefreshList();
        }

        private void btnhStartClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPath.Text))
            {
                MessageBox.Show($"請先選擇【{GameNameCN}】遊戲啟動程式路徑，才能啟動遊戲", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!txtPath.Text.ToLower().EndsWith("exe"))
            {
                MessageBox.Show($"請先選擇正確遊戲啟動程式路徑（注意不是目錄，是exe執行檔）", "提示", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = txtPath.Text,
                    Verb = "runas"
                };
                if (txtStartParam != null && !string.IsNullOrEmpty(txtStartParam.Text))
                {
                    startInfo.Arguments = txtStartParam.Text;
                }

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("遊戲啟動失敗！\n" + ex.Message + "\n" + ex.StackTrace, "錯誤", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void lvwAcct_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = lvwAcct.HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                Switch(info.Item.Text);
            }
        }

        private void RefreshList()
        {
            lvwAcct.Items.Clear();
            AcctMenuItemList.Clear();
            DirectoryInfo root = new DirectoryInfo(userDataPath);
            FileInfo[] files = root.GetFiles();
            foreach (FileInfo file in files)
            {
                lvwAcct.Items.Add(new ListViewItem()
                {
                    Text = file.Name
                });
                var m = new ToolStripMenuItem()
                {
                    Name = file.Name,
                    Text = $"【{GameNameShortCN}】-【{file.Name}】",
                    Tag = GameNameEN, // 用tag來標識
                };
                m.Click += ToolStripMenuClick;
                AcctMenuItemList.Add(m);
            }

            if (lvwAcct.Items.Count > 0)
            {
                btnDelete.Enabled = true;
                btnSwitch.Enabled = true;
            }
            else
            {
                btnDelete.Enabled = false;
                btnSwitch.Enabled = false;
            }

            formMain.RefreshNotifyIconContextMenu(); // 呼叫主介面刷新選單
        }

        private void ToolStripMenuClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem toolStripMenuItem)
            {
                Switch(toolStripMenuItem.Name);
                foreach (var menuItem in AcctMenuItemList)
                {
                    menuItem.Checked = false;
                }

                toolStripMenuItem.Checked = true;
            }
        }

        private void Switch(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("請選擇要切換的帳號", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            var pros = Process.GetProcessesByName(ProcessName);


            MiHoYoAccount acct = MiHoYoAccount.CreateFromDisk(userDataPath, name);
            if (string.IsNullOrWhiteSpace(acct.AccountRegDataValue))
            {
                MessageBox.Show("帳戶內容為空", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            acct.WriteToRegistry();
            formMain.UpdateBottomLabel($"帳戶切換至【{name}】成功！");
            if (chkAutoStart.Checked)
            {
                if (pros.Any() && ProcessName != "StarRail")
                {
                    pros[0].Kill();
                    Thread.Sleep(200);
                }

                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.UseShellExecute = true;
                    startInfo.WorkingDirectory = Environment.CurrentDirectory;
                    startInfo.FileName = txtPath.Text;
                    startInfo.Verb = "runas";
                    if (txtStartParam != null && !string.IsNullOrEmpty(txtStartParam.Text))
                    {
                        startInfo.Arguments = txtStartParam.Text;
                    }

                    Process.Start(startInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("遊戲啟動失敗！\n" + ex.Message + "\n" + ex.StackTrace, "錯誤", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 從登錄檔中尋找安裝路徑
        /// </summary>
        /// <param name="uninstallKeyName">
        /// 安裝資訊的註冊表鍵名 "原神", "雲·原神", "崩壞：星穹鐵道","崩壞3"
        /// </param>
        /// <returns>安裝路徑</returns>
        public static string FindInstallPathFromRegistry(string uninstallKeyName)
        {
            try
            {
                using (var hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                using (var key = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" +
                                                 uninstallKeyName))
                {
                    if (key == null)
                    {
                        return null;
                    }

                    var installLocation = key.GetValue("InstallPath");
                    if (installLocation != null && !string.IsNullOrEmpty(installLocation.ToString()))
                    {
                        return installLocation.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}