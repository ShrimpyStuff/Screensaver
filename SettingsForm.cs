using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Screensaver
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            LoadSettings();
        }

        private void SaveSettings()
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\ServantChild_ScreenSaver");
        
            key.SetValue("text", textBox.Text);
        }
 
        private void LoadSettings()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\ServantChild_ScreenSaver");
            if (key == null)
                textBox.Text = "Hello";
            else
                textBox.Text = (string)key.GetValue("text");
        }
        
        private void okButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void uninstallButton_Click(object sender, EventArgs e)
        {
            Uninstall();
        }

        private void Uninstall()
        {
            string fileLocation = ((string)Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop").GetValue("SCRNSAVE.EXE")).TrimEnd("SCREEN~1.SCR".ToCharArray());
            Registry.CurrentUser.DeleteSubKey("SOFTWARE\\ServantChild_ScreenSaver", false);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit", "LastKey", @"HKEY_CURRENT_USER\Control Panel\Desktop");
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.WorkingDirectory = "c:\\";
            process.StartInfo.FileName = "c:\\WINDOWS\\Regedit.exe";
            process.StartInfo.Verb = "runas";
            process.Start();
            File.WriteAllText(fileLocation + "DeleteKey.txt", "Delete value of key SCRNSAVE.EXE at opened Registry Editor/nThen you can delete the screensaver files.");
            Process notepad = Process.Start("notepad.exe", fileLocation + "DeleteKey.txt");
            notepad.WaitForExit();
            File.Delete(fileLocation + "DeleteKey.txt");
            Process.Start("explorer.exe", fileLocation);
            Close();
        }
    }
}