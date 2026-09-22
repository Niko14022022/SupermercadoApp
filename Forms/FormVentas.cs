using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SupermercadoApp.Datos;
using SupermercadoApp.Modelos;

namespace SupermercadoApp.Forms
{
    public class FormVentas : Form
    {
        private TextBox txtBuscarProducto, txtCantidad, txtCodigoBarra;
        private ComboBox cmbCliente, cmbMetodoPago, cmbCategoriaFiltro;
        private Button btnAgregarSeleccionado, btnQuitar, btnFinalizarVenta, btnReportes, btnLimpiar, btnEscanear;
        private DataGridView gridCatalogo, gridCarrito;
        private Label lblTotal;

        private readonly List<DetalleVenta> carrito = new List<DetalleVenta>();
        private DataTable tablaProductos;

        public FormVentas()
        {
            InicializarComponentes();
            CargarClientes();
            CargarCatalogoProductos();
            this.Shown += (s, e) => txtCodigoBarra.Focus();
        }

        private void InicializarComponentes()
        {
            this.Text = "Punto de Venta (POS)";
            this.Size = new Size(1150, 650);

            // ---- Barra superior: cliente y método de pago ----
            var panelSup = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(236, 240, 241) };

            var lblCliente = new Label { Text = "Cliente:", Location = new Point(20, 18), AutoSize = true };
            cmbCliente = new ComboBox { Location = new Point(90, 15), Size = new Size(220, 25), DropDownStyle = ComboBoxStyle.DropDownList };

            var lblMetodo = new Label { Text = "Método de pago:", Location = new Point(330, 18), AutoSize = true };
            cmbMetodoPago = new ComboBox { Location = new Point(450, 15), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMetodoPago.Items.AddRange(new object[] { "Efectivo", "Tarjeta", "SINPE Móvil" });
            cmbMetodoPago.SelectedIndex = 0;

            btnReportes = new Button { Text = "📊 Reportes", Location = new Point(870, 12), Size = new Size(120, 30), BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnReportes.FlatAppearance.BorderSize = 0;
            btnReportes.Click += (s, e) => new FormReportes().Show();

            btnLimpiar = new Button { Text = "🧹 Limpiar", Location = new Point(1000, 12), Size = new Size(110, 30), BackColor = Color.FromArgb(149, 165, 166), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnLimpiar.FlatAppearance.BorderSize = 0;
            btnLimpiar.Click += BtnLimpiar_Click;

            panelSup.Controls.AddRange(new Control[] { lblCliente, cmbCliente, lblMetodo, cmbMetodoPago, btnReportes, btnLimpiar });

            // ---- Panel izquierdo: catálogo de productos para elegir ----
            var panelCatalogo = new Panel { Dock = DockStyle.Left, Width = 520, Padding = new Padding(10) };

            var lblCatalogo = new Label { Text = "🛍️ Productos disponibles", Font = new Font("Segoe UI", 11, FontStyle.Bold), Location = new Point(10, 8), AutoSize = true };

            var lblBuscar = new Label { Text = "Buscar:", Location = new Point(10, 38), AutoSize = true };
            txtBuscarProducto = new TextBox { Location = new Point(70, 35), Size = new Size(180, 25), AutoCompleteMode = AutoCompleteMode.SuggestAppend, AutoCompleteSource = AutoCompleteSource.CustomSource };
            txtBuscarProducto.TextChanged += (s, e) => FiltrarCatalogo();

            var lblCategoriaFiltro = new Label { Text = "Categoría:", Location = new Point(265, 38), AutoSize = true };
            cmbCategoriaFiltro = new ComboBox { Location = new Point(335, 35), Size = new Size(165, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategoriaFiltro.SelectedIndexChanged += (s, e) => FiltrarCatalogo();

            gridCatalogo = new DataGridView
            {
                Location = new Point(10, 65),
                Size = new Size(490, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                MultiSelect = false
            };
            gridCatalogo.CellDoubleClick += (s, e) => AgregarProductoSeleccionadoDelCatalogo();
            EstiloVisual.EstilizarGrid(gridCatalogo);

            var lblCantidad = new Label { Text = "Cantidad:", Location = new Point(10, 475), AutoSize = true };
            txtCantidad = new TextBox { Location = new Point(80, 472), Size = new Size(60, 25), Text = "1" };

            btnAgregarSeleccionado = new Button
            {
                Text = "➕ Agregar al carrito",
                Location = new Point(160, 470),
                Size = new Size(180, 30),
                BackColor = Color.FromArgb(41, 128, 185),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAgregarSeleccionado.FlatAppearance.BorderSize = 0;
            btnAgregarSeleccionado.Click += (s, e) => AgregarProductoSeleccionadoDelCatalogo();

            // ---- Escanear código de barras ----
            var panelEscanear = new Panel { Location = new Point(10, 510), Size = new Size(490, 60), BackColor = Color.FromArgb(236, 240, 241) };
            var lblEscanear = new Label { Text = "📷 Escanear código", Font = new Font("Segoe UI", 9, FontStyle.Bold), Location = new Point(10, 8), AutoSize = true };
            txtCodigoBarra = new TextBox { Location = new Point(10, 30), Size = new Size(260, 26), Font = new Font("Segoe UI", 11) };
            txtCodigoBarra.KeyDown += TxtCodigoBarra_KeyDown;

            btnEscanear = new Button
            {
                Text = "📷 Escanear",
                Location = new Point(280, 28),
                Size = new Size(200, 30),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEscanear.FlatAppearance.BorderSize = 0;
            btnEscanear.Click += (s, e) => { txtCodigoBarra.Focus(); txtCodigoBarra.SelectAll(); };

            panelEscanear.Controls.Add(lblEscanear);
            panelEscanear.Controls.Add(txtCodigoBarra);
            panelEscanear.Controls.Add(btnEscanear);

            panelCatalogo.Controls.Add(lblCatalogo);
            panelCatalogo.Controls.Add(lblBuscar);
            panelCatalogo.Controls.Add(txtBuscarProducto);
            panelCatalogo.Controls.Add(lblCategoriaFiltro);
            panelCatalogo.Controls.Add(cmbCategoriaFiltro);
            panelCatalogo.Controls.Add(gridCatalogo);
            panelCatalogo.Controls.Add(lblCantidad);
            panelCatalogo.Controls.Add(txtCantidad);
            panelCatalogo.Controls.Add(btnAgregarSeleccionado);
            panelCatalogo.Controls.Add(panelEscanear);

            // ---- Panel derecho: carrito de la venta actual ----
            var lblCarrito = new Label { Text = "🧾 Carrito de la venta", Font = new Font("Segoe UI", 11, FontStyle.Bold), Dock = DockStyle.Top, Height = 30, Padding = new Padding(10, 8, 0, 0) };

            btnQuitar = new Button { Text = "✖ Quitar seleccionado", Dock = DockStyle.Top, Height = 32, BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnQuitar.FlatAppearance.BorderSize = 0;
            btnQuitar.Click += BtnQuitar_Click;

            gridCarrito = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            EstiloVisual.EstilizarGrid(gridCarrito);

            var panelInf = new Panel { Dock = DockStyle.Bottom, Height = 70 };

            // Panel acoplado a la derecha para el botón: evita el bug de anclar
            // con Anchor=Right cuando el panel padre todavía no tiene su tamaño final.
            var panelBotonFinalizar = new Panel { Dock = DockStyle.Right, Width = 220 };
            btnFinalizarVenta = new Button
            {
                Text = "✅ Finalizar Venta",
                Location = new Point(20, 15),
                Size = new Size(190, 40),
                BackColor = Color.FromArgb(0, 153, 76),
                ForeColor = Color.White
            };
            panelBotonFinalizar.Controls.Add(btnFinalizarVenta);

            lblTotal = new Label
            {
                Text = "Total: ₡0.00",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 22),
                AutoSize = true
            };
            btnFinalizarVenta.Click += BtnFinalizarVenta_Click;
            panelInf.Controls.Add(lblTotal);
            panelInf.Controls.Add(panelBotonFinalizar);

            var panelDerecho = new Panel { Dock = DockStyle.Fill };
            panelDerecho.Controls.Add(gridCarrito);
            panelDerecho.Controls.Add(panelInf);
            panelDerecho.Controls.Add(btnQuitar);
            panelDerecho.Controls.Add(lblCarrito);

            this.Controls.Add(panelDerecho);
            this.Controls.Add(panelCatalogo);
            this.Controls.Add(panelSup);
        }

        private void CargarClientes()
        {
            cmbCliente.Items.Clear();
            cmbCliente.Items.Add("Consumidor Final (Id=0)");
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Id, Nombre FROM Clientes ORDER BY Nombre", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            cmbCliente.Items.Add($"{reader["Nombre"]} (Id={reader["Id"]})");
                    }
                }
            }
            catch
            {
                // Si no hay conexión, se sigue con "Consumidor Final"
            }
            cmbCliente.SelectedIndex = 0;
        }

        private void CargarCatalogoProductos()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = "SELECT Id, Codigo, Nombre, Categoria, PrecioVenta, Existencias FROM Productos ORDER BY Nombre";
                    var adapter = new SqlDataAdapter(sql, conn);
                    tablaProductos = new DataTable();
                    adapter.Fill(tablaProductos);
                    gridCatalogo.DataSource = tablaProductos;
                    if (gridCatalogo.Columns["Id"] != null) gridCatalogo.Columns["Id"].Visible = false;

                    var nombres = new AutoCompleteStringCollection();
                    var categorias = new List<string> { "(Todas)" };
                    foreach (DataRow fila in tablaProductos.Rows)
                    {
                        nombres.Add(fila["Nombre"].ToString());
                        string cat = fila["Categoria"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(cat) && !categorias.Contains(cat))
                            categorias.Add(cat);
                    }
                    txtBuscarProducto.AutoCompleteCustomSource = nombres;

                    string categoriaPrevia = cmbCategoriaFiltro.SelectedItem?.ToString();
                    cmbCategoriaFiltro.Items.Clear();
                    cmbCategoriaFiltro.Items.AddRange(categorias.ToArray());
                    int indicePrevio = categoriaPrevia != null ? cmbCategoriaFiltro.Items.IndexOf(categoriaPrevia) : -1;
                    cmbCategoriaFiltro.SelectedIndex = indicePrevio >= 0 ? indicePrevio : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar el catálogo de productos. Verifique la conexión a la base de datos.\nDetalle: " + ex.Message);
            }
        }

        private void FiltrarCatalogo()
        {
            if (tablaProductos == null) return;
            string texto = txtBuscarProducto.Text.Trim().Replace("'", "''");
            string categoria = cmbCategoriaFiltro.SelectedItem?.ToString();

            var condiciones = new List<string>();
            if (!string.IsNullOrEmpty(texto))
                condiciones.Add($"(Codigo LIKE '%{texto}%' OR Nombre LIKE '%{texto}%')");
            if (!string.IsNullOrEmpty(categoria) && categoria != "(Todas)")
                condiciones.Add($"Categoria = '{categoria.Replace("'", "''")}'");

            tablaProductos.DefaultView.RowFilter = condiciones.Count == 0 ? "" : string.Join(" AND ", condiciones);
        }

        private void AgregarProductoSeleccionadoDelCatalogo()
        {
            if (gridCatalogo.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un producto de la lista.");
                return;
            }
            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida.");
                return;
            }

            var fila = gridCatalogo.CurrentRow;
            int productoId = Convert.ToInt32(fila.Cells["Id"].Value);
            string nombre = fila.Cells["Nombre"].Value.ToString();
            decimal precio = Convert.ToDecimal(fila.Cells["PrecioVenta"].Value);
            int existencias = Convert.ToInt32(fila.Cells["Existencias"].Value);

            var enCarrito = carrito.Find(d => d.ProductoId == productoId);
            int yaEnCarrito = enCarrito?.Cantidad ?? 0;
            int disponibleReal = existencias - yaEnCarrito;

            if (cantidad > disponibleReal)
            {
                MessageBox.Show($"Existencias insuficientes. Disponible: {disponibleReal}" +
                    (yaEnCarrito > 0 ? $" (ya tiene {yaEnCarrito} de este producto en el carrito)." : "."));
                return;
            }

            if (enCarrito != null)
                enCarrito.Cantidad += cantidad;
            else
                carrito.Add(new DetalleVenta
                {
                    ProductoId = productoId,
                    NombreProducto = nombre,
                    Cantidad = cantidad,
                    PrecioUnitario = precio
                });

            ActualizarCarrito();
            txtCantidad.Text = "1";
        }

        private void TxtCodigoBarra_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            EscanearYAgregar(txtCodigoBarra.Text.Trim());
            txtCodigoBarra.Clear();
            txtCodigoBarra.Focus();
        }

        private void EscanearYAgregar(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return;

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Id, Nombre, PrecioVenta, Existencias FROM Productos WHERE Codigo=@codigo", conn);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            MessageBox.Show($"No se encontró ningún producto con el código '{codigo}'.", "Código no encontrado",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        int existencias = Convert.ToInt32(reader["Existencias"]);
                        int productoId = Convert.ToInt32(reader["Id"]);
                        string nombre = reader["Nombre"].ToString();
                        decimal precio = Convert.ToDecimal(reader["PrecioVenta"]);

                        var existente = carrito.Find(d => d.ProductoId == productoId);
                        int yaEnCarrito = existente?.Cantidad ?? 0;

                        if (yaEnCarrito + 1 > existencias)
                        {
                            MessageBox.Show($"No hay suficientes existencias de '{nombre}'. Disponible: {existencias}" +
                                (yaEnCarrito > 0 ? $" (ya tiene {yaEnCarrito} en el carrito)." : "."),
                                "Sin stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Si el producto ya está en el carrito, se le suma 1 en vez de duplicar la línea.
                        if (existente != null)
                            existente.Cantidad += 1;
                        else
                            carrito.Add(new DetalleVenta { ProductoId = productoId, NombreProducto = nombre, Cantidad = 1, PrecioUnitario = precio });
                    }
                }

                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al escanear el producto: " + ex.Message);
            }
        }

        private void BtnQuitar_Click(object sender, EventArgs e)
        {
            if (gridCarrito.CurrentRow == null) return;
            int indice = gridCarrito.CurrentRow.Index;
            if (indice >= 0 && indice < carrito.Count)
            {
                carrito.RemoveAt(indice);
                ActualizarCarrito();
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0) return;
            if (MessageBox.Show("¿Vaciar el carrito actual?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;
            carrito.Clear();
            ActualizarCarrito();
            txtBuscarProducto.Clear();
        }

        private void ActualizarCarrito()
        {
            gridCarrito.DataSource = null;
            gridCarrito.DataSource = carrito.Select(d => new
            {
                Producto = d.NombreProducto,
                d.Cantidad,
                PrecioUnitario = d.PrecioUnitario.ToString("C2"),
                Subtotal = d.Subtotal.ToString("C2")
            }).ToList();

            decimal total = carrito.Sum(d => d.Subtotal);
            lblTotal.Text = $"Total: {total:C2}";
        }

        private void BtnFinalizarVenta_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("Agregue al menos un producto al carrito.");
                return;
            }

            int clienteId = 0;
            var seleccion = cmbCliente.SelectedItem?.ToString() ?? "";
            var idxIni = seleccion.LastIndexOf("Id=");
            if (idxIni >= 0)
                int.TryParse(seleccion.Substring(idxIni + 3).TrimEnd(')'), out clienteId);

            decimal total = carrito.Sum(d => d.Subtotal);

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var transaccion = conn.BeginTransaction())
                    {
                        var cmdVenta = new SqlCommand(@"INSERT INTO Ventas (Fecha, ClienteId, UsuarioId, Total, MetodoPago)
                            OUTPUT INSERTED.Id
                            VALUES (@fecha, @cliente, @usuario, @total, @metodo)", conn, transaccion);
                        cmdVenta.Parameters.AddWithValue("@fecha", DateTime.Now);
                        cmdVenta.Parameters.AddWithValue("@cliente", clienteId == 0 ? (object)DBNull.Value : clienteId);
                        cmdVenta.Parameters.AddWithValue("@usuario", Sesion.UsuarioId == 0 ? 1 : Sesion.UsuarioId);
                        cmdVenta.Parameters.AddWithValue("@total", total);
                        cmdVenta.Parameters.AddWithValue("@metodo", cmbMetodoPago.SelectedItem.ToString());

                        int ventaId = (int)cmdVenta.ExecuteScalar();

                        foreach (var item in carrito)
                        {
                            var cmdDetalle = new SqlCommand(@"INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario)
                                VALUES (@venta, @producto, @cantidad, @precio)", conn, transaccion);
                            cmdDetalle.Parameters.AddWithValue("@venta", ventaId);
                            cmdDetalle.Parameters.AddWithValue("@producto", item.ProductoId);
                            cmdDetalle.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            cmdDetalle.Parameters.AddWithValue("@precio", item.PrecioUnitario);
                            cmdDetalle.ExecuteNonQuery();

                            var cmdStock = new SqlCommand("UPDATE Productos SET Existencias = Existencias - @cantidad WHERE Id=@id", conn, transaccion);
                            cmdStock.Parameters.AddWithValue("@cantidad", item.Cantidad);
                            cmdStock.Parameters.AddWithValue("@id", item.ProductoId);
                            cmdStock.ExecuteNonQuery();

                            var cmdMovimiento = new SqlCommand(@"INSERT INTO MovimientoInventario (ProductoId, Fecha, Cantidad, Motivo, UsuarioId)
                                VALUES (@producto, @fecha, @cantidad, 'Venta', @usuario)", conn, transaccion);
                            cmdMovimiento.Parameters.AddWithValue("@producto", item.ProductoId);
                            cmdMovimiento.Parameters.AddWithValue("@fecha", DateTime.Now);
                            cmdMovimiento.Parameters.AddWithValue("@cantidad", -item.Cantidad); // negativo = salida
                            cmdMovimiento.Parameters.AddWithValue("@usuario", Sesion.UsuarioId == 0 ? 1 : Sesion.UsuarioId);
                            cmdMovimiento.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        ultimaVentaId = ventaId;
                    }
                }

                Auditoria.Registrar(Sesion.UsuarioId, "Registrar venta", $"Venta #{ultimaVentaId} por un total de {total:C2}.");

                if (MessageBox.Show("Venta registrada con éxito.\n\n¿Desea imprimir el comprobante?", "Venta exitosa",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    ImprimirComprobante(ultimaVentaId, total);
                }

                carrito.Clear();
                ActualizarCarrito();
                CargarCatalogoProductos(); // refresca existencias mostradas
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar la venta: " + ex.Message);
            }
        }

        private int ultimaVentaId = 0;

        private void ImprimirComprobante(int ventaId, decimal total)
        {
            var lineas = new List<string>();
            lineas.Add("SUPERMERCADO");
            lineas.Add("Comprobante de Venta");
            lineas.Add($"No. Venta: {ventaId}");
            lineas.Add($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}");
            lineas.Add($"Atendido por: {Sesion.NombreUsuario}");
            lineas.Add(new string('-', 40));
            foreach (var item in carrito)
                lineas.Add($"{item.NombreProducto,-20} x{item.Cantidad,-3} {item.Subtotal:C2}");
            lineas.Add(new string('-', 40));
            lineas.Add($"TOTAL: {total:C2}");
            lineas.Add($"Método de pago: {cmbMetodoPago.SelectedItem}");
            lineas.Add("¡Gracias por su compra!");

            var documento = new System.Drawing.Printing.PrintDocument();
            int lineaActual = 0;
            documento.PrintPage += (s, e) =>
            {
                float y = 20;
                var fuente = new Font("Consolas", 10);
                foreach (var linea in lineas)
                {
                    e.Graphics.DrawString(linea, fuente, Brushes.Black, 20, y);
                    y += 18;
                }
            };

            try
            {
                using (var vistaPrevia = new PrintPreviewDialog { Document = documento, Width = 500, Height = 650 })
                {
                    vistaPrevia.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir la vista de impresión: " + ex.Message);
            }
        }
    }
}
