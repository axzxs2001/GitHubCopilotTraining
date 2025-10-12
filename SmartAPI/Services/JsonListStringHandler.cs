using Dapper;
using System.Text.Json;

namespace SmartAPI.Services
{
    public class JsonListStringHandler : SqlMapper.TypeHandler<List<string>>
    {
        // 将数据库字段转为 C# List<string>
        public override List<string> Parse(object value)
        {
            return value == null ? new List<string>() : JsonSerializer.Deserialize<List<string>>(value.ToString());
        }

        // 将 C# List<string> 转为数据库 JSON 字符串
        public override void SetValue(System.Data.IDbDataParameter parameter, List<string> value)
        {
            parameter.Value = value == null ? "[]" : JsonSerializer.Serialize(value);
            parameter.DbType = System.Data.DbType.String;
        }
    }
    public class JsonListSIntHandler : SqlMapper.TypeHandler<List<int>>
    {
        // 将数据库字段转为 C# List<string>
        public override List<int> Parse(object value)
        {
            return value == null ? new List<int>() : JsonSerializer.Deserialize<List<int>>(value.ToString());
        }

        // 将 C# List<string> 转为数据库 JSON 字符串
        public override void SetValue(System.Data.IDbDataParameter parameter, List<int> value)
        {
            parameter.Value = value == null ? "[]" : JsonSerializer.Serialize(value);
            parameter.DbType = System.Data.DbType.String;
        }
    }
}
