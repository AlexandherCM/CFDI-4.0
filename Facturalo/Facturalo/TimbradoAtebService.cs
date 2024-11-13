using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Facturalo
{
    public class TimbradoAtebService
    {
        private readonly string _urlTest = "http://201.149.3.138:81/wsTimbrado.asmx"; // URL de pruebas
        private readonly string _urlProd = "https://cfdi33.timbrado.com.mx:443/wsTimbrado.asmx"; // URL de producción (HTTPS)
        private readonly string _userName = "autofac_t"; // USUARIO DE PRUEBAS
        private readonly string _password = "6Pj!N+5sbQ$4=t8Y"; // CONTRASEÑA PARA EL ENTORNO DE PRUEBAS
        private readonly bool _isProduction; // Define si el entorno es producción o pruebas

        public TimbradoAtebService(bool isProduction = false)
        {
            _isProduction = isProduction;
        }

        private class StringWriterWithEncoding : StringWriter
        {
            public StringWriterWithEncoding(Encoding encoding) : base()
            {
                this.m_Encoding = encoding;
            }
            private readonly Encoding m_Encoding;
            public override Encoding Encoding => this.m_Encoding;
        }

        public async Task<string> TimbrarCFDIAsync(string xmlBase64)
        {
            // Selecciona la URL según el ambiente
            var url = _isProduction ? _urlProd : _urlTest;

            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30); // Configura un timeout de 30 segundos

                var soapEnvelope = CreateSoapEnvelope(xmlBase64);
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml")
                };

                // Agrega los encabezados al contenido
                request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/xml");
                request.Headers.Add("SOAPAction", "https://cfdi.timbrado.com.mx/timbradov2/GeneraTimbre");

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    // Leer y procesar la respuesta
                    var responseString = await response.Content.ReadAsStringAsync();
                    return ProcessResponse(responseString);
                }
                catch (HttpRequestException e)
                {
                    throw new Exception($"Error al conectar al servicio: {e.Message}");
                }
            }
        }

        private string CreateSoapEnvelope(string xmlBase64)
        {
            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (var writer = XmlWriter.Create(sww, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("soap", "Envelope", "http://schemas.xmlsoap.org/soap/envelope/");
                    writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

                    writer.WriteStartElement("soap", "Header", null);

                    // Header de autenticación
                    writer.WriteStartElement("AuthenticationHeader", "https://cfdi.timbrado.com.mx/timbradov2");
                    writer.WriteElementString("UserName", _userName);
                    writer.WriteElementString("Password", _password);
                    writer.WriteEndElement(); // Fin de AuthenticationHeader

                    writer.WriteStartElement("TestConfigHeader", "https://cfdi.timbrado.com.mx/timbradov2");
                    writer.WriteElementString("ConsultaDB", "false");
                    writer.WriteElementString("UUIDPrueba", "false");
                    writer.WriteElementString("TimbrarDuplicados", "true");
                    writer.WriteEndElement(); // Fin de TestConfigHeader

                    writer.WriteEndElement(); // Fin de Header

                    writer.WriteStartElement("soap", "Body", null);
                    writer.WriteStartElement("GeneraTimbre", "https://cfdi.timbrado.com.mx/timbradov2");
                    writer.WriteElementString("cfdiBytes", xmlBase64);
                    writer.WriteEndElement(); // Fin de GeneraTimbre
                    writer.WriteEndElement(); // Fin de Body

                    writer.WriteEndElement(); // Fin de Envelope
                    writer.WriteEndDocument();
                }

                return sww.ToString();
            }
        }

        private string ProcessResponse(string response)
        {
            // Opcional: Procesa la respuesta para extraer UUID u otros datos
            // Aquí puedes realizar operaciones de búsqueda o parsing XML si es necesario.

            response = response.Replace("&lt;", "<").Replace("&gt;", ">");
            return response;
        }
    }


}