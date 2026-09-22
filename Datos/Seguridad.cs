using System.Security.Cryptography;
using System.Text;

namespace SupermercadoApp.Datos
{
    /// <summary>
    /// Convierte contraseñas en texto plano a un hash SHA256, para no
    /// guardar contraseñas legibles en la base de datos.
    /// </summary>
    public static class Seguridad
    {
        public static string Hash(string textoPlano)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(textoPlano));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
