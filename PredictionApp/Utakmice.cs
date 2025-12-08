
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace PredictionApp
{
  public partial class Utakmice : Form
  {
    List<Match> utakmice = new List<Match>();
    List<Team> klubovi = new List<Team>();
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
    private void UcitajKlubove()
    {
      // e.g. populate cmbKlubovi here
      try
      {
        cmbKlubovi.ValueMember = "Id";
        cmbKlubovi.DisplayMember = "Name";
        //TODO: Pozvati sql upit za učitavanje klubova iz baze podataka
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

    private void UcitajUtakmice()
    {
      //TODO: Implementirati SQL upit za učitavanje utakmica iz baze podataka (prvo sve, kasnije sa filterima)
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
        //TODO: Pozvati sql upit za učitavanje utakmica iz baze podataka sa zadatim filterima (ne zaboraviti osvježiti grid)
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
