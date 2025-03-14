using PacketDotNet;
using SharpPcap;
namespace MyPacketCapture
{
    public partial class Form1 : Form
    {

        CaptureDeviceList devices; // list of devices for this computer
        public static ICaptureDevice device; // the device we will be using
        public static string stringPackets = ""; // the data that is captured
        static int numPackets = 0;
        frmSend FSend;  //This will be our send form
        public Form1()
        {

            InitializeComponent();

            //get the list of devices
            devices = CaptureDeviceList.Instance;

            //make sure there is atleast one device
            if (devices.Count < 1)
            {
                MessageBox.Show("No Capture Devices Found!");
                Application.Exit();
            }

            //add devices to combo box
            foreach (ICaptureDevice dev in devices)
            {
                cmbDevices.Items.Add(dev.Description);
            }
            //get the second device and display in combo box
            device = devices[2];
            cmbDevices.Text = device.Description;

            //register out handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

        }
        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            //increment the number of packets captured
            numPackets++;

            //Put the packet number in the capture window
            stringPackets += "Packet Number: " + Convert.ToString(numPackets);
            stringPackets += Environment.NewLine;

            //array to store our data
            byte[] data = packet.Packet.Data;

            //keep track of the number of bytes displayed per line
            int byteCounter = 0;

            stringPackets += "Destination MAC Address: ";
            // Parsing the packets
            foreach (byte b in data)
            {
                //add the byte to our string (in hexidecimal)
                if (byteCounter <= 13)
                {
                    stringPackets += b.ToString("X2") + " ";
                    byteCounter++;
                }
                switch (byteCounter)
                {
                    case 6:
                        stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC Address: ";
                        break;
                    case 12:
                        stringPackets += Environment.NewLine;
                        stringPackets += "EtherType: ";
                        break;
                    case 14:
                        if (data[12] == 8)
                        {
                           if (data[13] == 0) stringPackets += "(IP)";
                           if (data[13] == 6) stringPackets += "(ARP)";
                        }
                        //stringPackets += Environment.NewLine;
                        break;

                }
            }

            stringPackets += Environment.NewLine + Environment.NewLine;
            byteCounter = 0;
            stringPackets += "Raw Data" + Environment.NewLine;
            // process each byte in our captured packet
            foreach (byte b in data)
            {
                //add the byte to our string (in hexidecimal)
                stringPackets += b.ToString("X2") + " ";
                byteCounter++;

                if (byteCounter == 16)
                {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                }
            }
            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text == "Start")
                {
                    device.StartCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    device.StopCapture();
                    timer1.Enabled = false;
                    btnStartStop.Text = "Start";
                }

            }
            catch (Exception ex)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtCapturedData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);

        }

        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            device = devices[cmbDevices.SelectedIndex];
            cmbDevices.Text = device.Description;
            txtGUID.Text = device.Name;

            //register out handler function to the 'packet arrival' event
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            //open the device for capturing
            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            //Check to see if sa filename was given
            if (openFileDialog1.FileName != "")
            {
                txtCapturedData.Text = File.ReadAllText(openFileDialog1.FileName);
            }
        }
        private void txtCapturedData_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            //Check to see if sa filename was given
            if (saveFileDialog1.FileName != "")
            {
                // File.WriteAllText(saveFileDialog1.FileName, txtCapturedData.Text);

            }
        }

        private void sendWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSend.instatiations == 0)
            { 
                FSend = new frmSend();  //creates a nbew frmSend
                FSend.Show();
                    
            }
        }

        private void txtGUID_TextChanged(object sender, EventArgs e)
        {

        }
    }
}