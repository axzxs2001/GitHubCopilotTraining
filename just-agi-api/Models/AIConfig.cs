namespace just_agi_api.Models
{
    public class AIConfig
    {
        public OpenAIConfig OpenAIConfig { get; set; }
        public AzureConfig AzureConfig { get; set; }

    }
    public class OpenAIConfig
    {
        public string ModelID { get; set; }
        public string Key { get; set; }
    }

    public class AzureConfig
    {

    }
}
