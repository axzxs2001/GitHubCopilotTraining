namespace SmartAPI.Models
{
    public class CodeSetting
    {
        public int Id { get; set; }
        /// <summary>
        /// 带版本号的语言
        /// </summary>
        public string? LanguageName { get; set; }
        /// <summary>
        /// 语言运行时
        /// </summary>
        public string? LanguageRuntime { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public string? Language { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SerialNumber { get; set; }
        /// <summary>
        /// 提示词
        /// </summary>
        public string? RuntimePrompt { get; set; }
        /// <summary>
        /// 代码的文件名称
        /// </summary>
        public string? CodeFilename { get; set; }
        /// <summary>
        /// 代码模板
        /// </summary>
        public string? CodeTemplate { get; set; }
        /// <summary>
        /// 附加文件
        /// </summary>
        public string? AdditionalFilename { get; set; }
        /// <summary>
        ///  附加文件模板
        /// </summary>
        public string? AdditionalTemplates { get; set; }
        /// <summary>
        /// 入口点
        /// </summary>
        public string? EntryPoint { get; set; }

        public bool Validate { get; set; }

        public DateTime? CreateTime { get; set; }

        public string? CreateUser { get; set; }

        public DateTime? ModifyTime { get; set; }

        public string? ModifyUser { get; set; }
    }
}
