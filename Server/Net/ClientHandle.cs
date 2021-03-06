﻿/*
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *	1. Redistributions of source code must retain the above copyright notice, this list of
 *	   conditions and the following disclaimer.
 * 
 *	2. Redistributions in binary form must reproduce the above copyright notice, this list
 *	   of conditions and the following disclaimer in the documentation and/or other materials
 *	   provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Tortoise.Server.XML;
using Tortoise.Shared.Net;
using Tortoise.Shared;

namespace Tortoise.Server.Connections
{
	/// <summary>
	/// Description of ClientHandle.
	/// </summary>
	public class ClientHandle
	{
		public static ClientHandle _instance;
		public static ClientHandle Instance
		{ get { return _instance; } }

		private List<Connection> _clients;

		
		private Thread _listenThread;
		private bool _threadRunning;
		private TcpListener _listiner;
		private TcpListener _secondaryListiner;
		private bool _secondaryListinerActive;
		
		private ClientHandle()
		{
			_clients = new List<Connection>();
			_threadRunning = true;
			
			_listenThread = new Thread(WorkThread);
			_listenThread.Start();
			_secondaryListinerActive = false;

			_instance = this;
		}

		public static void CreateInstance()
		{
			if (Instance != null)
				return;
			_instance = new ClientHandle();
		}


		private void WorkThread()
		{
			//If the Listen address is IPv6Any, then we possibly need to create a second listener for IPv4
			if (ServerConfig.Instance.ConvertedClientListenAddress == IPAddress.IPv6Any)
			{
				_secondaryListinerActive = true;
                _secondaryListiner = new TcpListener(IPAddress.Any, ServerConfig.Instance.ClientListenPort);
				_secondaryListiner.Start();
			}
			_listiner = new TcpListener(ServerConfig.Instance.ConvertedClientListenAddress, ServerConfig.Instance.ClientListenPort);
			_listiner.Start();

			_threadRunning = true;
			while (_threadRunning)
			{
				if (_listiner.Pending())
				{
                    AcceptConnection(_listiner.AcceptSocket());
				}

				if (_secondaryListinerActive && _secondaryListiner.Pending())
				{
                    AcceptConnection(_secondaryListiner.AcceptSocket());
				}

				for(int i = 0; i < _clients.Count; i++)
				{
                    _clients[i].Poll();
				}
			}

		}

        private void AcceptConnection(Socket client)
		{
			Connection Conn = new Connection(client);
			_clients.Add(Conn);
            StormLib.Console.WriteLine("Connection Received from '{0}'.", Conn.RemoteAddress.ToString());
			Conn.DisconnectedEvent += delegate(object sender, EventArgs e)
			{
                StormLib.Console.WriteLine("Client Disconnected.");
                //If its not a sender item, then it will be null
				//and a null should "NOT" be in the clients list
				if(_clients.Contains(sender as Connection))
					_clients.Remove(sender as Connection);
			};
		}

	}
}
