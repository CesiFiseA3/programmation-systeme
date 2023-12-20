using PROGRAMMATION_SYST_ME.Ressources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PROGRAMMATION_SYST_ME.Model;
using System.Net.Sockets;

namespace PROGRAMMATION_SYST_ME.View
{
    /// <summary>
    /// Interaction logic for Save.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        private readonly MainWindow mhandle;
        
        private readonly List<int> jobs;

        

        public bool End { set; get; } = false;
        public TcpClient tcpClient { get; private set; }

        public SaveWindow(MainWindow handleMain, List<int> jobsToExec)
        {
            // create a socket to listen for clients
            SocketModel socket = new SocketModel();

            tcpClient = new TcpClient("127.0.0.1", 49152);


            jobs = jobsToExec;
            mhandle = handleMain;
            InitializeComponent();
            ProgressListView.Items.Clear();
            while (!mhandle.userInteract.IsSetup) { Thread.Sleep(100); }
            int i = 0;
            foreach (var job in jobs)
            {
                int pro = (int)(mhandle.userInteract.RealTimeData[i].Progression * 100);
                ProgressListView.Items.Add(new Item
                {
                    Name = mhandle.userInteract.BackupJobsData[job].Name,
                    Progr = pro,
                    ProgrStr = pro.ToString() + " %",
                    Status = mhandle.userInteract.RealTimeData[i].State
                });
                i++;
            }
            Thread backThread = new Thread(() =>
            {
                while (!End)
                {
                    
                    int y = 0;
                    bool canEnd = true;
                    foreach (var job in jobs)
                    {
                        int pro = (int)mhandle.userInteract.RealTimeData[y].Progression;
                        if (pro == 100 && canEnd)
                            End = true;
                        else
                        {
                            End = false;
                            canEnd = false;
                        }
                        
                        this.Dispatcher.Invoke(() => ProgressListView.Items[y] = new Item
                        {
                            Name = mhandle.userInteract.BackupJobsData[job].Name,
                            Progr = pro,
                            ProgrStr = pro.ToString() + " %",
                            Status = mhandle.userInteract.RealTimeData[y].State
                        });
                        // envoie Name,Progr, ProgrStr, Status au client via socket 
                        socket.SendDataToClient(tcpClient, mhandle.userInteract.BackupJobsData[job].Name, pro, pro.ToString() + " %", mhandle.userInteract.RealTimeData[y].State);
                        y++;
                    }
                    Thread.Sleep(100);
                }
            })
            {

            };
            backThread.Start();

            Show();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!End)
            {
                e.Cancel = true;
                return;
            }

            mhandle.SaveWin = null;
        }
    }
    public class Item()
    {
        public string Name { get; set; }
        public int Progr { get; set; }
        public string ProgrStr { get; set; }
        public string Status { get; set; }
    }
}
