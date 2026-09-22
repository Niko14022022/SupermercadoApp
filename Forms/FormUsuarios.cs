using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormUsuarios : Form
    {
        private DataGridView grid;
        private TextBox txtUsuario, txtClave;
        private ComboBox cmbRol;
        private CheckBox chkActivo;
        private Button btnNuevo, btnGuardar, btnEliminar;
        private int idSeleccionado = 0;
        private Panel panelTarjetas;
        private DataTable tablaUsuarios;

        public FormUsuarios()
        {
            InicializarComponentes();
            CargarDatos();
        }

        private void InicializarComponentes()
        {
            this.Text = "Módulo de Usuarios (solo Administrador)";
            this.Size = new Size(900, 550);

            var panelForm = new Panel { Dock = DockStyle.Top, Height = 180 };

            var lbl1 = new Label { Text = "Usuario:", Location = new Point(20, 20), AutoSize = true };
            txtUsuario = new TextBox { Location = new Point(130, 17), Size = new Size(220, 25) };

            var lbl2 = new Label { Text = "Contraseña:", Location = new Point(20, 55), AutoSize = true };
            txtClave = new TextBox { Location = new Point(130, 52), Size = new Size(220, 25), UseSystemPasswordChar = true };

            var lbl3 = new Label { Text = "Rol:", Location = new Point(20, 90), AutoSize = true };
            cmbRol = new ComboBox { Location = new Point(130, 87), Size = new Size(220, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRol.Items.AddRange(new object[] { "Administrador", "Cajero" });
            cmbRol.SelectedIndex = 1;

            chkActivo = new CheckBox { Text = "Activo", Location = new Point(130, 125), Checked = true };

            btnNuevo = new Button { Text = "🆕 Nuevo", BackColor = Color.FromArgb(41, 128, 185), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(400, 20), Size = new Size(100, 30) };
            btnGuardar = new Button { Text = "💾 Guardar", BackColor = Color.FromArgb(39, 174, 96), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(400, 60), Size = new Size(100, 30) };
            btnEliminar = new Button { Text = "🗑️ Eliminar", BackColor = Color.FromArgb(192, 57, 43), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(400, 100), Size = new Size(100, 30) };

            btnNuevo.Click += (s, e) => Limpiar();
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;

            panelForm.Controls.AddRange(new Control[] { lbl1, txtUsuario, lbl2, txtClave, lbl3, cmbRol, chkActivo, btnNuevo, btnGuardar, btnEliminar });

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
            this.Controls.Add(panelTarjetas);
            this.Controls.Add(panelForm);
        }

        private void CargarDatos()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var adapter = new SqlDataAdapter("SELECT Id, NombreUsuario, Rol, Activo FROM Usuarios ORDER BY NombreUsuario", conn);
                    tablaUsuarios = new DataTable();
                    adapter.Fill(tablaUsuarios);
                    grid.DataSource = tablaUsuarios;
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
            if (tablaUsuarios == null) return;

            int total = tablaUsuarios.Rows.Count;
            int activos = 0, administradores = 0;
            foreach (DataRow fila in tablaUsuarios.Rows)
            {
                if (Convert.ToBoolean(fila["Activo"])) activos++;
                if (fila["Rol"]?.ToString() == "Administrador") administradores++;
            }

            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(20, "🔑 Total de usuarios", total.ToString(), Color.FromArgb(192, 57, 43)));
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(300, "✅ Usuarios activos", activos.ToString(), Color.FromArgb(39, 174, 96)));
            panelTarjetas.Controls.Add(EstiloVisual.CrearTarjeta(580, "👑 Administradores", administradores.ToString(), Color.FromArgb(0, 150, 136)));
        }

        private void Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var fila = grid.Rows[e.RowIndex];
            idSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtUsuario.Text = fila.Cells["NombreUsuario"].Value?.ToString();
            cmbRol.SelectedItem = fila.Cells["Rol"].Value?.ToString();
            chkActivo.Checked = Convert.ToBoolean(fila.Cells["Activo"].Value);
            txtClave.Clear(); // por seguridad no se muestra la clave guardada
        }

        private void Limpiar()
        {
            idSeleccionado = 0;
            txtUsuario.Clear();
            txtClave.Clear();
            cmbRol.SelectedIndex = 1;
            chkActivo.Checked = true;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("El nombre de usuario es obligatorio.");
                return;
            }
            if (idSeleccionado == 0 && string.IsNullOrWhiteSpace(txtClave.Text))
            {
                MessageBox.Show("La contraseña es obligatoria para un usuario nuevo.");
                return;
            }

            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    SqlCommand cmd;

                    if (idSeleccionado == 0)
                    {
                        cmd = new SqlCommand(@"INSERT INTO Usuarios (NombreUsuario, Clave, Rol, Activo)
                            VALUES (@u, @c, @r, @a)", conn);
                        cmd.Parameters.AddWithValue("@c", Seguridad.Hash(txtClave.Text));
                    }
                    else if (!string.IsNullOrWhiteSpace(txtClave.Text))
                    {
                        cmd = new SqlCommand(@"UPDATE Usuarios SET NombreUsuario=@u, Clave=@c, Rol=@r, Activo=@a WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@c", Seguridad.Hash(txtClave.Text));
                        cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    }
                    else
                    {
                        // no se cambia la contraseña si se deja en blanco
                        cmd = new SqlCommand(@"UPDATE Usuarios SET NombreUsuario=@u, Rol=@r, Activo=@a WHERE Id=@id", conn);
                        cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    }

                    cmd.Parameters.AddWithValue("@u", txtUsuario.Text.Trim());
                    cmd.Parameters.AddWithValue("@r", cmbRol.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@a", chkActivo.Checked);
                    cmd.ExecuteNonQuery();
                }
                Auditoria.Registrar(Sesion.UsuarioId, "Guardar usuario", $"Usuario '{txtUsuario.Text.Trim()}' guardado con rol {cmbRol.SelectedItem}.");
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
            if (idSeleccionado == 0) { MessageBox.Show("Seleccione un usuario."); return; }
            if (MessageBox.Show("¿Eliminar usuario?", "Confirmar", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                string nombreEliminado = txtUsuario.Text.Trim();
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("DELETE FROM Usuarios WHERE Id=@id", conn);
                    cmd.Parameters.AddWithValue("@id", idSeleccionado);
                    cmd.ExecuteNonQuery();
                }
                Auditoria.Registrar(Sesion.UsuarioId, "Eliminar usuario", $"Usuario '{nombreEliminado}' eliminado.");
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
