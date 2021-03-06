﻿using System;
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
        PjacForm m_pjac;
        int m_jobId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void CreateRandomPrintJob(bool outOfPaper)
        {
            m_jobId++;

            if (m_pjac == null)
            {
                m_pjac = new PjacForm();
                m_pjac.FormClosed += M_pjac_FormClosed;
                m_pjac.Show();
            }

            string docName = GetRandomName(DocumentNames);
            string printerName = GetRandomName(PrinterNames);
            m_pjac.AddCard(new Card(docName, printerName, outOfPaper));

            AddStatus($@"Printing job {m_jobId} ""{printerName}""...");
        }

        private void M_pjac_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_pjac = null;
        }

        private void AddStatus(string line)
        {
            List<string> lines = new List<string>(textBox1.Lines);
            lines.Add(line);
            textBox1.Lines = lines.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateRandomPrintJob(checkBoxOutOfPaper.Checked);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
        }
        private void button3_Click(object sender, EventArgs e)
        {
        }

        private string GetRandomName(string[] names)
        {
            var rnd = new Random((int)(DateTime.Now.Ticks % int.MaxValue));
            string name = names[(rnd.Next() % names.Length)];
            return name;
        }

        readonly string[] DocumentNames = new string[]
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

        readonly string[] PrinterNames = new string[]
        {
            "Brother MFC-J5830DW",
            "Brother MFC-L2740DW series",
            "Brother MFC-L2750DW series",
            "Brother MFC-9340CDW",
            "Brother MFC-J985DW",
            "Brother MFC-L2700DW",
            "Brother MFC-L6800DW",
            "Brother MFC J5620DW",
            "Brother MFC L2740DW",
            "Brother MFC L2750DW",
            "Brother PJ-773",
            "Canon TS9000 series",
            "Canon ImageClass MF735 CDW",
            "Canon MB2720",
            "Canon MG7720",
            "Canon Pixma TR8520",
            "Canon TS9020",
            "Epson WF2750",
            "Epson WF3620",
            "Epson XP640",
            "HP OfficeJet Pro 8740",
            "HpEnvy5530",
            "HpEnvy7640",
            "HpEnvy7640series",
            "HPEnvyPhoto7855",
            "HPPageWidePro477DW",
            "HP EnvyPhoto7800",
            "HP Envy 5530",
            "HP LaserJet 700 MDP M775",
            "HP LaserJet M506",
            "HP Officejetpro8630",
            "HP PageWidePro477MFP",
            "Lexmark CX725",
            "Lexmark MX610de",
            "Lexmark CX725",
            "Lexmark MX610de",
            "PantumM6800FDW",
            "Samsung C3060FW",
            "Xerox VersaLink C405",
            "Xerox AltaLink B8055"
        };

    }
}
