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
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using HeosNet.Interfaces;
using HeosNet.Models;

namespace HeosNet.Connection
{
    public class HeosClient
    {
        private const int HEOS_PORT = 1255;
        private readonly ITcpClient _client;
        private Stream _stream;
        private readonly IPAddress _heosIp;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly Dictionary<HeosCommand, Func<HeosResponse, Task>> _handlers;

        public bool Connected { get; private set; }

        public HeosClient(IPAddress ip) : this(ip, new TcpClientWrapper(), new Dictionary<HeosCommand, Func<HeosResponse, Task>>())
        {
        }

        public HeosClient(IPAddress ip, ITcpClient client, Dictionary<HeosCommand, Func<HeosResponse, Task>> handlers)
        {
            _client = client; 
            _heosIp = ip;
            _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            _handlers = handlers;
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync(_heosIp, HEOS_PORT);
            _stream = _client.Stream;
            Connected = true;

            await ReceiveMessagesAsync();

        }

        private async Task ReceiveMessagesAsync()
        {
            using (StreamReader sr = new StreamReader(_stream))
            {
                while (Connected && !sr.EndOfStream)
                {
                    var line = await sr.ReadLineAsync();
                    if (line != null)
                    {
                        HeosResponse lineParsed = JsonSerializer.Deserialize<HeosResponse>(line, _serializerOptions);
                        if (_handlers.ContainsKey(lineParsed.Header.Command))
                        {
                            await _handlers[lineParsed.Header.Command](lineParsed);
                        }
                    }
                }
                
            }
            
        }


    }
}
