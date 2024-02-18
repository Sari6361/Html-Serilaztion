using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    public class HtmlElement
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Attributes { get; set; }
        public List<string>? Classes { get; set; }
        public string? InnerHtml { get; set; }
        public HtmlElement? Parent { get; set; }
        public List<HtmlElement>? Children { get; set; } = new();

        IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> elements = new();
            elements.Enqueue(this);

            while (elements.Count > 0)
            {
                var element = elements.Dequeue();
                foreach (var child in element?.Children)
                    elements.Enqueue(child);
                yield return element;
            }
        }

        IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement element = this;
            while (element != null)
            {
                yield return element;
                element = element.Parent;
            }
        }

        public IEnumerable<HtmlElement> MatchElementToQuery(Selector q)
        {
            var res = new HashSet<HtmlElement>();
            FindElements(res, q, this.Descendants());
            return res;
        }
       public void FindElements(HashSet<HtmlElement> matches, Selector q, IEnumerable<HtmlElement> elements)
        {
            if (q == null)
                return;
            foreach (var e in elements)
            {
                if ((q.TagName == null || q.TagName.Equals(e.Name))
                    &&((e.Classes!=null && q.Classes.All(c => e.Classes.Any(cr => cr.Equals(c))))|| e.Classes==q.Classes)
                    && (q.Id == null || e.Id == null || q.Id.Equals(e.Id)))
                {
                    if (q.Child == null)
                        matches.Add(e);
                    FindElements(matches, q.Child, e.Descendants());
                }
            }
        }
        bool CheckSelector(Selector q, HtmlElement e)
        {
            if ((q.TagName != null && !q.TagName.Equals(e.Name)) || (q.Id != null && !q.Id.Equals(e.Id)))
                return false;
            foreach(var c in  q.Classes)
                if(!e.Classes.Contains(c)) return false;
            return true;

        }

    }
}
