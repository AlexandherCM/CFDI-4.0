using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.UI.WebControls;
using Image = iTextSharp.text.Image;
using System;

namespace Facturalo.Classes.FacturaPDF
{
    public class StructureFacturaPDF
    {
        public void CrearFactura(string path)
        {
            // Definir los márgenes (ejemplo, puedes ajustarlos como necesites)
            float marginY = 0.5f * 28.35f;
            float marginX = 0.5f * 28.35f;
            // Propiedades del PDF
            Document document = new Document(PageSize.LETTER, marginX, marginY, marginX, marginY);
            PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
            document.Open();

            // Especificar la fuentes
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
            Font Arial = FontManager.GetFont(FontType.Arial, 10);
            Font ArialBlack = FontManager.GetFont(FontType.ArialBlack, 10);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

            Paragraph paragraph = new Paragraph("Alexandher Cordoba Molina".ToUpper(), ArialBlack);
            paragraph.SpacingAfter = 5f; // Espacio en puntos
            document.Add(paragraph);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 

            PdfPTable GralData = RowGralData(Arial, 1, 2);
            document.Add(GralData);

            document.Close();
        }

        // DATOS DEL EMISOR - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
        private PdfPTable RowGralData(Font font, int Rows, int Cols)
        {
            PdfPTable table = new PdfPTable(Cols); // El número indica la cantidad de columnas
            table.WidthPercentage = 99; // 100% el ancho

            //Imagen del espacio de una foto de usuario - - - - - - - - - - - - - - - - - - 
            string fileName = "img/Captura de pantalla 2024-03-26 093608.png";
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string pathImage = Path.Combine(basePath, fileName);

            Image image = Image.GetInstance(pathImage);
            image.ScaleToFit(80f, 80f);

            PdfPCell imageCell = new PdfPCell(image);
            imageCell.Border = PdfPCell.NO_BORDER; // Opcional: eliminar el borde
            table.AddCell(imageCell);
            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            // Crear la tabla anidada
            PdfPTable datosFiscales = new PdfPTable(1); // Aquí puedes ajustar el número de columnas de la tabla anidada
            datosFiscales.AddCell("Información en tabla anidada");
            datosFiscales.AddCell("Información en tabla anidada");
            datosFiscales.AddCell("Información en tabla anidada");
            datosFiscales.AddCell("Información en tabla anidada");
            datosFiscales.AddCell("Información en tabla anidada");

            // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
            PdfPCell cell = new PdfPCell(new Phrase($"Celda 2", font));
            cell.Border = PdfPCell.NO_BORDER;
            table.AddCell(cell);
            return table;
        }
        // DATOS DEL EMISOR - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - 
    }
}