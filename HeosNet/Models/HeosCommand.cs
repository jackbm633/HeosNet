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

using HeosNet.Models.Conversion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace HeosNet.Models
{
    [JsonConverter(typeof(HeosCommandJsonConverter))]
    public sealed class HeosCommand : IEquatable<HeosCommand>
    {
        public string CommandGroup { get; set; }
        public string Command { get; set; }
        public Dictionary<string, object> Attributes { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is HeosCommand))
            {
                return false;
            }
            return ((HeosCommand)obj).Command == Command && ((HeosCommand)obj).CommandGroup == CommandGroup;
        }
        public bool Equals(HeosCommand other)
        {
            return Equals((object)other);
        }

        public override int GetHashCode()
        {
            return Command.GetHashCode() ^ CommandGroup.GetHashCode();
        }

        public static HeosCommand ParseHeosCommandString(string command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(command);
            }
            var commandParts = command.Split('/');
            if (commandParts.Length != 2)
            {
                throw new InvalidDataException("Invalid command: number of parts is incorrect");
            }
            return new HeosCommand
            {
                CommandGroup = commandParts[0],
                Command = commandParts[1],
            };
        }


    }
}
