using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;


namespace PredictionApp
{
  public partial class Klubovi : Form
  {
    public Klubovi()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      LoadTeams();
    }

    private void LoadTeams()
    {
      try
      {
        //TODO: Pozvati sql upit za učitavanje klubova iz baze podataka
        using (var connection = new SqlConnection(Properties.Settings.Default.connString)) 
        { 
            List<Team> klubovi = connection.Query<Team>("SELECT * FROM Team").ToList();
            dgvKlubovi.DataSource = klubovi;
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show("Greška pri učitavanju klubova: " + ex.Message, "Greška", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }
    }
  }
}
