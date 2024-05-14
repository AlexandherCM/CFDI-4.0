using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Facturalo.Classes.FacturaPDF
{
    public enum FontType
    {
        Arial,
        ArialBlack
    }
    public class FontManager
    {
        private static string baseFontPath = AppDomain.CurrentDomain.BaseDirectory;

        // Método para obtener la fuente
        public static Font GetFont(FontType fontType, float size)
        {
            BaseFont baseFont = null;
            switch (fontType)
            {
                case FontType.Arial:
                    string arialPath = Path.Combine(baseFontPath, "Fonts/Arial.ttf");
                    baseFont = BaseFont.CreateFont(arialPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
                case FontType.ArialBlack:
                    string arialBlackPath = Path.Combine(baseFontPath, "Fonts/ariblk.ttf");
                    baseFont = BaseFont.CreateFont(arialBlackPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
            }
            // Retorna la instancia de la fuente con el tamaño especificado
            return new Font(baseFont, size);
        }
    }
}