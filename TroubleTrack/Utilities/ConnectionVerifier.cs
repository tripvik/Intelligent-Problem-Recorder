using System.Text;
using System.Xml;
using TroubleTrack.Models;

namespace TroubleTrack.Utilities
{
    public static class ConnectionVerifier
    {
        public static async Task<(bool, string)> TestControllerConnection(ConnectionInfo connectionInfo)
        {
            // Construct the URL.
            var protocol = connectionInfo.UseSsl ? "https://" : "http://";
            var url = $"{protocol}{connectionInfo.Host}:{connectionInfo.Port}/controller/rest/applications/{connectionInfo.ApplicationName}";

            // Construct the authorization header value.
            var authInfo = $"singularity-agent@{connectionInfo.AccountName}:{connectionInfo.Key}";
            var authHeaderValue = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));

            // Prepare the request.
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", $"Basic {authHeaderValue}");

            try
            {
                HttpClient _httpClient = new HttpClient();
                _httpClient.Timeout = TimeSpan.FromSeconds(5);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return (true, "Connection successful");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, "Oops! Authorization failed. Please Verify the Account Name and Account Key.");
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (responseBody.Contains("<html") && responseBody.Contains("<body>"))
                    {
                        try
                        {
                            var xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(responseBody);
                            var ns = new XmlNamespaceManager(xmlDoc.NameTable);
                            ns.AddNamespace("ns", "http://www.w3.org/1999/xhtml");

                            var messageNode = xmlDoc.SelectSingleNode("/ns:html/ns:body/ns:p[ns:b[text()='message']]/text()", ns);

                            if (messageNode != null)
                            {
                                return (false, messageNode.Value.Trim());
                            }
                            return (false, "Connection failed. Please verify the connection details.");
                        }
                        catch (XmlException)
                        {
                            return (false, "Connection failed. Please verify the connection details.");
                        }
                    }
                    else
                    {
                        return (false, $"Error code: {(int)response.StatusCode}. Connection Failed.");
                    }
                }
            }
            catch
            {
                return (false, "Failed to connect. Ensure the server has access to the controller URL.");
            }
        }
    }
}
