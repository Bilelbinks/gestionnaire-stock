using System;
using System.Windows.Forms;
using GestionnaireStock.Data;
using GestionnaireStock.Models;

namespace GestionnaireStock.Forms
{
    public class MouvementForm : Form
    {
        private readonly MouvementRepository _mouvementRepository = new();
        private readonly Produit _produit;

        private readonly ComboBox _cmbType = new() { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown _numQuantite = new() { Width = 200, Minimum = 1, Maximum = 100000, Value = 1 };
        private readonly TextBox _txtCommentaire = new() { Width = 200 };

        public MouvementForm(Produit produit)
        {
            _produit = produit;

            Text = $"Mouvement — {produit.Nom}";
            Width = 340;
            Height = 280;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;

            _cmbType.Items.Add("Entrée (réapprovisionnement)");
            _cmbType.Items.Add("Sortie (vente / retrait)");
            _cmbType.SelectedIndex = 0;

            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(15) };
            layout.Controls.Add(new Label { Text = $"Stock actuel : {produit.Quantite}", AutoSize = true }, 0, 0);
            layout.SetColumnSpan(layout.Controls[0], 2);
            layout.Controls.Add(new Label { Text = "Type", AutoSize = true }, 0, 1);
            layout.Controls.Add(_cmbType, 1, 1);
            layout.Controls.Add(new Label { Text = "Quantité", AutoSize = true }, 0, 2);
            layout.Controls.Add(_numQuantite, 1, 2);
            layout.Controls.Add(new Label { Text = "Commentaire", AutoSize = true }, 0, 3);
            layout.Controls.Add(_txtCommentaire, 1, 3);

            var btnValider = new Button { Text = "Enregistrer" };
            var btnAnnuler = new Button { Text = "Annuler", DialogResult = DialogResult.Cancel };
            btnValider.Click += BtnValider_Click;

            var panelBoutons = new FlowLayoutPanel { Dock = DockStyle.Bottom, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(10) };
            panelBoutons.Controls.Add(btnAnnuler);
            panelBoutons.Controls.Add(btnValider);

            Controls.Add(layout);
            Controls.Add(panelBoutons);
            AcceptButton = btnValider;
            CancelButton = btnAnnuler;
        }

        private void BtnValider_Click(object? sender, EventArgs e)
        {
            var type = _cmbType.SelectedIndex == 0 ? TypeMouvement.Entree : TypeMouvement.Sortie;
            var commentaire = string.IsNullOrWhiteSpace(_txtCommentaire.Text) ? null : _txtCommentaire.Text.Trim();

            try
            {
                _mouvementRepository.Ajouter(_produit.Id, type, (int)_numQuantite.Value, commentaire);
                DialogResult = DialogResult.OK;
            }
            catch (StockInsuffisantException ex)
            {
                MessageBox.Show(ex.Message, "Stock insuffisant", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
