

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace PredictionAppApiClient
{
  public partial class Prognoze : Form
  {
    List<Prediction> prognoze = new List<Prediction>();
    public ApiClient ApiClient { get; set; }
    public Prognoze()
    {
      InitializeComponent();
      dgvPrognoze.AutoGenerateColumns = false;
      dgvPrognoze.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      UcitajPrognoze();
    }

    async void  UcitajPrognoze()
    {
      try
      {
        var query = new { userName = txtUser.Text.Length > 0 ? txtUser.Text : null };
        var response = await ApiClient.GetAsync<List<Prediction>>(
          $"{Properties.Settings.Default.apiUrl}prediction",
          query);
        prognoze = response.Data;
        AzurirajGrid();

      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri učitavanju prognoza: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    void AzurirajGrid()
    {
      dgvPrognoze.DataSource = null;
      dgvPrognoze.DataSource = prognoze;
    }

    private async void btnDodaj_Click(object sender, EventArgs e)
    {
      var editForm = new PredictionEdit();
      editForm.ApiClient = ApiClient;
      //using (var dlg = new PredictionEdit())
      {
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
          var p = editForm.Result;
          try
          {

            await ApiClient.PostAsync<Prediction, Prediction>(
              $"{Properties.Settings.Default.apiUrl}prediction",
              p);

            //Zbog jednostavnosti, ponovno učitavamo sve prognoze iz baze nakon dodavanja nove (umjesto da samo dodajemo novi objekt u lokalnu listu) - zbog veza s drugim tablicama
            UcitajPrognoze();
            AzurirajGrid();

          }
          catch (Exception ex)
          {
            MessageBox.Show("Greška pri spremanju prognoze: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void btnUredi_Click(object sender, EventArgs e)
    {
      if (dgvPrognoze.SelectedRows.Count == 0)
      {
        MessageBox.Show("Odaberite prognozu za uređivanje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      var selected = dgvPrognoze.SelectedRows[0].DataBoundItem as Prediction;
      if (selected == null)
      {
        MessageBox.Show("Neispravan odabir.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      // Radimo kopiju odabranog objekta kako bismo izbjegli izmjene originalnog dok ne potvrdimo izmjene
      var kopija = new Prediction
      {
        Id = selected.Id,
        MatchId = selected.MatchId,
        PredictionTypeId = selected.PredictionTypeId,
        UserName = selected.UserName,
        CreatedAt = selected.CreatedAt
      };
      var editForm = new PredictionEdit();
      editForm.ApiClient = ApiClient;
      //using (var dlg = new PredictionEdit(kopija))
      {
        if (editForm.ShowDialog(this) == DialogResult.OK)
        {
          var p = editForm.Result;
          try
          {




            // Ažuriraj lokalnu listu i grid
            var item = prognoze.FirstOrDefault(x => x.Id == p.Id);
            if (item != null)
            {
              item.MatchId = p.MatchId;
              item.PredictionTypeId = p.PredictionTypeId;
              item.UserName = p.UserName;
              item.PredictionType = p.PredictionTypeId == 1 ? "1" : p.PredictionTypeId == 2 ? "2" : "X"; // Simplified for example
              AzurirajGrid();
            }

          }
          catch (Exception ex)
          {
            MessageBox.Show("Greška pri ažuriranju prognoze: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
          }
        }
      }
    }

    private void btnPretrazi_Click(object sender, EventArgs e)
    {

    }

    private void btnObrisi_Click(object sender, EventArgs e)
    {
      if (dgvPrognoze.SelectedRows.Count == 0)
      {
        MessageBox.Show("Odaberite prognozu za brisanje.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        return;
      }

      int selectedId = (int)dgvPrognoze.SelectedRows[0].Cells["colId"].Value;
      DialogResult dr = MessageBox.Show("Jeste li sigurni da želite obrisati odabranu prognozu?", "Potvrda brisanja", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
      if (dr != DialogResult.Yes)
        return;
      try
      {
        //TODO: Pozvati sql upit za brisanje prognoze iz baze podataka. Učitati ponovo listu prognoza nakon brisanja i osvježiti grid

        UcitajPrognoze();
        AzurirajGrid();
      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri brisanju prognoze: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
