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

using System.Net;
using System.Threading.Tasks;
using HeosNet.Interfaces;

namespace HeosNet.Connection
{
    public class HeosClient
    {
        private const int HEOS_PORT = 1255;
        private readonly ITcpClient _client;
        private readonly IPAddress _heosIp;

        public bool Connected { get; private set; }

        public HeosClient(IPAddress ip) : this(ip, new TcpClientWrapper())
        {
        }

        public HeosClient(IPAddress ip, ITcpClient client)
        {
            _client = client; 
            _heosIp = ip;
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_heosIp, HEOS_PORT);
            Connected = true;
        }

    }
}
