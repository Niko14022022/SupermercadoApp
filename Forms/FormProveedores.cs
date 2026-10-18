using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormProveedores : Form
    {
        private DataGridView grid;
        private TextBox txtNombre, txtTelefono, txtCorreo, txtDireccion, txtBuscar;
        private Button btnNuevo, btnGuardar, btnEliminar;
        private int idSeleccionado = 0;
        private DataTable tablaProveedores;
        private Panel panelTarjetas;

        public FormProveedores()
        {
            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            this.Text = "Módulo de Proveedores";
            this.Size = new Size(900, 550);

            var panelForm = new Panel { Dock = DockStyle.Top, Height = 180 };

            var lbl1 = new Label { Text = "Nombre:", Location = new Point(20, 20), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(130, 17), Size = new Size(250, 25) };

            var lbl2 = new Label { Text = "Teléfono:", Location = new Point(20, 55), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(130, 52), Size = new Size(250, 25) };

            var lbl3 = new Label { Text = "Correo:", Location = new Point(20, 90), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(130, 87), Size = new Size(250, 25) };

            var lbl4 = new Label { Text = "Dirección:", Location = new Point(20, 125), AutoSize = true };
            txtDireccion = new TextBox { Location = new Point(130, 122), Size = new Size(250, 25) };

            btnNuevo = new Button { Text = "🆕 Nuevo", BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 20), Size = new Size(100, 30) };
            btnGuardar = new Button { Text = "💾 Guardar", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 60), Size = new Size(100, 30) };
            btnEliminar = new Button { Text = "🗑️ Eliminar", BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 100), Size = new Size(100, 30) };

            btnNuevo.Click += (s, e) => Limpiar();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            panelForm.Controls.AddRange(new Control[] { lbl1, txtNombre, lbl2, txtTelefono, lbl3, txtCorreo, lbl4, txtDireccion, btnNuevo, btnGuardar, btnEliminar });

            var panelBusqueda = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 8, 10, 5) };
            var lblBuscar = new Label { Text = "🔎 Buscar:", Location = new Point(10, 10), AutoSize = true };
            txtBuscar = new TextBox { Location = new Point(85, 7), Size = new Size(300, 25) };
            txtBuscar.TextChanged += (s, e) => FiltrarProveedores();
            panelBusqueda.Controls.Add(lblBuscar);
            panelBusqueda.Controls.Add(txtBuscar);

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
            this.Controls.Add(panelBusqueda);
            this.Controls.Add(panelTarjetas);
            this.Controls.Add(panelForm);
        }

        private void FiltrarProveedores()
        {
            if (tablaProveedores == null) return;
            string texto = txtBuscar.Text.Trim().Replace("'", "''");
            tablaProveedores.DefaultView.RowFilter = string.IsNullOrEmpty(texto)
                ? ""
                : $"Nombre LIKE '%{texto}%' OR Telefono LIKE '%{texto}%'";
        }

        private void CargarDatos()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT Id, Nombre, Telefono, Correo, Direccion FROM Proveedores ORDER BY Nombre", conn);
                    tablaProveedores = new DataTable();
                    adapter.Fill(tablaProveedores);
                    grid.DataSource = tablaProveedores;
                }
                ActualizarTarjetas();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo conectar a la base de datos.\nDetalle: " + ex.Message);
            }
        }

        private void ActualizarTarjetas()
        {
            panelTarjetas.Controls.Clear();
            if (tablaProveedores == null) return;
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(20, "🚚 Total de proveedores", tablaProveedores.Rows.Count.ToString(), Color.FromArgb(155, 89, 182)));
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = grid.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString();
            txtCorreo.Text = fila.Cells["Correo"].Value?.ToString();
            txtDireccion.Text = fila.Cells["Direccion"].Value?.ToString();
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtNombre.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
            txtDireccion.Clear();
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return;
            }
            if (!string.IsNullOrWhiteSpace(txtCorreo.Text) &&
                !System.Text.RegularExpressions.Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El correo no tiene un formato válido (ejemplo@dominio.com).");
                return;
            }

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();

                    // No permitir dos proveedores con el mismo nombre
                    var existe = new SqlCommand("SELECT COUNT(*) FROM Proveedores WHERE LTRIM(RTRIM(Nombre)) = @n AND Id <> @id", conn);
                    existe.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                    existe.Parameters.AddWithValue("@id", idSeleccionado);
                    if ((int)existe.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("Ya existe un proveedor con ese nombre. Búsquelo en la lista y edítelo en vez de crear otro.");
                        return;
                    }

                    SqlCommand cmd = idSeleccionado == 0
                        ? new SqlCommand("INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES (@n,@t,@c,@d)", conn)
                        : new SqlCommand("UPDATE Proveedores SET Nombre=@n, Telefono=@t, Correo=@c, Direccion=@d WHERE Id=@id", conn);

                    if (idSeleccionado != 0) cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@t", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@c", txtCorreo.Text.Trim());
                    cmd.Parameters.AddWithValue("@d", txtDireccion.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
                Auditoria.Registrar(Sesion.UsuarioId, "Guardar proveedor", $"Proveedor '{txtNombre.Text.Trim()}' guardado.");
                Limpiar();
                CargarDatos();
            }
            catch (SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                MessageBox.Show("Ya existe un proveedor con ese nombre.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) { MessageBox.Show("Seleccione un proveedor."); return; }
            if (MessageBox.Show("¿Eliminar proveedor?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Proveedores WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }
                Limpiar();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message);
            }
        }
    }
}
