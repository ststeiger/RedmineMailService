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


    public class TextileParser : Parser
    {
        private ParserNode CurrentLink { get; set; }

        private int _listDepth = 0;
        private System.Collections.Generic.Stack<ListType> _inList = new System.Collections.Generic.Stack<ListType>();
        private bool _inTable = false;

        private bool _inExtendedBlock = false;
        private bool _exitExtendedBlock = false;

        private static System.Text.RegularExpressions.Regex NUMRE = 
            new System.Text.RegularExpressions.Regex("[0-9]+", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static System.Text.RegularExpressions.Regex GLYPH_NUMRE = 
            new System.Text.RegularExpressions.Regex("&#([0-9]+);", System.Text.RegularExpressions.RegexOptions.Compiled);

        private static System.Collections.Generic.Dictionary<string, string> GLYPHS = new System.Collections.Generic.Dictionary<string, string>(){
            {"&nbsp;", " "},
            {"&amp;", "&"},
            {"&ndash;", "-"}, {"&#8211;", "-"}, {"&mdash;", "--"}, {"&#8212;", "--"},
            {"&quot;", "\""},   {"&apos;", "'"},   {"&#8217;", "'"},   {"&#8216;", "'"},
            {"&lt;", "<"},      {"&gt;", ">"},
            {"&copy;", "(C)"}, {"&#169;", "(C)"}, {"&reg;", "(R)"},  {"&#8482;", "(TM)"}
        };

        //http://www.w3schools.com/css/css_reference.asp
        private static System.Collections.Generic.HashSet<string> STYLE_WHITELIST = new System.Collections.Generic.HashSet<string>(){
            "background", "background-color",
            "border", "border-bottom", "border-top", "border-left", "border-right",
            "height", "width",
            "font", "font-family", "font-size", "font-style", "font-weight",
            "margin", "margin-bottom", "margin-top", "margin-left", "margin-right",
            "padding", "padding-left", "padding-top", "padding-bottom", "padding-top",
            "color", "direction",
            "line-height", "text-align", "text-decoration", "text-indent", "text-transform"
        };

        public TextileParser()
        {
        }

        private string ProcessCharCodeGlyph(System.Text.RegularExpressions.Match m)
        {
            return System.Convert.ToChar(int.Parse(m.Groups[1].Value)).ToString();
        }

        /// <summary>
        /// Extracts the style="" attribute from the element 
        /// and properly formats the style for some element. 
        /// Removes any properties that are not 'on the list' 
        /// including any properties that use IE's expression.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetStyleStr(ParserNode node)
        {
            string style = node.Attributes["style"];
            string ret = "";
            if (style != null)
            {
                System.Collections.Generic.List<string> str = new System.Collections.Generic.List<string>();
                string[] split = style.Trim().Split(new char[1] { ';' });
                foreach (string s in split)
                {
                    string trimmed = s.Trim();
                    if (trimmed.Length > 0)
                    {
                        string[] elem = trimmed.Split(':');
                        if (elem.Length == 2)
                        {
                            elem[0] = elem[0].Trim().ToLower();
                            elem[1] = elem[1].Trim();
                            if (!elem[1].ToLower().Contains("expression") && STYLE_WHITELIST.Contains(elem[0]))
                                str.Add(elem[0] + ": " + elem[1]);
                        }
                    }
                }

                ret = "{" + string.Join(";", str.ToArray()) + "}";
            }
            return ret;
        }

        private string GetClassStr(ParserNode node)
        {
            string cl = node.Attributes["class"];
            string ret = "";
            if (cl != null)
            {
                ret = string.Format("({0})", cl);
            }
            return ret;
        }

        private string GetStyleOrClassStr(ParserNode node)
        {
            string ret = GetClassStr(node);
            if (ret.Length < 1)
                ret = GetStyleStr(node);
            return ret;
        }

        private string GetTextileHref(string href)
        {
            href = href.Trim();
            return href;
        }

        private void AddQuickTag(ParserNode node, string repl, bool isStart)
        {
            if (isStart)
            {
                AddOutput(" ");
                AddOutput(repl);
                AddOutput(GetStyleStr(node));
            }
            else
                AddOutput(repl + " ");
        }

        private void HandleQuickTags(ParserNode node, bool isStart)
        {
            switch (node.Name)
            {
                case "i":
                case "em":
                    AddQuickTag(node, "_", isStart);
                    break;
                case "b":
                case "strong":
                    AddQuickTag(node, "*", isStart);
                    break;
                case "s":
                case "del":
                case "strike":
                    AddQuickTag(node, "-", isStart);
                    break;
                case "sub":
                    AddQuickTag(node, "~", isStart);
                    break;
                case "sup":
                    AddQuickTag(node, "^", isStart);
                    break;
                case "cite":
                    AddQuickTag(node, "??", isStart);
                    break;
                case "span":
                    AddQuickTag(node, "%", isStart);
                    break;

                case "code":
                    ParserNode cnode;
                    if (isStart)
                    {
                        cnode = node.PrevNode;
                        //if the previous node is a pre tag, then we dont do the code quick tag.
                        if (!(cnode != null && cnode.NodeType == System.Xml.XmlNodeType.Element && cnode.Name == "pre"))
                            AddQuickTag(node, "@", isStart);
                    }
                    else
                    {
                        cnode = node.NextNode;
                        //if the previous node is a pre tag, then we dont do the code quick tag.
                        if (!(cnode != null && cnode.NodeType == System.Xml.XmlNodeType.EndElement && cnode.Name == "pre"))
                            AddQuickTag(node, "@", isStart);
                    }
                    break;
            }
        }

        private void AddBlockTag(ParserNode node, string tag, bool isStart)
        {
            _exitExtendedBlock = false;

            if (isStart) AddOutput(tag + GetStyleOrClassStr(node) + ". ", 2, true);
            else AddOutput("", 2, false);
        }

        private void AddExtendedBlockTag(ParserNode node, string tag, bool isStart)
        {
            if (isStart)
            {
                _inExtendedBlock = true;
                _exitExtendedBlock = false;
                AddOutput(tag + GetStyleOrClassStr(node) + ".. ", 2, true);
            }
            else
            {
                _inExtendedBlock = false;
                _exitExtendedBlock = true;
                AddOutput("", 2, false);
            }
        }


        protected override string ProcessGlyphs(string s)
        {
            foreach (string k in GLYPHS.Keys)
                s = s.Replace(k, GLYPHS[k]);

            s = GLYPH_NUMRE.Replace(s, new System.Text.RegularExpressions.MatchEvaluator(ProcessCharCodeGlyph));

            return s;
        }

        protected override void ConvertStart()
        {
            _inList.Clear();
            _listDepth = 0;
            _inTable = false;
            CurrentLink = null;
        }

        protected override void HandleElementStart(ParserNode node)
        {
            HandleQuickTags(node, true);

            ParserNode next = node.NextNode;
            ParserNode prev = node.PrevNode;

            switch (node.Name)
            {
                case "p":
                    if (node.Attributes["style"] != null || node.Attributes["class"] != null || _exitExtendedBlock)
                        AddBlockTag(node, "p", true);
                    else
                        AddOutput("", 2, true);

                    break;

                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "h7":
                    AddBlockTag(node, node.Name, true);
                    break;

                case "blockquote":
                    AddBlockTag(node, "bq", true);
                    break;

                case "a":
                    AddOutput(" \"");
                    AddOutput(GetStyleOrClassStr(CurrentNode));
                    //text node will be aded next, then on the end tag, 
                    //we will output the href and all that.
                    CurrentLink = CurrentNode;
                    break;

                case "img":
                    string src = node.Attributes["src"];
                    if (src != null)
                        AddOutput(string.Format(" !{0}{1}! ", GetStyleOrClassStr(CurrentNode), src));
                    break;

                case "br":
                    AddOutput("\n");
                    break;

                case "pre":

                    //lookahead. If the next element is code, 
                    //dont print the pre. .
                    if (next != null &&
                        next.NodeType == System.Xml.XmlNodeType.Element &&
                        next.Name == "code")
                    {
                        break;
                    }
                    AddExtendedBlockTag(node, "pre", true);
                    break;

                case "code":
                    if (prev != null &&
                        prev.NodeType == System.Xml.XmlNodeType.Element &&
                        prev.Name == "pre")
                    {
                        AddExtendedBlockTag(node, "bc", true);
                    }
                    break;


                //lists!
                case "ol":
                    AddOutput("", _listDepth > 0 ? 1 : 2, true);
                    _listDepth++;
                    _inList.Push(ListType.Ordered);
                    break;
                case "ul":
                    AddOutput("", _listDepth > 0 ? 1 : 2, true);
                    _listDepth++;
                    _inList.Push(ListType.Unordered);
                    break;
                case "li":

                    //lookahead. If the next element is a nested list, 
                    //dont print the list indicator.
                    if (next != null &&
                        next.NodeType == System.Xml.XmlNodeType.Element &&
                        (next.Name == "ul" || next.Name == "ol"))
                    {
                        break;
                    }

                    if (_inList.Peek() == ListType.Ordered)
                        AddOutput(RepeatStr("#", _listDepth) + " ", 1, true);
                    else
                        AddOutput(RepeatStr("*", _listDepth) + " ", 1, true);

                    break;

                //TABLES
                case "table":
                    AddOutput("", 2, true);
                    _inTable = true;
                    break;
                case "td":
                case "th":
                    string pre = "";
                    string colspan = node.Attributes["colspan"];
                    string rowspan = node.Attributes["rowspan"];

                    if (node.Name == "th")
                        pre += "_";
                    if (colspan != null && NUMRE.IsMatch(colspan))
                        pre += "/" + colspan;
                    if (rowspan != null && NUMRE.IsMatch(rowspan))
                        pre += "\\" + rowspan;

                    if (pre.Length > 0)
                        pre += ". ";

                    AddOutput("|" + pre);
                    break;


                default:
                    break;
            }

        }

        protected override void HandleElementEnd(ParserNode node)
        {
            HandleQuickTags(node, false);

            ParserNode next = node.NextNode;
            ParserNode prev = node.PrevNode;

            switch (node.Name)
            {
                case "p":
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                case "h7":
                    AddOutput("", 2, false);
                    break;

                case "blockquote":
                    AddBlockTag(node, "bq", false);
                    break;

                case "a":
                    if (CurrentLink.Attributes["title"] != null)
                        AddOutput("(" + CurrentLink.Attributes["title"] + ")");

                    AddOutput("\"");

                    if (CurrentLink != null && CurrentLink.Attributes.ContainsKey("href"))
                        AddOutput(":" + GetTextileHref(CurrentLink.Attributes["href"]) + " ");
                    CurrentLink = null;
                    break;

                case "pre":
                    //if the previous element is a code element, we dont do the end...
                    if (prev != null &&
                        prev.NodeType == System.Xml.XmlNodeType.EndElement &&
                        prev.Name == "code")
                    {
                        break;
                    }

                    AddOutput("", 1, false);
                    AddExtendedBlockTag(node, "pre", false);
                    break;

                case "code":
                    if (next != null &&
                        next.NodeType == System.Xml.XmlNodeType.EndElement &&
                        next.Name == "pre")
                    {
                        AddOutput("", 1, false);
                        AddExtendedBlockTag(node, "bc", false);
                    }
                    break;

                //lists
                case "ol":
                    _listDepth--;
                    _inList.Pop();
                    AddOutput("", _listDepth > 0 ? 1 : 2, false);
                    break;
                case "ul":
                    _listDepth--;
                    _inList.Pop();
                    AddOutput("", _listDepth > 0 ? 1 : 2, false);
                    break;
                case "li":
                    AddOutput("", 1, false);
                    break;

                //tables
                case "table":
                    AddOutput("", 2, false);
                    _inTable = false;
                    break;
                case "tr":
                    AddOutput("|", 1, false);
                    break;

                default:
                    break;
            }
        }

        protected override void HandleText(ParserNode node)
        {
            //special case: if extended block ends and just text is the next element,
            //we will use a paragraph tag.
            if (_exitExtendedBlock)
            {
                AddOutput("p. ", 2, true);
                _exitExtendedBlock = false;
            }

            string output = node.Value;

            AddOutput(output.Trim());
        }


    }


}
