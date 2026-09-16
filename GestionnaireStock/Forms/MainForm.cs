using System;
using System.Drawing;
using System.Windows.Forms;
using GestionnaireStock.Data;
using GestionnaireStock.Models;

namespace GestionnaireStock.Forms
{
    public class MainForm : Form
    {
        private readonly ProduitRepository _produitRepository = new();
        private DataGridView _grille = null!;
        private Button _btnAjouter = null!;
        private Button _btnModifier = null!;
        private Button _btnSupprimer = null!;
        private Button _btnMouvement = null!;
        private Button _btnHistorique = null!;

        public MainForm()
        {
            ConstruireInterface();
            ChargerProduits();
        }

        private void ConstruireInterface()
        {
            Text = "Gestionnaire de stock";
            Width = 900;
            Height = 550;
            StartPosition = FormStartPosition.CenterScreen;

            _grille = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 400,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };
            _grille.CellFormatting += Grille_CellFormatting;

            var panelBoutons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
            };

            _btnAjouter = new Button { Text = "Ajouter un produit", AutoSize = true };
            _btnModifier = new Button { Text = "Modifier", AutoSize = true };
            _btnSupprimer = new Button { Text = "Supprimer", AutoSize = true };
            _btnMouvement = new Button { Text = "Enregistrer un mouvement", AutoSize = true };
            _btnHistorique = new Button { Text = "Historique", AutoSize = true };

            _btnAjouter.Click += BtnAjouter_Click;
            _btnModifier.Click += BtnModifier_Click;
            _btnSupprimer.Click += BtnSupprimer_Click;
            _btnMouvement.Click += BtnMouvement_Click;
            _btnHistorique.Click += BtnHistorique_Click;

            panelBoutons.Controls.Add(_btnAjouter);
            panelBoutons.Controls.Add(_btnModifier);
            panelBoutons.Controls.Add(_btnSupprimer);
            panelBoutons.Controls.Add(_btnMouvement);
            panelBoutons.Controls.Add(_btnHistorique);

            Controls.Add(_grille);
            Controls.Add(panelBoutons);
        }

        private void ChargerProduits()
        {
            var produits = _produitRepository.GetAll();
            _grille.DataSource = null;
            _grille.DataSource = produits;

            SetColumnVisible("EnAlerte", false);
            SetColumnVisible("Id", false);
            SetColumnHeader("Reference", "Référence");
            SetColumnHeader("SeuilAlerte", "Seuil d'alerte");
        }

        private void SetColumnVisible(string nomColonne, bool visible)
        {
            if (_grille.Columns[nomColonne] is DataGridViewColumn colonne)
            {
                colonne.Visible = visible;
            }
        }

        private void SetColumnHeader(string nomColonne, string entete)
        {
            if (_grille.Columns[nomColonne] is DataGridViewColumn colonne)
            {
                colonne.HeaderText = entete;
            }
        }

        // Colore en rouge clair la ligne d'un produit dont le stock est
        // descendu au niveau (ou en dessous) de son seuil d'alerte.
        private void Grille_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (_grille.Rows[e.RowIndex].DataBoundItem is Produit produit && produit.EnAlerte)
            {
                _grille.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.MistyRose;
            }
        }

        private Produit? ProduitSelectionne()
        {
            if (_grille.CurrentRow?.DataBoundItem is Produit produit)
            {
                return produit;
            }
            MessageBox.Show("Sélectionne d'abord un produit dans la liste.", "Aucune sélection");
            return null;
        }

        private void BtnAjouter_Click(object? sender, EventArgs e)
        {
            using var form = new ProduitForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _produitRepository.Add(form.Produit);
                ChargerProduits();
            }
        }

        private void BtnModifier_Click(object? sender, EventArgs e)
        {
            var produit = ProduitSelectionne();
            if (produit == null) return;

            using var form = new ProduitForm(produit);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _produitRepository.Update(form.Produit);
                ChargerProduits();
            }
        }

        private void BtnSupprimer_Click(object? sender, EventArgs e)
        {
            var produit = ProduitSelectionne();
            if (produit == null) return;

            var confirmation = MessageBox.Show(
                $"Supprimer « {produit.Nom} » ? Son historique de mouvements sera aussi supprimé.",
                "Confirmer la suppression",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmation == DialogResult.Yes)
            {
                _produitRepository.Delete(produit.Id);
                ChargerProduits();
            }
        }

        private void BtnMouvement_Click(object? sender, EventArgs e)
        {
            var produit = ProduitSelectionne();
            if (produit == null) return;

            using var form = new MouvementForm(produit);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                ChargerProduits();
            }
        }

        private void BtnHistorique_Click(object? sender, EventArgs e)
        {
            using var form = new HistoriqueForm();
            form.ShowDialog(this);
        }
    }
}
