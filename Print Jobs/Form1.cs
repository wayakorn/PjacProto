using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        readonly PjacForm m_pjac;

        public Form1()
        {
            UrlProtocol = AppName.Replace(' ', '-');

            InitializeComponent();

            RegisterProtocol();
            RegisterShortcut();


            m_pjac = new PjacForm();
            m_pjac.Show();
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

        private void RegisterProtocol()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Classes\\" + UrlProtocol);
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + UrlProtocol);
                key.SetValue(string.Empty, "URL: " + UrlProtocol);
                key.SetValue("URL Protocol", string.Empty);

                key = key.CreateSubKey(@"shell\open\command");
                key.SetValue(string.Empty, $@"{AppPath} ""%1""");
            }

            key.Close();
        }

        private void UnregisterProtocol()
        {
            Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\" + UrlProtocol, false);
        }

        int m_jobId = 0;
        readonly List<int> m_activeJobs = new List<int>();

        private void CreatePrintJob(string name)
        {
            m_jobId++;
            m_activeJobs.Add(m_jobId);

            string submitted = DateTime.Now.ToString();
            string xml =
                $@"<toast scenario='reminder' activationType='protocol' launch='{UrlProtocol}'>
                    <visual>
                        <binding template='ToastGeneric'>
                            <text>Printing ""{name}""...</text>
                            <text>Job {m_jobId}, submitted {submitted}</text>
                        </binding>
                    </visual>
                    <actions>
                        <action
                            content='Cancel job {m_jobId}'
                            activationType='foreground'
                            arguments='check'/>
                    </actions>
                </toast>";

            var toastXml = new XmlDocument();
            toastXml.LoadXml(xml);

            DateTimeOffset expirationTime = DateTime.Now.AddMinutes(1);

            var toast = new ToastNotification(toastXml)
            {
                ExpirationTime = expirationTime,
                Group = AppName,
                Tag = m_jobId.ToString()
            };

            toast.Activated += Toast_Activated;

            ToastNotificationManager.CreateToastNotifier(AppName).Show(toast);
            AddStatus($@"Printing job {m_jobId} ""{name}""...");
        }

        private void AddStatus(string line)
        {
            List<string> lines = new List<string>(textBox1.Lines);
            lines.Add(line);
            textBox1.Lines = lines.ToArray();
        }

        private void Toast_Activated(ToastNotification sender, object args)
        {
            Invoke(new Action(() =>
            {
                AddStatus(string.Format("Job {0} cancelled.", sender.Tag));
            }));
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            UnregisterProtocol();
            UnregisterShortcut();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreatePrintJob(GetRandomName());
            timer1.Enabled = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (var notif in ToastNotificationManager.History.GetHistory(AppName))
            {
                AddStatus($"Job {notif.Tag} completed (removed from Action Center).");
                ToastNotificationManager.History.Remove(notif.Tag, AppName, AppName);
            }
        }

        private string GetRandomName()
        {
            string[] DocumentNames = new string[]
            {
                "Back to the Future",
                "Desperado",
                "Night at the Museum",
                "Robocop",
                "Ghostbusters",
                "Cool World",
                "Donnie Darko",
                "Double Indemnity",
                "The Spanish Prisoner",
                "The Smurfs",
                "Dead Alive",
                "Army of Darkness",
                "Peter Pan",
                "The Jungle Story",
                "Red Planet",
                "Deep Impact",
                "The Long Kiss Goodnight",
                "Juno",
                "(500) Days of Summer",
                "The Dark Knight",
                "Bringing Down the House",
                "Se7en",
                "Chocolat",
                "The American",
                "The American President",
                "Hudsucker Proxy",
                "Conan the Barbarian",
                "Shrek",
                "The Fox and the Hound",
                "Lock, Stock, and Two Barrels",
                "Date Night",
                "200 Cigarettes",
                "9 1/2 Weeks",
                "Iron Man 2",
                "Tombstone",
                "Young Guns",
                "Fight Club",
                "The Cell",
                "The Unborn",
                "Black Christmas",
                "The Change-Up",
                "The Last of the Mohicans",
                "Shutter Island",
                "Ronin",
                "Ocean’s 11",
                "Philadelphia",
                "Chariots of Fire",
                "M*A*S*H",
                "Walking and Talking",
                "Walking Tall",
                "The 40 Year Old Virgin",
                "Superman III",
                "The Hour",
                "The Slums of Beverly Hills",
                "Secretary",
                "Secretariat",
                "Pretty Woman",
                "Sleepless in Seattle",
                "The Iron Mask",
                "Smoke",
                "Schindler’s List",
                "The Beverly Hillbillies",
                "The Ugly Truth",
                "Bounty Hunter",
                "Say Anything",
                "8 Seconds",
                "Metropolis",
                "Indiana Jones and the Temple of Doom",
                "Kramer vs. Kramer",
                "The Manchurian Candidate",
                "aging Bull",
                "Heat",
                "About Schmidt",
                "Re-Animator",
                "Evolution",
                "Gone in 60 Seconds",
                "Wanted",
                "The Man with One Red Shoe",
                "The Jerk",
                "Whip It",
                "Spanking the Monkey",
                "Steel Magnolias",
                "Horton Hears a Who",
                "Honey",
                "Brazil",
                "Gorillas in the Mist",
                "Before Sunset",
                "After Dark",
                "From Dusk til Dawn",
                "Cloudy with a Chance of Meatballs",
                "Harvey",
                "Mr. Smith Goes to Washington",
                "L.A. Confidential",
                "Little Miss Sunshine",
                "The Future",
                "Howard the Duck",
                "Howard’s End",
                "The Innkeeper",
                "Revolutionary Road"
            };

            var rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            string name = DocumentNames[(rnd.Next() % DocumentNames.Length)];
            return name;
        }
    }
}
