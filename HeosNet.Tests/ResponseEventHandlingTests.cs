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
    private readonly HeosResponse message =
        new()
        {
            Header = new HeosHeader
            {
                Command = new HeosCommand
                {
                    CommandGroup = "system",
                    Command = "heart_beat",
                    Attributes = new Dictionary<string, object>() { { "pid", 2 }, { "step", 5 } },
                },
                Result = "success",
                Message = new HeosResponseMessage(),
            },
        };
    private readonly HeosResponse strippedMessage =
        new()
        {
            Header = new HeosHeader
            {
                Command = new HeosCommand { CommandGroup = "system", Command = "heart_beat" },
                Result = "success",
                Message = new HeosResponseMessage(),
            },
        };

    /// <summary>
    /// An event can be listened to.
    /// </summary>
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
        await func.ReceivedWithAnyArgs().Invoke(Arg.Any<HeosResponse>());
    }

    /// <summary>
    /// Attributes in responses should be ignored.
    /// </summary>
    [TestMethod]
    public async Task ResponseEventHandler_IgnoresAttributes()
    {
        // Arrange
        var handler = new ResponseEventHandler();
        var func = Substitute.For<Func<HeosResponse, Task>>();

        // Act
        handler.On(strippedMessage.Header.Command, func);
        await handler.PutAsync(message);

        // Assert
        await func.ReceivedWithAnyArgs().Invoke(Arg.Any<HeosResponse>());
    }

    /// <summary>
    /// Once and On functions work as expected.
    /// </summary>
    [TestMethod]
    public async Task ResponseEventHandler_Once_On_WorkAsExpected()
    {
        // Arrange
        var handler = new ResponseEventHandler();
        var onceFunc = Substitute.For<Func<HeosResponse, Task>>();
        var onFunc = Substitute.For<Func<HeosResponse, Task>>();

        // Act
        handler.On(strippedMessage.Header.Command, onFunc);
        handler.Once(strippedMessage.Header.Command, onceFunc);

        await handler.PutAsync(message);
        await handler.PutAsync(message);
        await handler.PutAsync(message);


        // Assert
        await onFunc.ReceivedWithAnyArgs(3).Invoke(Arg.Any<HeosResponse>());
        await onceFunc.ReceivedWithAnyArgs(1).Invoke(Arg.Any<HeosResponse>());

    }
}
