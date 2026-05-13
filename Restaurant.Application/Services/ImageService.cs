using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Restaurant.Application.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

using Image = SixLabors.ImageSharp.Image;

namespace Restaurant.Application.Services
{
    public class ImageService : IImageService
    {
        private readonly IWebHostEnvironment _env;

        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };

        public ImageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // 🔥 Upload with Compression + Resize
        public async Task<string> UploadAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is empty");

            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                throw new Exception("Only JPG and PNG are allowed");

            var fileName = $"{Guid.NewGuid()}.jpg"; // نحولها JPG عشان ضغط أفضل

            var folderPath = Path.Combine(_env.WebRootPath, "images", folderName);

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var filePath = Path.Combine(folderPath, fileName);

            using var image = await Image.LoadAsync(file.OpenReadStream());

            // 🔥 Resize (تصغير الأبعاد)
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(600, 600) // تقدر تغيّرها
            }));

            // 🔥 Compression (Quality control)
            var encoder = new JpegEncoder
            {
                Quality = 60 // بين 50 - 70 أفضل حاجة
            };

            await image.SaveAsync(filePath, encoder);

            return $"/images/{folderName}/{fileName}";
        }

        // 🔥 Delete Image
        public Task<bool> DeleteAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return Task.FromResult(false);

            var fullPath = Path.Combine(_env.WebRootPath, imageUrl.TrimStart('/'));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}