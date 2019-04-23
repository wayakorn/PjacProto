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
        readonly Random m_random;
        readonly int m_totalPages;
        int m_curAntsIndent = 0;

        public Card(string docName, string printerName)
        {
            m_random = new Random((int)DateTime.Now.Ticks);
            m_totalPages = (m_random.Next() % 8) + 1;

            InitializeComponent();

            textBoxDocumentName.Text = docName;
            textBoxPrinterName.Text = printerName;
            textBoxStatus.Text = "Queued";
            textBoxSubmitTime.Text = DateTime.Now.ToString();
        }

        private void timerAnts_Tick(object sender, EventArgs e)
        {
            m_curAntsIndent = m_curAntsIndent > 10 ? 5 : m_curAntsIndent + 1;

            string ants = ".....";
            textBoxAnts.Text = ants.PadLeft(m_curAntsIndent, ' ');

            char state = textBoxStatus.Text[0];

            switch (state)
            {
                case 'Q':  // Queued
                    if (m_random.Next() % 10 == 0)
                    {
                        textBoxStatus.Text = "Printing page 1";
                        textBoxAnts.Visible = true;
                    }
                    break;
                case 'P': // Printing page N
                    int currentPage = textBoxStatus.Text[14] - '0';
                    if (m_random.Next() % 8 == 0)
                    {
                        currentPage++;
                        if (currentPage < m_totalPages)
                        {
                            textBoxStatus.Text = $"Printing page {currentPage}";
                        }
                        else
                        {
                            textBoxStatus.Text = "Completed";
                        }
                    }
                    break;
                case 'C': // Completed
                    textBoxAnts.Visible = false;
                    timerAnts.Enabled = false;
                    break;
                default:
                    throw new Exception("unhandled state");
            }
        }

        private void buttonAbort_Click(object sender, EventArgs e)
        {
            timerAnts.Enabled = false;
            timerAnts.Enabled = false;
            textBoxStatus.Text = "Cancelled";
        }
    }
}
