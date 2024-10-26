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
using System.Threading.Tasks;
using HeosNet.Models;

namespace HeosNet.Listener
{
    /// <summary>
    /// Handles incoming HEOS responses and events.
    /// </summary>
    public class ResponseEventHandler
    {
        private EventHandler<ResponseEventHandlerArgs> EventHandler { get; set; }
        readonly List<Func<HeosResponse, Task>> _listenersOnAll =
            new List<Func<HeosResponse, Task>>();

        /// <summary>
        /// Puts a message into the response handler, and triggers events.
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Task PutAsync(HeosResponse message)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            return PutInternalAsync(message);
        }

        private async Task PutInternalAsync(HeosResponse message)
        {
            var eventString = message.Header.Command.ToHeosCommandString();
            foreach (var listener in _listenersOnAll)
            {
                await listener.Invoke(message);
            }
            EventHandler(
                this,
                new ResponseEventHandlerArgs { EventString = eventString, Message = message }
            );
        }

        public ResponseEventHandler On(HeosCommand evt, Func<HeosResponse, Task> listener)
        {
            if (evt != null)
            {
                var eventString = evt.ToHeosCommandString();
                EventHandler += async (sender, e) =>
                {
                    if (e.EventString == eventString)
                    {
                        await listener.Invoke(e.Message);
                    }
                };
            }
            return this;
        }

        public ResponseEventHandler Once(HeosCommand evt, Func<HeosResponse, Task> listener)
        {
            if (evt != null)
            {
                var eventString = evt.ToHeosCommandString();
                async void Handler(object sender, ResponseEventHandlerArgs e)
                {
                    if (e.EventString == eventString)
                    {
                        await listener.Invoke(e.Message);
                        EventHandler -= Handler;
                    }
                }
                EventHandler += Handler;
            }
            return this;
        }
    }
}
