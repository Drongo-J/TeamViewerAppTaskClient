using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TeamViewerClient.Commands;
using TeamViewerClient.Services.NetworkService;

namespace TeamViewerClient.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }

        private int port = 27001;

        public int Port
        {
            get { return port; }
            set { port = value; OnPropertyChanged(); }
        }

        private string ipAddress;

        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; OnPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            IpAddress = NetworkHelper.GetLocalIpAddress();

            ConnectCommand = new RelayCommand((c) =>
            {
                try
                {
                    Thread thread = new Thread(() =>
                    {
                        App.Current.Dispatcher.BeginInvoke(() =>
                        {
                            NetworkHelper.Start(IpAddress, Port);
                        });
                    });
                    thread.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });

            DisconnectCommand = new RelayCommand((c) =>
            {
                NetworkHelper.WriteDataToServer(NetworkHelper.ExitCommand);
            });
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            NetworkHelper.WriteDataToServer(NetworkHelper.ExitCommand);
        }
    }
}
