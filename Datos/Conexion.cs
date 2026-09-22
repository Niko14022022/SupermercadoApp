using System.Data.SqlClient;

namespace SupermercadoApp.Datos
{
    /// <summary>
    /// Maneja la cadena de conexión a la base de datos SQL Server.
    /// Ajustar "Server" y "Database" según el equipo donde se ejecute.
    /// </summary>
    public static class Conexion
    {
        private static readonly string cadena =
            @"Server=NIKOLAS_LAPTOP\SQLEXPRESS02;Database=SupermercadoDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadena);
        }
    }
}
