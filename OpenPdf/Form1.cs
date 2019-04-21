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

        public Form1()
        {
            InitializeComponent();

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

        private void ShowPdf(string filename)
        {
            string xml =
                $@"<toast activationType='protocol' launch='file:///{filename}'>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>""{filename}"" sent to printer</text>
                            <text>Click to open the file.</text>
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
            ShowPdf(@"c:/temp/test.pdf");
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterShortcut();
        }
    }
}
