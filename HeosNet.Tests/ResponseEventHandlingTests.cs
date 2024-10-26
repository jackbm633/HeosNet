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

using HeosNet.Listener;
using HeosNet.Models;
using NSubstitute;

namespace HeosNet.Tests;

[TestClass]
public class ResponseEventHandlingTests
{
    private readonly HeosResponse strippedMessage = new HeosResponse
    {
        Header = new HeosHeader
        {
            Command = new HeosCommand { CommandGroup = "system", Command = "heart_beat" },
            Result = "success",
            Message = new HeosResponseMessage(),
        },
    };

    [TestMethod]
    public async Task ResponseEventHandler_CanListenToEvent()
    {
        // Arrange
        var handler = new ResponseEventHandler();
        var func = Substitute.For<Func<HeosResponse, Task>>();

        // Act
        handler.On(strippedMessage.Header.Command, func);
        await handler.PutAsync(strippedMessage);

        // Assert
        await func.Received().Invoke(strippedMessage);
    }
}
