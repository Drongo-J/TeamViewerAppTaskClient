using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Net.NetworkInformation;
using TeamViewerClient.Helpers;

namespace TeamViewerClient.Services.NetworkService
{
    internal class NetworkHelper
    {
        public static string ExitCommand = "exit";
        public static void WriteDataToServer(string text)
        {
            Thread thread = new Thread(() =>
            {
                MessageBox.Show("Exited");
                try
                {
                    var stream = client.GetStream();
                    var bw = new BinaryWriter(stream);
                    bw.Write(text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            thread.Start();
        }
        public static TcpClient client = new TcpClient();
        public static IPAddress IP;

        public static void Start(string ipString, int port)
        {
            client = new TcpClient();
            IP = IPAddress.Parse(ipString);
            var ep = new IPEndPoint(IP, port);

            try
            {
                client.Connect(ep);
                if (client.Connected)
                {
                    MessageBox.Show("Connected");
                    var writer = Task.Run(() =>
                    {
                        ImageHelper im = new ImageHelper();
                        im.CreateFolder();

                        while (true)
                        {
                            try
                            {
                                Task.Delay(10);

                                ImageHelper helper = new ImageHelper();
                                var path = helper.TakeScreenShot();
                                var bytes = helper.GetBytesOfImage(path);

                                var stream = client.GetStream();
                                stream.Write(bytes, 0, bytes.Length);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    });

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static string GetLocalIpAddress()
        {
            UnicastIPAddressInformation mostSuitableIp = null;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;
                var properties = network.GetIPProperties();
                if (properties.GatewayAddresses.Count == 0)
                    continue;
                foreach (var address in properties.UnicastAddresses)
                {
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;
                    if (IPAddress.IsLoopback(address.Address))
                        continue;
                    if (!address.IsDnsEligible)
                    {
                        if (mostSuitableIp == null)
                            mostSuitableIp = address;
                        continue;
                    }
                    // The best IP is the IP got from DHCP server
                    if (address.PrefixOrigin != PrefixOrigin.Dhcp)
                    {
                        if (mostSuitableIp == null || !mostSuitableIp.IsDnsEligible)
                            mostSuitableIp = address;
                        continue;
                    }
                    return address.Address.ToString();
                }
            }
            return mostSuitableIp != null
                ? mostSuitableIp.Address.ToString()
                : "";
        }
    }
}

