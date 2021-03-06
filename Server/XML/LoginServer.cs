﻿/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 5/2/2010
 * Time: 2:24 AM
 * 
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
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using System.Security;
using System.Diagnostics;

using Tortoise.Server.Exceptions;

namespace Tortoise.Server.XML
{
    /// <summary>
    /// configuration Database for Login Server
    /// </summary>
    [Serializable]
    [XmlRoot("Server")]
    public class ServerConfig
    {

        private static ServerConfig _instance;
        public static ServerConfig Instance
        {
            get { return _instance; }
        }
        private const string DefaultConfigPath = "Tortoise.Server.XML.DefaultConfig.xml";


        /// <summary>
        /// This is the name that the server uses when sharing info about its self.
        /// </summary>
        public string ServerName;


        /// <summary>
        /// This is the port the server listen on for clients.
        /// </summary>
        public int ClientListenPort;

        /// <summary>
        /// this is the address the server listens on for clients.
        /// </summary>
        public string ClientListenAddress;

        [System.Xml.Serialization.XmlIgnore]
        public IPAddress ConvertedClientListenAddress;



        /// <summary>
        /// This is the port the server listen on for servers.
        /// </summary>
        public int ServerListenPort;

        /// <summary>
        /// this is the address the server listens on for clients.
        /// </summary>
        public string ServerListenAddress;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public IPAddress ConvertedServerListenAddress;


        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool AcceptAnyAddress;


        public string[] ServerListenAcceptedAddresses;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public IPAddress[] ConvertedAcceptedServerAddresses;



        /// <summary>
        /// This is the Port for the Mysql Database.
        /// </summary>
        public int MysqlPort;

        /// <summary>
        /// This is the address for the Mysql Database.
        /// </summary>
        public string MysqlAddress;

        /// <summary>
        /// This is the Account Databse for the Mysql Database.
        /// </summary>
        public string MysqlAccountDatabse;

        /// <summary>
        /// This is the Server Databse for the Mysql Database.
        /// </summary>	
        public string MysqlServerDatabse;

        /// <summary>
        /// This is the User for the Mysql Database.
        /// </summary>
        public string MysqlUser;

        /// <summary>
        /// This is the Password for the Mysql Database.
        /// </summary>
        public string MysqlPass;

        /// <summary>
        /// This is the number of threads that the server will use to handle Clients.
        /// </summary>
        public int ClientListenThreads;

        /// <summary>
        /// This is the number of Clients each thread will handle.
        /// </summary>
        public int MaxUsersPerThread;

        ///<summary>This is the Clients Major version number.</summary>
        public int ClientMajor;
        ///<summary> This is the Clients Minor version number. </summary>
        public int CLientMinor;
        ///<summary> This is the Clients Build version number. </summary>
        public int CLientBuild;
        ///<summary> This is the Clients Revision version number. </summary>
        public int ClientRevision;



        /// <summary>
        /// This creates a default Config and saves it.
        /// </summary>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseMissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseFileException">The configuration cannot be saved.</exception>
        public static void CreateDefault()
        {

            Assembly Selfassembly;
            StreamReader SR;
            String DefaultFile;

            Selfassembly = Assembly.GetExecutingAssembly();


            try
            {
                SR = new StreamReader(Selfassembly.GetManifestResourceStream(DefaultConfigPath));
            }
            catch (ArgumentNullException)
            {
                throw new TortoiseMissingResourceException("The resource could not be loaded.", DefaultConfigPath);
            }

            DefaultFile = SR.ReadToEnd();


            try
            {
                File.WriteAllText("./LoginConfig.xml", DefaultFile);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new TortoiseFileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
            catch (SecurityException ex)
            {
                throw new TortoiseFileException(string.Format("The application does not have permission to save the file ./LoginConfig.xml"), "./LoginConfig.xml", ex);
            }
        }

        /// <summary>
        /// This Loads the configuration file into the static field Server.XML.Server.Instance.
        /// </summary>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseMissingResourceException">The embedded resource cannot be loaded.</exception>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseFileException">The configuration cannot be loaded or initially created.</exception>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseFormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.innerException">innerException</see>  property. </exception>
        public static void LoadConfig()
        {
            LoadConfig(true);
        }
        /// <summary>
        /// This Loads the configuration file into the static feild Server.XML.Server.Instance.
        /// </summary>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseMissingResourceException">The embeded resource cannot be loaded.</exception>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseFileException">The configuration cannot be loaded or initally created.</exception>
        /// <exception cref="Tortoise.Server.Exceptions.TortoiseFormatException">An IP or Hostname is invalid.</exception>
        /// <exception cref="System.InvalidOperationException"> An error occurred during deserialization. The original exception is available using the <see cref="System.innerException">innerException</see>  property. </exception>
        public static void LoadConfig(bool ignoreErrors)
		{
			if (!File.Exists("./LoginConfig.xml"))
			{
				CreateDefault();
			}

			TextReader reader = new StreamReader("./LoginConfig.xml");

			XmlSerializer serializer = new XmlSerializer(typeof(ServerConfig));
			ServerConfig._instance = (ServerConfig)serializer.Deserialize(reader);
			reader.Close();

			string[] AcceptedAddresses = ServerConfig.Instance.ServerListenAcceptedAddresses;
			int AddressLen = AcceptedAddresses.Length;

			ServerConfig.Instance.ConvertedAcceptedServerAddresses = new IPAddress[AddressLen];
			for (int Index = 0; Index < AddressLen; Index++)
			{
				if (!IPAddress.TryParse(AcceptedAddresses[Index], out ServerConfig.Instance.ConvertedAcceptedServerAddresses[Index]))
				{
					//Maybe its a hostname
					System.Net.IPAddress[] Addresses;
					try
					{
						Addresses = Dns.GetHostEntry(AcceptedAddresses[Index]).AddressList;
					}
					catch (ArgumentOutOfRangeException)
					{
						if (ignoreErrors)
						{
                            Debug.WriteLine("Value is not a valid IP Address or DNS Host: {0}", AcceptedAddresses[Index]);
							continue;
						}
						else
							throw new TortoiseFormatException("Value is not a valid IP Address or DNS Host", AcceptedAddresses[Index], "Any IP Address or DNS host");
					}

					if (Addresses.Length == 0)
					{
						if (ignoreErrors)
						{
                            Debug.WriteLine("DNS Host did not resolve to an IP address: {0}", AcceptedAddresses[Index]);
							continue;
						}
						else
							throw new TortoiseFormatException("DNS Host did not resolve to an IP address", AcceptedAddresses[Index], "Any IP Address or DNS host");
					}

					ServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] = Addresses[0];
				}

				if (ServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.Any ||
					ServerConfig.Instance.ConvertedAcceptedServerAddresses[Index] == IPAddress.IPv6Any)
				{
					ServerConfig.Instance.AcceptAnyAddress = true;
				}


			}


            if (!IPAddress.TryParse(ServerConfig.Instance.ClientListenAddress, out ServerConfig.Instance.ConvertedClientListenAddress))
			{
				throw new TortoiseFormatException("Value is not a valid IP Address or DNS Host", ServerConfig.Instance.ClientListenAddress, "Any IP Address or DNS host");
			}

			if (!IPAddress.TryParse(ServerConfig.Instance.ServerListenAddress, out ServerConfig.Instance.ConvertedServerListenAddress))
			{
				throw new TortoiseFormatException("Value is not a valid IP Address or DNS Host", ServerConfig.Instance.ServerListenAddress, "Any IP Address or DNS host");
			}

		}


    }



}
