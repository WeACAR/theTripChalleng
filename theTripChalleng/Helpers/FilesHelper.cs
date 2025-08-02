//creat full file helper for files


using System.IO;
using System.Threading.Tasks;
namespace theTripChalleng.Helpers
{
    public static class FilesHelper
    {
        // Method to save a file and return its path
        public static async Task<string> SaveFileAsync(Stream fileStream, string fileName, string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, fileName);
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(file);
            }

            return filePath;
        }
        // Helper method to convert IFormFile to byte array
        public static byte[] ConvertToBytes(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null) return null;
            using (var memoryStream = new System.IO.MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        // Method to delete a file
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
