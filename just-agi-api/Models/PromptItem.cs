namespace just_agi_api.Models
{
    /// <summary>
    /// 提示词项实体类
    /// </summary>
    public class PromptItem
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 提示词
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// 提示词编号
        /// </summary>
        public long PromptID { get; set; }

        /// <summary>
        /// 有效性
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public long CreateUser { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 修改用户
        /// </summary>
        public long ModifyUser { get; set; }
    }


}
