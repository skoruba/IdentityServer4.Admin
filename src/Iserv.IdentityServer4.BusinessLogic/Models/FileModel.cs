namespace Iserv.IdentityServer4.BusinessLogic.Models
{
    /// <summary>
    /// Модель файла
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// Тег файла
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Данные файла
        /// </summary>
        public byte[] FileData { get; set; }

        /// <summary>
        /// Наименование файла
        /// </summary>
        public string Name { get; set; }
    }
}
