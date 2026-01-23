
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PredictionAppApiClient
{
  public partial class Klubovi : Form
  {
    public Klubovi()
    {
      InitializeComponent();
    }
    public ApiClient ApiClient { get; set; }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      LoadTeams();
    }

    private async Task LoadTeams()
    {
      try
      {
        var response = await ApiClient.GetAsync<List<Team>>(
          $"{Properties.Settings.Default.apiUrl}team");
        dgvKlubovi.DataSource = response.Data;


      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri učitavanju klubova: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
