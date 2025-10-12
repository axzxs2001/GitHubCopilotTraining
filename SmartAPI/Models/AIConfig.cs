namespace SmartAPI.Models
{

    public class AIConfig
    {
        public string UserModel { get; set; }
        public GPTConfig GPT { get; set; }
        public BedRockConfig Bedrock { get; set; }
        public GeminiConfig Gemini { get; set; }
    }
    public class GeminiConfig
    {
        public string ChatModelId { get; set; }     
    }
    public class GPTConfig
    {
        public string ChatModelId { get; set; }
        public string EmbeddingId { get; set; }
        public int RecordLimit { get; set; }
        public double MinRelevanceScore { get; set; }
        public string SystemPrompt { get; set; }
    }
    public class BedRockConfig
    {
        public string ModelID { get; set; }
        public string AccesskeyID { get; set; }
        public string SecretAccessKey { get; set; }
    }
}
