namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Class for creating and updating a hive section.
    /// </summary>
    public class UpdateHiveSectionRequest
    {
        /// <summary>
        /// Gets or sets a hive section name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a hive section code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the hive section ID.
        /// </summary>
        public int StoreHiveId { get; set; }
    }
}
