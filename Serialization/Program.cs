using Serialization;
using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;


var html = await Load("https://learn.malkabruk.co.il/");
var cleanHtml = new Regex("(\\n)|(\\t)|(\\f)|(\\v)|(\\r)").Replace(html, "");
List<string> htmlLines = new Regex("(<.*?>)").Split(cleanHtml).Where(l => l.Length > 0 &&!l.Equals(" ") &&!l.Equals("    ") && !l.Equals("      ")).ToList();

htmlLines.RemoveAt(0);
var root = Serialize(htmlLines);
var htmlQuery = Selector.ConvertQuery("div.home-container div.home-header");
var resualt = root.MatchElementToQuery(htmlQuery);
Console.ReadLine();


//load the interent page
async static Task<string> Load(string url)
{
    HttpClient client = new();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

static HtmlElement Serialize(List<string> elements)
{
    HtmlElement root = new(), parent = new();
    elements.ForEach(l =>
    {
        bool s = false;
        string tagName = l.Substring(1).Split(new string[] { " ", "<\\", "\\>",">" },StringSplitOptions.None)[0];
        Console.WriteLine(tagName);
        if (HtmlHelper.Instance.Tags.Any(t => t.Contains(tagName)) &&! l.EndsWith("/>"))
            s = true;

        if (l.StartsWith("</html"))
            Console.WriteLine("page scanned successfully");

        if (l.StartsWith("</"))
            parent = parent?.Parent;

        else if (l.StartsWith("<"))
        {
            var attributes = new Regex("([^\\s]*?)=\"(.*?)\"").Matches(l).ToList();
            List<string> attributeslist = new();
            attributes.ForEach(a => attributeslist.Add(a.ToString()));

            var stringClasses = attributeslist?.FirstOrDefault(t => t.Contains("class"))?.Split("=")?[1].Replace("\"", "");
            var classes = stringClasses?.Split(' ').ToList();

            var id = attributeslist?.Find(l => l.Contains("id"))?.Split("=")?[1].Replace("\"", "");
            attributeslist?.Remove(attributeslist.Find(l => l.Contains("id")));
            attributeslist?.Remove(attributeslist.Find(l => l.Contains("class")));

            HtmlElement n = new()
            {
                Name = tagName,
                Id = id,
                Classes = classes,
                Attributes = attributeslist,
            };

            if (s)
                if (tagName.Equals("html"))
                    root = parent = n;
                else
                {
                    parent?.Children?.Add(n);
                    n.Parent = parent;
                    parent = n;
                }
            else
            {
                if (HtmlHelper.Instance.VoidTags.Any(t => t == tagName))
                {
                    n.Name = tagName;
                    parent?.Children?.Add(n);
                    n.Parent = parent;
                }
            }
        }
        else
            parent.InnerHtml += l;
    });
    return root;
}
