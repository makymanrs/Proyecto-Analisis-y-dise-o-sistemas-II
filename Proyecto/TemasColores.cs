using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto
{
    internal class TemasColores
    {
        public static Color PrimaryColor { get; set; }
        public static Color SecondaryColor { get; set; }
        public static List<string> listaColores = new List<string>() {
            "#3F51B5", // Índigo
            "#009688", // Verde Azulado
            "#FF5722", // Naranja Profundo
            "#607D8B", // Gris Azulado
            "#FF9800", // Naranja
            "#9C27B0", // Púrpura
            "#2196F3", // Azul
            "#EA676C", // Coral Claro
            "#E41A4A", // Carmesí
            "#5978BB", // Azul Yonder
            "#018790", // Laguna Azul
            "#0E3441", // Gris Pizarra Oscuro
            "#00B0AD", // Verde Azulado Claro
            "#721D47", // Frambuesa Oscura
            "#EA4833", // Naranja Rojo
            "#EF937E", // Salmón Claro
            "#F37521", // Naranja Zanahoria
            "#A12059", // Frambuesa Oscura
            "#126881", // Cerúleo Profundo
          //  "#8BC240", // Verde Amarillo
            "#364D5B", // Carbón
            "#C7DC5B", // Verde Amarillo
            "#0094BC", // Azul Pacífico
            "#E4126B", // Magenta
            "#43B76E", // Verde Mar Medio
            "#7BCFE9", // Azul Cielo
            "#B71C46"  // Granate
        };

        public static Color CambiarBrillo(Color color, double correctionFactor)
        {
            double red = color.R;
            double green = color.G;
            double blue = color.B;
            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                // Se calcula el nuevo valor de cada componente de color para aclarar el color
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
