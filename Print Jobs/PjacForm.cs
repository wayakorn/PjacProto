using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Print_Jobs
{
    public partial class PjacForm : Form
    {
        public PjacForm()
        {
            InitializeComponent();
        }

        public void AddCard(string docName, string printerName)
        {
            var card = new Card(docName, printerName);
            card.TopLevel = false;
            card.Show();
            flowLayoutPanel1.Controls.Add(card);
        }
    }
}
