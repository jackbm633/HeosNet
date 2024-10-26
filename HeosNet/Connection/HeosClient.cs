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
using System.Threading;
using System.Threading.Tasks;
using HeosNet.Interfaces;
using HeosNet.Listener;
using HeosNet.Models;

namespace HeosNet.Connection
{
    /// <summary>
    /// Represents a client that connects to a HEOS device.
    /// </summary>
    public class HeosClient
    {
        private const int HEOS_PORT = 1255;
        private readonly ITcpClient _client;
        private Stream _stream;
        private readonly IPAddress _heosIp;
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// Exposes the event handler so that the user can add custom code
        /// to respond to events.
        /// </summary>
        public ResponseEventHandler EventHandler { get; } = new ResponseEventHandler();

        /// <summary>
        /// Gets a value indicating whether the client is connected to the HEOS device.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeosClient"/> class with the specified IP address.
        /// </summary>
        /// <param name="ip">The IP address of the HEOS device.</param>
        public HeosClient(IPAddress ip)
            : this(ip, new TcpClientWrapper()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeosClient"/> class with the specified parameters.
        /// </summary>
        /// <param name="ip">The IP address of the HEOS device.</param>
        /// <param name="client">The TCP client to use for the connection.</param>
        /// <param name="handlers">The dictionary of handlers for HEOS commands.</param>
        public HeosClient(IPAddress ip, ITcpClient client)
        {
            _client = client;
            _heosIp = ip;
            _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        /// <summary>
        /// Asynchronously connects to the HEOS device.
        /// </summary>
        /// <returns>A task that represents the asynchronous connect operation.</returns>
        public async Task ConnectAsync()
        {
            var ct = default(CancellationToken);
            await ConnectAsync(ct);
        }

        /// <summary>
        /// Asynchronously connects to the HEOS device with a cancellation token.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task that represents the asynchronous connect operation.</returns>
        public async Task ConnectAsync(CancellationToken ct)
        {
            await _client.ConnectAsync(_heosIp, HEOS_PORT);
            _stream = _client.Stream;
            Connected = true;

            var recvTask = Task.Run(() => ReceiveMessagesAsync(), ct);
            await Task.WhenAll(new[] { recvTask });
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
                        HeosResponse lineParsed = JsonSerializer.Deserialize<HeosResponse>(
                            line,
                            _serializerOptions
                        );
                        await EventHandler.PutAsync(lineParsed);
                    }
                }
            }
        }
    }
}
