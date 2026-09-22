# Sistema de Ventas e Inventario - Supermercado (Windows Forms)

## Cómo abrirlo
1. Abrí `SupermercadoApp.sln` con Visual Studio 2022 (con la carga de trabajo ".NET desktop development").
2. Si es la **primera vez**: ejecutá `BaseDeDatos.sql` en SQL Server Management Studio para crear la base de datos completa.
   Si **ya tenías** la base de datos creada de antes: ejecutá `ActualizarBaseDatos.sql` en su lugar — agrega las tablas nuevas (Bitácora, Movimientos de Inventario), el usuario cajero, y encripta la contraseña del admin, sin borrar nada de lo que ya tenías.
3. En `Datos/Conexion.cs`, verificá que el servidor sea el correcto.
4. Compilá y ejecutá (F5).

## Usuarios de prueba
- **Administrador:** usuario `admin`, contraseña `admin123` (acceso completo, incluye Usuarios).
- **Cajero:** usuario `cajero1`, contraseña `cajero123` (puede vender y ver reportes, no puede entrar a Usuarios).

## Correcciones recientes
- Se corrigió un bug donde el botón "Cerrar sesión" (y la etiqueta de usuario) en la barra superior, y el botón "✅ Finalizar Venta" del POS, podían aparecer cortados o fuera de la pantalla al maximizar la ventana. Ya no depende del tamaño de la ventana al crearse los controles.
- Se corrigió un error al finalizar una venta con "Consumidor Final" (mandaba ClienteId=0, que no existe; ahora manda NULL correctamente).
- Se corrigió que el texto "Administrador"/"Cajero" en la barra superior se cortara junto al botón de cerrar sesión.
- Se corrigió que el título del login se viera cortado/encimado con el campo de usuario (más espacio y una ventana más alta).
- Reportes ahora genera el reporte automáticamente al abrir el módulo y al cambiar el tipo de reporte, y muestra un aviso claro cuando no hay datos en vez de una tabla vacía y confusa.
- Productos ahora tiene un filtro "Ver solo categoría" arriba de la tabla (por ejemplo, para ver solo Carnes).
- Ya no se puede guardar un producto con existencias o stock mínimo negativos. Si ya tenías alguno en negativo, corré `CorregirStockNegativo.sql` para dejarlo en 0.
- Se corrigió el bug real detrás de las existencias negativas: el POS no restaba lo que ya llevabas en el carrito al validar el stock disponible, así que se podía vender más de lo que había. Ahora si intentás agregar más de lo disponible (contando lo que ya está en el carrito) te avisa. También se agregó una restricción a nivel de base de datos (`CHECK Existencias >= 0`) como última barrera de seguridad.
- Reportes ahora se ve más visual: encabezados de tabla en color, filas alternadas, tarjetas de resumen arriba de cada reporte (total vendido, cantidad de ventas, productos bajo mínimo, etc.) y las filas de productos agotados se resaltan en rojo.
- El mismo estilo (encabezados de tabla a color, filas alternadas, y tarjetas de resumen arriba) se extendió a Productos (total de productos, valor del inventario, bajo stock), Proveedores (total de proveedores), Clientes (total de clientes, con correo registrado) y Usuarios (total, activos, administradores), además del catálogo y carrito del POS.

## Si ya tenés productos metidos a mano
Corré **`AgregarMasDatos.sql`** — suma 8 proveedores, 32 productos (en 10 categorías: Abarrotes, Lácteos, Carnes, Panadería, Bebidas, Limpieza, Higiene Personal, Frutas y Verduras, Congelados, Snacks) y 8 clientes de ejemplo, sin duplicar ni tocar lo que ya tengas metido a mano (cada uno se agrega solo si su código/cédula todavía no existe).

## Datos de ejemplo ya incluidos
Tanto `BaseDeDatos.sql` (instalación nueva) como `ActualizarBaseDatos.sql` (si ya tenías la base creada) cargan datos de muestra para que el sistema no se vea vacío:
- 5 proveedores, 20 productos repartidos en Abarrotes, Lácteos, Carnes, Panadería, Bebidas, Limpieza e Higiene Personal.
- 4 clientes.
- 3 ventas de ejemplo (con su detalle y movimientos de inventario), para que los Reportes muestren datos desde el primer momento.
- El producto "Jabón de baño" queda a propósito con existencias (3) por debajo del mínimo (10), para que puedas mostrar la alerta de bajo stock al iniciar sesión y el reporte correspondiente.

## Qué incluye el sistema
- Login con contraseñas **encriptadas** (hash SHA256), aviso automático de bajo stock al iniciar sesión, pantalla dividida con degradado de bienvenida.
- Dashboard visual con tarjetas de colores para cada módulo.
- CRUD completo: Productos/Inventario (categoría y proveedor como **listas desplegables**, no texto libre; validación de código duplicado), Proveedores, Clientes, Usuarios — todos con validación de formato de correo, teléfono y cédula.
- Punto de Venta (POS): catálogo visual de productos, **buscador con autocompletado** + **filtro por categoría**, **📷 escaneo de código de barras** (funciona con lectores USB o escribiendo el código y Enter — agrega el producto automáticamente al carrito, sumando cantidad si ya estaba), carrito, control de existencias, impresión de comprobante con vista previa, uso del usuario realmente logueado.
- Proveedores y Clientes: **buscador rápido** arriba de la tabla para filtrar mientras se escribe.
- **Reportes**: ventas por rango de fechas, bajo stock, productos más vendidos, historial de movimientos de inventario, bitácora de auditoría — todos **exportables a CSV**.
- **Bitácora de auditoría**: registra quién hizo qué (login, logout, guardar/eliminar en cada módulo, ventas) y cuándo.
- **Historial de movimientos de inventario**: cada entrada/salida de producto queda registrada (por venta o por ajuste manual), no solo el número final de existencias.

## Qué queda pendiente (para pulir aún más, opcional)
- Gráficos visuales en reportes (barras/pastel) — hoy los reportes son tablas exportables a CSV; agregar gráficos requiere el paquete `System.Windows.Forms.DataVisualization`.
- Exportar reportes directamente a Excel/PDF (hoy se exporta a CSV, que se abre bien en Excel).
- Numeración de factura con formato fiscal (hoy usa el número de venta como número de comprobante).
- Diagramas finales (ERD y casos de uso) y manual de usuario con capturas de pantalla reales del sistema ya compilado y corriendo — recomendamos generarlos una vez que compiles el proyecto en tu máquina, tomando las capturas reales.

## Estructura de carpetas
```
SupermercadoApp/
├── SupermercadoApp.sln
├── SupermercadoApp.csproj
├── Program.cs
├── BaseDeDatos.sql            (script completo, para instalación nueva)
├── ActualizarBaseDatos.sql    (script de migración, si ya tenías la BD)
├── Datos/
│   ├── Conexion.cs
│   ├── Seguridad.cs           (hash de contraseñas)
│   ├── Sesion.cs              (usuario que inició sesión)
│   └── Auditoria.cs           (registro en la bitácora)
├── Modelos/
│   └── Entidades.cs
└── Forms/
    ├── FormLogin.cs
    ├── FormPrincipal.cs
    ├── FormProductos.cs
    ├── FormProveedores.cs
    ├── FormClientes.cs
    ├── FormUsuarios.cs
    ├── FormVentas.cs
    └── FormReportes.cs
```
