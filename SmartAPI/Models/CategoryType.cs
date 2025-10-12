namespace SmartAPI.Models
{
    public static class CategoryType
    {
        public static string None = "None";
        [IsAPI]
        public static string API = "API";
        //[IsAPI]
        //public static string Sign = "Sign";
        [IsAPI]
        public static string NeedCallBack = "NeedCallBack";
        public static string Memo = "Memo";
        public static string CallBack = "CallBack";        
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class IsAPIAttribute : Attribute
    {
    }
}
