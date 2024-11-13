using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Poochatting.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MessageTypeEnum
    {
        Text,
        Deleted,
        Screenshot,
        Share
    }
}
