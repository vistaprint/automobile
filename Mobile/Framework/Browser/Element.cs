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

Element.cs 
*/

using System.Collections.Generic;
using System.Linq;

namespace Automobile.Mobile.Framework.Browser
{
    public class Element
    {
        public Element(IMobileBrowser browser, string selector)
        {
            OwnerBrowser = browser;
            Selector = selector;
        }

        public IMobileBrowser OwnerBrowser { get; set; }

        public string Selector { get; set; }

        public void Click()
        {
            OwnerBrowser.ExecJavascript(JQuery.Format(Selector, "[0].click()"));
        }

        public IEnumerable<Element> Children
        {
            get 
            { 
                var count = int.Parse(OwnerBrowser.ExecJavascript(JQuery.Format(JQuery.ChildSelector(Selector), ".length")));

                return
                    Enumerable.Range(0, count).Select(
                        i => new Element(OwnerBrowser, string.Format("{0} > :eq({1})", Selector, i)));
            }
        }

        public Element Parent
        {
            get { return new Element(OwnerBrowser, JQuery.ParentSelector(Selector)); }
        }

        public string TagName
        {
            get { return OwnerBrowser.ExecJavascript(JQuery.Format(Selector, "[0].nodeName")).ToLower(); }
        }

        public string GetAttribute(string name)
        {
            string member = string.Format(".attr('{0}')", name);
            return OwnerBrowser.ExecJavascript(JQuery.Format(Selector, member));
        }
    }
}