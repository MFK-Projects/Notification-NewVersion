using MFKIanApi.Enums;

namespace MFKIanApi.Models
{
    public class RequestFilterModel
    {
        public string Item { get; set; }
        public object Value { get; set; }
        public string Key { get; set; }
        public RequestFiltersType Type { get; set; }
    }
}
