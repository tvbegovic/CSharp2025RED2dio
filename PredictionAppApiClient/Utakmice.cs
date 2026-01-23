

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredictionAppApiClient
{
  public partial class Utakmice : Form
  {
    List<Match> utakmice = new List<Match>();
    List<Team> klubovi = new List<Team>();
    public ApiClient ApiClient { get; set; }
    public Utakmice()
    {
      InitializeComponent();
      dgvUtakmice.AutoGenerateColumns = false;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      UcitajKlubove();
      UcitajUtakmice();
    }

    // Placeholder: user will implement SQL loading later
    private async Task UcitajKlubove()
    {
      // e.g. populate cmbKlubovi here
      try
      {
        cmbKlubovi.ValueMember = "Id";
        cmbKlubovi.DisplayMember = "Name";
        var response = await ApiClient.GetAsync<List<Team>>(
          $"{Properties.Settings.Default.apiUrl}team");
        klubovi = response.Data;
        if (klubovi != null)
        {

          Team sviKlubovi = new Team();
          sviKlubovi.Id = 0;
          sviKlubovi.Name = "Svi klubovi";
          klubovi.Insert(0, sviKlubovi);
          cmbKlubovi.DataSource = klubovi;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri učitavanju klubova: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }

    private void btnPretrazi_Click(object sender, EventArgs e)
    {
      UcitajUtakmice();
    }

    private async void UcitajUtakmice()
    {
      
      string text = txtTekst.Text.Trim();
      int? klubId = (cmbKlubovi.SelectedItem as Team)?.Id;
      DateTime? dateFrom = null;
      DateTime? dateTo = null;
      if (klubId == 0)
        klubId = null;
      bool ok = DateTime.TryParse(txtDatumOd.Text.Trim(), out DateTime dtFrom);
      if (ok)
        dateFrom = dtFrom;
      ok = DateTime.TryParse(txtDatumDo.Text.Trim(), out DateTime dtTo);
      if (ok)
        dateTo = dtTo;
      try
      {
        var query = new { teamId = klubId, dateFrom, dateTo };
        var response = await ApiClient.GetAsync<List<Match>>(
         $"{Properties.Settings.Default.apiUrl}match/search", query);
        utakmice = response.Data;
        AzurirajGrid();

      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri učitavanju utakmica: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);

      }
    }

    void AzurirajGrid() 
    {
      dgvUtakmice.DataSource = null;
      dgvUtakmice.DataSource = utakmice;
    }
  }
}
