using System;
using System.Linq;
using System.Collections.Generic;

namespace Recognizers.Xml
{
    /// <summary>
    /// A basic XML abstraction.
    /// </summary>
    public readonly struct Xml
    {
        /// <summary>
        /// Construct a new XML element.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attributes"></param>
        /// <param name="children"></param>
        public Xml(string tag, Dictionary<string, string> attributes, IEnumerable<Xml> children)
        {
            Tag = tag;
            Attributes = attributes ?? Enumerable.Empty<KeyValuePair<string, string>>();
            Children = children ?? Enumerable.Empty<Xml>();
        }

        /// <summary>
        /// The XML tag.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// The XML element's attributes.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Attributes { get; }

        /// <summary>
        /// The XML element's children.
        /// </summary>
        public IEnumerable<Xml> Children { get; }
        
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override string ToString()
        {
            var buf = new System.Text.StringBuilder();
            WriteTo(buf, 0);
            return buf.ToString();
        }

        void WriteTo(System.Text.StringBuilder buf, int level)
        {
            buf.Append(' ', level).Append('<').Append(Tag);
            foreach (var x in Attributes)
                buf.Append(x.Key).Append("=\"").Append(x.Value).Append("\" ");
            var ie = Children.GetEnumerator();
            if (ie.MoveNext())
            {
                buf.AppendLine(">");
                do ie.Current.WriteTo(buf, level + 2);
                while (ie.MoveNext());
                buf.Append(' ', level).Append("</").Append(Tag).AppendLine(">");
            }
            else
            {
                buf.AppendLine(" />");
            }
        }
    }
}
