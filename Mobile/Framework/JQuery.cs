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

JQuery.cs 
*/
namespace Automobile.Mobile.Framework
{
    /// <summary>
    /// Builds JQuery statements
    /// </summary>
    public static class JQuery
    {
        /// <summary>
        /// Formats the JQuery constructor with a selector
        /// </summary>
        /// <param name="selector">selector to use</param>
        /// <returns>the formatted string</returns>
        public static string Format(string selector)
        {
            return Format(selector, string.Empty);
        }

        /// <summary>
        /// Formats the JQuery constructor with a selector and member
        /// </summary>
        /// <param name="selector">selector to use</param>
        /// <param name="member">member to call</param>
        /// <returns>the formatted string</returns>
        public static string Format(string selector, string member)
        {
            return string.Format("$('{0}'){1}", selector, member);
        }

        /// <summary>
        /// Formats an id selector
        /// </summary>
        /// <param name="id">The id to select</param>
        /// <returns>the formatted string</returns>
        public static string IdSelector(string id)
        {
            return string.Format("#{0}", id);
        }

        /// <summary>
        /// Formats a class selector
        /// </summary>
        /// <param name="className">class to select</param>
        /// <returns>the formatted string</returns>
        public static string ClassSelector(string className)
        {
            return string.Format(".{0}", className);
        }

        /// <summary>
        /// Formats an attribute selector
        /// </summary>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="value">value of the attribute</param>
        /// <param name="mod">Modifier (|, *, ~, $, !, ^)</param>
        /// <returns>the formatted string</returns>
        public static string AttributeSelector(string attrName, string value, string mod)
        {
            return string.Format("[{0}{1}=\\\"{2}\\\"]", attrName, mod, value);
        }

        /// <summary>
        /// Formats an attribute selector with no modifier
        /// </summary>
        /// <param name="attrName">name of the attribute</param>
        /// <param name="value">value of the attribute</param>
        /// <returns>the formatted string</returns>
        public static string AttributeSelector(string attrName, string value)
        {
            return AttributeSelector(attrName, value, string.Empty);
        }

        /// <summary>
        /// Formats an index selector
        /// </summary>
        /// <param name="baseSelector">base selector to index</param>
        /// <param name="index">index</param>
        /// <returns>the formatted string</returns>
        public static string IndexSelector(string baseSelector, int index)
        {
            return string.Format("{0}:eq({1})", baseSelector, index);
        }

        /// <summary>
        /// Formats a selector which is eqivalent to selecting the first parent of the child
        /// </summary>
        /// <param name="child">Child to select the parent of</param>
        /// <returns>the formatted string</returns>
        public static string ParentSelector(string child)
        {
            return string.Format(":has('{0}'):last", child);
        }

        /// <summary>
        /// Selects all immediate children of a parent
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static string ChildSelector(string parent)
        {
            return ChildSelector(parent, "*");
        }

        /// <summary>
        /// Applies a selector on all children of a parent
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static string ChildSelector(string parent, string child)
        {
            return string.Format("{0} > {1}", parent, child);
        }

        /// <summary>
        /// Formats a contains selector
        /// </summary>
        /// <param name="text">Text too search for</param>
        /// <returns>the formatted string</returns>
        public static string ContainsSelector(string text)
        {
            return string.Format(":contains('{0}'):last", text);
        }
    }
}