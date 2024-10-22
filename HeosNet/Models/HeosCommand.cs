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
using System.Text.Json.Serialization;

namespace HeosNet.Models
{
    [JsonConverter(typeof(HeosCommandJsonConverter))]
    public sealed class HeosCommand : IEquatable<HeosCommand>
    {
        public string CommandGroup { get; set; }
        public string Command { get; set; }
        public Dictionary<string, object> Attributes { get; set; }

        public bool Equals(HeosCommand other)
        {
            if (other == null)
            {
                return false;
            }
            return other.Command == Command && other.CommandGroup == CommandGroup;
        }

        public override int GetHashCode()
        {
            return Command.GetHashCode() ^ CommandGroup.GetHashCode();
        }
    }
}
