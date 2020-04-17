﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Recognizers.Xml
{
    public static class XmlRecognizers
    {
        public static bool XmlStart(this Input x, ref Position pos, out Dictionary<string, string> attributes) =>
               pos.Save(out var i)
            && x.Literal("<?xml", ref i)
            && x.KeyValuePairs(ref i, out attributes)
            && x.Literal("?>", ref i)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out attributes);

        public static bool XmlOpen(this Input x, ref Position pos, out ReadOnlySpan<char> tag, out bool selfClose, out Dictionary<string, string> attributes) =>
               pos.Save(out var i)
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Char('<', ref i)
            && x.CharsUntil(ref i, out tag, ' ', '/', '>')
            && x.Optional(x.KeyValuePairs(ref i, out attributes))
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Optional(x.Char('/', ref i, out var closed))
            && x.Char('>', ref i)
            && pos.AdvanceTo(i)
            && ((selfClose = closed.Length > 0) || true)
            || Recognizers.Fail(out tag) | Recognizers.Fail(out attributes) | (selfClose = false);

        public static bool XmlClose(this Input x, ref Position pos, out ReadOnlySpan<char> tag) =>
               pos.Save(out var i)
            && x.Optional(x.WhiteSpaces(ref i))
            && x.Char('<', ref i)
            && x.Char('/', ref i)
            && x.CharsUntil('>', ref i, out tag)
            && x.Char('>', ref i)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out tag);

        public static bool XmlNode(this Input x, ref Position pos, out Xml xml)
        {
            var i = pos;
            List<Xml> children = null;
            if (x.XmlOpen(ref i, out var tag, out var selfClose, out var attributes))
            {
                children = selfClose ? null : new List<Xml>();
                while (!selfClose)
                {
                    if (x.XmlClose(ref i, out var closeTag))
                    {
                        if (tag.Equals(closeTag, StringComparison.Ordinal))
                            break;
                        else
                            throw new System.Xml.XmlException($"Found unmatched closing tag {closeTag.ToString()} at position {i.Pos}.");
                    }
                    else if (x.XmlNode(ref i, out var xml1))
                        children.Add(xml1);
                }
                xml = new Xml(tag.ToString(), attributes, children);
            }
            else
            {
                xml = default(Xml);
            }
            return pos.AdvanceTo(i);
        }

        public static bool Xml(this Input x, ref Position pos, out Xml xml, out Dictionary<string, string> attributes) =>
               pos.Save(out var i)
            && x.XmlStart(ref i, out attributes)
            && x.XmlNode(ref i, out xml)
            && pos.AdvanceTo(i)
            || Recognizers.Fail(out xml) | Recognizers.Fail(out attributes);
    }
}
