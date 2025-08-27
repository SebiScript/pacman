using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;

namespace PacMan.Services
{
    public static class ImageLoader
    {
        private static readonly Dictionary<string, Bitmap> _imageCache = new();

        public static Bitmap LoadImage(string fileName)
        {
            if (_imageCache.TryGetValue(fileName, out var cachedBitmap))
                return cachedBitmap;

            try
            {
                var uri = new Uri($"avares://PacMan/Assets/{fileName}");
                using var stream = AssetLoader.Open(uri);
                var bitmap = new Bitmap(stream);
                _imageCache[fileName] = bitmap;
                return bitmap;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error loading image {fileName}: {ex.Message}");
                return CreateFallbackBitmap();
            }
        }

        private static Bitmap CreateFallbackBitmap()
        {
            
            var bitmap = new WriteableBitmap(new Avalonia.PixelSize(20, 20), new Avalonia.Vector(96, 96), Avalonia.Platform.PixelFormat.Bgra8888, Avalonia.Platform.AlphaFormat.Premul);
            using (var buffer = bitmap.Lock())
            {
                unsafe
                {
                    var ptr = (uint*)buffer.Address;
                    for (int i = 0; i < buffer.Size.Width * buffer.Size.Height; i++)
                    {
                        ptr[i] = 0xFFFFFF00; // Amarillo en formato BGRA
                    }
                }
            }
            return bitmap;
        }

        public static void ClearCache()
        {
            foreach (var bitmap in _imageCache.Values)
            {
                bitmap.Dispose();
            }
            _imageCache.Clear();
        }
    }
}