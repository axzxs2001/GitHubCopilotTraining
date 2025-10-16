/*
  CopilotPrompt
  #file:'2-architecture.md' 根据2-architecture.md中的Prompt和PromptItem的内容，生成C#的实体类，并且加上中文注释 
 */

namespace just_agi_api.Models
{
    /// <summary>
    /// 提示词实体类
    /// </summary>
    public class PromptViewModel : Prompt
    {
        public List<PromptItem> PromptItems { get; set; }
    }

}
