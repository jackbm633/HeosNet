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


using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using HeosNet.Interfaces;

namespace HeosNet.Connection
{
    [ExcludeFromCodeCoverage]
    public class TcpClientWrapper : ITcpClient
    {
        private readonly TcpClient _client;
        private bool disposedValue;

        public TcpClientWrapper()
        {
            _client = new TcpClient();
        }

        public Stream Stream { get => _client.GetStream(); }

        public async Task ConnectAsync(IPAddress address, int port)
        {
            await _client.ConnectAsync(address, port);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _client.Dispose();
                }
                disposedValue = true;
            }
        }


        public void Dispose()
        {
            Dispose(disposing: true);
            System.GC.SuppressFinalize(this);
        }
    }
}
