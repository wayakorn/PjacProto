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
    public partial class Card : Form
    {
        public Card(string docName, string printerName, string status, string submitTime)
        {
            InitializeComponent();

            labelDocumentName.Text = docName;
            labelPrinterName.Text = printerName;
            labelStatus.Text = status;
            labelSubmitTime.Text = submitTime;
        }
    }
}
