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
        private ListViewItem selectedItem;

        delegate void RefreshListView();

        Thread n1;

        public Form1()
        {
            InitializeComponent();
            lvwColumnSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = lvwColumnSorter;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("ProcessID", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Image Name", 150, HorizontalAlignment.Left);
            listView1.Columns.Add("Memory Usage", 120,HorizontalAlignment.Right);
            listView1.Columns.Add("ThreadPriority", 100, HorizontalAlignment.Center);
            n1 = new Thread(new ThreadStart(IntervalRefresh));
            n1.Start();
        }

        private void RefreshProcesses()
        {
            clsProcess Proc1;

            Process[] templist = Process.GetProcesses();

            //Create items in list view
            listView1.Items.Clear();

            foreach (Process p1 in templist)
            {
                if (p1.ProcessName != "TaskManagerProj.vshost")
                {
                   //Create instance of Process class
                    Proc1 = new clsProcess();
                    Proc1.ProcessName = p1.ProcessName.ToString();
                    Proc1.ProcessID = p1.Id;
                    Proc1.MemoryUsage = p1.WorkingSet64.ToString();

                    // Added by Tenzin
                    Proc1.Thread_Priority = p1.BasePriority;

                    //Add values to List Item
                    ListViewItem List = new ListViewItem(Proc1.ProcessID.ToString());
                    
                    List.SubItems.Add(Proc1.ProcessName);
                    List.SubItems.Add(String.Format("{0:0,0}",Int64.Parse(Proc1.MemoryUsage.ToString())/1000)+"K");
                    List.SubItems.Add(Proc1.Thread_Priority.ToString());

                    listView1.Items.Add(List);
                }
            }

            //listView1.Refresh();

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
            System.Timers.Timer timer1 = new System.Timers.Timer(1000);

            timer1.Elapsed += new ElapsedEventHandler(timer1_Elapsed);
            timer1.Interval = 4000;

            timer1.Enabled = false;
            timer1.Start();
        }

        void timer1_Elapsed(object sender, ElapsedEventArgs e)
        {
            //listView1.BeginInvoke(new RefreshListView(RefreshProcesses));
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
                        //MessageBox.Show(p1.Id.ToString());
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
                        //MessageBox.Show(p1.Id.ToString());
                        break;
                    }
                }
            }
        }

        private void EnterKeyPress(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                KillPid(killProcessTextBox.Text);
            }
        }
    }
}
