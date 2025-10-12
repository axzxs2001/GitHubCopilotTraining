namespace SmartAPI.Models
{
    public class APIQuest
    {
        public int CodeSettingID { get; set; }
        public string Language { get; set; }
        public string Runtime { get; set; }
        public string Product { get; set; }
        public int APIID { get; set; }
        #region v1.1
        /// <summary>
        /// 用户提示词
        /// </summary>
        public string Prompt { get; set; }


        public string ConnectionID { get; set; }


        public string UserEmail { get; set; }
        #endregion
    }
}
