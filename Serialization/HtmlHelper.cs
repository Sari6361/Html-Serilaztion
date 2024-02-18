using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace Serialization
{
    //implements singleton design
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;
        public List<string> Tags { get; set; }
        public List<string> VoidTags { get; set; }

        private HtmlHelper()
        {
            FileStream tagsStream = File.OpenRead("seed/HtmlTags.json");
            Tags = JsonSerializer.Deserialize<List<string>>(tagsStream);

            FileStream voidTagsStream = File.OpenRead("seed/HtmlVoidTags.json");
            VoidTags = JsonSerializer.Deserialize<List<string>>(voidTagsStream);
        }

    }
}
