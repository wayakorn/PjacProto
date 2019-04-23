using System;
using System.Runtime.InteropServices;
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

            var screen = Screen.GetWorkingArea(Screen.PrimaryScreen.Bounds);
            int left = screen.Width - Bounds.Width;
            int top = screen.Height - Bounds.Height;
            left = left < 0 ? 0 : left;
            top = top < 0 ? 0 : top;
            SetWindowPos(Handle, 0, left, top, Bounds.Width, Bounds.Height, 0);
        }

        public void AddCard(Card card)
        {
            card.TopLevel = false;
            card.Dock = DockStyle.Top;
            card.Show();
            Controls.Add(card);
        }
    }
}
