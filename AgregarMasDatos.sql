-- =========================================================
-- Script para AGREGAR más datos de ejemplo a una base de
-- datos que ya existe (aunque ya tengas productos metidos
-- a mano). No borra nada, y no duplica: cada producto se
-- agrega solo si su Código todavía no existe.
-- =========================================================

USE SupermercadoDB;
GO

-- Proveedores (se agregan solo si el nombre no existe todavía)
IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Lácteos del Valle')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Lácteos del Valle', '2233-4455', 'contacto@lacteosvalle.cr', 'Cartago, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Carnicería El Buen Corte')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Carnicería El Buen Corte', '2244-5566', 'pedidos@buencorte.cr', 'Heredia, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Panificadora Dorada')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Panificadora Dorada', '2255-6677', 'info@panidorada.cr', 'Alajuela, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Limpieza Total S.A.')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Limpieza Total S.A.', '2266-7788', 'ventas@limpiezatotal.cr', 'San José, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Distribuidora Central S.A.')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Distribuidora Central S.A.', '2222-1111', 'ventas@distcentral.cr', 'San José, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Frutas y Verduras La Cosecha')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Frutas y Verduras La Cosecha', '2277-8899', 'pedidos@lacosecha.cr', 'Cartago, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Congelados del Norte')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Congelados del Norte', '2288-9900', 'ventas@congenorte.cr', 'Alajuela, Costa Rica');

IF NOT EXISTS (SELECT 1 FROM Proveedores WHERE Nombre = 'Snacks y Golosinas CR')
    INSERT INTO Proveedores (Nombre, Telefono, Correo, Direccion) VALUES ('Snacks y Golosinas CR', '2299-0011', 'info@snackscr.cr', 'San José, Costa Rica');
GO

DECLARE @provLacteos INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Lácteos del Valle');
DECLARE @provCarnes INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Carnicería El Buen Corte');
DECLARE @provPan INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Panificadora Dorada');
DECLARE @provLimpieza INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Limpieza Total S.A.');
DECLARE @provCentral INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Distribuidora Central S.A.');
DECLARE @provFrutas INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Frutas y Verduras La Cosecha');
DECLARE @provCongelados INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Congelados del Norte');
DECLARE @provSnacks INT = (SELECT TOP 1 Id FROM Proveedores WHERE Nombre = 'Snacks y Golosinas CR');

-- Productos (cada uno se agrega solo si su código todavía no existe)
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P001') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P001', 'Arroz 1kg', 'Abarrotes', 600, 850, 100, 20, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P002') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P002', 'Aceite 900ml', 'Abarrotes', 1200, 1650, 50, 10, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P003') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P003', 'Frijoles 900g', 'Abarrotes', 700, 950, 60, 15, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P004') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P004', 'Azúcar 1kg', 'Abarrotes', 500, 700, 90, 20, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P005') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P005', 'Café molido 400g', 'Abarrotes', 1500, 2100, 40, 10, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P006') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P006', 'Leche 1L', 'Lácteos', 550, 780, 80, 15, @provLacteos);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P007') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P007', 'Queso fresco 500g', 'Lácteos', 1800, 2500, 25, 8, @provLacteos);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P008') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P008', 'Yogurt 1L', 'Lácteos', 900, 1300, 35, 10, @provLacteos);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P009') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P009', 'Mantequilla 250g', 'Lácteos', 1100, 1550, 20, 5, @provLacteos);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P010') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P010', 'Pechuga de pollo 1kg', 'Carnes', 2200, 3100, 30, 8, @provCarnes);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P011') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P011', 'Carne molida 1kg', 'Carnes', 2800, 3900, 25, 6, @provCarnes);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P012') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P012', 'Chorizo 500g', 'Carnes', 1600, 2200, 18, 5, @provCarnes);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P013') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P013', 'Pan cuadrado', 'Panadería', 900, 1300, 40, 10, @provPan);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P014') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P014', 'Pan dulce (unidad)', 'Panadería', 150, 250, 100, 20, @provPan);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P015') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P015', 'Tortillas de maíz 1kg', 'Panadería', 500, 750, 60, 15, @provPan);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P016') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P016', 'Refresco de cola 2L', 'Bebidas', 900, 1300, 70, 15, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P017') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P017', 'Agua embotellada 1L', 'Bebidas', 350, 550, 120, 25, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P018') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P018', 'Jugo de naranja 1L', 'Bebidas', 800, 1150, 45, 10, @provCentral);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P019') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P019', 'Detergente en polvo 1kg', 'Limpieza', 1400, 1950, 30, 8, @provLimpieza);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P020') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P020', 'Jabón de baño (unidad)', 'Higiene Personal', 400, 600, 3, 10, @provLimpieza);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P021') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P021', 'Manzana roja (kg)', 'Frutas y Verduras', 900, 1300, 45, 10, @provFrutas);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P022') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P022', 'Banano (kg)', 'Frutas y Verduras', 350, 550, 70, 15, @provFrutas);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P023') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P023', 'Tomate (kg)', 'Frutas y Verduras', 600, 950, 40, 10, @provFrutas);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P024') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P024', 'Cebolla (kg)', 'Frutas y Verduras', 500, 800, 55, 12, @provFrutas);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P025') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P025', 'Papa (kg)', 'Frutas y Verduras', 450, 700, 60, 15, @provFrutas);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P026') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P026', 'Helado de vainilla 1L', 'Congelados', 1600, 2300, 22, 6, @provCongelados);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P027') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P027', 'Papas fritas congeladas 1kg', 'Congelados', 1400, 2000, 30, 8, @provCongelados);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P028') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P028', 'Nuggets de pollo 500g', 'Congelados', 1900, 2650, 18, 5, @provCongelados);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P029') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P029', 'Papitas tostadas 150g', 'Snacks', 700, 1050, 65, 15, @provSnacks);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P030') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P030', 'Galletas de chocolate 200g', 'Snacks', 600, 900, 8, 10, @provSnacks);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P031') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P031', 'Maní salado 200g', 'Snacks', 800, 1200, 40, 10, @provSnacks);
IF NOT EXISTS (SELECT 1 FROM Productos WHERE Codigo='P032') INSERT INTO Productos (Codigo, Nombre, Categoria, PrecioCompra, PrecioVenta, Existencias, StockMinimo, ProveedorId) VALUES ('P032', 'Chocolate en barra 100g', 'Snacks', 900, 1350, 50, 12, @provSnacks);
GO

-- Clientes (se agregan solo si la cédula no existe todavía)
IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '1-1111-1111')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('1-1111-1111', 'María Fernández Solano', '8888-1111', 'maria.fernandez@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '2-2222-2222')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('2-2222-2222', 'Carlos Rojas Vindas', '8888-2222', 'carlos.rojas@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '3-3333-3333')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('3-3333-3333', 'Ana Gómez Castro', '8888-3333', 'ana.gomez@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '4-4444-4444')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('4-4444-4444', 'Luis Chacón Araya', '8888-4444', 'luis.chacon@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '5-5555-5555')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('5-5555-5555', 'Paula Jiménez Solís', '8888-5555', 'paula.jimenez@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '6-6666-6666')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('6-6666-6666', 'Diego Vargas Mora', '8888-6666', 'diego.vargas@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '7-7777-7777')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('7-7777-7777', 'Sofía Alvarado Núñez', '8888-7777', 'sofia.alvarado@correo.com');

IF NOT EXISTS (SELECT 1 FROM Clientes WHERE Cedula = '8-8888-8888')
    INSERT INTO Clientes (Cedula, Nombre, Telefono, Correo) VALUES ('8-8888-8888', 'Esteban Méndez López', '8888-9999', 'esteban.mendez@correo.com');
GO

PRINT 'Listo: 8 proveedores, 32 productos y 8 clientes de ejemplo agregados (sin duplicar lo que ya tenías).';
