namespace eu.iamia.AAOP.ISP.Models
{
    public interface ILoanStorageMetadata
    {
        /// <summary>
        /// Internal Primary Key in repository
        /// </summary>
        int Id { get; set; }


        /// <summary>
        /// Name in storage.
        /// </summary>
        string Name { get; set; }
    }
}
