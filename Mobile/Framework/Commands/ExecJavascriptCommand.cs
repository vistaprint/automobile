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

ExecJavascriptCommand.cs 
*/
using System;

namespace Automobile.Mobile.Framework.Commands
{
    /// <summary>
    /// Command to execute a javascript statement
    /// </summary>
    [Serializable]
    public class ExecJavascriptCommand : Command
    {
        public ExecJavascriptCommand(string js)
        {
            Javascript = js;
        }

        /// <summary>
        /// Javascript to execute
        /// </summary>
        public string Javascript { get; set; }

        /// <summary>
        /// Always invoke
        /// </summary>
        public override CommandMode Mode
        {
            get { return CommandMode.Invoke; }
            set { }
        }

        /// <summary>
        /// Always ExecJavascript
        /// </summary>
        public override CommandType CommandType
        {
            get { return CommandType.ExecJavascript; }
        }
    }
}
