using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Timers;
using System.Collections;

namespace TaskManagerProj
{
    public partial class Form1 : Form
    {
        private ListViewColumnSorter lvwColumnSorter;

        delegate void RefreshListView();

        Thread n1;

        private int totalMemoryUsage = 0;

        private bool onceFilled = false;

        public Form1()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = lvwColumnSorter;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("ProcessID", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Image Name", 120, HorizontalAlignment.Left);
            listView1.Columns.Add("Memory Usage", 100,HorizontalAlignment.Right);
            listView1.Columns.Add("CPU", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Start Time", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("Thread Count", 80, HorizontalAlignment.Center);
            listView1.Columns.Add("ThreadPriority", 80, HorizontalAlignment.Center);
            n1 = new Thread(new ThreadStart(IntervalRefresh));
            n1.Start();
        }

        private void RefreshProcesses()
        {
            clsProcess Proc1;

            // variable used to add-up all the processes
            int totalRunningProcesses = 0;

            // variable used to add-up all Thread count
            int totalThreadCount = 0;
            bool idExists = false;

            Process[] templist = Process.GetProcesses();

            int totalItems = listView1.Items.Count - 1;

            if (listView1.Items.Count > 0)
            {
                for (int x = 0; x < totalItems; x++)
                {
                    foreach (Process p0 in templist)
                    {
                        if (p0.Id.ToString().Equals(listView1.Items[x].Text))
                        {
                            idExists = true;
                            break;
                        }
                    }
                    if (idExists == false)
                    {
                        listView1.Items[x].Remove();
                    }
                    else
                    {
                        idExists = false;
                    }
                }
            }
            foreach (Process p1 in templist)
            {
                if (p1.ProcessName != "TaskManagerProj.vshost")
                {
                    try
                    {
                        //Create instance of Process class
                        Proc1 = new clsProcess();
                        Proc1.ProcessName = p1.ProcessName.ToString();
                        Proc1.ProcessID = p1.Id;
                        Proc1.MemoryUsage = p1.WorkingSet64.ToString();

                        // Added by Tenzin
                        Proc1.Thread_Priority = p1.BasePriority;
                        Proc1.StartTime = p1.StartTime.ToShortTimeString();
                        Proc1.CpuTime = (p1.TotalProcessorTime.Duration().Hours.ToString() + ":" +
                                          p1.TotalProcessorTime.Duration().Minutes.ToString() + ":" +
                                          p1.TotalProcessorTime.Duration().Seconds.ToString());
                        Proc1.ThreadCount = p1.Threads.Count.ToString();

                        //Add values to List Item
                        ListViewItem List = new ListViewItem(Proc1.ProcessID.ToString());

                        List.SubItems.Add(Proc1.ProcessName);
                        List.SubItems.Add(String.Format("{0:0,0}", Int64.Parse(Proc1.MemoryUsage.ToString()) / 1000) + "K");
                        List.SubItems.Add(Proc1.CpuTime); // add cputime
                        List.SubItems.Add(Proc1.StartTime); // add starttime
                        List.SubItems.Add(Proc1.ThreadCount); // add thread count
                        List.SubItems.Add(Proc1.Thread_Priority.ToString());

                        totalMemoryUsage += (int)(p1.WorkingSet64 / 1000);
                        totalRunningProcesses++; // add up all the processes
                        totalThreadCount += p1.Threads.Count; // add up all the threads from each process

                        if (onceFilled)
                        {
                            for (int x = 0; x < totalItems; x++)
                            {
                                if (p1.Id.ToString().Equals(listView1.Items[x].Text))
                                {
                                    listView1.Items[x].SubItems[1].Text = List.SubItems[1].Text;
                                    listView1.Items[x].SubItems[2].Text = List.SubItems[2].Text;
                                    listView1.Items[x].SubItems[3].Text = List.SubItems[3].Text;
                                    listView1.Items[x].SubItems[4].Text = List.SubItems[4].Text;
                                    listView1.Items[x].SubItems[5].Text = List.SubItems[5].Text;
                                    listView1.Items[x].SubItems[6].Text = List.SubItems[6].Text;
                                    idExists = true;
                                    break;
                                }
                            }
                            if (!idExists)
                            {
                                listView1.Items.Add(List);
                            }
                            else
                            {
                                idExists = false;
                            }
                        }
                        else
                        {
                            listView1.Items.Add(List);
                        }
                        
                    }
                    catch { }

                    //memUsePieBox.Invalidate();
                }
            }
            onceFilled = true;
            // Display the total number of Processes currently running.
            processLabel.Text = "Process: " + totalRunningProcesses.ToString();

            // Display the total number of threads
            threadLabel.Text = "Threads: " + totalThreadCount.ToString();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshProcesses();
        }

        private void SortColumnOrder(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            this.listView1.Sort();

        }

        private void IntervalRefresh()
        {
            System.Timers.Timer timer1 = new System.Timers.Timer(500);

            timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
            timer1.Interval = 1000;

            timer1.Enabled = false;
            timer1.Start();
        }

        void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            listView1.BeginInvoke(new RefreshListView(RefreshProcesses));
        }

        private void killButton_Click(object sender, EventArgs e)
        {
            Process[] templist = Process.GetProcesses();

            foreach (Process p1 in templist)
            {
                if (p1.ProcessName != "TaskManagerProj.vshost")
                {
                    //Create instance of Process class
                    if (p1.Id.ToString().Equals(listView1.SelectedItems[0].Text))
                    {
                        p1.Kill();
                        RefreshProcesses();
                        break;
                    }
                }
            }
        }

        private void KillPid(String myPid)
        {
            Process[] templist = Process.GetProcesses();

            foreach (Process p1 in templist)
            {
                if (p1.ProcessName != "TaskManagerProj.vshost")
                {
                    //Create instance of Process class
                    if (p1.Id.ToString().Equals(myPid))
                    {
                        p1.Kill();
                        RefreshProcesses();
                        break;
                    }
                }
            }
        }

        private void KillEnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KillPid(killProcessTextBox.Text);
                killProcessTextBox.Text = "";
            }
        }

        private void RunEnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Process.Start(runProcessTextBox.Text);
                runProcessTextBox.Text = "";
            }
        }


        /*private void memUsePieBox_Paint(object sender, PaintEventArgs e)
        {
            if (listView1.Items.Count > 0)
            {
                Pen p = new Pen(Color.Black, 2);
                Graphics g = e.Graphics;
                Rectangle rec = new Rectangle(0, 0, memUsePieBox.Width, memUsePieBox.Height);
                int startAngle = 0, red, green, blue, pctDegrees, memUsagePct;

                Color[] c = new Color[listView1.Items.Count];
                Brush[] b = new Brush[listView1.Items.Count];

                Random r = new Random();

                g.Clear(Form1.DefaultBackColor);

                for (int x = 0; x < listView1.Items.Count; x++)
                {
                    red = r.Next(255);
                    blue = r.Next(255);
                    green = r.Next(255);

                    c[x] = Color.FromArgb(red, blue, green);
                    b[x] = new SolidBrush(c[x]);

                    String memUsage1 = System.Text.RegularExpressions.Regex.Replace(listView1.Items[x].SubItems[2].Text,",","");
                    memUsage1 = System.Text.RegularExpressions.Regex.Replace(memUsage1,"K","");

                    memUsagePct = (int)((Convert.ToInt32(memUsage1)*1000) / totalMemoryUsage);
                    pctDegrees = (int)(360 * memUsagePct / 100);

                    g.DrawPie(p, rec, startAngle, pctDegrees);
                    g.FillPie(b[x], rec, startAngle, pctDegrees);

                    startAngle += pctDegrees;
                }
            }
        }*/
    }
}
