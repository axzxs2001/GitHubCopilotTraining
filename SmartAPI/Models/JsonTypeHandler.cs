using Dapper;
using System.Data;
using System.Text.Json;

namespace SmartAPI.Models
{
    public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T> where T : class, new()
    {
        public override void SetValue(IDbDataParameter parameter, T value)
        {
            parameter.Value = JsonSerializer.Serialize(value);
            parameter.DbType = DbType.String;
        }

        public override T Parse(object value)
        {
            if (value == null || value == DBNull.Value) return new T();
            return JsonSerializer.Deserialize<T>(value.ToString());
        }
    }

}
