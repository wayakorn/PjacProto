using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

using Print_Jobs.ShellHelpers;
using Microsoft.Win32;

namespace Print_Jobs
{
    public partial class Form1 : Form
    {
        readonly string AppName = Assembly.GetExecutingAssembly().GetName().Name;
        readonly string AppPath = Assembly.GetExecutingAssembly().Location;
        readonly string UrlProtocol;

        public Form1()
        {
            UrlProtocol = AppName.Replace(' ', '-');

            InitializeComponent();

///            RegisterProtocol();
            RegisterShortcut();
        }

        private void RegisterShortcut()
        {
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\Microsoft\\Windows\\Start Menu\\Programs\\{AppName}.lnk";
            if (!File.Exists(shortcutPath))
            {
                IShellLinkW newShortcut = (IShellLinkW)new CShellLink();
                // Create a shortcut to the exe
                newShortcut.SetPath(AppPath);

                // Open the shortcut property store, set the AppUserModelId property
                IPropertyStore newShortcutProperties = (IPropertyStore)newShortcut;

                PropVariantHelper varAppId = new PropVariantHelper();

                varAppId.SetValue(AppName);
                newShortcutProperties.SetValue(PROPERTYKEY.AppUserModel_ID, varAppId.Propvariant);

                // Commit the shortcut to disk
                IPersistFile newShortcutSave = (IPersistFile)newShortcut;
                newShortcutSave.Save(shortcutPath, true);
            }
        }

        private void UnregisterShortcut()
        {
            string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\Microsoft\\Windows\\Start Menu\\Programs\\{AppName}.lnk";
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }
        }

/*
        private void RegisterProtocol()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + UrlProtocol);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + UrlProtocol);
                key.SetValue(string.Empty, "URL: " + UrlProtocol);
                key.SetValue("URL Protocol", string.Empty);

                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, $@"{AppPath} ""%1"""); // %1 represents the argument - this tells windows to open this program with an argument / parameter
            }

            key.Close();
        }

        private void UnregisterProtocol()
        {
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\" + UrlProtocol, false);
        }
*/

        private void ShowPrintJob(string name, string details, string filename)
        {
            string xml =
                $@"<toast activationType='protocol' launch='file:///{filename}'>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>""{name}"" sent to printer</text>
                            <text>{details}</text>
                        </binding>
                    </visual>
                </toast>";

            var toastXml = new XmlDocument();
            toastXml.LoadXml(xml);

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier(AppName).Show(toast);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
///            UnregisterProtocol();
            UnregisterShortcut();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowPrintJob("My Document", string.Format("Submitted at {0}", DateTime.Now), @"c:/temp/test.pdf");
        }
    }
}
