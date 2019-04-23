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

        private void PjacForm_Load(object sender, EventArgs e)
        {
            Card card = new Card("docName", "Canon MFP 2432", "Printing", DateTime.Now.ToString());
            card.TopLevel = false;
            Controls.Add(card);
            card.Show();
        }
    }
}
