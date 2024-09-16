using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Extensions
{
    public static class Extension
    {
        public static bool CheckContentType(this IFormFile file)
        {
            return file.ContentType.Contains("image/");
        }
        public static bool CheckSize(this IFormFile file, int size)
        {
            return file.Length <= size * 1024;
        }
        public static string Save(this IFormFile file, string root, string folder)
        {
            if (file == null || string.IsNullOrEmpty(root) || string.IsNullOrEmpty(folder))
                throw new ArgumentException("Invalid arguments for file saving.");

            string newFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string directoryPath = Path.Combine(root, "wwwroot", folder);

            // Ensure directory exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string path = Path.Combine(directoryPath, newFileName);
            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return newFileName;
        }
        public static void DeleteFile(this string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img",  fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
