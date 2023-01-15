using Ironclad.Entities;
using Ironclad.Extensions;
using Ironclad.Helper;
using Ironclad.ViewModel;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Ironclad
{
    public partial class MainWindow : Window
    {
        public static String Config;
        public static String LogFile;
        public static String launchCFG;
        public static int FeatureCount;

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            refreshSettings();
            Properties.Settings.Default.Save();
            base.OnClosing(e);
        }

        public MainWindow()
        {
            while (!File.Exists(Properties.Settings.Default.GameExeFullpath))
            {
                MessageBox.Show("Ironclad needs to register your medieval2.exe - please select it. (Usually in your Steam game folder)");
                pickGameExeFile();
            }
            InitializeComponent();
            Config = Settings.P(@"ironclad\config.xlsx");
            LogFile = Settings.P(@"ironclad\ironclad.log");
            launchCFG = Settings.P(@"m2twex.cfg");
            FeatureCount = Properties.Settings.Default.FeatureCount;
            this.DataContext = new MainWindowViewModel();
            File.WriteAllText(LogFile, "");
            IO.Log("Ironclad launched");
            World.LoadObjects(Config);
            World.ValidateObjects();
            lblVersion.Content = lblVersion.Content.ToString().Replace("v.v", Settings.VERSION);
            cbLogAll.IsChecked = Properties.Settings.Default.cbLogAll;
            cbWindowed.IsChecked = Properties.Settings.Default.cbWindowed;
            cbMuted.IsChecked = Properties.Settings.Default.cbMuted;
            cbLaunch.IsChecked = Properties.Settings.Default.cbLaunch;
            cbValidateUnitModels.IsChecked = Properties.Settings.Default.cbValidateUnitModels;
            cbValidateOtherFiles.IsChecked = Properties.Settings.Default.cbValidateOtherFiles;
            cbShowStatsFactions.IsChecked = Properties.Settings.Default.cbShowStatsFactions;
            cbShowStatsRegions.IsChecked = Properties.Settings.Default.cbShowStatsRegions;
            cbShowStatsResources.IsChecked = Properties.Settings.Default.cbShowStatsResources;
            cbShowStatsScripts.IsChecked = Properties.Settings.Default.cbShowStatsScripts;
            cbShowStatsNamesets.IsChecked = Properties.Settings.Default.cbShowStatsNamesets;
            cbRoadTest.IsChecked = Properties.Settings.Default.cbRoadTest;
            ddResolution.SelectedIndex = Properties.Settings.Default.ResolutionIndex;
            Properties.Settings.Default.Save();
            IO.Log($"Installation path is correct: {Settings.PG()}");
        }

        private void btnFeaturesClick(object sender, RoutedEventArgs e)
        {
            refreshSettings();
            FeaturesWindow FeaturesWindow = new FeaturesWindow();
            FeaturesWindow.Show();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            refreshSettings();
            CreateCfg();
            Rndm.RandomizeLoadingScreens();
            Rndm.RandomizeMusic();
            GenerateMenuTxt();
            foreach (string file in Directory.GetFiles(Settings.P(@"data\sounds"), "*.idx"))
                File.Delete(file);
            foreach (string file in Directory.GetFiles(Settings.P(@"data\sounds"), "*.dat"))
                File.Delete(file);
            RenewAndRunYouneuoycfg("@mods\\retrofit\\m2twex.cfg");
            // To run without Hardcode-Unlocks: var proc = Process.Start(Settings.PG("medieval2.exe"), cfg);
        }

        private void refreshSettings()
        {
            Properties.Settings.Default.cbLogAll = cbLogAll.IsChecked == true;
            Properties.Settings.Default.cbLaunch = cbLaunch.IsChecked == true;
            Properties.Settings.Default.cbShowStatsFactions = cbShowStatsFactions.IsChecked == true;
            Properties.Settings.Default.cbShowStatsRegions = cbShowStatsRegions.IsChecked == true;
            Properties.Settings.Default.cbShowStatsResources = cbShowStatsResources.IsChecked == true;
            Properties.Settings.Default.cbShowStatsScripts = cbShowStatsScripts.IsChecked == true;
            Properties.Settings.Default.cbShowStatsNamesets = cbShowStatsNamesets.IsChecked == true;
            Properties.Settings.Default.cbRoadTest = cbRoadTest.IsChecked == true;
            Properties.Settings.Default.cbWindowed = cbWindowed.IsChecked == true;
            Properties.Settings.Default.cbMuted = cbMuted.IsChecked == true;
            Properties.Settings.Default.ResolutionIndex = ddResolution.SelectedIndex;
            Properties.Settings.Default.cbValidateOtherFiles = cbValidateOtherFiles.IsChecked == true;
            Properties.Settings.Default.cbValidateUnitModels = cbValidateUnitModels.IsChecked == true;
        }

        private void CreateCfg()
        {
            var res = ddResolution.Text.Replace("x", " ");
            var loglevel = cbLogAll.IsChecked == true ? "warning" : "error";
            var windowed = cbWindowed.IsChecked == true ? "true" : "false";
            var sound = cbMuted.IsChecked == true ? "false" : "true";
            File.WriteAllText(launchCFG, "[controls]\ncampaign_scroll_min_zoom = 0\ncampaign_scroll_max_zoom = 100\n\n[log]\nto = mods/retrofit/logs/system.log.txt\n");
            File.AppendAllText(launchCFG, $"level = * {loglevel}\n\n[io]\nfile_first = true\ndisable_file_cache = false\n\n[features]\nmod = mods/retrofit\n\n[ui]\nfull_battle_HUD = 0");
            File.AppendAllText(launchCFG, $"\n\n[video]\nwindowed = {windowed}\ncampaign_resolution = {res}\nbattle_resolution = {res}\nunit_detail = highest\ngrass_distance = 900");
            File.AppendAllText(launchCFG, $"\n\n[audio]\nenable = {sound}\n\n[game]\nfirst_time_play = false\ndisable_events = false\nauto_save = false\ncampaign_map_speed_up = true");
            File.AppendAllText(launchCFG, $"\nlabel_characters = false\nlabel_settlements = true\nmicromanage_all_settlements = true\nunit_size = huge\n");
            File.AppendAllText(launchCFG, $"unlimited_men_on_battlefield = true\ntutorial_battle_played = true\n");
        }

        private void GenerateMenuTxt()
        {
            File.Delete(Hardcoded.MENUTXT);
            var txt = File.ReadAllText($"{Hardcoded.MENUTXT}".Replace("menu", "pre_menu")).Replace("v.v", Settings.VERSION);
            FO.WriteAllTextUCS2LEBOM(Hardcoded.MENUTXT, txt.ToString().Replace("\n", "\r\n"));
        }

        private void RenewAndRunYouneuoycfg(string cfg)
        {
            File.Delete(Hardcoded.LIMITS);
            cfg = cfg.Rem("@mods\\retrofit\\");
            File.WriteAllText(Hardcoded.LIMITS, $"Siege_Ram_Cost = {Tuner.CostSiegeRam}\nSiege_Ladder_Cost = {Tuner.CostSiegeLadder}\nSiege_Tower_Cost = {Tuner.CostTower}\nStarting_mod=1\nMod_CFG_Name={cfg}");
            Process p = new Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = @"/c HotSeatTool.exe";
            p.StartInfo.WorkingDirectory = Path.GetDirectoryName(Settings.P("HotSeatTool.exe"));
            p.StartInfo.UseShellExecute = false;
            p.Start();
            System.Threading.Thread.Sleep(5000);
            p.Kill();
            p.StartInfo.Arguments = @"/c taskkill /IM HotSeatTool.exe >nul";
            p.Start();
        }

        private void IroncladLogTextChanged(object sender, TextChangedEventArgs e)
        {
            tbIroncladLog.ScrollToEnd();
        }

        private void btnRegenerate_Click(object sender, RoutedEventArgs e)
        {
            refreshSettings();
            btnRegenerate.IsEnabled = false;
            Generator.GenerateGame();
            btnRegenerate.IsEnabled = true;
            if (cbLaunch.IsChecked == true)
                btnLaunch_Click(sender, e);
        }

        private void btnCheckAndValidate_Click(object sender, RoutedEventArgs e)
        {
            refreshSettings();
            Helper.Validator.ValidateWorld();
        }

        private void pickGameExeFile()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (Properties.Settings.Default.GameExeFullpath != "")
                dlg.InitialDirectory = Path.GetDirectoryName(Properties.Settings.Default.GameExeFullpath);
            dlg.FileName = "Medieval II Executable";
            dlg.DefaultExt = ".exe";
            dlg.Filter = "medieval2.exe (medieval2.exe)|medieval2.exe";
            Nullable<bool> result = dlg.ShowDialog();
            string file = dlg.FileName;
            if (result == true && file.EndsWith("medieval2.exe"))
            {
                Properties.Settings.Default.GameExeFullpath = file;
                MessageBox.Show("Medieval II installation has been linked to Ironclad. Do not forget to apply 4-GB-Patch to your .exe file, otherwise you will experience crashes! (Google 4-GB-exe Patch)");
            }
            else
            {
                MessageBox.Show("Medieval II installation not set up/changed.");
            }
        }

        private void btnShowStats_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.cbShowStatsResources)
                Statistics.ShowResources();
        }

        private void btnChangeSetGamePath_Click(object sender, RoutedEventArgs e)
        {
            pickGameExeFile();
        }
    }
}
