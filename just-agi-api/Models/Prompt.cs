/*
  CopilotPrompt
  #file:'2-architecture.md' 根据2-architecture.md中的Prompt和PromptItem的内容，生成C#的实体类，并且加上中文注释 
 */

namespace just_agi_api.Models
{
    /// <summary>
    /// 提示词实体类
    /// </summary>
    public class Prompt
    {
        /// <summary>
        /// 编号
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 提示词URL
        /// </summary>
        public string LLMUrl { get; set; }

        /// <summary>
        /// 企业编号
        /// </summary>
        public long EnterpriseID { get; set; }

        /// <summary>
        /// 企业角色编号
        /// </summary>
        public long RoleID { get; set; }

        /// <summary>
        /// 状态（0临时，1正式）
        /// </summary>
        public bool Status { get; set; }

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
