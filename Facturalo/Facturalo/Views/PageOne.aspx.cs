using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using XSDToXML.Utils;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using Facturalo.Classes.FacturaPDF;
using System.Linq;
using devServicioNET;
using System.Web.UI.WebControls;
using Org.BouncyCastle.Utilities.Encoders;
using System.Threading.Tasks;

namespace Facturalo.Views
{
    public partial class PageOne : System.Web.UI.Page
    {
        //Proveedor ejemplo
        private string ApiKey = "93edc4af66b84c938f66a56ca0596205";

        private string ClavePrivada = "12345678a";
        private string PathCadenaOriginal = "~/Docs/CadenaOriginal/cadenaoriginal_4_0.xslt";

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        private string CSD_Cer =
            "~/Docs/CACX7605101P8_20230509114423/CSD_CACX7605101P8_20230509130305/CSD_Sucursal_1_CACX7605101P8_20230509_130254.cer";
        private string CSD_Key =
            "~/Docs/CACX7605101P8_20230509114423/CSD_CACX7605101P8_20230509130305/CSD_Sucursal_1_CACX7605101P8_20230509_130254.key";
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        //private string CSD_Cer =
        //    "~/Docs/FUNK671228PH6_20230509114807/CSD_FUNK671228PH6_20230509130458/CSD_Sucursal_1_FUNK671228PH6_20230509_130451.cer";
        //private string CSD_Key =
        //    "~/Docs/FUNK671228PH6_20230509114807/CSD_FUNK671228PH6_20230509130458/CSD_Sucursal_1_FUNK671228PH6_20230509_130451.key";
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

        //private string CSD_Cer2 = "~/Docs/EKU9003173C9/CSD_Sucursal_1_EKU9003173C9.cer";
        //private string CSD_Key2 = "~/Docs/EKU9003173C9/CSD_Sucursal_1_EKU9003173C9.key";

        private string PathFactura = "~/Docs/FacturasXML/NuevaFactura.xml";
        private string PathFacturaSellada = "~/Docs/FacturasXML/FacturaSellada.xml";
        private string PathFacturaTimbrada = "~/Docs/FacturasXML/FacturaTimbrada.xml";

        protected void Page_Load(object sender, EventArgs e) { }

        [Obsolete]
        protected async void btnGeneratePem_Click(object sender, EventArgs e)
        {
            string numeroCertificado, aa, b, c;
            SelloDigital.leerCER(Server.MapPath(this.CSD_Cer), out aa, out b, out c, out numeroCertificado);

            decimal tasaIva = 0.16m;
            decimal importe = 1500.00m;
            decimal importeIva = Math.Round(importe * tasaIva, 2);

            decimal total = importe + importeIva;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            Comprobante oComprobante = new Comprobante();
            oComprobante.Version = "4.0";
            oComprobante.Serie = "H";
            oComprobante.Folio = "1";
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //oComprobante.Sello = ""; // Pendiente
            oComprobante.FormaPago = "01";
            //oComprobante.NoCertificado = "30001000000500003416"; // Asegúrate de tener este valor asignado correctamente
            oComprobante.NoCertificado = numeroCertificado; // Asegúrate de tener este valor asignado correctamente
            //oComprobante.Certificado = ""; // Pendiente
            oComprobante.SubTotal = importe; // Subtotal antes de IVA
            // Agrega el IVA al total
            oComprobante.Total = total;
            //oComprobante.Descuento = 1; // Si se aplica algún descuento, debe ser considerado en los cálculos
            oComprobante.Moneda = "MXN";
            oComprobante.Exportacion = "01";
            // El total se calculará después de agregar el IVA al subtotal
            oComprobante.TipoDeComprobante = "I";
            oComprobante.MetodoPago = "PUE";
            oComprobante.LugarExpedicion = "07020";
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            ComprobanteEmisor oEmisor = new ComprobanteEmisor
            {
                Rfc = "CACX7605101P8", // RFC obtenido del certificado
                Nombre = "XOCHILT CASAS CHAVEZ", // Nombre obtenido del certificado
                RegimenFiscal = "621" // Este valor debe ser verificado y asignado según el régimen fiscal del emisor

                //Rfc = "FUNK671228PH6", 
                //Nombre = "KARLA FUENTE NOLASCO",
                //RegimenFiscal = "621" 
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            ComprobanteReceptor oReceptor = new ComprobanteReceptor
            {
                Rfc = "URE180429TM6",
                Nombre = "UNIVERSIDAD ROBOTICA ESPAÑOLA",
                DomicilioFiscalReceptor = "86991",
                RegimenFiscalReceptor = "601",
                UsoCFDI = "G01"

                //Rfc = "CACX7605101P8",
                //Nombre = "XOCHILT CASAS CHAVEZ",
                //DomicilioFiscalReceptor = "01000",
                //RegimenFiscalReceptor = "612",
                //UsoCFDI = "G01"
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            List<ComprobanteConcepto> listConceptos = new List<ComprobanteConcepto>();
            ComprobanteConcepto concepto = new ComprobanteConcepto
            {
                Importe = importe,
                ClaveProdServ = "84111506",
                Cantidad = 1m,
                ClaveUnidad = "E48",
                Descripcion = "Depósito a saldo en plataforma CAMM",
                ValorUnitario = importe,
                ObjetoImp = "02" // Indica que sí es objeto de impuesto (En este caso si).
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            ComprobanteConceptoImpuestosTraslado trasladoIVA = new ComprobanteConceptoImpuestosTraslado
            {
                Base = concepto.Importe,
                Impuesto = "002", // IVA
                TipoFactor = "Tasa",
                TasaOCuota = 0.160000m, // Tasa de IVA del 16%
                Importe = importeIva
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            concepto.Impuestos = new ComprobanteConceptoImpuestos
            {
                Traslados = new ComprobanteConceptoImpuestosTraslado[] { trasladoIVA }
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            listConceptos.Add(concepto);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            oComprobante.Conceptos = listConceptos.ToArray();
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;

            // Finalmente, ajusta el objeto Impuestos del comprobante para incluir el total de impuestos trasladados
            oComprobante.Impuestos = new ComprobanteImpuestos
            {
                TotalImpuestosTrasladados = importeIva,
                Traslados = new ComprobanteImpuestosTraslado[]
                {
                    new ComprobanteImpuestosTraslado
                    {
                        Base = oComprobante.SubTotal,
                        Impuesto = "002",
                        TipoFactor = "Tasa",
                        TasaOCuota = 0.160000m,
                        Importe = importeIva
                    }
                }
            };

            //Crear XML's, sellado, timbrado y conversión del PDF
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            await SaveXML(oComprobante);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        }

        [Obsolete]
        private async Task SaveXML(Comprobante oComprobante)
        {
            //Se crea el XML sin sellar
            CreateXML(oComprobante, Server.MapPath(this.PathFactura));

            string CadenaOriginal = string.Empty;

            System.Xml.Xsl.XslCompiledTransform transformador = new System.Xml.Xsl.XslCompiledTransform(true);
            transformador.Load(Server.MapPath(this.PathCadenaOriginal));

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter xwo = XmlWriter.Create(sw, transformador.OutputSettings))
                {
                    transformador.Transform(Server.MapPath(this.PathFactura), xwo);
                    CadenaOriginal += sw.ToString();
                }
            }

            //Se crean los sellos con ayuda del xslt que contiene la cadena original
            SelloDigital oSelloDigital = new SelloDigital();
            oComprobante.Certificado = oSelloDigital.Certificado(Server.MapPath(this.CSD_Cer));
            oComprobante.Sello = oSelloDigital.Sellar(CadenaOriginal, Server.MapPath(this.CSD_Key), ClavePrivada);

            //Se crea ahora el XML sellado
            CreateXML(oComprobante, Server.MapPath(PathFacturaSellada));
            // Timbrar - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            string contenidoXML = System.IO.File.ReadAllText(Server.MapPath(PathFacturaSellada)); //Obtengo el XML sellado




            //PROVEEDOR DE TIMBRADO ATEB - - - - - - -

            //var bytes = Encoding.UTF8.GetBytes(contenidoXML);
            //var XMLbase64 = Convert.ToBase64String(bytes);


            //TimbradoAtebService ateb = new TimbradoAtebService(false);
            //var resultado = await ateb.TimbrarCFDIAsync(XMLbase64);

            //var timbreFiscal = DeserializeTimbreFiscalDigital(resultado);
            //var factura = GetXmlATEB(this.PathFacturaSellada, CadenaOriginal);
            //factura = AgregarTimbreFiscalAComplemento(factura, timbreFiscal);

            //CreateXML(factura, Server.MapPath(PathFacturaTimbrada));

            //PROVEEDOR DE TIMBRADO FACTURALO - - - - - - -
            //Realizó el timbrado con el proveedor de timbrado Facturalo

            ServicioTimbradoWS cliente = new ServicioTimbradoWS();
            RespuestaTimbrado res = cliente.timbrar(this.ApiKey, contenidoXML);

            Response.Write(res.code);
            Response.Write("<br>" + res.message);

            if (res.code == "200")
            {
                System.IO.File.WriteAllText(Server.MapPath(this.PathFacturaTimbrada), res.data);
                GetXml(this.PathFacturaTimbrada, CadenaOriginal);
            }
        }

        public static TimbreFiscalDigital DeserializeTimbreFiscalDigital(string soapResponseXml)
        {
            // Cargar el documento XML
            XmlDocument document = new XmlDocument();
            document.LoadXml(soapResponseXml);

            // Seleccionar el nodo TimbreFiscalDigital
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
            nsmgr.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");

            XmlNode timbreNode = document.SelectSingleNode("//tfd:TimbreFiscalDigital", nsmgr);

            if (timbreNode == null)
            {
                throw new InvalidOperationException("No se encontró el nodo TimbreFiscalDigital en la respuesta SOAP.");
            }

            // Deserializar el nodo
            XmlSerializer serializer = new XmlSerializer(typeof(TimbreFiscalDigital));
            using (StringReader reader = new StringReader(timbreNode.OuterXml))
            {
                return (TimbreFiscalDigital)serializer.Deserialize(reader);
            }
        }

        [Obsolete]
        private void CreatePDF(Comprobante oComprobante)
        {
            string pathHTMLTemp = Server.MapPath("~/TemporalPdf.html"); //Temporal
            string pathHtmlPlantilla = Server.MapPath("~/Plantillas/Factura.html");
            string ContentHTML = File.ReadAllText(pathHtmlPlantilla);
            //Se manda el modelo de datos a la plantilla
            string resultHtml = RazorEngine.Razor.Parse(ContentHTML, oComprobante);

            // Crear el archivo temporal
            System.IO.File.WriteAllText(pathHTMLTemp, resultHtml);
            string pathWKHTMLTOPDF = Server.MapPath("~/wkhtmltopdf/wkhtmltopdf.exe"); //Wrapper
            // Definir la ruta del archivo PDF de destino
            string pathPDF = Server.MapPath("~/PDFs/Factura.pdf");

            ProcessStartInfo oProcessStartInfo = new ProcessStartInfo();
            oProcessStartInfo.UseShellExecute = false;
            oProcessStartInfo.FileName = pathWKHTMLTOPDF;
            oProcessStartInfo.Arguments = $"\"{pathHTMLTemp}\" \"{pathPDF}\"";

            using (Process oProcess = Process.Start(oProcessStartInfo))
            {
                oProcess.WaitForExit();
            }
            // Eliminar el archivo temporal
            System.IO.File.Delete(pathHTMLTemp);
        }
        private void CreateXML(Comprobante oComprobante, string pathXML)
        {
            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();

            // Agregar los namespaces necesarios
            xmlNamespaces.Add("cfdi", "http://www.sat.gob.mx/cfd/4");
            xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

            // Serialización del Xml - - - - - - - - - - - - - - - - - - - - - - - - - 
            XmlSerializer OXmlSerializer = new XmlSerializer(typeof(Comprobante));
            // Serialización del Xml - - - - - - - - - - - - - - - - - - - - - - - - - 

            string sXml = string.Empty;

            using (var sww = new StringWriterWithEncoding(Encoding.UTF8))
            {
                using (XmlWriter writter = new XmlTextWriter(sww))
                {
                    OXmlSerializer.Serialize(writter, oComprobante, xmlNamespaces);
                    sXml = sww.ToString();
                }
            }
            System.IO.File.WriteAllText(pathXML, sXml);
        }

        [Obsolete]
        private void GetXml(string pathXML, string CadenaOriginal)
        {
            string path = Server.MapPath(pathXML);
            string xmlContent = File.ReadAllText(path);

            // Deserializar el contenido del archivo XML en un objeto
            XmlSerializer serializer = new XmlSerializer(typeof(Comprobante), "http://www.sat.gob.mx/cfd/4");
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                Comprobante oComprobante = (Comprobante)serializer.Deserialize(fileStream);
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                foreach (var oComplemento in oComprobante.Complemento.Any)
                {
                    if (!oComplemento.Name.Contains("TimbreFiscalDigital"))
                        continue;

                    XmlSerializer serializerComplemento = new XmlSerializer(typeof(TimbreFiscalDigital));
                    using (var readerComplemento = new StringReader(oComplemento.OuterXml))
                    {
                        oComprobante.TimbreFiscalDigital =
                            (TimbreFiscalDigital)serializerComplemento.Deserialize(readerComplemento);
                    }
                }
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                oComprobante.CadenaOriginal = CadenaOriginal;
                CreatePDF(oComprobante);
            }
        }
        
        private Comprobante GetXmlATEB(string pathXML, string CadenaOriginal)
        {
            string path = Server.MapPath(pathXML);
            string xmlContent = File.ReadAllText(path);

            // Deserializar el contenido del archivo XML en un objeto
            XmlSerializer serializer = new XmlSerializer(typeof(Comprobante), "http://www.sat.gob.mx/cfd/4");
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                Comprobante oComprobante = (Comprobante)serializer.Deserialize(fileStream);
                // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
                oComprobante.CadenaOriginal = CadenaOriginal;
                return oComprobante;
            }
        }
        private void TimbrarJSON(Comprobante oComprobante)
        {
            byte[] bytesJson = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(oComprobante));
            string base64 = Convert.ToBase64String(bytesJson);

            string keyFilePath = Server.MapPath("~/Docs/Pem/CSD_Sucursal_1_EKU9003173C9_key_pem.pem");
            string certFilePath = Server.MapPath("~/Docs/Pem/CSD_Sucursal_1_EKU9003173C9_cer_pem.pem");

            string keyPEMData = File.ReadAllText(keyFilePath);
            string cerPEMData = File.ReadAllText(certFilePath);

            ServicioTimbradoWS cliente = new ServicioTimbradoWS();
            RespuestaTimbrado res = cliente.timbrarJSON(this.ApiKey, base64, keyPEMData, cerPEMData);

            Response.Write(res.code);
            Response.Write("<br>" + res.message);
            Response.Write("<br>" + res.data);
        }

        [Obsolete]
        protected void btnDeserializeXML_Click(object sender, EventArgs e)
        {
            //GetXml("~/EJEMPLO XML VÁLIDO/FacturaValida.xml");
        }

        [Obsolete]
        protected void btnPDF_Click(object sender, EventArgs e)
        {
            StructureFacturaPDF pdf = new StructureFacturaPDF();
            pdf.CrearFactura(Server.MapPath("~/PDFs/FacturaPDF.pdf"));
        }

        protected void btnSearchRFC_Click(object sender, EventArgs e)
        {
            //string RFC = txtRFC.Text.Trim();

            //ServicioTimbradoWS cliente = new ServicioTimbradoWS();
            //RespuestaCreditos res = cliente.consultarCreditosDisponibles(ApiKey);

            //RespuestaTimbrado res = cliente.timbrarJSON(this.ApiKey, base64, keyPEMData, cerPEMData);
        }


        /// DATOSSSS

        private Comprobante AgregarTimbreFiscalAComplemento(Comprobante comprobante, TimbreFiscalDigital timbreFiscal)
        {
            // Crear un elemento XML para TimbreFiscalDigital
            XmlDocument doc = new XmlDocument();
            XmlElement timbreElement = doc.CreateElement("tfd", "TimbreFiscalDigital", "http://www.sat.gob.mx/TimbreFiscalDigital");

            timbreElement.SetAttribute("Version", timbreFiscal.Version);
            timbreElement.SetAttribute("UUID", timbreFiscal.UUID);
            timbreElement.SetAttribute("FechaTimbrado", timbreFiscal.FechaTimbrado.ToString("yyyy-MM-ddTHH:mm:ss"));
            timbreElement.SetAttribute("RfcProvCertif", timbreFiscal.RfcProvCertif);
            timbreElement.SetAttribute("SelloCFD", timbreFiscal.SelloCFD);
            timbreElement.SetAttribute("NoCertificadoSAT", timbreFiscal.NoCertificadoSAT);
            timbreElement.SetAttribute("SelloSAT", timbreFiscal.SelloSAT);

            // Verificar si el Complemento está inicializado
            if (comprobante.Complemento == null)
            {
                comprobante.Complemento = new ComprobanteComplemento();
            }

            // Convertir el arreglo Any a una lista temporal, agregar el timbre y volver a asignarlo como arreglo
            var elementos = comprobante.Complemento.Any?.ToList() ?? new List<XmlElement>();
            elementos.Add(timbreElement);
            comprobante.Complemento.Any = elementos.ToArray();

            return comprobante;
        }

    }
}