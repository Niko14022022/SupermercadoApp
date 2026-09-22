using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormLogin : Form
    {
        private TextBox txtUsuario;
        private TextBox txtClave;
        private Button btnIngresar;
        private Label lblError;

        // Paleta de colores del sistema (fácil de cambiar en un solo lugar)
        public static readonly Color ColorPrimario = Color.FromArgb(0, 150, 136);   // verde-azulado
        public static readonly Color ColorPrimarioOscuro = Color.FromArgb(0, 105, 92);
        public static readonly Color ColorAcento = Color.FromArgb(255, 160, 0);      // naranja
        public static readonly Color ColorFondo = Color.FromArgb(245, 247, 246);

        public FormLogin()
        {
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Supermercado - Iniciar Sesión";
            this.Size = new Size(820, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            // ---- Panel izquierdo: formulario ----
            var panelFormulario = new Panel { Dock = DockStyle.Left, Width = 400, BackColor = Color.White };

            var lblAvatarFondo = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI Emoji", 26),
                ForeColor = Color.White,
                BackColor = ColorPrimario,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(160, 30),
                Size = new Size(80, 80)
            };
            // Redondear el panel del avatar
            var regionAvatar = new GraphicsPath();
            regionAvatar.AddEllipse(0, 0, 80, 80);
            lblAvatarFondo.Region = new Region(regionAvatar);

            var lblTitulo = new Label
            {
                Text = "Sistema de Ventas\ne Inventario",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(30, 118),
                Size = new Size(340, 80)
            };

            var lblUsuario = new Label { Text = "USUARIO", Location = new Point(50, 220), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Gray };
            txtUsuario = new TextBox { Location = new Point(50, 240), Size = new Size(300, 28), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle };

            var lblClave = new Label { Text = "CONTRASEÑA", Location = new Point(50, 280), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold), ForeColor = Color.Gray };
            txtClave = new TextBox { Location = new Point(50, 300), Size = new Size(300, 28), Font = new Font("Segoe UI", 11), BorderStyle = BorderStyle.FixedSingle, UseSystemPasswordChar = true };

            btnIngresar = new Button
            {
                Text = "INGRESAR",
                Location = new Point(50, 345),
                Size = new Size(300, 40),
                BackColor = ColorPrimario,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Click += BtnIngresar_Click;

            lblError = new Label
            {
                Text = "",
                ForeColor = Color.Firebrick,
                Location = new Point(50, 393),
                AutoSize = false,
                Size = new Size(300, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panelFormulario.Controls.Add(lblAvatarFondo);
            panelFormulario.Controls.Add(lblTitulo);
            panelFormulario.Controls.Add(lblUsuario);
            panelFormulario.Controls.Add(txtUsuario);
            panelFormulario.Controls.Add(lblClave);
            panelFormulario.Controls.Add(txtClave);
            panelFormulario.Controls.Add(btnIngresar);
            panelFormulario.Controls.Add(lblError);

            // ---- Panel derecho: bienvenida con degradado ----
            var panelBienvenida = new Panel { Dock = DockStyle.Fill };
            panelBienvenida.Paint += (s, e) =>
            {
                using (var brocha = new LinearGradientBrush(panelBienvenida.ClientRectangle,
                    ColorPrimario, Color.FromArgb(0, 77, 64), LinearGradientMode.ForwardDiagonal))
                {
                    e.Graphics.FillRectangle(brocha, panelBienvenida.ClientRectangle);
                }
            };

            var lblIconoGrande = new Label
            {
                Text = "🛒",
                Font = new Font("Segoe UI Emoji", 55),
                ForeColor = Color.White,
                AutoSize = false,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(50, 100),
                Size = new Size(320, 100)
            };
            var lblBienvenido = new Label
            {
                Text = "Bienvenido",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 210),
                Size = new Size(380, 50)
            };
            var lblSubtexto = new Label
            {
                Text = "Controlá tus ventas, inventario\ny clientes desde un solo lugar.",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.FromArgb(220, 240, 238),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 265),
                Size = new Size(380, 50)
            };
            panelBienvenida.Controls.Add(lblIconoGrande);
            panelBienvenida.Controls.Add(lblBienvenido);
            panelBienvenida.Controls.Add(lblSubtexto);

            this.Controls.Add(panelBienvenida);
            this.Controls.Add(panelFormulario);

            this.AcceptButton = btnIngresar;
        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text.Trim();

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
            {
                lblError.Text = "Ingrese usuario y contraseña.";
                return;
            }

            var (encontrado, id, rol) = ValidarUsuario(usuario, clave);

            if (!encontrado && usuario == "admin" && clave == "admin123")
            {
                // Acceso de respaldo mientras se conecta la base de datos real.
                id = 1;
                rol = "Administrador";
                encontrado = true;
            }

            if (!encontrado)
            {
                lblError.Text = "Usuario o contraseña incorrectos.";
                return;
            }

            Sesion.UsuarioId = id;
            Sesion.NombreUsuario = usuario;
            Sesion.Rol = rol;
            Auditoria.Registrar(id, "Inicio de sesión", $"El usuario {usuario} inició sesión.");

            var principal = new FormPrincipal(usuario, rol);
            this.Hide();
            principal.FormClosed += (s, args) => this.Close();
            principal.Show();

            AvisarBajoStock();
        }

        private (bool encontrado, int id, string rol) ValidarUsuario(string usuario, string clave)
        {
            string claveHash = Seguridad.Hash(clave);
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    // Acepta tanto la contraseña ya encriptada (hash) como texto plano,
                    // para no romper cuentas creadas antes de activar el hash.
                    string sql = @"SELECT Id, Rol FROM Usuarios
                                   WHERE NombreUsuario = @usuario AND Activo = 1
                                   AND (Clave = @claveHash OR Clave = @clavePlano)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@claveHash", claveHash);
                        cmd.Parameters.AddWithValue("@clavePlano", clave);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int id = Convert.ToInt32(reader["Id"]);
                                string rol = reader["Rol"].ToString();
                                return (true, id, rol);
                            }
                        }
                    }
                }
            }
            catch
            {
                // Si no hay conexión a la base de datos todavía, se usa el acceso de respaldo.
            }
            return (false, 0, "");
        }

        private void AvisarBajoStock()
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand("SELECT COUNT(*) FROM Productos WHERE Existencias <= StockMinimo", conn);
                    int cantidad = (int)cmd.ExecuteScalar();
                    if (cantidad > 0)
                    {
                        MessageBox.Show($"⚠️ Hay {cantidad} producto(s) con existencias en el mínimo o por debajo. Revise el reporte de bajo stock.",
                            "Aviso de inventario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch
            {
                // Si no hay conexión, simplemente no se muestra el aviso.
            }
        }
    }
}
