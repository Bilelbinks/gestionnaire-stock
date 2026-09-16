# Gestionnaire de stock

Application desktop C# (WinForms) pour gérer le stock d'un magasin :
catalogue de produits, entrées/sorties de stock, alertes de rupture, et
historique des mouvements.

## Fonctionnalités

- **Catalogue produits** — ajouter, modifier, supprimer (référence, nom,
  prix, seuil d'alerte)
- **Mouvements de stock** — entrée (réapprovisionnement) ou sortie
  (vente/retrait), avec vérification qu'il y a bien assez de stock pour
  une sortie
- **Alertes visuelles** — un produit dont la quantité est descendue au
  niveau du seuil d'alerte est mis en évidence dans la liste
- **Historique** — tous les mouvements enregistrés, avec date et produit
  concerné

La quantité en stock ne se modifie jamais directement : elle ne change
qu'au travers d'un mouvement enregistré, pour que l'historique reste
toujours cohérent avec le stock réel.

## Stack technique

- **C# / .NET 9** — WinForms (interface construite entièrement en code,
  sans le designer visuel)
- **SQL Server** — base relationnelle avec contrainte de clé étrangère
  (`ON DELETE CASCADE` sur les mouvements liés à un produit supprimé)
- **Microsoft.Data.SqlClient** — requêtes paramétrées (`SqlCommand` +
  `Parameters`), l'équivalent C# des requêtes préparées PDO
- **Transactions** — l'enregistrement d'un mouvement et la mise à jour du
  stock se font dans une seule transaction SQL : soit les deux réussissent,
  soit aucune n'est appliquée

## Installation

1. Un serveur SQL Server accessible (le projet utilise l'authentification
   Windows intégrée — adapte `Data/DatabaseHelper.cs` si besoin)
2. Crée la base et les données de départ :
```
sqlcmd -S "localhost\NOM_INSTANCE" -E -i schema.sql
```
3. Ouvre `GestionnaireStock/GestionnaireStock.csproj` avec Visual Studio,
   ou lance directement :
```
dotnet run --project GestionnaireStock
```

## Structure du projet

```
gestionnaire-stock/
├── schema.sql                      # tables + données de départ
└── GestionnaireStock/
    ├── Program.cs                     # point d'entrée
    ├── Models/
    │   ├── Produit.cs
    │   └── Mouvement.cs
    ├── Data/
    │   ├── DatabaseHelper.cs             # connexion SQL Server
    │   ├── ProduitRepository.cs           # CRUD produits
    │   └── MouvementRepository.cs           # enregistrement + transaction
    └── Forms/
        ├── MainForm.cs                       # liste des produits, actions
        ├── ProduitForm.cs                      # ajout/modification
        ├── MouvementForm.cs                      # entrée/sortie de stock
        └── HistoriqueForm.cs                       # liste des mouvements
```
