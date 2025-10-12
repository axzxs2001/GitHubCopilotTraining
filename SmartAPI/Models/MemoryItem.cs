using NetTopologySuite.Noding;

namespace SmartAPI.Models
{
    public class MemoryItem
    {
        public string Key { get; set; }
        public string Content { get; set; }
    }
    public class ProductMemoryItem : MemoryItem
    {
        public int ProductID { get; set; }
    }
    public class BatchProductMemoryItem
    {
        public int ProductID { get; set; }
        public List<MemoryItem> MemoryItems { get; set; } = new List<MemoryItem>();
    }
}
