//Copyright (c) 2008 Ben Ogle

//Permission is hereby granted, free of charge, to any person
//obtaining a copy of this software and associated documentation
//files (the "Software"), to deal in the Software without
//restriction, including without limitation the rights to use,
//copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the
//Software is furnished to do so, subject to the following
//conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
//OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
//HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
//OTHER DEALINGS IN THE SOFTWARE.


namespace HTML2Markup
{


    public class ParserNode
    {
        public System.Xml.XmlNodeType NodeType { get; private set; }
        public string Name { get; private set; }
        public string Value { get; private set; }
        public System.Collections.Generic.Dictionary<string, string> Attributes { get; private set; }

        public ParserNode NextNode { get; set; }
        public ParserNode PrevNode { get; set; }


        public ParserNode(string name, string value, System.Xml.XmlNodeType nodeType)
        {
            Name = name;
            Value = value;
            NodeType = nodeType;
            Attributes = new System.Collections.Generic.Dictionary<string, string>();
        }


        public ParserNode(string name, System.Xml.XmlNodeType nodeType) 
            : this(name, null, nodeType) { }


        public void AddAttribute(string key, string val)
        {
            //if(val != null)
            Attributes[key] = val;
        }


    }


}
