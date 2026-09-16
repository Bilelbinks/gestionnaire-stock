using System.Windows.Forms;
using GestionnaireStock.Data;

namespace GestionnaireStock.Forms
{
    public class HistoriqueForm : Form
    {
        public HistoriqueForm()
        {
            Text = "Historique des mouvements";
            Width = 700;
            Height = 450;
            StartPosition = FormStartPosition.CenterParent;

            var grille = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = new MouvementRepository().GetAll(),
            };

            if (grille.Columns["Id"] is DataGridViewColumn colId) colId.Visible = false;
            if (grille.Columns["ProduitId"] is DataGridViewColumn colProduitId) colProduitId.Visible = false;
            if (grille.Columns["ProduitNom"] is DataGridViewColumn colProduitNom) colProduitNom.HeaderText = "Produit";
            if (grille.Columns["Type"] is DataGridViewColumn colType) colType.HeaderText = "Type";
            if (grille.Columns["Date"] is DataGridViewColumn colDate) colDate.HeaderText = "Date";

            Controls.Add(grille);
        }
    }
}
