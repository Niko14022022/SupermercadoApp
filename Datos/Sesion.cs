namespace SupermercadoApp.Datos
{
    /// <summary>
    /// Guarda los datos del usuario que inició sesión para que cualquier
    /// formulario los pueda usar (por ejemplo, para registrar quién hizo una venta
    /// o quién hizo un cambio en la bitácora).
    /// </summary>
    public static class Sesion
    {
        public static int UsuarioId { get; set; }
        public static string NombreUsuario { get; set; }
        public static string Rol { get; set; }

        public static void Cerrar()
        {
            UsuarioId = 0;
            NombreUsuario = null;
            Rol = null;
        }
    }
}
