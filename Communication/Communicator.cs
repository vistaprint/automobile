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

Communicator.cs 
*/
using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Automobile.Communication.Messaging;

namespace Automobile.Communication
{
    /// <summary>
    /// Delagate for Async message callback
    /// </summary>
    /// <typeparam name="TResponse">Expected response type</typeparam>
    /// <param name="response">Response recived</param>
    public delegate void MessageCallback<TResponse>(TResponse response) where TResponse : IResponse;

    /// <summary>
    /// Provides communication over a stream
    /// </summary>
    public abstract class Communicator
    {

        private const int DEFAULT_TIMEOUT = 15000;

        /// <summary>
        /// A Communicator
        /// </summary>
        protected Communicator()
        {
            Serializer = new BinaryFormatter();
            Serializer.Binder = new OverrideBinder();
        }

        /// <summary>
        /// True if there is data to be read in the stream
        /// </summary>
        public bool DataAvailable
        {
            get { return (Stream as NetworkStream) == null ? Stream.Length > 0 : (Stream as NetworkStream).DataAvailable; }
        }

        /// <summary>
        /// The stream to communicate over
        /// </summary>
        protected Stream Stream { get; set; }

        /// <summary>
        /// Binary formatter
        /// </summary>
        private BinaryFormatter Serializer { get; set; }

        /// <summary>
        /// Prepare the stream for reading and writing
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Send a message, blocking until a response is recieved
        /// </summary>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="message">Message to send</param>
        /// <returns>Response recieved</returns>
        public virtual TResponse SendMessage<TResponse>(IMessageInfo message) where TResponse : IResponse
        {
            message.Timestamp = DateTime.Now;
            Write(message);
            var obj = Read() as IResponse;

            if(obj == null)
            {
                throw new CommunicationException("Non-Response object recieved when response expected.");
            }

            if(obj is IPayload<Exception>)
            {
                throw new CommunicationException("Recieved exception from endpoint: ", (obj as IPayload<Exception>).Contents);
            }

            if(message.Guid != obj.Guid)
            {
                throw new CommunicationException("Recieved response did not match sent message.");
            }

            return (TResponse) obj;
        }

        /// <summary>
        /// Asyncronously send a message, calling callback on response
        /// </summary>
        /// <typeparam name="TResponse">Expected response type</typeparam>
        /// <param name="message">Message to send</param>
        /// <param name="callback">Callback function</param>
        public virtual void SendMessage<TResponse>(IMessageInfo message, MessageCallback<TResponse> callback) where TResponse : IResponse
        {
            message.Timestamp = DateTime.Now;
            Write(message);
            // Yea, this is kinda weird. Simplest way to do it though.
            var thread = new Thread(() => callback((TResponse)Read()));
            thread.Start();
        }

        /// <summary>
        /// Send a response.
        /// </summary>
        /// <param name="response">Response to send</param>
        public virtual void SendResponse(IResponse response)
        {
            response.Timestamp = DateTime.Now;
            Write(response);
        }

        /// <summary>
        /// Block until a message is recieved
        /// </summary>
        /// <typeparam name="TMessage">Expected message type</typeparam>
        /// <returns>Recieved message</returns>
        public TMessage WaitForMessage<TMessage>()
        {
            return (TMessage) Read();
        }

        /// <summary>
        /// Close the stream
        /// </summary>
        public virtual void Close()
        {
            if(Stream != null)
            {
                Stream.Flush();
                Stream.Close();
                Stream = null;
            }
        }

        /// <summary>
        /// Read a object from the stream
        /// </summary>
        /// <param name="blocking">If true, blocks while the stream is empty</param>
        /// <returns>Object read from stream</returns>
        private object Read(bool blocking = true, int timeout = DEFAULT_TIMEOUT)
        {
            int i = 0;
            while (!DataAvailable && blocking)
            {
                Thread.Sleep(50);
                i += 50;
                if(i > timeout)
                {
                    //throw new TimeoutException("Timed out waiting for data from the stream");
                }
            }

            return !DataAvailable ? null : Serializer.Deserialize(Stream);
        }

        /// <summary>
        /// Write an object to the stream
        /// </summary>
        /// <param name="o">Object to write to the stream</param>
        private void Write(object o)
        {
            Serializer.Serialize(Stream, o);
        }
    }
}