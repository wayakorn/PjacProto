using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Print_Jobs
{
    public partial class PjacForm : Form
    {
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        public PjacForm()
        {
            InitializeComponent();

            var screen = Screen.PrimaryScreen.Bounds;
            int left = screen.Width - Bounds.Width;
            int top = screen.Height - Bounds.Height;
            SetWindowPos(Handle, 0, left, top, Bounds.Width, Bounds.Height, 0);
        }

        public void AddCard(Card card)
        {
            card.TopLevel = false;
            card.Show();
            flowLayoutPanel1.Controls.Add(card);
        }
    }
}
