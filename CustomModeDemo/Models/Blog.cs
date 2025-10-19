using System;

namespace CustomModeDemo.Models
{
    public class Blog
    {
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public required string Author { get; set; }
    public required DateTime PublishedDate { get; set; }
    public required string[] Tags { get; set; }
    }
}
