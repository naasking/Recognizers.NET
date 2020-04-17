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
        public Xml(string tag, Dictionary<string, string> attributes, IEnumerable<Xml> children)
        {
            Tag = tag;
            Attributes = attributes ?? new Dictionary<string, string>();
            Children = children ?? Enumerable.Empty<Xml>();
        }

        public string Tag { get; }

        public Dictionary<string, string> Attributes { get; }

        public IEnumerable<Xml> Children { get; }
    }
}
