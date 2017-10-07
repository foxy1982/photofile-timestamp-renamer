using System;
using IO = System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace file_timestamp_renamer
{
    class Program
    {
        private const string InputImageFilePath = @"C:\temp\images\in\";
        
        private const string OutputImageFilePath = @"C:\temp\images\out\";

        static void Main(string[] args)
        {
            var files = IO.Directory.EnumerateFiles(InputImageFilePath);

            var dateFileNames = files.Select(f => {
                var file = new IO.FileInfo(f);
                var imageData = ImageMetadataReader.ReadMetadata(f);
                var subIfdDirectory = imageData.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var dateTime = subIfdDirectory?.GetDescription(36867).Replace(":", "").Replace(" ", "-");
                return new { File = file, NewFileName = $"{dateTime}-{file.Name}"};
            });

            foreach (var message in dateFileNames)
            {
                var outputImagePath = IO.Path.Combine(OutputImageFilePath, message.NewFileName);
                Console.WriteLine(message.File + " ---> " + outputImagePath);
                message.File.CopyTo(outputImagePath, overwrite: true);
            }
        }
    }
}
