using System;

namespace SmartAPI.Models
{
    /// <summary>
    /// Contract entity that maps to the contracts database table
    /// </summary>
    public class Contract
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Contract title
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Contract content
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        /// Version of the contract
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime? CreateTime { get; set; }
        
        /// <summary>
        /// Modification timestamp
        /// </summary>
        public DateTime? ModifyTime { get; set; }
        
        /// <summary>
        /// User who created the contract
        /// </summary>
        public string CreateUser { get; set; }
        
        /// <summary>
        /// User who last modified the contract
        /// </summary>
        public string ModifyUser { get; set; }
    }
}