using System;
using System.ComponentModel;
using System.Windows.Forms;
using GestionnaireStock.Models;

namespace GestionnaireStock.Forms
{
    public class ProduitForm : Form
    {
        private readonly TextBox _txtReference = new() { Width = 200 };
        private readonly TextBox _txtNom = new() { Width = 200 };
        private readonly NumericUpDown _numPrix = new() { Width = 200, DecimalPlaces = 2, Maximum = 100000 };
        private readonly NumericUpDown _numQuantite = new() { Width = 200, Maximum = 100000 };
        private readonly NumericUpDown _numSeuil = new() { Width = 200, Maximum = 100000 };
        private readonly bool _modeEdition;
        private readonly int _idExistant;

        // Propriété purement applicative (récupérer le résultat de la boîte de
        // dialogue) — pas destinée au designer visuel, d'où l'attribut ci-dessous.
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Produit Produit { get; private set; } = new();

        public ProduitForm() : this(null) { }

        public ProduitForm(Produit? produitExistant)
        {
            _modeEdition = produitExistant != null;
            _idExistant = produitExistant?.Id ?? 0;

            Text = _modeEdition ? "Modifier le produit" : "Ajouter un produit";
            Width = 340;
            Height = 300;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(15) };
            layout.Controls.Add(new Label { Text = "Référence", AutoSize = true }, 0, 0);
            layout.Controls.Add(_txtReference, 1, 0);
            layout.Controls.Add(new Label { Text = "Nom", AutoSize = true }, 0, 1);
            layout.Controls.Add(_txtNom, 1, 1);
            layout.Controls.Add(new Label { Text = "Prix (€)", AutoSize = true }, 0, 2);
            layout.Controls.Add(_numPrix, 1, 2);
            layout.Controls.Add(new Label { Text = "Quantité initiale", AutoSize = true }, 0, 3);
            layout.Controls.Add(_numQuantite, 1, 3);
            layout.Controls.Add(new Label { Text = "Seuil d'alerte", AutoSize = true }, 0, 4);
            layout.Controls.Add(_numSeuil, 1, 4);

            if (_modeEdition)
            {
                // La quantité ne se modifie que via un mouvement (entrée/sortie),
                // jamais directement, pour garder l'historique cohérent avec le stock réel.
                _numQuantite.Enabled = false;
            }

            var btnValider = new Button { Text = _modeEdition ? "Enregistrer" : "Ajouter", DialogResult = DialogResult.OK };
            var btnAnnuler = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel };
            btnValider.Click += BtnValider_Click;

            var panelBoutons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10) };
            panelBoutons.Controls.Add(btnAnnuler);
            panelBoutons.Controls.Add(btnValider);

            Controls.Add(layout);
            Controls.Add(panelBoutons);
            AcceptButton = btnValider;
            CancelButton = btnAnnuler;

            if (produitExistant != null)
            {
                _txtReference.Text = produitExistant.Reference;
                _txtNom.Text = produitExistant.Nom;
                _numPrix.Value = produitExistant.Prix;
                _numQuantite.Value = produitExistant.Quantite;
                _numSeuil.Value = produitExistant.SeuilAlerte;
            }
        }

        private void BtnValider_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtReference.Text) || string.IsNullOrWhiteSpace(_txtNom.Text))
            {
                MessageBox.Show("La référence et le nom sont obligatoires.", "Champs manquants");
                DialogResult = DialogResult.None;
                return;
            }

            Produit = new Produit
            {
                Id = _idExistant,
                Reference = _txtReference.Text.Trim(),
                Nom = _txtNom.Text.Trim(),
                Prix = _numPrix.Value,
                Quantite = (int)_numQuantite.Value,
                SeuilAlerte = (int)_numSeuil.Value,
            };
        }
    }
}
