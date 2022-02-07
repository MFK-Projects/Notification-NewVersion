using System.Collections.Generic;

namespace MFKIanApi.Models
{
    public class RequestModel
    {
        public string Url { get; set; }
        public string[] SelectItem { get; set; }
        public string RequestEntityName { get; set; }
        public int Count { get; set; }
        public List<RequestFilterModel> Filters { get; set; }
    }
}
