using System;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Plotter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // get available com ports to the combobox
            sendBtn.Image = Image.FromFile("play24.png");
            comboPort.Items.Clear();
            string[] names = SerialPort.GetPortNames();
            foreach (string x in names)
                comboPort.Items.Add(x);
            if (names.Length > 0)
            {
                comboPort.SelectedIndex = 0;
                comboBaud.Enabled = true;
            }
           
            // select 9600 as default
            comboBaud.SelectedIndex = 5;

            // setting the chart 
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(2, 200);
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = false;
            chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;

            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;

            chart1.Series.Clear();
            chart1.Series.Add("X");
            //chart1.Series.Add("Y");
            //chart1.Series.Add("Z");
            chart1.Series["X"].ChartType = SeriesChartType.Point;
            //chart1.Series["Y"].ChartType = SeriesChartType.Point;
            //chart1.Series["Z"].ChartType = SeriesChartType.Point;
        }

        ulong count = 0;

        // timer function that gets activated every time interval (20 ms)
        private void timer1_Tick(object sender, EventArgs e)
        {

            if (comboPort.Text.Length > 0)
            {
                if (!serialPort1.IsOpen)
                    serialPort1.Open();
                string read = serialPort1.ReadLine();
                read = serialPort1.ReadLine().TrimEnd();
                if (read.Split(',').Length > 0)
                {
                    try
                    {
                        float y0 = float.Parse(read.Split(',')[0]);
                        
                        chart1.Series[0].Points.AddXY(count, y0);
                        count++;

                        if (count >= 200)
                        {
                            chart1.ChartAreas[0].AxisX.ScaleView.Position = count-198;
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            comboPort.Items.Clear();
            string[] names = SerialPort.GetPortNames();
            foreach (string x in names)
                comboPort.Items.Add(x);
            if (names.Length > 0)
            {
                comboPort.SelectedIndex = 0;
                comboBaud.Enabled = true;
            }
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (comboPort.Text.Length > 0)
            {
                if (timer1.Enabled == false)
                {
                    timer1.Enabled = true;
                    sendBtn.ForeColor = Color.Green;
                    serialPort1.PortName = comboPort.Text;
                    serialPort1.BaudRate = int.Parse(comboBaud.Text);
                    serialPort1.Open();
                    sendBtn.Image = Image.FromFile("stop24.png");
                }
                else
                {
                    timer1.Enabled = false;
                    sendBtn.ForeColor = Color.Red;
                    serialPort1.Close();
                    sendBtn.Image = Image.FromFile("play24.png");
                }
            }
            else
            {
                MessageBox.Show("No Port Selected, Please Select COM Port First", "Warinig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBaud.Enabled = true;
        }
    }
}