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


    /// <summary>
    /// Simply reads the nodes from the reader and loads them into 
    /// ParserNodes then puts them in a list. This is so the client 
    /// has lookahead.
    /// </summary>
    public class MemorySgmlReader
    {
        public System.Collections.Generic.List<ParserNode> Nodes { get; private set; }


        public MemorySgmlReader(Sgml.SgmlReader sgmlReader)
        {
            Nodes = new System.Collections.Generic.List<ParserNode>();
            LoadReader(sgmlReader);
        }


        private void LoadReader(Sgml.SgmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case System.Xml.XmlNodeType.Element:
                        HandleElementStart(reader);
                        break;
                    case System.Xml.XmlNodeType.EndElement:
                        HandleElementEnd(reader);
                        break;
                    case System.Xml.XmlNodeType.Text:
                        HandleText(reader);
                        break;
                }
            }
        }


        /// <summary>
        /// Adds the node to the previous's linked list, then puts the node in the node list.
        /// </summary>
        /// <param name="node"></param>
        private void AddNode(ParserNode node)
        {
            if (Nodes.Count > 0)
            {
                ParserNode prev = Nodes[Nodes.Count - 1];
                prev.NextNode = node;
                node.PrevNode = prev;
            }
            Nodes.Add(node);
        }


        private void HandleElementStart(Sgml.SgmlReader reader)
        {
            //ghetto, but the SgmlReader has no way to get ALL attributes. 
            ParserNode node = new ParserNode(reader.Name, System.Xml.XmlNodeType.Element);
            node.AddAttribute("style", reader.GetAttribute("style"));
            node.AddAttribute("title", reader.GetAttribute("title"));
            node.AddAttribute("class", reader.GetAttribute("class"));
            node.AddAttribute("href", reader.GetAttribute("href"));
            node.AddAttribute("src", reader.GetAttribute("src"));
            node.AddAttribute("colspan", reader.GetAttribute("colspan"));
            node.AddAttribute("rowspan", reader.GetAttribute("rowspan"));

            AddNode(node);
        }


        private void HandleElementEnd(Sgml.SgmlReader reader)
        {
            ParserNode node = new ParserNode(reader.Name, System.Xml.XmlNodeType.EndElement);

            AddNode(node);
        }


        private void HandleText(Sgml.SgmlReader reader)
        {
            ParserNode node = new ParserNode(null, reader.Value, System.Xml.XmlNodeType.Text);

            AddNode(node);
        }


    }


}
