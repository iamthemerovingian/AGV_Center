﻿using CommonLibraries.Extensions;
using CommonLibraries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Socket_Client.Models
{
    public class AsynchonousClient
    {
        private const int SEND_REC_TIMEOUT = 1000;

        private const int port = 11000;

        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent recieveDone = new ManualResetEvent(false);

        private static string response = string.Empty;
        public static Socket client = null;

        public async Task<string> SendRecTCPCommand(string data, string serverorIPAddress, int port = 11000)
        {
            var result = await Task.Run(()=>
            {
                ConnectToServer(serverorIPAddress);

                Send(client, data);
                sendDone.WaitOne(SEND_REC_TIMEOUT);

                Recieve(client);
                recieveDone.WaitOne(SEND_REC_TIMEOUT);

                EndConnection();

                return true;
            }); 

            return response;
        }

        private static void ConnectToServer(string serverorIPAddress, int port = 11000)
        {
            try
            {
                IPHostEntry ipHostEntry;
                IPAddress ipAddress;

                bool success = false;
                success = IPAddress.TryParse(serverorIPAddress, out ipAddress);

                if(!success)
                {
                    ipHostEntry = Dns.GetHostEntry(serverorIPAddress);
                    ipAddress = ipHostEntry.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork).FirstOrDefault();
                }

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

                connectDone.Reset();
                sendDone.Reset();
                recieveDone.Reset();

                client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (Exception e)
            {
                e.WriteLog().SaveToDataBase().Display();
            }
        }

        private static void EndConnection()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private static void Recieve(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.workSocket = client;

                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(RecieveCallback), state);
            }
            catch (Exception e)
            {
                e.WriteLog().SaveToDataBase().Display();
            }
        }

        private static void RecieveCallback(IAsyncResult ar)
        {
            StateObject state = (StateObject)ar.AsyncState;
            Socket client = state.workSocket;

            int bytesRead = client.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, SocketFlags.None, new AsyncCallback(RecieveCallback), state);
            }
            else
            {
                if (state.sb.Length > 1)
                {
                    response = state.sb.ToString();
                }
                recieveDone.Set();
            }
        }

        private static void Send(Socket client, string data)
        {
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            client.BeginSend(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);
                sendDone.Set();
            }
            catch (Exception e)
            {
                e.WriteLog().SaveToDataBase().Display();
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndConnect(ar);
                connectDone.Set();
            }
            catch (Exception e)
            {
                e.WriteLog().SaveToDataBase().Display();
            }
        }
    }
}
