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

ISimpleMessage.cs 
*/

using System;

namespace Automobile.Communication.Messaging
{
    [Serializable]
    public class Message : IMessage
    {
        public Message()
        {
            Guid = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public Guid Guid { get; private set; }
        public DateTime Timestamp { get; set; }

        public virtual IResponse CreateResponse()
        {
            return CreateResponse(true);
        }

        public virtual IResponse CreateResponse(bool success)
        {
            return new Response(Guid, success);
        }
    }
}