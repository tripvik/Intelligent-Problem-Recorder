using MimeKit;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace TroubleTrack.Utilities
{
    public static class MhtExtractor
    {
        public static List<string> ExtractImages(string mhtFilePath)
        {
            if (!File.Exists(mhtFilePath))
            {
                throw new FileNotFoundException("The specified MHT file was not found.", mhtFilePath);
            }

            // Parse the MHT file
            MimeMessage message;
            using (var stream = File.OpenRead(mhtFilePath))
            {
                message = MimeMessage.Load(stream);
            }

            // Define the temp directory to store images
            string imagesDirectory = Path.Combine(Path.GetTempPath(), "images");

            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            var imagePaths = new List<string>();
            int imageCount = 1;

            // Extract images
            foreach (var part in message.BodyParts)
            {
                if (part is MimePart mimePart && mimePart.ContentType.MimeType.StartsWith("image/"))
                {
                    // Generate image file name with appropriate extension
                    string imageFileName = $"Image{imageCount}.JPEG";
                    string imagePath = Path.Combine(imagesDirectory, imageFileName);

                    using (var stream = File.Create(imagePath))
                    {
                        mimePart.Content.DecodeTo(stream);
                    }

                    imagePaths.Add(imagePath);
                    imageCount++;
                }
            }

            return imagePaths;
        }

        public static List<string> ExtractSteps(string mhtFilePath)
        {
            if (string.IsNullOrWhiteSpace(mhtFilePath) || !File.Exists(mhtFilePath))
            {
                throw new ArgumentException("Invalid file path.", nameof(mhtFilePath));
            }

            // Read the MHT file content
            string mhtContent = File.ReadAllText(mhtFilePath);

            // Extract XML part using regex
            var xmlMatch = Regex.Match(mhtContent, @"(?s)<script id=""myXML"" type=""text/xml"">(.+?)</script>");
            if (!xmlMatch.Success)
            {
                throw new Exception("XML part not found in the MHT file.");
            }

            string xmlContent = xmlMatch.Groups[1].Value;

            // Clean the XML content to ensure it is well-formed
            xmlContent = CleanXmlContent(xmlContent);

            // Parse and extract descriptions
            return ExtractDescriptions(xmlContent);
        }

        private static string CleanXmlContent(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                throw new ArgumentException("XML content cannot be null or empty.", nameof(xmlContent));
            }

            // Remove leading whitespace and any extra characters before the XML declaration
            xmlContent = Regex.Replace(xmlContent, @"^\s*<\?xml[^?>]*\?>", match => match.Value.Trim(), RegexOptions.Singleline);

            return xmlContent.Trim(); // Ensure no extra whitespace
        }

        private static List<string> ExtractDescriptions(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                throw new ArgumentException("XML content cannot be null or empty.", nameof(xmlContent));
            }

            // Load the XML content into an XDocument
            var xDoc = XDocument.Parse(xmlContent);

            // Extract all <Description> elements
            var descriptions = xDoc.Descendants("Description")
                .Select(desc => desc.Value.Trim());

            // Join the descriptions with newlines
            return descriptions.ToList();
        }
    }
}
