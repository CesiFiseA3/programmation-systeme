using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace PROGRAMMATION_SYST_ME.Model
{
    class SocketModel
    {
        private TcpListener tcpListener;
        private Thread listenerThread;
        private const int BufferSize = 1024; // Taille du buffer

        public SocketModel()
        {
            // Écoute des clients            
            this.tcpListener = new TcpListener(IPAddress.Any, 49152);
            this.tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            this.listenerThread = new Thread(new ThreadStart(ListenForClients));
            this.listenerThread.Start();
        }

        public void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                TcpClient client = this.tcpListener.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        public void HandleClientComm(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;

            // Traitez la connexion client ici

            // Fermer la connexion client à la fin du traitement.
            tcpClient.Close();
        }

        public void SendDataToClient(TcpClient tcpClient, string Name, int Progr, string ProgrStr, string Status)
        {
            try
            {
                string data = $"{Name},{Progr},{ProgrStr},{Status}";
                NetworkStream clientStream = tcpClient.GetStream();
                byte[] buffer = Encoding.ASCII.GetBytes(data);

                // Assurez-vous que la taille du buffer est constante
                if (buffer.Length <= BufferSize)
                {
                    clientStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    Console.WriteLine("La taille du buffer dépasse la limite spécifiée.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'envoi de données au client: " + ex.Message);
            }
        }
    }
}
