using System.Collections.Generic;

namespace ElasticSearchExample.Web.Models
{
    public class ImageDTO
    {
        public List<Image> data { get; set; }
    }
    public class Image
    {
        public string image_url { get; set; }
        public string description { get; set; }
        public string url { get; set; }
    }
}
