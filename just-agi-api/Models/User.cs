/*
  CopilotPrompt
  #file:'2-architecture.md' 根据2-architecture.md中的User的内容，生成C#的实体类，并且加上中文注释
 
 */


namespace just_agi_api.Models
{
    /// <summary>
    /// 用户实体类
    /// </summary>
    public class User
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 用户名（邮箱）
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 盐码
        /// </summary>
        public string Salt { get; set; }

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
