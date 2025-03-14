using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyPacketCapture
{
    public partial class frmSend : Form
    {

        public static int instatiations = 0;
        public frmSend()
        {
            InitializeComponent();
            instatiations++;

        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            //Check to see if sa filename was given
            if (saveFileDialog1.FileName != "")
            {
                File.WriteAllText(saveFileDialog1.FileName, txtPacket.Text);

            }
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            //Check to see if sa filename was given
            if (openFileDialog1.FileName != "")
            {
                txtPacket.Text = File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        private void frmSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instatiations--;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";
            //Get Hex Values from the file
            foreach(string s in txtPacket.Lines)
            {
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;
            }
            //extract the hex values into a string array
            string[] sBytes = stringBytes.Split(new string[] {"\n", "\r\n", " ", "\t"}, StringSplitOptions.RemoveEmptyEntries);

            //change the strings into bytes
            byte[] packet = new byte[sBytes.Length];
            int i = 0;
            foreach(string s in sBytes)
            {
                packet[i] = Convert.ToByte(s, 16);
                i++;
            }
            //sending out packet
            try
            {
                Form1.device.SendPacket(packet);
            }
            catch (Exception exp)
            {

            }
        }   //End btnSend
    }
}
