using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormClientes : Form
    {
        private DataGridView grid;
        private TextBox txtCedula, txtNombre, txtTelefono, txtCorreo, txtBuscar;
        private Button btnNuevo, btnGuardar, btnEliminar;
        private int idSeleccionado = 0;
        private DataTable tablaClientes;
        private Panel panelTarjetas;

        public FormClientes()
        {
            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            this.Text = "Módulo de Clientes";
            this.Size = new Size(900, 550);

            var panelForm = new Panel { Dock = DockStyle.Top, Height = 180 };

            var lbl1 = new Label { Text = "Cédula:", Location = new Point(20, 20), AutoSize = true };
            txtCedula = new TextBox { Location = new Point(130, 17), Size = new Size(250, 25) };

            var lbl2 = new Label { Text = "Nombre:", Location = new Point(20, 55), AutoSize = true };
            txtNombre = new TextBox { Location = new Point(130, 52), Size = new Size(250, 25) };

            var lbl3 = new Label { Text = "Teléfono:", Location = new Point(20, 90), AutoSize = true };
            txtTelefono = new TextBox { Location = new Point(130, 87), Size = new Size(250, 25) };

            var lbl4 = new Label { Text = "Correo:", Location = new Point(20, 125), AutoSize = true };
            txtCorreo = new TextBox { Location = new Point(130, 122), Size = new Size(250, 25) };

            btnNuevo = new Button { Text = "🆕 Nuevo", BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 20), Size = new Size(100, 30) };
            btnGuardar = new Button { Text = "💾 Guardar", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 60), Size = new Size(100, 30) };
            btnEliminar = new Button { Text = "🗑️ Eliminar", BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(420, 100), Size = new Size(100, 30) };

            btnNuevo.Click += (s, e) => Limpiar();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            panelForm.Controls.AddRange(new Control[] { lbl1, txtCedula, lbl2, txtNombre, lbl3, txtTelefono, lbl4, txtCorreo, btnNuevo, btnGuardar, btnEliminar });

            var panelBusqueda = new Panel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(10, 8, 10, 5) };
            var lblBuscar = new Label { Text = "🔎 Buscar:", Location = new Point(10, 10), AutoSize = true };
            txtBuscar = new TextBox { Location = new Point(85, 7), Size = new Size(300, 25) };
            txtBuscar.TextChanged += (s, e) => FiltrarClientes();
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

        private void FiltrarClientes()
        {
            if (tablaClientes == null) return;
            string texto = txtBuscar.Text.Trim().Replace("'", "''");
            tablaClientes.DefaultView.RowFilter = string.IsNullOrEmpty(texto)
                ? ""
                : $"Nombre LIKE '%{texto}%' OR Cedula LIKE '%{texto}%' OR Telefono LIKE '%{texto}%'";
        }

        private void CargarDatos()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT Id, Cedula, Nombre, Telefono, Correo FROM Clientes ORDER BY Nombre", conn);
                    tablaClientes = new DataTable();
                    adapter.Fill(tablaClientes);
                    grid.DataSource = tablaClientes;
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
            if (tablaClientes == null) return;

            int totalClientes = tablaClientes.Rows.Count;
            int conCorreo = 0;
            foreach (DataRow fila in tablaClientes.Rows)
                if (!string.IsNullOrWhiteSpace(fila["Correo"]?.ToString())) conCorreo++;

            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(20, "🧍 Total de clientes", totalClientes.ToString(), Color.FromArgb(230, 126, 34)));
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(300, "📧 Con correo registrado", conCorreo.ToString(), Color.FromArgb(41, 128, 185)));
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = grid.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtCedula.Text = fila.Cells["Cedula"].Value?.ToString();
            txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();
            txtTelefono.Text = fila.Cells["Telefono"].Value?.ToString();
            txtCorreo.Text = fila.Cells["Correo"].Value?.ToString();
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtCedula.Clear();
            txtNombre.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("El nombre es obligatorio.");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtCorreo.Text) &&
                !System.Text.RegularExpressions.Regex.IsMatch(txtCorreo.Text.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("El correo no tiene un formato válido (ejemplo@dominio.com).");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtTelefono.Text) &&
                !System.Text.RegularExpressions.Regex.IsMatch(txtTelefono.Text.Trim(), @"^[0-9\-\+\s]{8,15}$"))
            {
                MessageBox.Show("El teléfono debe tener solo números (8 a 15 dígitos).");
                return false;
            }
            if (!string.IsNullOrWhiteSpace(txtCedula.Text) &&
                !System.Text.RegularExpressions.Regex.IsMatch(txtCedula.Text.Trim(), @"^[0-9\-]{8,15}$"))
            {
                MessageBox.Show("La cédula debe contener solo números (y guiones si aplica).");
                return false;
            }
            return true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (!ValidarCampos()) return;

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    SqlCommand cmd = idSeleccionado == 0
                        ? new SqlCommand("INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES (@ced,@n,@t,@c)", conn)
                        : new SqlCommand("UPDATE Clientes SET Cedula=@ced, Nombre=@n, Telefono=@t, Correo=@c WHERE Id=@id", conn);

                    if (idSeleccionado != 0) cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.Parameters.AddWithValue("@ced", txtCedula.Text.Trim());
                    cmd.Parameters.AddWithValue("@n", txtNombre.Text.Trim());
                    cmd.Parameters.AddWithValue("@t", txtTelefono.Text.Trim());
                    cmd.Parameters.AddWithValue("@c", txtCorreo.Text.Trim());
                    cmd.ExecuteNonQuery();
                }
                Auditoria.Registrar(Sesion.UsuarioId, "Guardar cliente", $"Cliente '{txtNombre.Text.Trim()}' guardado.");
                Limpiar();
                CargarDatos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (idSeleccionado == 0) { MessageBox.Show("Seleccione un cliente."); return; }
            if (MessageBox.Show("¿Eliminar cliente?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Clientes WHERE Id=@id", conn);
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
