-- =========================================================
-- Script de creación de base de datos: Sistema de Ventas
-- e Inventario para Supermercado
-- =========================================================

CREATE DATABASE SupermercadoDB;
GO
USE SupermercadoDB;
GO

CREATE TABLE Proveedores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Telefono NVARCHAR(30),
    Correo NVARCHAR(150),
    Direccion NVARCHAR(250)
);
GO

CREATE TABLE Productos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Codigo NVARCHAR(50) NOT NULL UNIQUE,
    Nombre NVARCHAR(150) NOT NULL,
    Categoria NVARCHAR(100),
    PrecioCompra DECIMAL(10,2) NOT NULL DEFAULT 0,
    PrecioVenta DECIMAL(10,2) NOT NULL DEFAULT 0,
    Existencias INT NOT NULL DEFAULT 0 CHECK (Existencias >= 0),
    StockMinimo INT NOT NULL DEFAULT 0 CHECK (StockMinimo >= 0),
    ProveedorId INT NULL,
    FOREIGN KEY (ProveedorId) REFERENCES Proveedores(Id)
);
GO

CREATE TABLE Clientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Cedula NVARCHAR(30),
    Nombre NVARCHAR(150) NOT NULL,
    Telefono NVARCHAR(30),
    Correo NVARCHAR(150)
);
GO

CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Clave NVARCHAR(100) NOT NULL, -- se guarda encriptada (hash SHA256)
    Rol NVARCHAR(30) NOT NULL, -- 'Administrador' o 'Cajero'
    Activo BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Ventas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    ClienteId INT NULL,
    UsuarioId INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL DEFAULT 0,
    MetodoPago NVARCHAR(30),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(Id),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

CREATE TABLE DetalleVenta (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    VentaId INT NOT NULL,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
    FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);
GO

-- Historial de entradas/salidas de inventario (positivo = entrada, negativo = salida)
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
GO

-- Bitácora de auditoría: quién hizo qué cambio y cuándo
CREATE TABLE Bitacora (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Accion NVARCHAR(100) NOT NULL,
    Detalle NVARCHAR(400),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);
GO

-- Usuario administrador inicial (usuario: admin / clave: admin123)
-- La clave ya está guardada encriptada (hash SHA256), no en texto plano.
INSERT INTO Usuarios (NombreUsuario, Clave, Rol, Activo)
VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrador', 1);
GO

-- Usuario cajero (usuario: cajero1 / clave: cajero123)
INSERT INTO Usuarios (NombreUsuario, Clave, Rol, Activo)
VALUES ('cajero1', '1ed4353e845e2e537e017c0fac3a0d402d231809b7989e90da15191c1148a93f', 'Cajero', 1);
GO

-- =========================================================
-- Datos de ejemplo para probar el sistema con contenido real
-- =========================================================

-- Proveedores
INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES
('Distribuidora Central S.A.', '2222-1111', 'ventas@distcentral.cr', 'San José, Costa Rica'),
('Lácteos del Valle', '2233-4455', 'contacto@lacteosvalle.cr', 'Cartago, Costa Rica'),
('Carnicería El Buen Corte', '2244-5566', 'pedidos@buencorte.cr', 'Heredia, Costa Rica'),
('Panificadora Dorada', '2255-6677', 'info@panidorada.cr', 'Alajuela, Costa Rica'),
('Limpieza Total S.A.', '2266-7788', 'ventas@limpiezatotal.cr', 'San José, Costa Rica');
GO

-- Productos (varias categorías, para que se vean bien los filtros y reportes)
INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES
('P001', 'Arroz 1kg', 'Abarrotes', 600, 850, 100, 20, 1),
('P002', 'Aceite 900ml', 'Abarrotes', 1200, 1650, 50, 10, 1),
('P003', 'Frijoles 900g', 'Abarrotes', 700, 950, 60, 15, 1),
('P004', 'Azúcar 1kg', 'Abarrotes', 500, 700, 90, 20, 1),
('P005', 'Café molido 400g', 'Abarrotes', 1500, 2100, 40, 10, 1),
('P006', 'Leche 1L', 'Lácteos', 550, 780, 80, 15, 2),
('P007', 'Queso fresco 500g', 'Lácteos', 1800, 2500, 25, 8, 2),
('P008', 'Yogurt 1L', 'Lácteos', 900, 1300, 35, 10, 2),
('P009', 'Mantequilla 250g', 'Lácteos', 1100, 1550, 20, 5, 2),
('P010', 'Pechuga de pollo 1kg', 'Carnes', 2200, 3100, 30, 8, 3),
('P011', 'Carne molida 1kg', 'Carnes', 2800, 3900, 25, 6, 3),
('P012', 'Chorizo 500g', 'Carnes', 1600, 2200, 18, 5, 3),
('P013', 'Pan cuadrado', 'Panadería', 900, 1300, 40, 10, 4),
('P014', 'Pan dulce (unidad)', 'Panadería', 150, 250, 100, 20, 4),
('P015', 'Tortillas de maíz 1kg', 'Panadería', 500, 750, 60, 15, 4),
('P016', 'Refresco de cola 2L', 'Bebidas', 900, 1300, 70, 15, 1),
('P017', 'Agua embotellada 1L', 'Bebidas', 350, 550, 120, 25, 1),
('P018', 'Jugo de naranja 1L', 'Bebidas', 800, 1150, 45, 10, 1),
('P019', 'Detergente en polvo 1kg', 'Limpieza', 1400, 1950, 30, 8, 5),
('P020', 'Jabón de baño (unidad)', 'Higiene Personal', 400, 600, 3, 10, 5);
GO

-- Clientes
INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES
('1-1111-1111', 'María Fernández Solano', '8888-1111', 'maria.fernandez@correo.com'),
('2-2222-2222', 'Carlos Rojas Vindas', '8888-2222', 'carlos.rojas@correo.com'),
('3-3333-3333', 'Ana Gómez Castro', '8888-3333', 'ana.gomez@correo.com'),
('4-4444-4444', 'Luis Chacón Araya', '8888-4444', 'luis.chacon@correo.com');
GO

-- Ventas de ejemplo (para que Reportes muestre datos desde el primer momento)
INSERT INTO Ventas (Fecha, ClienteId, UsuarioId, Total, MetodoPago) VALUES
(DATEADD(DAY, -3, GETDATE()), 1, 1, 3810, 'Efectivo'),
(DATEADD(DAY, -1, GETDATE()), 2, 2, 5100, 'Tarjeta'),
(GETDATE(), 3, 2, 2100, 'SINPE Móvil');
GO

INSERT INTO DetalleVenta (VentaId, ProductoId, Cantidad, PrecioUnitario) VALUES
(1, 1, 2, 850), (1, 6, 2, 780), (1, 17, 1, 550),
(2, 10, 1, 3100), (2, 16, 1, 1300), (2, 4, 1, 700),
(3, 5, 1, 2100);
GO

-- Movimientos de inventario correspondientes a las ventas de ejemplo
INSERT INTO MovimientoInventario (ProductoId, Fecha, Cantidad, Motivo, UsuarioId) VALUES
(1, DATEADD(DAY, -3, GETDATE()), -2, 'Venta', 1),
(6, DATEADD(DAY, -3, GETDATE()), -2, 'Venta', 1),
(17, DATEADD(DAY, -3, GETDATE()), -1, 'Venta', 1),
(10, DATEADD(DAY, -1, GETDATE()), -1, 'Venta', 2),
(16, DATEADD(DAY, -1, GETDATE()), -1, 'Venta', 2),
(4, DATEADD(DAY, -1, GETDATE()), -1, 'Venta', 2),
(5, GETDATE(), -1, 'Venta', 2);
GO
