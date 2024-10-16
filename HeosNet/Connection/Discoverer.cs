/*
 * Heos.NET
 * Copyright (C) 2024 Jack Beckitt-Marshall
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeosNet.Connection
{
    /// <summary>
    /// Utility which is used to discover HEOS devices on the network.
    /// </summary>
    public class Discoverer : IDisposable
    {
        public Discoverer()
        {
            _udpClient = new UdpClientWrapper();
        }

        public Discoverer(IUdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        internal const int DEFAULT_TIMEOUT = 5000;
        private const string SEARCH_TARGET_NAME = "urn:schemas-denon-com:device:ACT-Denon:1";
        
        private static string _ssdpMessage = string.Join("\r\n",

            "M-SEARCH * HTTP/1.1",
            "HOST: 239.255.255.250:1900",
            $"ST: {SEARCH_TARGET_NAME}",
            "MX: 5",
            "MAN: \"ssdp:discover\"",
            "\r\n"
        );
        private readonly IUdpClient _udpClient;

        /// <summary>
        /// Finds one HEOS device on the network. 
        /// </summary>
        /// <param name="timeout">Maximum amount of time (in ms) that a response will be waited
        /// for.</param>
        /// <returns>IP Address of a discovered HEOS device, otherwise null if a timeout occurs.</returns>
        public async Task<IPAddress> DiscoverOneDeviceAsync(int timeout)
        {
            return (await DiscoverDevicesAsync(timeout, 1)).FirstOrDefault();
        }

        /// <summary>
        /// Discovers devices on the network within a specified timeout period.
        /// </summary>
        /// <param name="timeout">The timeout period in milliseconds.</param>
        /// <param name="maxDevices">The maximum number of devices to discover. If null, discovers all available devices.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of discovered IP addresses.</returns>
        public async Task<IEnumerable<IPAddress>> DiscoverDevicesAsync(int timeout, int? maxDevices)
        {
            _udpClient.Bind(new IPEndPoint(IPAddress.Any, 0));
            var message = Encoding.ASCII.GetBytes(_ssdpMessage);
            var addresses = new HashSet<IPAddress>();

            var recvTask = Task.Run(async () =>
            {
                while (true)
                {
                    var result = await _udpClient.ReceiveAsync();
                    var msg = Encoding.ASCII.GetString(result.Buffer);
                    if (msg.Contains(SEARCH_TARGET_NAME))
                    {
                        addresses.Add(result.RemoteEndPoint.Address);
                    }
                    if (maxDevices != null && addresses.Count >= maxDevices)
                    {
                        return;
                    }
                }
            });

            var timeoutTask = Task.Delay(timeout);
            await _udpClient.SendAsync(message, message.Length, new IPEndPoint(IPAddress.Parse("239.255.255.250"), 1900));

            await Task.WhenAny(timeoutTask, recvTask);
            _udpClient.Close();

            return addresses;


        }

        

        protected virtual void Dispose(bool disposing)
        {
            _udpClient.Dispose();
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
