using System.IO.Compression;

namespace TroubleTrack.Utilities
{
    public static class ZipExtractor
    {
        /// <summary>
        /// Extracts the single file from the root of the ZIP archive and returns the extracted file's name.
        /// </summary>
        /// <param name="zipFilePath">Path to the ZIP file.</param>
        /// <returns>The name of the extracted file.</returns>
        public static string ExtractFile(string zipFilePath)
        {
            string outputDirectory = Path.GetTempPath();

            if (!File.Exists(zipFilePath))
            {
                throw new FileNotFoundException("The specified ZIP file was not found.", zipFilePath);
            }

            string extractedFileName = null;
            string extractedFilePath = null;

            using (var zipArchive = ZipFile.OpenRead(zipFilePath))
            {
                if (zipArchive.Entries.Count != 1)
                {
                    throw new InvalidOperationException("The ZIP archive does not contain exactly one file.");
                }

                var entry = zipArchive.Entries[0];
                extractedFileName = entry.FullName;

                extractedFilePath = Path.Combine(outputDirectory, extractedFileName);

                entry.ExtractToFile(extractedFilePath, overwrite: true);
            }

            return extractedFilePath;
        }
    }
}
