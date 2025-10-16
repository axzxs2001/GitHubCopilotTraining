namespace just_agi_api.Models
{
    public class SmartFillUser
    {
        public long ID { get; set; }
        public string UserUrl { get; set; }
        public UserUIData[] UserData { get; set; }
        public bool Validate { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime ModifyTime { get; set; }
        public string ModifyUser { get; set; }
    }
}
