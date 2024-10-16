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
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace HeosNet.Connection
{
    [ExcludeFromCodeCoverage]
    public class UdpClientWrapper : IUdpClient
    {
        private readonly UdpClient _udpClient;

        public UdpClientWrapper()
        {
            _udpClient = new UdpClient();
        }

        public void Bind(IPEndPoint endPoint)
        {
            _udpClient.Client.Bind(endPoint);
        }

        public async Task<int> SendAsync(byte[] data, int bytes, IPEndPoint endPoint)
        {
            return await _udpClient.SendAsync(data, bytes, endPoint);
        }

        public async Task<UdpReceiveResult> ReceiveAsync()
        {
            return await _udpClient.ReceiveAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _udpClient.Dispose();
        }

        public void Close()
        {
            _udpClient.Close();
        }
    }
}
