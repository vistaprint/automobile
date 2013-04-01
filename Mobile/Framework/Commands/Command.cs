/*
Copyright 2013 Vistaprint

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

Command.cs 
*/
using System;
using Automobile.Communication.Messaging;

namespace Automobile.Mobile.Framework.Commands
{
    /// <summary>
    /// Basic command
    /// </summary>
    [Serializable]
    public abstract class Command : Message
    {
        /// <summary>
        /// Type of the command
        /// </summary>
        public abstract CommandType CommandType { get; }

        /// <summary>
        /// Mode of the command
        /// </summary>
        public virtual CommandMode Mode { get; set; }

        public CommandResponse<T> CreateResponse<T>(T contents)
        {
            return new CommandResponse<T>(true, Guid, contents);
        }
    }
}