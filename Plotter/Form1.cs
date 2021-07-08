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
            chart1.ChartAreas[0].AxisX.ScaleView.Zoom(2, 100);
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            chart1.ChartAreas[0].AxisX.ScrollBar.ButtonStyle = ScrollBarButtonStyles.SmallScroll;
            chart1.ChartAreas[0].AxisX.ScaleView.SizeType = DateTimeIntervalType.Number;

            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            //chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;

            chart1.ChartAreas[0].CursorX.AutoScroll = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;

            chart1.Series.Clear();
            //chart1.Series.Add("X");
            //chart1.Series.Add("Y");
            //chart1.Series.Add("Z");
            //chart1.Series["X"].ChartType = SeriesChartType.Line;
            //chart1.Series["Y"].ChartType = SeriesChartType.Line;
            //chart1.Series["Z"].ChartType = SeriesChartType.Point;
        }

        ulong count = 0;

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
                if (!serialPort1.IsOpen)
                {
                    sendBtn.ForeColor = Color.Green;
                    serialPort1.PortName = comboPort.Text;
                    serialPort1.BaudRate = int.Parse(comboBaud.Text);
                    serialPort1.Open();
                    sendBtn.Image = Image.FromFile("stop24.png");
                }
                else
                {
                    sendBtn.ForeColor = Color.Red;
                    serialPort1.DiscardInBuffer();
                    serialPort1.DiscardOutBuffer();
                    serialPort1.Close();
                    serialPort1.Dispose();
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

        string serialDataIn;
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            { 
                serialDataIn = serialPort1.ReadLine();
                BeginInvoke(new EventHandler(ShowData));
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void ShowData(object sender, EventArgs e)
        {
            if (comboPort.Text.Length > 0)
            {
                string read = serialDataIn.TrimEnd();

                if (read.Split(',').Length > 0)
                {

                    for (int i = chart1.Series.Count; i < read.Split(',').Length; i++)
                    {
                        textBox1.AppendText(chart1.Series.Count.ToString() + Environment.NewLine);

                        chart1.Series.Add(i.ToString());
                        chart1.Series[i.ToString()].ChartType = SeriesChartType.Spline;
                    }
                    try
                    {
                        string[] readings = read.Split(',');
                        textBox1.Text = read + Environment.NewLine;
                        for (int i = 0; i < readings.Length; i++)
                        {
                            textBox1.AppendText(readings[i] + Environment.NewLine);
                            chart1.Series[i].Points.AddXY(count, float.Parse(readings[i]));
                        }
                        count++;

                        if (count >= 100 && checkScroll.Checked)
                        {
                            chart1.ChartAreas[0].AxisX.ScaleView.Position = count - 98;
                        }

                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
            }
        }
    }
}