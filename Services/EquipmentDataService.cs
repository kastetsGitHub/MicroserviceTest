using MicroserviceTest.View;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace MicroserviceTest.Services
{
    public class EquipmentDataService
    {
        private readonly string _xmlLocalPath;
        private readonly int _serverPort;

        public EquipmentDataService(int serverPort)
        {
            _serverPort = serverPort;
            _xmlLocalPath = @"D:\Microservice\MicroserviceTest\StorageData\EquipmentData.xml";
        }

        public async Task<ServiceResult<Equipment>> GetIDAsync(string xmlPath)
        {
            ServiceResult<Equipment> result = await Task.Run(() =>
            {
#if DEBUG
                XDocument xmlDoc = XDocument.Load(_xmlLocalPath);
#endif
#if RELEASE
                XDocument xmlDoc = LoadXDocumentAsync().Result;
#endif

                List<string> segments = xmlPath.Split('/').ToList();
                IEnumerable<XElement> nodes = xmlDoc.Descendants("node");
                Equipment equipment = new Equipment();
                string id = string.Empty;

                foreach (string name in segments)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        XElement xElement = nodes.FirstOrDefault(node => (string)node.Attribute("name") == name);

                        if (xElement != null)
                        {
                            equipment.ID = (string)xElement.Attribute("id");
                            nodes = xElement.Elements();
                        }
                        else return new ServiceResult<Equipment>
                        {
                            StatusCode = StatusCodes.Status404NotFound,
                            ErrorMessage = "id not found",
                            IsSuccess = false
                        };
                    }
                    else return new ServiceResult<Equipment>
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorMessage = "bad request",
                        IsSuccess = false

                    };
                }

                if (equipment.ID == null)
                {
                    return new ServiceResult<Equipment>
                    {
                        StatusCode = StatusCodes.Status406NotAcceptable,
                        ErrorMessage = "id is not acceptable",
                        IsSuccess = false
                    };
                }

                return new ServiceResult<Equipment>
                {
                    Data = equipment,
                    IsSuccess = true
                };
            });

            return result;
        }

        /// <summary>
        /// загрузка xml документа с сервера .."Debian 12"
        /// </summary>
        /// <returns></returns>
        public async Task<XDocument> LoadXDocumentAsync()
        {
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(IPAddress.Any, _serverPort);

                using (var stream = client.GetStream()) 
                {
                    int bufferSize = 1024;
                    byte[] buffer = new byte[bufferSize];
                    StringBuilder response = new StringBuilder();
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, offset:0, buffer.Length)) > 0)
                    {
                        response.Append(Encoding.UTF8.GetString(buffer, index:0, bytesRead));
                    }

                    return XDocument.Parse(response.ToString());
                }
            }
        }
    }
}
