namespace SmartAPI.Models
{
    /// <summary>
    /// ContractUser entity that maps to the contract_user database table
    /// </summary>
    public class ContractUser
    {
        /// <summary>
        /// Primary key
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// User associated with the contract
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Contract ID
        /// </summary>
        public int? ContractId { get; set; }

        /// <summary>
        /// Creation timestamp
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}