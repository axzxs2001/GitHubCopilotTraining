namespace SmartAPI.Models
{   
    public class Scene
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Describe { get; set; }
        public int ParentId { get; set; }
    }
}
