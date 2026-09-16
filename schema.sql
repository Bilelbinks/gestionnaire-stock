IF DB_ID('GestionnaireStockDB') IS NULL
BEGIN
    CREATE DATABASE GestionnaireStockDB;
END
GO

USE GestionnaireStockDB;
GO

IF OBJECT_ID('Mouvements', 'U') IS NOT NULL DROP TABLE Mouvements;
IF OBJECT_ID('Produits', 'U') IS NOT NULL DROP TABLE Produits;
GO

CREATE TABLE Produits (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Reference NVARCHAR(50) NOT NULL UNIQUE,
    Nom NVARCHAR(100) NOT NULL,
    Prix DECIMAL(10,2) NOT NULL,
    Quantite INT NOT NULL DEFAULT 0,
    SeuilAlerte INT NOT NULL DEFAULT 5
);
GO

CREATE TABLE Mouvements (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProduitId INT NOT NULL,
    TypeMouvement NVARCHAR(10) NOT NULL CHECK (TypeMouvement IN ('Entree', 'Sortie')),
    Quantite INT NOT NULL CHECK (Quantite > 0),
    DateMouvement DATETIME NOT NULL DEFAULT GETDATE(),
    Commentaire NVARCHAR(255) NULL,
    FOREIGN KEY (ProduitId) REFERENCES Produits(Id) ON DELETE CASCADE
);
GO

INSERT INTO Produits (Reference, Nom, Prix, Quantite, SeuilAlerte) VALUES
    ('REF-001', 'T-shirt noir', 14.90, 32, 10),
    ('REF-002', 'Jean slim bleu', 39.90, 4, 5),
    ('REF-003', 'Casquette logo', 9.90, 18, 8),
    ('REF-004', 'Sneakers blanches', 59.90, 2, 5);
GO
