using System;
using System.Drawing;
using System.Windows.Forms;
using SupermercadoApp.Datos;

namespace SupermercadoApp.Forms
{
    public class FormPrincipal : Form
    {
        private readonly string usuarioActual;
        private readonly string rolActual;
        private Panel panelContenido; // aquí se muestran los módulos
        private Panel panelInicio;
        private Panel panelLateral;
        private Button botonLateralActivo;

        private static readonly Color ColorLateral = Color.FromArgb(23, 42, 58);
        private static readonly Color ColorLateralActivo = Color.FromArgb(0, 150, 136);

        public FormPrincipal(string usuario, string rol)
        {
            usuarioActual = usuario;
            rolActual = rol;
            InicializarComponentes();
        }

        private void InicializarComponentes()
        {
            this.Text = "Sistema de Ventas e Inventario - Supermercado";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = FormLogin.ColorFondo;

            // ---- Barra superior ----
            var barraSuperior = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.White };
            barraSuperior.Paint += (s, e) => e.Graphics.DrawLine(Pens.Gainsboro, 0, 54, barraSuperior.Width, 54);

            // Panel acoplado a la derecha: evita el bug de calcular la posición con
            // this.Width antes de que la ventana esté realmente maximizada.
            var panelDerechoSuperior = new Panel { Dock = DockStyle.Right, Width = 420 };

            var btnSalir = new Button
            {
                Text = "⏻ Cerrar sesión",
                BackColor = FormLogin.ColorAcento,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 34),
                Location = new Point(260, 10),
                Cursor = Cursors.Hand
            };
            var lblUsuarioConectado = new Label
            {
                Text = $"👤 {usuarioActual}  ·  {rolActual}",
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                MaximumSize = new Size(240, 0),
                Location = new Point(10, 18)
            };
            panelDerechoSuperior.Controls.Add(btnSalir);
            panelDerechoSuperior.Controls.Add(lblUsuarioConectado);
            btnSalir.FlatAppearance.BorderSize = 0;
            btnSalir.Click += (s, e) =>
            {
                Auditoria.Registrar(Sesion.UsuarioId, "Cerrar sesión", $"El usuario {usuarioActual} cerró sesión.");
                Sesion.Cerrar();
                var login = new FormLogin();
                login.Show();
                this.Close();
            };
            barraSuperior.Controls.Add(panelDerechoSuperior);

            // ---- Menú lateral ----
            panelLateral = new Panel { Dock = DockStyle.Left, Width = 210, BackColor = ColorLateral };

            var lblLogo = new Label
            {
                Text = "🛒 Supermercado",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 60
            };
            panelLateral.Controls.Add(lblLogo);

            var contenedorBotones = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                Location = new Point(0, 60)
            };
            panelLateral.Controls.Add(contenedorBotones);

            var btnNavInicio = CrearBotonLateral("🏠", "Inicio", () => VolverAlInicio());
            var btnNavVentas = CrearBotonLateral("🧾", "Ventas (POS)", () => MostrarModulo(new FormVentas()));
            var btnNavProductos = CrearBotonLateral("📦", "Productos", () => MostrarModulo(new FormProductos()));
            var btnNavProveedores = CrearBotonLateral("🚚", "Proveedores", () => MostrarModulo(new FormProveedores()));
            var btnNavClientes = CrearBotonLateral("🧍", "Clientes", () => MostrarModulo(new FormClientes()));
            var btnNavReportes = CrearBotonLateral("📊", "Reportes", () => MostrarModulo(new FormReportes()));
            var btnNavUsuarios = CrearBotonLateral("🔑", "Usuarios", () =>
            {
                if (rolActual == "Administrador")
                    MostrarModulo(new FormUsuarios());
                else
                    MessageBox.Show("Solo el administrador puede acceder a este módulo.", "Acceso restringido",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
            var btnNavAyuda = CrearBotonLateral("❓", "Ayuda", MostrarAyuda);

            contenedorBotones.Controls.Add(btnNavInicio);
            contenedorBotones.Controls.Add(btnNavVentas);
            contenedorBotones.Controls.Add(btnNavProductos);
            contenedorBotones.Controls.Add(btnNavProveedores);
            contenedorBotones.Controls.Add(btnNavClientes);
            contenedorBotones.Controls.Add(btnNavReportes);
            contenedorBotones.Controls.Add(btnNavUsuarios);
            contenedorBotones.Controls.Add(btnNavAyuda);

            MarcarActivo(btnNavInicio);

            // ---- Panel de inicio con tarjetas ----
            panelInicio = new Panel { Dock = DockStyle.Fill, BackColor = FormLogin.ColorFondo, AutoScroll = true };
            var lblBienvenida = new Label
            {
                Text = "¿Qué querés hacer hoy?",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60),
                Location = new Point(30, 20),
                AutoSize = true
            };
            panelInicio.Controls.Add(lblBienvenida);

            int x = 30, y = 70, anchoTarjeta = 220, altoTarjeta = 140, espacio = 25;

            AgregarTarjeta(panelInicio, "🧾", "Ventas (POS)", Color.FromArgb(0, 150, 136), x, y, anchoTarjeta, altoTarjeta,
                () => { MostrarModulo(new FormVentas()); MarcarActivo(btnNavVentas); });
            x += anchoTarjeta + espacio;

            AgregarTarjeta(panelInicio, "📦", "Productos / Inventario", Color.FromArgb(41, 128, 185), x, y, anchoTarjeta, altoTarjeta,
                () => { MostrarModulo(new FormProductos()); MarcarActivo(btnNavProductos); });
            x += anchoTarjeta + espacio;

            AgregarTarjeta(panelInicio, "🚚", "Proveedores", Color.FromArgb(155, 89, 182), x, y, anchoTarjeta, altoTarjeta,
                () => { MostrarModulo(new FormProveedores()); MarcarActivo(btnNavProveedores); });
            x += anchoTarjeta + espacio;

            AgregarTarjeta(panelInicio, "🧍", "Clientes", Color.FromArgb(230, 126, 34), x, y, anchoTarjeta, altoTarjeta,
                () => { MostrarModulo(new FormClientes()); MarcarActivo(btnNavClientes); });

            x = 30; y += altoTarjeta + espacio;

            AgregarTarjeta(panelInicio, "📊", "Reportes", Color.FromArgb(39, 174, 96), x, y, anchoTarjeta, altoTarjeta,
                () => { MostrarModulo(new FormReportes()); MarcarActivo(btnNavReportes); });
            x += anchoTarjeta + espacio;

            AgregarTarjeta(panelInicio, "🔑", "Usuarios", Color.FromArgb(192, 57, 43), x, y, anchoTarjeta, altoTarjeta,
                () =>
                {
                    if (rolActual == "Administrador")
                    {
                        MostrarModulo(new FormUsuarios());
                        MarcarActivo(btnNavUsuarios);
                    }
                    else
                        MessageBox.Show("Solo el administrador puede acceder a este módulo.", "Acceso restringido",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                });

            panelContenido = new Panel { Dock = DockStyle.Fill };
            panelContenido.Controls.Add(panelInicio);

            this.Controls.Add(panelContenido);
            this.Controls.Add(barraSuperior);
            this.Controls.Add(panelLateral);
            barraSuperior.BringToFront();

            // Actualizar cuál botón queda "activo" al navegar también desde las tarjetas
            btnNavInicio.Click += (s, e) => MarcarActivo(btnNavInicio);
            btnNavVentas.Click += (s, e) => MarcarActivo(btnNavVentas);
            btnNavProductos.Click += (s, e) => MarcarActivo(btnNavProductos);
            btnNavProveedores.Click += (s, e) => MarcarActivo(btnNavProveedores);
            btnNavClientes.Click += (s, e) => MarcarActivo(btnNavClientes);
            btnNavReportes.Click += (s, e) => MarcarActivo(btnNavReportes);
            btnNavUsuarios.Click += (s, e) => MarcarActivo(btnNavUsuarios);
        }

        private Button CrearBotonLateral(string icono, string texto, Action alHacerClick)
        {
            var boton = new Button
            {
                Text = $"   {icono}   {texto}",
                Width = 210,
                Height = 46,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                BackColor = ColorLateral,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(35, 58, 78);
            boton.Click += (s, e) =>
            {
                alHacerClick();
                MarcarActivo(boton);
            };
            return boton;
        }

        private void MarcarActivo(Button boton)
        {
            if (botonLateralActivo != null) botonLateralActivo.BackColor = ColorLateral;
            boton.BackColor = ColorLateralActivo;
            botonLateralActivo = boton;
        }

        private void MostrarAyuda()
        {
            MessageBox.Show(
                "🧾 Ventas (POS): registrar una venta y descontar inventario automáticamente.\n" +
                "📦 Productos: dar de alta, editar o eliminar productos e insumos.\n" +
                "🚚 Proveedores: gestionar la información de proveedores.\n" +
                "🧍 Clientes: gestionar la información de clientes.\n" +
                "📊 Reportes: consultar ventas, bajo stock, más vendidos, movimientos y bitácora.\n" +
                "🔑 Usuarios: crear y administrar cuentas de acceso (solo administrador).\n\n" +
                "Si hay algún error de conexión, revise Datos/Conexion.cs.",
                "Ayuda del sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AgregarTarjeta(Panel contenedor, string icono, string titulo, Color color, int x, int y, int ancho, int alto, Action alHacerClick)
        {
            var tarjeta = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(ancho, alto),
                BackColor = color,
                Cursor = Cursors.Hand
            };

            var lblIcono = new Label
            {
                Text = icono,
                Font = new Font("Segoe UI Emoji", 30),
                ForeColor = Color.White,
                Location = new Point(15, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };

            var lblTitulo = new Label
            {
                Text = titulo,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 75),
                Size = new Size(ancho - 30, 55),
                BackColor = Color.Transparent
            };

            tarjeta.Controls.Add(lblIcono);
            tarjeta.Controls.Add(lblTitulo);

            EventHandler clicHandler = (s, e) => alHacerClick();
            tarjeta.Click += clicHandler;
            lblIcono.Click += clicHandler;
            lblTitulo.Click += clicHandler;

            tarjeta.MouseEnter += (s, e) => tarjeta.BackColor = ControlPaint.Light(color, 0.15f);
            tarjeta.MouseLeave += (s, e) => tarjeta.BackColor = color;

            contenedor.Controls.Add(tarjeta);
        }

        private void MostrarModulo(Form modulo)
        {
            panelContenido.Controls.Clear();
            modulo.TopLevel = false;
            modulo.FormBorderStyle = FormBorderStyle.None;
            modulo.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(modulo);
            modulo.Show();
        }

        private void VolverAlInicio()
        {
            panelContenido.Controls.Clear();
            panelContenido.Controls.Add(panelInicio);
        }
    }
}
