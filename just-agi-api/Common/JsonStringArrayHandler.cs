using Dapper;
using just_agi_api.Models;
using System.Data;
using System.Text.Json;

namespace just_agi_api.Common
{
    public class JsonStringArrayHandler : SqlMapper.TypeHandler<string[]>
    {
        public override string[] Parse(object value)
        {
            return JsonSerializer.Deserialize<string[]>(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, string[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
    public class JsonUserUIDataArrayHandler : SqlMapper.TypeHandler<UserUIData[]>
    { 

        public override UserUIData[] Parse(object value)
        {
            return JsonSerializer.Deserialize<UserUIData[]>(value.ToString(),new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive=true });
        }

        public override void SetValue(IDbDataParameter parameter, UserUIData[] value)
        {
            parameter.Value = JsonSerializer.Serialize(value, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
