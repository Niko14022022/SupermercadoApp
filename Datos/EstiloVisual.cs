using System.Drawing;
using System.Windows.Forms;

namespace SupermercadoApp.Datos
{
    /// <summary>
    /// Métodos compartidos para que todos los módulos tengan el mismo estilo
    /// visual: tarjetas de resumen arriba, y tablas con encabezado a color.
    /// </summary>
    public static class EstiloVisual
    {
        public static readonly Color ColorEncabezado = Color.FromArgb(0, 150, 136);
        public static readonly Color ColorFilaAlterna = Color.FromArgb(240, 247, 246);

        public static Panel CrearTarjeta(int x, string titulo, string valor, Color color)
        {
            var tarjeta = new Panel { Location = new Point(x, 10), Size = new Size(260, 70), BackColor = color };
            var lblTitulo = new Label { Text = titulo, ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(15, 9), AutoSize = true, BackColor = Color.Transparent };
            var lblValor = new Label { Text = valor, ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(15, 29), AutoSize = true, BackColor = Color.Transparent };
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblValor);
            return tarjeta;
        }

        public static void EstilizarGrid(DataGridView grid)
        {
            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.None;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.GridColor = Color.FromArgb(230, 230, 230);
            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorEncabezado;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 223, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = ColorFilaAlterna;
            grid.RowTemplate.Height = 28;
        }
    }
}
