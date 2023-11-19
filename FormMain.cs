﻿using Microsoft.Win32;
using MiHoYoStarter.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace MiHoYoStarter
{
    public partial class FormMain : Form
    {
        private GameFormControl genshinFormControl = new GameFormControl("原神", "原神", "Genshin", "YuanShen");

        private GameFormControl genshinCloudFormControl =
            new GameFormControl("雲·原神", "雲原神", "GenshinCloud", "Genshin Impact Cloud Game");

        private GameFormControl starRailFormControl = new GameFormControl("崩壞：星穹鐵道", "崩鐵", "StarRail", "StarRail");
        private GameFormControl honkaiImpact3FormControl = new GameFormControl("崩壞3", "崩壞3", "HonkaiImpact3", "BH3");

        private GameFormControl genshinOverseaFormControl =
            new GameFormControl("原神（國際服）", "原神（國際服）", "GenshinOversea", "GenshinImpact");

        private GameFormControl starRailOverseaFormControl =
            new GameFormControl("崩壞：星穹鐵道（國際服）", "崩鐵（國際服）", "StarRailOversea", "StarRail");

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            // 標題加上版本號
            var version = HuiUtils.GetMyVersion();
            this.Text += version;
            GAHelper.Instance.RequestPageView($"MiHoYoStarter_{version}", $"進入{version}版本MHY帳號切換工具主介面");

            // 初始化介面控制
            genshinFormControl.InitControl(this, tabPageGenshin, Properties.Settings.Default.GenshinPath);
            genshinOverseaFormControl.InitControl(this, tabPageGenshinOversea,
                Properties.Settings.Default.GenshinOverseaPath);
            genshinCloudFormControl.InitControl(this, tabPageGenshinCloud,
                Properties.Settings.Default.GenshinCloudPath);
            starRailFormControl.InitControl(this, tabPageSatrRail, Properties.Settings.Default.StarRailPath);
            starRailOverseaFormControl.InitControl(this, tabPageSatrRailOversea,
                Properties.Settings.Default.StarRailOverseaPath);
            honkaiImpact3FormControl.InitControl(this, tabPageHonkaiImpact3,
                Properties.Settings.Default.HonkaiImpact3Path);

            // 預設配置初始化
            DisplayGenshinTabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayGenshinEnabled;
            DisplayGenshinOverseaTabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayGenshinOverseaEnabled;
            DisplayGenshinCloudTabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayGenshinCloudEnabled;
            DisplayStarRailTabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayStarRailEnabled;
            DisplayStarRailOverseaTabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayStarRailOverseaEnabled;
            DisplayHonkaiImpact3TabToolStripMenuItem.Checked = Properties.Settings.Default.DisplayHonkaiImpact3Enabled;

            txtGenshinStartParam.Text = Properties.Settings.Default.GenshinStartParam;
            txtGenshinOverseaStartParam.Text = Properties.Settings.Default.GenshinStartParam;
            txtStarRailStartParam.Text = Properties.Settings.Default.StarRailStartParam;
            txtStarRailOverseaStartParam.Text = Properties.Settings.Default.StarRailStartParam;
            txtHonkaiImpact3StartParam.Text = Properties.Settings.Default.HonkaiImpact3StartParam;

            chkGenshinAutoStart.Checked = Properties.Settings.Default.GenshinAutoStartEnabled;
            chkGenshinOverseaAutoStart.Checked = Properties.Settings.Default.GenshinAutoStartEnabled;
            chkGenshinCloudAutoStart.Checked = Properties.Settings.Default.GenshinCloudAutoStartEnabled;
            chkStarRailAutoStart.Checked = Properties.Settings.Default.StarRailAutoStartEnabled;
            chkStarRailOverseaAutoStart.Checked = Properties.Settings.Default.StarRailAutoStartEnabled;
            chkHonkaiImpact3AutoStart.Checked = Properties.Settings.Default.HonkaiImpact3AutoStartEnabled;

            RefreshTab();
        }

        public void RefreshNotifyIconContextMenu()
        {
            this.contextMenuStrip1.Items.Clear();
            if (DisplayGenshinTabToolStripMenuItem.Checked && genshinFormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(genshinFormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }

            if (DisplayGenshinOverseaTabToolStripMenuItem.Checked &&
                genshinOverseaFormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(genshinOverseaFormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }

            if (DisplayGenshinCloudTabToolStripMenuItem.Checked && genshinCloudFormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(genshinCloudFormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
            }

            if (DisplayStarRailTabToolStripMenuItem.Checked && starRailFormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(starRailFormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                ;
            }

            if (DisplayStarRailOverseaTabToolStripMenuItem.Checked &&
                starRailOverseaFormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(starRailOverseaFormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                ;
            }

            if (DisplayHonkaiImpact3TabToolStripMenuItem.Checked && honkaiImpact3FormControl.AcctMenuItemList.Count > 0)
            {
                this.contextMenuStrip1.Items.AddRange(honkaiImpact3FormControl.AcctMenuItemList.ToArray());
                this.contextMenuStrip1.Items.Add(new ToolStripSeparator());
                ;
            }

            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[]
            {
                this.显示主界面ToolStripMenuItem,
                this.退出ToolStripMenuItem
            });
        }

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized && notifyIcon.Visible)
            {
                this.ShowInTaskbar = false;
                this.Visible = false;
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Visible = true;
            }

            this.Activate();
        }

        private void btnStarRailFPSEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\miHoYo\崩坏：星穹铁道",
                    "GraphicsSettings_Model_h2986158309", null);
                if (value != null)
                {
                    var json = Encoding.UTF8.GetString((byte[])value);
                    // 一般长这样：{"FPS":60,"EnableVSync":true,"RenderScale":1.4,"ResolutionQuality":5,"ShadowQuality":5,"LightQuality":5,"CharacterQuality":5,"EnvDetailQuality":5,"ReflectionQuality":5,"BloomQuality":5,"AAMode":1}
                    // JavaScriptSerializer 沒法反序列化成通用對象，我也很絕望呀
                    Regex r = new Regex("\"FPS\":\\d*,");
                    if (r.IsMatch(json))
                    {
                        string newJson = r.Replace(json, $"\"FPS\":{numericUpDownFPS.Value},");
                        Registry.SetValue(@"HKEY_CURRENT_USER\Software\miHoYo\崩坏：星穹铁道",
                            "GraphicsSettings_Model_h2986158309", Encoding.UTF8.GetBytes(newJson));
                        MessageBox.Show("套用成功！", "提示");
                    }
                    else
                    {
                        MessageBox.Show("沒有找到FPS相關配置，大機率是程式有問題啦，聯絡作者解決~", "提示");
                    }
                }
                else
                {
                    MessageBox.Show("取得註冊表內容失敗，請在遊戲內重新修改圖形設定後重試", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("套用時發生異常\n" + ex.Message + "\n" + ex.StackTrace, "提示");
            }
        }

        private void btnStarRailOverseaFPSEdit_Click(object sender, EventArgs e)
        {
            try
            {
                var value = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Cognosphere\Star Rail",
                    "GraphicsSettings_Model_h2986158309", null);
                if (value != null)
                {
                    var json = Encoding.UTF8.GetString((byte[])value);
                    // 一般長這樣：{"FPS":60,"EnableVSync":true,"RenderScale":1.4,"ResolutionQuality":5,"ShadowQuality":5,"LightQuality":5,"CharacterQuality":5,"EnvDetailQuality":5,"ReflectionQuality":5,"BloomQuality":5,"AAMode":1}
                    // JavaScriptSerializer 沒法反序列化成通用對象，我也很絕望呀
                    Regex r = new Regex("\"FPS\":\\d*,");
                    if (r.IsMatch(json))
                    {
                        string newJson = r.Replace(json, $"\"FPS\":{numericUpDownFPS.Value},");
                        Registry.SetValue(@"HKEY_CURRENT_USER\Software\Cognosphere\Star Rail",
                            "GraphicsSettings_Model_h2986158309", Encoding.UTF8.GetBytes(newJson));
                        MessageBox.Show("套用成功！", "提示");
                    }
                    else
                    {
                        MessageBox.Show("沒有找到FPS相關配置，大機率是程式有問題啦，聯絡作者解決~", "提示");
                    }
                }
                else
                {
                    MessageBox.Show("取得註冊表內容失敗，請在遊戲內重新修改圖形設定後重試", "提示");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("套用時發生異常\n" + ex.Message + "\n" + ex.StackTrace, "提示");
            }
        }

        private void DisplayGenshinTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayGenshinTabToolStripMenuItem.Checked = !DisplayGenshinTabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        private void DisplayGenshinCloudTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayGenshinCloudTabToolStripMenuItem.Checked = !DisplayGenshinCloudTabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        private void DisplayStarRailTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayStarRailTabToolStripMenuItem.Checked = !DisplayStarRailTabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        private void DisplayHonkaiImpact3TabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayHonkaiImpact3TabToolStripMenuItem.Checked = !DisplayHonkaiImpact3TabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        private void DisplayGenshinOverseaTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayGenshinOverseaTabToolStripMenuItem.Checked = !DisplayGenshinOverseaTabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        private void DisplayStarRailOverseaTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayStarRailOverseaTabToolStripMenuItem.Checked = !DisplayStarRailOverseaTabToolStripMenuItem.Checked;
            RefreshTab();
            RefreshNotifyIconContextMenu();
        }

        public void RefreshTab()
        {
            if (DisplayGenshinTabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageGenshin))
                {
                    tab1.TabPages.Add(tabPageGenshin);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageGenshin))
                {
                    tab1.TabPages.Remove(tabPageGenshin);
                }
            }

            if (DisplayGenshinOverseaTabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageGenshinOversea))
                {
                    tab1.TabPages.Add(tabPageGenshinOversea);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageGenshinOversea))
                {
                    tab1.TabPages.Remove(tabPageGenshinOversea);
                }
            }

            if (DisplayGenshinCloudTabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageGenshinCloud))
                {
                    tab1.TabPages.Add(tabPageGenshinCloud);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageGenshinCloud))
                {
                    tab1.TabPages.Remove(tabPageGenshinCloud);
                }
            }

            if (DisplayStarRailTabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageSatrRail))
                {
                    tab1.TabPages.Add(tabPageSatrRail);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageSatrRail))
                {
                    tab1.TabPages.Remove(tabPageSatrRail);
                }
            }

            if (DisplayStarRailOverseaTabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageSatrRailOversea))
                {
                    tab1.TabPages.Add(tabPageSatrRailOversea);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageSatrRailOversea))
                {
                    tab1.TabPages.Remove(tabPageSatrRailOversea);
                }
            }

            if (DisplayHonkaiImpact3TabToolStripMenuItem.Checked)
            {
                if (!tab1.TabPages.Contains(tabPageHonkaiImpact3))
                {
                    tab1.TabPages.Add(tabPageHonkaiImpact3);
                }
            }
            else
            {
                if (tab1.TabPages.Contains(tabPageHonkaiImpact3))
                {
                    tab1.TabPages.Remove(tabPageHonkaiImpact3);
                }
            }
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon_DoubleClick(sender, e);
        }

        private void 主页ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/babalae/mihoyo-starter");
        }

        private void 请作者喝咖啡ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com");
        }

        private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.DisplayGenshinEnabled = DisplayGenshinTabToolStripMenuItem.Checked;
            Properties.Settings.Default.DisplayGenshinOverseaEnabled = DisplayGenshinOverseaTabToolStripMenuItem.Checked;
            Properties.Settings.Default.DisplayGenshinCloudEnabled = DisplayGenshinCloudTabToolStripMenuItem.Checked;
            Properties.Settings.Default.DisplayStarRailEnabled = DisplayStarRailTabToolStripMenuItem.Checked;
            Properties.Settings.Default.DisplayStarRailOverseaEnabled = DisplayStarRailOverseaTabToolStripMenuItem.Checked;
            Properties.Settings.Default.DisplayHonkaiImpact3Enabled = DisplayHonkaiImpact3TabToolStripMenuItem.Checked;

            Properties.Settings.Default.GenshinPath = txtGenshinPath.Text;
            Properties.Settings.Default.GenshinOverseaPath = txtGenshinOverseaPath.Text;
            Properties.Settings.Default.GenshinCloudPath = txtGenshinCloudPath.Text;
            Properties.Settings.Default.StarRailPath = txtStarRailPath.Text;
            Properties.Settings.Default.StarRailOverseaPath = txtStarRailOverseaPath.Text;
            Properties.Settings.Default.HonkaiImpact3Path = txtHonkaiImpact3Path.Text;

            Properties.Settings.Default.GenshinStartParam = txtGenshinStartParam.Text;
            Properties.Settings.Default.GenshinOverseaStartParam = txtGenshinOverseaStartParam.Text;
            Properties.Settings.Default.StarRailStartParam = txtStarRailStartParam.Text;
            Properties.Settings.Default.StarRailOverseaStartParam = txtStarRailOverseaStartParam.Text;
            Properties.Settings.Default.HonkaiImpact3StartParam = txtHonkaiImpact3StartParam.Text;


            Properties.Settings.Default.GenshinAutoStartEnabled = chkGenshinAutoStart.Checked;
            Properties.Settings.Default.GenshinOverseaAutoStartEnabled = chkGenshinOverseaAutoStart.Checked;
            Properties.Settings.Default.GenshinCloudAutoStartEnabled = chkGenshinCloudAutoStart.Checked;
            Properties.Settings.Default.StarRailAutoStartEnabled = chkStarRailAutoStart.Checked;
            Properties.Settings.Default.StarRailOverseaAutoStartEnabled = chkStarRailOverseaAutoStart.Checked;
            Properties.Settings.Default.HonkaiImpact3AutoStartEnabled = chkHonkaiImpact3AutoStart.Checked;

            Properties.Settings.Default.Save();
        }

        public void UpdateBottomLabel(string info)
        {
            this.toolStripStatusLabel1.Text = info;
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void chkGenshinOverseaAutoStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void chkGenshinAutoStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnGenshinDelete_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void btnGenshinCloudSwitch_Click(object sender, EventArgs e)
        {

        }

        private void chkGenshinCloudAutoStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDownFPS_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnStarRailDelete_Click(object sender, EventArgs e)
        {

        }

        private void btnStarRailAdd_Click(object sender, EventArgs e)
        {

        }

        private void chkHonkaiImpact3AutoStart_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {

        }
    }
}