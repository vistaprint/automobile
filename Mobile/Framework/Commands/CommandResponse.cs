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

CommandResponse.cs 
*/
using System;
using Automobile.Communication.Messaging;

namespace Automobile.Mobile.Framework.Commands
{
    /// <summary>
    /// A response to a mobile command
    /// </summary>
    /// <typeparam name="TContents">Type of playload of the response</typeparam>
    [Serializable]
    public class CommandResponse<TContents> : Response, IPayload<TContents>
    {
        
        public CommandResponse(bool success, TContents contents) : base(success)
        {
            Contents = contents;
        }

        public TContents Contents { get; set; }
    }
}