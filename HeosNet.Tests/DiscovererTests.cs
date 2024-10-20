﻿/*
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
using System.Text;
using HeosNet.Connection;
using NSubstitute;

namespace HeosNet.Tests
{
    [TestClass]
    [TestCategory("Unit")]
    public sealed class DiscovererTests
    {
        /// <summary>
        /// Checks that the discoverer returns the correct IP when receiving.
        /// </summary>
        [TestMethod]
        public async Task Discoverer_DiscoverOneDeviceAsync_ReturnsCorrectIP()
        {
            // Arrange
            IUdpClient mockUdpClient = Substitute.For<IUdpClient>();
            Discoverer d = new(mockUdpClient);
            mockUdpClient
                .ReceiveAsync()
                .Returns(
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.5"), 12345)
                    )
                );
            // Act
            var ip = await d.DiscoverOneDeviceAsync(1);

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.0.5"), ip);

            // Clean up
            d.Dispose();
        }

        /// <summary>
        /// Checks that the discoverer returns the correct IP when receiving.
        /// </summary>
        [TestMethod]
        public async Task DiscoverOneDeviceAsync_CustomEndpoint_ReturnsCorrectIP()
        {
            // Arrange
            IUdpClient mockUdpClient = Substitute.For<IUdpClient>();
            Discoverer d = new(mockUdpClient);
            mockUdpClient
                .ReceiveAsync()
                .Returns(
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.5"), 12345)
                    )
                );
            // Act
            var ip = await d.DiscoverOneDeviceAsync(1, new IPEndPoint(IPAddress.Loopback, 12345));

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.0.5"), ip);

            // Clean up
            d.Dispose();
        }

        /// <summary>
        /// Checks that the discoverer only returns one IP, even if multiple IPs are received over
        /// the network.
        /// </summary>
        [TestMethod]
        public async Task DiscoverOneDeviceAsync_MultipleDevicesFound_OnlyReturnsOne()
        {
            // Arrange
            IUdpClient mockUdpClient = Substitute.For<IUdpClient>();
            Discoverer d = new(mockUdpClient);
            mockUdpClient
                .ReceiveAsync()
                .Returns(
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.5"), 12345)
                    ),
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.6"), 12345)
                    ),
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.7"), 12345)
                    )
                );
            // Act
            var ip = await d.DiscoverOneDeviceAsync();

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.0.5"), ip);

            // Clean up
            d.Dispose();
        }

        /// <summary>
        /// Checks that the discoverer only returns a Denon device, not another brand.
        /// the network.
        /// </summary>
        [TestMethod]
        public async Task DiscoverOneDeviceAsync_MultipleDevicesFound_OnlyReturnsDenon()
        {
            // Arrange
            IUdpClient mockUdpClient = Substitute.For<IUdpClient>();
            Discoverer d = new(mockUdpClient);
            mockUdpClient
                .ReceiveAsync()
                .Returns(
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:microsoft:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.5"), 12345)
                    ),
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:ACT-Denon:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.6"), 12345)
                    ),
                    new UdpReceiveResult(
                        Encoding.ASCII.GetBytes("urn:schemas-denon-com:device:google:1"),
                        new IPEndPoint(IPAddress.Parse("192.168.0.7"), 12345)
                    )
                );
            // Act
            var ip = await d.DiscoverOneDeviceAsync();

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.0.6"), ip);

            // Clean up
            d.Dispose();
        }
    }
}
