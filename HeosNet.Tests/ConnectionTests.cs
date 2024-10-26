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
using System.Net.Sockets;
using HeosNet.Connection;
using HeosNet.Interfaces;
using HeosNet.Models;
using NSubstitute;

namespace HeosNet.Tests
{
    [TestClass]
    public class ConnectionTests
    {
        /// <summary>
        /// Checks that a mock HEOS client works properly.
        /// </summary>
        [TestMethod]
        public async Task HeosClient_CanBeMocked()
        {
            // Arrange
            ITcpClient client = Substitute.For<ITcpClient>();
            client.Stream.Returns(new MemoryStream());

            // Act
            HeosClient c = new(IPAddress.Parse("192.168.0.7"), client);
            await c.ConnectAsync();

            // Assert
            Assert.IsTrue(c.Connected);
        }

        /// <summary>
        /// Checks that a mock HEOS client works properly.
        /// </summary>
        [TestMethod]
        public async Task HeosClient_ReceivesMessages()
        {
            // Arrange
            MemoryStream mockStream = new();
            using StreamWriter sw = new(mockStream);
            await sw.WriteLineAsync(
                "{\"heos\": {\"command\": \"system/heart_beat\", \"result\": \"success\", \"message\": \"m\"}}"
            );
            await sw.FlushAsync();
            ITcpClient client = Substitute.For<ITcpClient>();
            mockStream.Position = 0;
            client.Stream.Returns(mockStream);
            // Act & Assert
            HeosClient c = new(IPAddress.Parse("192.168.0.7"), client);
            c.EventHandler.On(
                new HeosCommand { CommandGroup = "system", Command = "heart_beat" },
                (message) =>
                {
                    Assert.AreEqual("success", message.Header.Result);
                    return Task.CompletedTask;
                }
            );
            await c.ConnectAsync();
        }
    }
}
