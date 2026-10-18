using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormProductos : Form
    {
        private DataGridView grid;
        private Panel panelTarjetas;
        private TextBox txtCodigo, txtNombre, txtPrecioCompra, txtPrecioVenta, txtExistencias, txtStockMinimo;
        private ComboBox cmbCategoria, cmbProveedor, cmbFiltroCategoria;
        private Button btnNuevo, btnGuardar, btnEliminar;
        private int idSeleccionado = 0;
        private int existenciasAntes = 0;
        private DataTable tablaProductos;

        // Categorías comunes de supermercado, para no tener que escribirlas cada vez.
        // Es una lista editable: si se escribe una categoría nueva, también se acepta.
        private static readonly string[] CategoriasComunes = new[]
        {
            "Abarrotes", "Lácteos", "Carnes", "Frutas y Verduras", "Panadería",
            "Bebidas", "Limpieza", "Higiene Personal", "Congelados", "Snacks"
        };

        public FormProductos()
        {
            InicializarComponentes();
            CargarProveedores();
            CargarProductos();
        }

        private void InicializarComponentes()
        {
            this.Text = "Módulo de Productos e Inventario";
            this.Size = new Size(980, 620);

            var panelForm = new Panel { Dock = DockStyle.Top, Height = 250 };

            txtCodigo = CrearCampo(panelForm, "Código:", 20, 20);
            txtNombre = CrearCampo(panelForm, "Nombre:", 20, 60);

            var lblCategoria = new Label { Text = "Categoría:", Location = new Point(20, 100), AutoSize = true };
            cmbCategoria = new ComboBox { Location = new Point(130, 97), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDown };
            cmbCategoria.Items.AddRange(CategoriasComunes);
            cmbCategoria.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbCategoria.AutoCompleteSource = AutoCompleteSource.ListItems;
            panelForm.Controls.Add(lblCategoria);
            panelForm.Controls.Add(cmbCategoria);

            var lblProveedor = new Label { Text = "Proveedor:", Location = new Point(20, 140), AutoSize = true };
            cmbProveedor = new ComboBox { Location = new Point(130, 137), Size = new Size(200, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            panelForm.Controls.Add(lblProveedor);
            panelForm.Controls.Add(cmbProveedor);

            txtPrecioCompra = CrearCampo(panelForm, "Precio Compra:", 380, 20);
            txtPrecioVenta = CrearCampo(panelForm, "Precio Venta:", 380, 60);
            txtExistencias = CrearCampo(panelForm, "Existencias:", 380, 100);
            txtStockMinimo = CrearCampo(panelForm, "Stock Mínimo:", 380, 140);

            btnNuevo = new Button { Text = "🆕 Nuevo", BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(700, 20), Size = new Size(110, 30) };
            btnGuardar = new Button { Text = "💾 Guardar", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(700, 60), Size = new Size(110, 30) };
            btnEliminar = new Button { Text = "🗑️ Eliminar", BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(700, 100), Size = new Size(110, 30) };

            btnNuevo.Click += (s, e) => LimpiarFormulario();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            panelForm.Controls.Add(btnNuevo);
            panelForm.Controls.Add(btnGuardar);
            panelForm.Controls.Add(btnEliminar);

            var panelFiltro = new Panel { Dock = DockStyle.Top, Height = 40, BackColor = Color.FromArgb(236, 240, 241) };
            var lblFiltro = new Label { Text = "🔎 Ver solo categoría:", Location = new Point(20, 11), AutoSize = true };
            cmbFiltroCategoria = new ComboBox { Location = new Point(170, 8), Size = new Size(220, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFiltroCategoria.SelectedIndexChanged += (s, e) => FiltrarPorCategoria();
            panelFiltro.Controls.Add(lblFiltro);
            panelFiltro.Controls.Add(cmbFiltroCategoria);

            panelTarjetas = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Color.White };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grid.CellClick += Grid_CellClick;
            EstiloVisual.EstilizarGrid(grid);

            this.Controls.Add(grid);
            this.Controls.Add(panelFiltro);
            this.Controls.Add(panelTarjetas);
            this.Controls.Add(panelForm);
        }

        private void ActualizarTarjetas()
        {
            panelTarjetas.Controls.Clear();
            if (tablaProductos == null) return;

            int totalProductos = tablaProductos.Rows.Count;
            decimal valorInventario = 0;
            int bajoStock = 0;
            foreach (DataRow fila in tablaProductos.Rows)
            {
                decimal precio = Convert.ToDecimal(fila["PrecioVenta"]);
                int existencias = Convert.ToInt32(fila["Existencias"]);
                int stockMin = Convert.ToInt32(fila["StockMinimo"]);
                valorInventario += precio * existencias;
                if (existencias <= stockMin) bajoStock++;
            }

            int x = 20;
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(x, "📦 Total de productos", totalProductos.ToString(), Color.FromArgb(41, 128, 185)));
            x += 280;
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(x, "💰 Valor del inventario", valorInventario.ToString("C2"), Color.FromArgb(0, 150, 136)));
            x += 280;
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(x, "⚠️ Bajo stock mínimo", bajoStock.ToString(), Color.FromArgb(192, 57, 43)));
        }

        private void FiltrarPorCategoria()
        {
            if (tablaProductos == null) return;
            string categoria = cmbFiltroCategoria.SelectedItem?.ToString();
            tablaProductos.DefaultView.RowFilter = (string.IsNullOrEmpty(categoria) || categoria == "(Todas)")
                ? ""
                : $"Categoria = '{categoria.Replace("'", "''")}'";
        }

        private TextBox CrearCampo(Panel panel, string etiqueta, int x, int y)
        {
            var lbl = new Label { Text = etiqueta, Location = new Point(x, y), AutoSize = true };
            var txt = new TextBox { Location = new Point(x + 110, y - 3), Size = new Size(200, 25) };
            panel.Controls.Add(lbl);
            panel.Controls.Add(txt);
            return txt;
        }

        private void CargarProveedores()
        {
            cmbProveedor.Items.Clear();
            cmbProveedor.Items.Add("(Sin proveedor)");
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Id, Nombre FROM Proveedores ORDER BY Nombre", conn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            cmbProveedor.Items.Add($"{reader["Nombre"]} (Id={reader["Id"]})");
                    }
                }
            }
            catch
            {
                // Si no hay conexión, se sigue solo con "(Sin proveedor)"
            }
            cmbProveedor.SelectedIndex = 0;
        }

        private void CargarProductos()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT p.Id, p.Codigo, p.Nombre, p.Categoria, p.PrecioCompra, p.PrecioVenta,
                                   p.Existencias, p.StockMinimo, p.ProveedorId, pr.Nombre AS Proveedor
                                   FROM Productos p
                                   LEFT JOIN Proveedores pr ON p.ProveedorId = pr.Id
                                   ORDER BY p.Nombre";
                    var adapter = new SqlDataAdapter(sql, conn);
                    tablaProductos = new DataTable();
                    adapter.Fill(tablaProductos);
                    grid.DataSource = tablaProductos;
                    if (grid.Columns["ProveedorId"] != null) grid.Columns["ProveedorId"].Visible = false;

                    string categoriaPrevia = cmbFiltroCategoria.SelectedItem?.ToString();
                    var categorias = new List<string> { "(Todas)" };
                    foreach (DataRow fila in tablaProductos.Rows)
                    {
                        string cat = fila["Categoria"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(cat) && !categorias.Contains(cat))
                            categorias.Add(cat);
                    }
                    cmbFiltroCategoria.Items.Clear();
                    cmbFiltroCategoria.Items.AddRange(categorias.ToArray());
                    int indicePrevio = categoriaPrevia != null ? cmbFiltroCategoria.Items.IndexOf(categoriaPrevia) : -1;
                    cmbFiltroCategoria.SelectedIndex = indicePrevio >= 0 ? indicePrevio : 0;
                }
                ActualizarTarjetas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo cargar productos desde la base de datos.\n" +
                    "Verifique la conexión en Datos/Conexion.cs.\n\nDetalle: " + ex.Message,
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = grid.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtCodigo.Text = fila.Cells["Codigo"].Value?.ToString();
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            cmbCategoria.Text = fila.Cells["Categoria"].Value?.ToString();
            txtPrecioCompra.Text = fila.Cells["PrecioCompra"].Value?.ToString();
            txtPrecioVenta.Text = fila.Cells["PrecioVenta"].Value?.ToString();
            txtExistencias.Text = fila.Cells["Existencias"].Value?.ToString();
            txtStockMinimo.Text = fila.Cells["StockMinimo"].Value?.ToString();
            int.TryParse(txtExistencias.Text, out existenciasAntes);

            var proveedorId = fila.Cells["ProveedorId"].Value;
            if (proveedorId == null || proveedorId == DBNull.Value)
            {
                cmbProveedor.SelectedIndex = 0;
            }
            else
            {
                string idBuscado = $"(Id={proveedorId})";
                int encontrado = 0;
                for (int i = 0; i < cmbProveedor.Items.Count; i++)
                {
                    if (cmbProveedor.Items[i].ToString().EndsWith(idBuscado)) { encontrado = i; break; }
                }
                cmbProveedor.SelectedIndex = encontrado;
            }
        }

        private void LimpiarFormulario()
        {
            idSeleccionado = 0;
            existenciasAntes = 0;
            txtCodigo.Clear();
            txtNombre.Clear();
            cmbCategoria.Text = "";
            txtPrecioCompra.Clear();
            txtPrecioVenta.Clear();
            txtExistencias.Clear();
            txtStockMinimo.Clear();
            if (cmbProveedor.Items.Count > 0) cmbProveedor.SelectedIndex = 0;
        }

        private int? ObtenerProveedorIdSeleccionado()
        {
            var seleccion = cmbProveedor.SelectedItem?.ToString() ?? "";
            int idxIni = seleccion.LastIndexOf("Id=");
            if (idxIni < 0) return null; // "(Sin proveedor)"
            if (int.TryParse(seleccion.Substring(idxIni + 3).TrimEnd(')'), out int id))
                return id;
            return null;
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre del producto es obligatorio.");
                return false;
            }
            if (!decimal.TryParse(txtPrecioVenta.Text, out _))
            {
                MessageBox.Show("El precio de venta debe ser numérico.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtCodigo.Text) && CodigoDuplicado(txtCodigo.Text.Trim()))
            {
                MessageBox.Show("Ya existe otro producto con ese código. Use un código distinto.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtExistencias.Text) && (!int.TryParse(txtExistencias.Text, out int existenciasVal) || existenciasVal < 0))
            {
                MessageBox.Show("Las existencias no pueden ser negativas. Ingrese 0 o un número positivo.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtStockMinimo.Text) && (!int.TryParse(txtStockMinimo.Text, out int stockMinVal) || stockMinVal < 0))
            {
                MessageBox.Show("El stock mínimo no puede ser negativo. Ingrese 0 o un número positivo.");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Si la categoría escrita ya existe con otra forma (mayúsculas, tildes,
        /// espacios o plural: "bebida", "LACTEOS"), devuelve la forma ya usada
        /// ("Bebidas", "Lácteos") para no crear categorías repetidas.
        /// </summary>
        private string NormalizarCategoria(string texto)
        {
            texto = (texto ?? "").Trim();
            if (texto.Length == 0) return texto;

            var comparador = System.Globalization.CultureInfo.InvariantCulture.CompareInfo;
            var opciones = System.Globalization.CompareOptions.IgnoreCase | System.Globalization.CompareOptions.IgnoreNonSpace;
            bool Igual(string a, string b) => comparador.Compare(a, b, opciones) == 0;

            var conocidas = CategoriasComunes.Concat(cmbCategoria.Items.Cast<object>().Select(i => i.ToString()));
            foreach (var categoria in conocidas)
            {
                if (Igual(texto, categoria) || Igual(texto + "s", categoria) || Igual(texto, categoria + "s"))
                    return categoria;
            }
            return texto;
        }

        private bool CodigoDuplicado(string codigo)
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT Id FROM Productos WHERE Codigo=@codigo", conn);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    var resultado = cmd.ExecuteScalar();
                    if (resultado == null) return false;
                    int idExistente = Convert.ToInt32(resultado);
                    return idExistente != idSeleccionado;
                }
            }
            catch
            {
                return false; // si no hay conexión, se deja pasar y la base de datos lo bloqueará igual (UNIQUE)
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;
            cmbCategoria.Text = NormalizarCategoria(cmbCategoria.Text);

            try
            {
                int? proveedorId = ObtenerProveedorIdSeleccionado();

                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    SqlCommand cmd;

                    if (idSeleccionado == 0)
                    {
                        cmd = new SqlCommand(@"INSERT INTO Productos
                            (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId)
                            VALUES (@codigo, @nombre, @categoria, @pcompra, @pventa, @existencias, @stockmin, @proveedor)", conn);
                    }
                    else
                    {
                        cmd = new SqlCommand(@"UPDATE Productos SET
                            Codigo=@codigo, Nombre=@nombre, Categoria=@categoria, PrecioCompra=@pcompra,
                            PrecioVenta=@pventa, Existencias=@existencias, StockMinimo=@stockmin, ProveedorId=@proveedor
                            WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    }

                    cmd.Parameters.AddWithValue("@codigo", txtCodigo.Text.Trim());
                    cmd.Parameters.AddWithValue("@nombre", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@categoria", cmbCategoria.Text.Trim());
                    cmd.Parameters.AddWithValue("@pcompra", decimal.Parse(string.IsNullOrEmpty(txtPrecioCompra.Text) ? "0" : txtPrecioCompra.Text));
                    cmd.Parameters.AddWithValue("@pventa", decimal.Parse(txtPrecioVenta.Text));
                    cmd.Parameters.AddWithValue("@existencias", int.Parse(string.IsNullOrEmpty(txtExistencias.Text) ? "0" : txtExistencias.Text));
                    cmd.Parameters.AddWithValue("@stockmin", int.Parse(string.IsNullOrEmpty(txtStockMinimo.Text) ? "0" : txtStockMinimo.Text));
                    cmd.Parameters.AddWithValue("@proveedor", (object)proveedorId ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                int existenciasNuevas = int.Parse(string.IsNullOrEmpty(txtExistencias.Text) ? "0" : txtExistencias.Text);
                int diferencia = existenciasNuevas - existenciasAntes;
                if (diferencia != 0)
                {
                    RegistrarMovimientoInventario(txtCodigo.Text.Trim(), diferencia,
                        idSeleccionado == 0 ? "Alta de producto" : "Ajuste manual de inventario");
                }

                // Si se escribió una categoría nueva, se agrega a la lista para la próxima vez.
                if (!string.IsNullOrWhiteSpace(cmbCategoria.Text) && !cmbCategoria.Items.Contains(cmbCategoria.Text.Trim()))
                    cmbCategoria.Items.Add(cmbCategoria.Text.Trim());

                Auditoria.Registrar(Sesion.UsuarioId, "Guardar producto", $"Producto '{txtNombre.Text.Trim()}' (código {txtCodigo.Text.Trim()}) guardado.");
                MessageBox.Show("Producto guardado correctamente.");
                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void RegistrarMovimientoInventario(string codigoProducto, int cantidad, string motivo)
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"INSERT INTO MovimientoInventario (ProductoId, Fecha, Cantidad, Motivo, UsuarioId)
                        SELECT Id, @fecha, @cantidad, @motivo, @usuario FROM Productos WHERE Codigo=@codigo", conn);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@cantidad", cantidad);
                    cmd.Parameters.AddWithValue("@motivo", motivo);
                    cmd.Parameters.AddWithValue("@usuario", Sesion.UsuarioId == 0 ? 1 : Sesion.UsuarioId);
                    cmd.Parameters.AddWithValue("@codigo", codigoProducto);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Si falla el registro del movimiento no debe detener el guardado del producto.
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista.");
                return;
            }

            if (MessageBox.Show("¿Eliminar este producto?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                string nombreEliminado = txtNombre.Text.Trim();
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Productos WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }
                Auditoria.Registrar(Sesion.UsuarioId, "Eliminar producto", $"Producto '{nombreEliminado}' eliminado.");
                LimpiarFormulario();
                CargarProductos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }
    }
}
