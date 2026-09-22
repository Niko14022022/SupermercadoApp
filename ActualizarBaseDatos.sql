-- =========================================================
-- Script de ACTUALIZACIÓN para quienes ya habían creado
-- SupermercadoDB con el script anterior. No borra nada,
-- solo agrega lo nuevo: tablas de auditoría/movimientos,
-- el usuario cajero, y encripta las contraseñas existentes.
-- =========================================================

USE SupermercadoDB;
GO

-- 1) Crear MovimientoInventario si no existe
IF OBJECT_ID('dbo.MovimientoInventario') IS NULL
BEGIN
    CREATE TABLE MovimientoInventario (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ProductoId INT NOT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Cantidad INT NOT NULL,
        Motivo NVARCHAR(100),
        UsuarioId INT NULL,
        FOREIGN KEY (ProductoId) REFERENCES Productos(Id),
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
    );
END
GO

-- 2) Crear Bitacora si no existe
IF OBJECT_ID('dbo.Bitacora') IS NULL
BEGIN
    CREATE TABLE Bitacora (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NULL,
        Fecha DATETIME NOT NULL DEFAULT GETDATE(),
        Accion NVARCHAR(100) NOT NULL,
        Detalle NVARCHAR(400),
        FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
    );
END
GO

-- 3) Agregar el usuario cajero si todavía no existe
IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE NombreUsuario = 'cajero1')
BEGIN
    INSERT INTO Usuarios (NombreUsuario, Clave, Rol, Activo)
    VALUES ('cajero1', '1ed4353e845e2e537e017c0fac3a0d402d231809b7989e90da15191c1148a93f', 'Cajero', 1);
END
GO

-- 4) Encriptar la contraseña del admin si todavía está en texto plano ("admin123")
UPDATE Usuarios
SET Clave = '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9'
WHERE NombreUsuario = 'admin' AND Clave = 'admin123';
GO

-- Nota: la aplicación acepta tanto la contraseña encriptada como texto plano
-- para no romper cuentas que aún no se hayan actualizado con este script,
-- pero es recomendable correr este script para dejar todo encriptado.

-- 5) Agregar datos de ejemplo si la tabla de Productos está vacía o casi vacía
-- (evita duplicar si ya tenías tus propios productos cargados)
IF (SELECT COUNT(*) FROM Productos) <= 3
BEGIN
    IF (SELECT COUNT(*) FROM Proveedores) <= 1
    BEGIN
        INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES
        ('Lácteos del Valle', '2233-4455', 'contacto@lacteosvalle.cr', 'Cartago, Costa Rica'),
        ('Carnicería El Buen Corte', '2244-5566', 'pedidos@buencorte.cr', 'Heredia, Costa Rica'),
        ('Panificadora Dorada', '2255-6677', 'info@panidorada.cr', 'Alajuela, Costa Rica'),
        ('Limpieza Total S.A.', '2266-7788', 'ventas@limpiezatotal.cr', 'San José, Costa Rica');
    END

    DECLARE @provLacteos INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Lácteos del Valle');
    DECLARE @provCarnes INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Carnicería El Buen Corte');
    DECLARE @provPan INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Panificadora Dorada');
    DECLARE @provLimpieza INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Limpieza Total S.A.');
    DECLARE @provCentral INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Distribuidora Central S.A.');

    INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES
    ('P004', 'Azúcar 1kg', 'Abarrotes', 500, 700, 90, 20, @provCentral),
    ('P005', 'Café molido 400g', 'Abarrotes', 1500, 2100, 40, 10, @provCentral),
    ('P006', 'Leche 1L', 'Lácteos', 550, 780, 80, 15, @provLacteos),
    ('P007', 'Queso fresco 500g', 'Lácteos', 1800, 2500, 25, 8, @provLacteos),
    ('P008', 'Yogurt 1L', 'Lácteos', 900, 1300, 35, 10, @provLacteos),
    ('P010', 'Pechuga de pollo 1kg', 'Carnes', 2200, 3100, 30, 8, @provCarnes),
    ('P011', 'Carne molida 1kg', 'Carnes', 2800, 3900, 25, 6, @provCarnes),
    ('P013', 'Pan cuadrado', 'Panadería', 900, 1300, 40, 10, @provPan),
    ('P014', 'Pan dulce (unidad)', 'Panadería', 150, 250, 100, 20, @provPan),
    ('P016', 'Refresco de cola 2L', 'Bebidas', 900, 1300, 70, 15, @provCentral),
    ('P017', 'Agua embotellada 1L', 'Bebidas', 350, 550, 120, 25, @provCentral),
    ('P019', 'Detergente en polvo 1kg', 'Limpieza', 1400, 1950, 30, 8, @provLimpieza),
    ('P020', 'Jabón de baño (unidad)', 'Higiene Personal', 400, 600, 3, 10, @provLimpieza);
END
GO

IF (SELECT COUNT(*) FROM Clientes) = 0
BEGIN
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES
    ('1-1111-1111', 'María Fernández Solano', '8888-1111', 'maria.fernandez@correo.com'),
    ('2-2222-2222', 'Carlos Rojas Vindas', '8888-2222', 'carlos.rojas@correo.com'),
    ('3-3333-3333', 'Ana Gómez Castro', '8888-3333', 'ana.gomez@correo.com'),
    ('4-4444-4444', 'Luis Chacón Araya', '8888-4444', 'luis.chacon@correo.com');
END
GO
