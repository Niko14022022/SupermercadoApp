-- Corrige cualquier producto que haya quedado con existencias negativas
-- (por ejemplo, si se editó manualmente antes de que el sistema lo bloqueara).
-- Los deja en 0 en vez de un número negativo.

USE SupermercadoDB;
GO

UPDATE Productos
SET Existencias = 0
WHERE Existencias < 0;
GO

-- Restricción a nivel de base de datos: ni siquiera un error de programación
-- va a poder dejar existencias negativas de aquí en adelante.
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CHK_Existencias_NoNegativo')
BEGIN
    ALTER TABLE Productos ADD CONSTRAINT CHK_Existencias_NoNegativo CHECK (Existencias >= 0);
END
GO

PRINT 'Listo: existencias negativas corregidas a 0, y se agregó una restricción para que no vuelva a pasar.';
