using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessManager
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<int> dsProcess = new List<int>();

        protected System.Diagnostics.PerformanceCounter cpuCounter;
        static double GetTotalMemoryInBytes()
        {
            return new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;
        }
        private void btnGetAll_Click(object sender, EventArgs e)
        {
            DanhSachProcess();

        }
        private void DanhSachProcess()
        {
            dsProcess = new List<int>();
            lsbProcess.Items.Clear();
            Process[] p = Process.GetProcesses();
            double ram = GetTotalMemoryInBytes() / (1024 * 1024); //MB
            RAMText.Text = Math.Round(ram / 1024, 2).ToString();
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            System.Threading.Thread.Sleep(1000);

            //PerformanceCounter myAppCpu = new PerformanceCounter("Process", "% Processor Time", "_Total");
            //double total_cpu = Convert.ToInt32(myAppCpu.NextValue());

            int total_cpu = (int)cpuCounter.NextValue();
            CPUText.Text = total_cpu.ToString();
            foreach (Process ct in p)
            {
                try
                {
                    //CPU
                    double cpu = new PerformanceCounter("Process", "% Processor Time", ct.ProcessName).NextValue();

                    //RAM
                    double memsize = 0;
                    PerformanceCounter PC = new PerformanceCounter("Process", "Working Set - Private", ct.ProcessName, true);
                    memsize = Convert.ToInt32(PC.NextValue()) / (1024 * 1024);
                    PC.Close();
                    lsbProcess.Items.Add("Name: " + ct.ProcessName + "\t" + " id: " + ct.Id + "\t" + "CPU:" + cpu + "\t" + "RAM usage: " + memsize + " MB");
                    dsProcess.Add(ct.Id);
                    if (memsize > 800)
                    {
                        DialogResult dialogResult = MessageBox.Show("Are you sure to close " + '"' + ct.ProcessName + '"' + "?", "Notice", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            Process.GetProcessById(ct.Id).Kill();
                        }
                    
                        else if (dialogResult == DialogResult.No)
                        {
                            continue;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DanhSachProcess();
        }

        private void btnKillProcess_Click(object sender, EventArgs e)
        {
            if (lsbProcess.SelectedIndices.Count > 0)
            {
                int id = dsProcess[lsbProcess.SelectedIndex];
                Process.GetProcessById(id).Kill();
                DanhSachProcess();
            }
        }
    }
}
