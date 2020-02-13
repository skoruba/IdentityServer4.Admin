using Iserv.IdentityServer4.BusinessLogic.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Iserv.IdentityServer4.BusinessLogic.Extension
{
    public static class IFormFileExtension
    {
        /// <summary>
        /// Конвертация в модель файла FileModel
        /// </summary>
        public static FileModel ConvertToFileModel(this IFormFile file)
        {
            if (file == null)
            {
                return null;
            }
            var result = new FileModel { Name = file.FileName };
            using (var readStream = file.OpenReadStream())
            {
                using (var stream = new BinaryReader(readStream))
                {
                    result.FileData = stream.ReadBytes((int)file.Length);
                }
            }
            result.Tag = file.Name;
            return result;
        }
    }
}
