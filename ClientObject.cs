using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace GigaGameServer
{
    public class ClientObject
    {
        protected internal string Id { get; set; }
        protected internal NetworkStream Stream { get; set; }
        public string username;
        TcpClient client;
        ServerObject server;

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = GetMessage();
                username = message;
                server.nikClients.Add(username);

                Console.WriteLine(message + " присоеденился на сервер.");
                // в бесконечном цикле получаем и отправляем сообщения клиента
                while (true)
                {
                    try
                    {        
                        message = GetMessage();
                        if (message == "1001")
                        {
                            string buffString = "";
                            foreach (var item in server.nikClients)
                            {
                                buffString += item.ToString();
                                buffString += '\n';
                            }
                            server.BroadcastMessage(buffString, "228");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(username + " - Разорвал соединение");
                        foreach (var item in server.nikClients)
                        {
                            if (item == username)
                            {
                                server.nikClients.Remove(item);
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[1024]; 
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}