using System;
using System.Collections.Generic;
using CFDI4._0.CFDI;
using CFDI4._0.ToolsXML;
using Newtonsoft.Json;

namespace Facturalo.Views
{
    public partial class crearPrefacturaBiblioteca : System.Web.UI.Page
    {

        private string ClavePrivada = "12345678a";

        private string CSD_Cer =
           "~/Docs/FUNK671228PH6_20230509114807/CSD_FUNK671228PH6_20230509130458/CSD_Sucursal_1_FUNK671228PH6_20230509_130451.cer";
        private string CSD_Key =
            "~/Docs/FUNK671228PH6_20230509114807/CSD_FUNK671228PH6_20230509130458/CSD_Sucursal_1_FUNK671228PH6_20230509_130451.key";

        protected async void Page_Load(object sender, EventArgs e)
        {
            OpXML opXML = new OpXML(Server.MapPath(CSD_Cer), Server.MapPath(CSD_Key), ClavePrivada);


            decimal tasaIva = 0.16m;
            decimal importe = 1500.00m;
            decimal importeIva = Math.Round(importe * tasaIva, 2);

            decimal total = importe + importeIva;

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            CFDI4._0.CFDI.Comprobante oComprobante = new CFDI4._0.CFDI.Comprobante();
            oComprobante.Version = "4.0";
            oComprobante.Serie = "H";
            oComprobante.Folio = "1";
            oComprobante.Fecha = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            //oComprobante.Sello = ""; // Pendiente
            oComprobante.FormaPago = "01";
            //oComprobante.NoCertificado = "30001000000500003416"; // Asegúrate de tener este valor asignado correctamente
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
            CFDI4._0.CFDI.ComprobanteEmisor oEmisor = new CFDI4._0.CFDI.ComprobanteEmisor
            {
                //Rfc = "CACX7605101P8", // RFC obtenido del certificado
                //Nombre = "XOCHILT CASAS CHAVEZ", // Nombre obtenido del certificado
                //RegimenFiscal = "621" // Este valor debe ser verificado y asignado según el régimen fiscal del emisor

                Rfc = "FUNK671228PH6",
                Nombre = "KARLA FUENTE NOLASCO",
                RegimenFiscal = "621"
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            CFDI4._0.CFDI.ComprobanteReceptor oReceptor = new CFDI4._0.CFDI.ComprobanteReceptor
            {
                //Rfc = "URE180429TM6",
                //Nombre = "UNIVERSIDAD ROBOTICA ESPAÑOLA",
                //DomicilioFiscalReceptor = "86991",
                //RegimenFiscalReceptor = "601",
                //UsoCFDI = "G01"

                Rfc = "CACX7605101P8",
                Nombre = "XOCHILT CASAS CHAVEZ",
                DomicilioFiscalReceptor = "01000",
                RegimenFiscalReceptor = "612",
                UsoCFDI = "G01"
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            List<CFDI4._0.CFDI.ComprobanteConcepto> listConceptos = new List<CFDI4._0.CFDI.ComprobanteConcepto>();
            CFDI4._0.CFDI.ComprobanteConcepto concepto = new CFDI4._0.CFDI.ComprobanteConcepto
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
            CFDI4._0.CFDI.ComprobanteConceptoImpuestosTraslado trasladoIVA = new CFDI4._0.CFDI.ComprobanteConceptoImpuestosTraslado
            {
                Base = concepto.Importe,
                Impuesto = "002", // IVA
                TipoFactor = "Tasa",
                TasaOCuota = 0.160000m, // Tasa de IVA del 16%
                Importe = importeIva
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            concepto.Impuestos = new CFDI4._0.CFDI.ComprobanteConceptoImpuestos
            {
                Traslados = new CFDI4._0.CFDI.ComprobanteConceptoImpuestosTraslado[] { trasladoIVA }
            };
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            listConceptos.Add(concepto);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            oComprobante.Conceptos = listConceptos.ToArray();
            oComprobante.Emisor = oEmisor;
            oComprobante.Receptor = oReceptor;

            // Finalmente, ajusta el objeto Impuestos del comprobante para incluir el total de impuestos trasladados
            oComprobante.Impuestos = new CFDI4._0.CFDI.ComprobanteImpuestos
            {
                TotalImpuestosTrasladados = importeIva,
                Traslados = new CFDI4._0.CFDI.ComprobanteImpuestosTraslado[]
                {
                    new CFDI4._0.CFDI.ComprobanteImpuestosTraslado
                    {
                        Base = oComprobante.SubTotal,
                        Impuesto = "002",
                        TipoFactor = "Tasa",
                        TasaOCuota = 0.160000m,
                        Importe = importeIva
                    }
                }
            };

            string xml = opXML.CrearXmlSellado(oComprobante);
            string base64Request = opXML.ConvertXmlToBase64(xml);   

            HttpService httpService = new HttpService();

            var jsonContent = JsonConvert.SerializeObject(base64Request);
            var resultado = await httpService.PostAsync("https://localhost:7159/api/Timbrado/ObtenerTimbreFiscal", jsonContent, false);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {

        }
    }
}