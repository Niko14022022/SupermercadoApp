using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormReportes : Form
    {
        private ComboBox cmbTipoReporte;
        private DateTimePicker dtpDesde, dtpHasta;
        private Button btnGenerar, btnExportar;
        private DataGridView grid;
        private Panel panelFilaFechas;
        private Panel panelTarjetas;
        private Label lblFilaVacia;

        private static readonly Color ColorEncabezado = Color.FromArgb(0, 150, 136);
        private static readonly Color ColorFilaAlterna = Color.FromArgb(240, 247, 246);
        private static readonly Color ColorAlerta = Color.FromArgb(255, 205, 210);
        private static readonly Color ColorAlertaTexto = Color.FromArgb(183, 28, 28);

        public FormReportes()
        {
            InicializarComponentes();
            this.Shown += (s, e) => GenerarReporte();
        }

        private void InicializarComponentes()
        {
            this.Text = "Módulo de Reportes";
            this.Size = new Size(1050, 680);
            this.BackColor = Color.White;

            // ---- Fila 1: tipo de reporte + botones ----
            var panelFila1 = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(236, 240, 241) };
            var lbl = new Label { Text = "📊 Reporte:", Location = new Point(20, 17), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            cmbTipoReporte = new ComboBox { Location = new Point(110, 13), Size = new Size(300, 26), Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbTipoReporte.Items.AddRange(new object[]
            {
                "Ventas por rango de fechas",
                "Productos con bajo stock",
                "Productos más vendidos",
                "Historial de movimientos de inventario",
                "Bitácora de auditoría"
            });
            cmbTipoReporte.SelectedIndex = 0;
            cmbTipoReporte.SelectedIndexChanged += (s, e) => { ActualizarVisibilidadFechas(); GenerarReporte(); };

            btnGenerar = new Button { Text = "🔄 Generar", Location = new Point(430, 12), Size = new Size(120, 28), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnGenerar.FlatAppearance.BorderSize = 0;
            btnGenerar.Click += (s, e) => GenerarReporte();

            btnExportar = new Button { Text = "⬇️ Exportar a CSV", Location = new Point(560, 12), Size = new Size(160, 28), BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnExportar.FlatAppearance.BorderSize = 0;
            btnExportar.Click += BtnExportar_Click;

            panelFila1.Controls.Add(lbl);
            panelFila1.Controls.Add(cmbTipoReporte);
            panelFila1.Controls.Add(btnGenerar);
            panelFila1.Controls.Add(btnExportar);

            // ---- Fila 2: filtro de fechas (solo aplica a "Ventas por rango de fechas") ----
            panelFilaFechas = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = Color.FromArgb(220, 237, 235) };
            var lblDesde = new Label { Text = "Desde:", Location = new Point(20, 14), AutoSize = true };
            dtpDesde = new DateTimePicker { Location = new Point(70, 10), Size = new Size(140, 25), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(-7) };
            dtpDesde.ValueChanged += (s, e) => GenerarReporte();
            var lblHasta = new Label { Text = "Hasta:", Location = new Point(230, 14), AutoSize = true };
            dtpHasta = new DateTimePicker { Location = new Point(280, 10), Size = new Size(140, 25), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
            dtpHasta.ValueChanged += (s, e) => GenerarReporte();
            panelFilaFechas.Controls.Add(lblDesde);
            panelFilaFechas.Controls.Add(dtpDesde);
            panelFilaFechas.Controls.Add(lblHasta);
            panelFilaFechas.Controls.Add(dtpHasta);

            // ---- Fila 3: tarjetas de resumen (cambian según el reporte) ----
            panelTarjetas = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.White, Padding = new Padding(20, 10, 20, 10) };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                GridColor = Color.FromArgb(230, 230, 230)
            };
            grid.ColumnHeadersDefaultCellStyle.BackColor = ColorEncabezado;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            grid.ColumnHeadersHeight = 34;
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(178, 223, 219);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = ColorFilaAlterna;
            grid.RowTemplate.Height = 28;
            grid.CellFormatting += Grid_CellFormatting;

            lblFilaVacia = new Label
            {
                Text = "No hay datos para este reporte todavía.\nProbá registrar una venta o ajustar el rango de fechas.",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray,
                Visible = false
            };

            this.Controls.Add(grid);
            this.Controls.Add(lblFilaVacia);
            this.Controls.Add(panelTarjetas);
            this.Controls.Add(panelFilaFechas);
            this.Controls.Add(panelFila1);

            ActualizarVisibilidadFechas();
        }

        private void ActualizarVisibilidadFechas()
        {
            panelFilaFechas.Visible = cmbTipoReporte.SelectedItem?.ToString() == "Ventas por rango de fechas";
        }

        private string ObtenerSql()
        {
            switch (cmbTipoReporte.SelectedItem.ToString())
            {
                case "Ventas por rango de fechas":
                    return @"SELECT v.Id, v.Fecha, c.Nombre AS Cliente, u.NombreUsuario AS Vendedor, v.Total, v.MetodoPago
                            FROM Ventas v
                            LEFT JOIN Clientes c ON v.ClienteId = c.Id
                            LEFT JOIN Usuarios u ON v.UsuarioId = u.Id
                            WHERE v.Fecha >= @desde AND v.Fecha < @hasta
                            ORDER BY v.Fecha DESC";
                case "Productos con bajo stock":
                    return @"SELECT Codigo, Nombre, Categoria, Existencias, StockMinimo
                            FROM Productos
                            WHERE Existencias <= StockMinimo
                            ORDER BY Existencias ASC";
                case "Historial de movimientos de inventario":
                    return @"SELECT m.Fecha, p.Codigo, p.Nombre AS Producto, m.Cantidad, m.Motivo, u.NombreUsuario AS Usuario
                            FROM MovimientoInventario m
                            JOIN Productos p ON m.ProductoId = p.Id
                            LEFT JOIN Usuarios u ON m.UsuarioId = u.Id
                            ORDER BY m.Fecha DESC";
                case "Bitácora de auditoría":
                    return @"SELECT b.Fecha, u.NombreUsuario AS Usuario, b.Accion, b.Detalle
                            FROM Bitacora b
                            LEFT JOIN Usuarios u ON b.UsuarioId = u.Id
                            ORDER BY b.Fecha DESC";
                default: // Productos más vendidos
                    return @"SELECT p.Nombre, SUM(d.Cantidad) AS UnidadesVendidas, SUM(d.Cantidad * d.PrecioUnitario) AS TotalGenerado
                            FROM DetalleVenta d
                            JOIN Productos p ON d.ProductoId = p.Id
                            GROUP BY p.Nombre
                            ORDER BY UnidadesVendidas DESC";
            }
        }

        private void GenerarReporte()
        {
            string sql = ObtenerSql();
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter(sql, conn);
                    if (cmbTipoReporte.SelectedItem.ToString() == "Ventas por rango de fechas")
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@desde", dtpDesde.Value.Date);
                        adapter.SelectCommand.Parameters.AddWithValue("@hasta", dtpHasta.Value.Date.AddDays(1));
                    }
                    var tabla = new DataTable();
                    adapter.Fill(tabla);
                    grid.DataSource = tabla;

                    bool sinDatos = tabla.Rows.Count == 0;
                    lblFilaVacia.Visible = sinDatos;
                    grid.Visible = !sinDatos;

                    ActualizarTarjetas(tabla);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar el reporte. Verifique la conexión a la base de datos.\nDetalle: " + ex.Message);
            }
        }

        private void ActualizarTarjetas(DataTable tabla)
        {
            panelTarjetas.Controls.Clear();
            string tipo = cmbTipoReporte.SelectedItem.ToString();
            int x = 0;

            switch (tipo)
            {
                case "Ventas por rango de fechas":
                    decimal total = 0;
                    foreach (DataRow fila in tabla.Rows) total += Convert.ToDecimal(fila["Total"]);
                    AgregarTarjeta(ref x, "💰 Total vendido", total.ToString("C2"), Color.FromArgb(0, 150, 136));
                    AgregarTarjeta(ref x, "🧾 Cantidad de ventas", tabla.Rows.Count.ToString(), Color.FromArgb(41, 128, 185));
                    AgregarTarjeta(ref x, "📈 Promedio por venta", tabla.Rows.Count > 0 ? (total / tabla.Rows.Count).ToString("C2") : "₡0.00", Color.FromArgb(230, 126, 34));
                    break;
                case "Productos con bajo stock":
                    AgregarTarjeta(ref x, "⚠️ Productos bajo mínimo", tabla.Rows.Count.ToString(), Color.FromArgb(192, 57, 43));
                    int agotados = 0;
                    foreach (DataRow fila in tabla.Rows) if (Convert.ToInt32(fila["Existencias"]) <= 0) agotados++;
                    AgregarTarjeta(ref x, "🚫 Agotados (0 o menos)", agotados.ToString(), Color.FromArgb(120, 40, 31));
                    break;
                case "Productos más vendidos":
                    AgregarTarjeta(ref x, "🏆 Productos distintos vendidos", tabla.Rows.Count.ToString(), Color.FromArgb(0, 150, 136));
                    if (tabla.Rows.Count > 0)
                        AgregarTarjeta(ref x, "🥇 Más vendido", tabla.Rows[0]["Nombre"].ToString(), Color.FromArgb(230, 126, 34));
                    break;
                case "Historial de movimientos de inventario":
                    int entradas = 0, salidas = 0;
                    foreach (DataRow fila in tabla.Rows)
                    {
                        int cant = Convert.ToInt32(fila["Cantidad"]);
                        if (cant > 0) entradas += cant; else salidas += -cant;
                    }
                    AgregarTarjeta(ref x, "⬆️ Unidades ingresadas", entradas.ToString(), Color.FromArgb(39, 174, 96));
                    AgregarTarjeta(ref x, "⬇️ Unidades vendidas/salidas", salidas.ToString(), Color.FromArgb(192, 57, 43));
                    break;
                case "Bitácora de auditoría":
                    AgregarTarjeta(ref x, "📋 Acciones registradas", tabla.Rows.Count.ToString(), Color.FromArgb(0, 150, 136));
                    break;
            }
        }

        private void AgregarTarjeta(ref int x, string titulo, string valor, Color color)
        {
            var tarjeta = new Panel { Location = new Point(x, 10), Size = new Size(280, 75), BackColor = color };
            var lblTitulo = new Label { Text = titulo, ForeColor = Color.White, Font = new Font("Segoe UI", 9), Location = new Point(15, 10), AutoSize = true, BackColor = Color.Transparent };
            var lblValor = new Label { Text = valor, ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(15, 32), AutoSize = true, BackColor = Color.Transparent };
            tarjeta.Controls.Add(lblTitulo);
            tarjeta.Controls.Add(lblValor);
            panelTarjetas.Controls.Add(tarjeta);
            x += 300;
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (grid.Columns.Count == 0 || e.RowIndex < 0) return;
            string tipo = cmbTipoReporte.SelectedItem?.ToString();
            string nombreColumna = grid.Columns[e.ColumnIndex].Name;

            // Resalta en rojo las filas de productos agotados o con existencias muy bajas.
            if (tipo == "Productos con bajo stock" && nombreColumna == "Existencias" && e.Value != null)
            {
                if (int.TryParse(e.Value.ToString(), out int existencias) && existencias <= 0)
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = ColorAlerta;
                    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = ColorAlertaTexto;
                    grid.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(grid.Font, FontStyle.Bold);
                }
            }

            // Formatea columnas de dinero
            if ((nombreColumna == "Total" || nombreColumna == "TotalGenerado") && e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal monto))
            {
                e.Value = monto.ToString("C2");
                e.FormattingApplied = true;
            }
        }

        private void BtnExportar_Click(object sender, EventArgs e)
        {
            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("Primero genere un reporte con datos para poder exportarlo.");
                return;
            }

            using (var guardar = new SaveFileDialog { Filter = "Archivo CSV (*.csv)|*.csv", FileName = "reporte.csv" })
            {
                if (guardar.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();
                    var columnas = new string[grid.Columns.Count];
                    for (int i = 0; i < grid.Columns.Count; i++) columnas[i] = grid.Columns[i].HeaderText;
                    sb.AppendLine(string.Join(",", columnas));

                    foreach (DataGridViewRow fila in grid.Rows)
                    {
                        if (fila.IsNewRow) continue;
                        var valores = new string[grid.Columns.Count];
                        for (int i = 0; i < grid.Columns.Count; i++)
                        {
                            string valor = fila.Cells[i].Value?.ToString() ?? "";
                            valores[i] = "\"" + valor.Replace("\"", "\"\"") + "\"";
                        }
                        sb.AppendLine(string.Join(",", valores));
                    }

                    File.WriteAllText(guardar.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Reporte exportado correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }
    }
}
