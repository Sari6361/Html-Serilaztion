using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Serialization
{
    public class Selector
    {
        public string? TagName { get; set; }
        public string? Id { get; set; }
        public List<string> Classes { get; set; } = new();
        public Selector? Parent { get; set; }
        public Selector? Child { get; set; }

        public static Selector ConvertQuery(string query)
        {
            var queryParts = query.Split(' ').ToList();
            Selector root = new(), current = root, prev=current;
            List<string> classes_=new List<string>();
            string valid1, valid2;
            queryParts.ForEach(q =>
            {
                var elements = new Regex("(?=[#\\.])").Split(q).Where(l => l.Length > 0).ToList();
                var classes = elements.Where(e => e.StartsWith('.')).ToList();

                classes.ForEach(e =>classes_.Add(e.Substring(1)));
                var id = elements?.Find(e => e.StartsWith("#"))?.Substring(1);
                var tagName = elements.Find(e => !e.StartsWith('.') && !e.EndsWith("#"));
                valid1 = HtmlHelper.Instance.Tags.Find(e => e.Equals(tagName));
                valid2 = HtmlHelper.Instance?.VoidTags?.Find(e => !e.Equals(tagName));
                tagName = valid1 != null || valid2 != null ? tagName : null;

                current.TagName = tagName; current.Classes = classes_; current.Id = id;

                Selector son = new();
                current.Child = son;
                son.Parent = current;
                prev = current;
                current = son;

                valid1 = valid2 = null;
            });
            prev.Child = null;
            current = null;
            return root;
        }



    }
}
