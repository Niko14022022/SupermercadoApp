using System;
using System.Data.SqlClient;

namespace SupermercadoApp.Datos
{
    /// <summary>
    /// Registra en la tabla Bitacora quién hizo qué cambio y cuándo.
    /// Se llama después de cada guardado, edición o eliminación importante.
    /// </summary>
    public static class Auditoria
    {
        public static void Registrar(int usuarioId, string accion, string detalle)
        {
            try
            {
                using (var conn = Conexion.ObtenerConexion())
                {
                    conn.Open();
                    var cmd = new SqlCommand(@"INSERT INTO Bitacora (UsuarioId, Fecha, Accion, Detalle)
                        VALUES (@usuario, @fecha, @accion, @detalle)", conn);
                    cmd.Parameters.AddWithValue("@usuario", usuarioId);
                    cmd.Parameters.AddWithValue("@fecha", DateTime.Now);
                    cmd.Parameters.AddWithValue("@accion", accion);
                    cmd.Parameters.AddWithValue("@detalle", detalle);
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Si falla la auditoría no debe detener la operación principal del usuario.
            }
        }
    }
}
