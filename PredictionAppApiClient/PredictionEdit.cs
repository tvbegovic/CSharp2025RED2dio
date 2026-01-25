
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace PredictionAppApiClient
{
  public partial class PredictionEdit : Form
  {
    private Prediction editing;

    public Prediction Result { get; private set; }
    public ApiClient ApiClient { get; set; }
    internal List<MatchItem> Matches { get; set; }
    internal List<PredictionType> PredictionTypes { get; set; }

    public PredictionEdit()
    {
      InitializeComponent();
    }

    public PredictionEdit(Prediction toEdit) : this()
    {
      editing = toEdit;
    }

    protected override async void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      await UcitajUtakmice();
      await UcitajTipovePrognoza();

      if (editing != null)
      {
        txtId.Text = editing.Id.ToString();
        // select match in combo
        //if (cmbMatch.DataSource is List<Match> matches)
        {
          var sel = Matches.FirstOrDefault(m => m.Id == editing.MatchId);
          if (sel != null) cmbMatch.SelectedItem = sel;
        }

        //if (cmbPredictionType.DataSource is List<PredictionType> pt)
        {
          var sel = PredictionTypes.FirstOrDefault(p => p.Id == editing.PredictionTypeId);
          if (sel != null) cmbPredictionType.SelectedItem = sel;
        }

        txtUserName.Text = editing.UserName;
        txtCreatedAt.Text = editing.CreatedAt.ToString("g");
      }
      else
      {
        txtId.Text = "0";
        txtCreatedAt.Text = DateTime.Now.ToString("g");
      }
    }

    private async Task UcitajUtakmice()
    {
      try
      {

        var response = await ApiClient.GetAsync<List<Match>>($"{Properties.Settings.Default.apiUrl}match/search");
        var items = response.Data;



        Matches = items.Select(m => new MatchItem { Match = m, Display = $"{m.MatchDate:g} - {m.HomeTeam} vs {m.AwayTeam}" }).ToList();
          cmbMatch.DataSource = Matches;
          cmbMatch.DisplayMember = "Display";
          cmbMatch.ValueMember = "Match"; // ValueMember won't map to object, we'll get SelectedItem as MatchItem
        
      }
      catch
      {
        // ignore - user can populate later
      }
    }

    private async Task UcitajTipovePrognoza()
    {
      try
      {

        var response = await ApiClient.GetAsync<List<PredictionType>>(
          $"{Properties.Settings.Default.apiUrl}prediction/types");
        PredictionTypes = response.Data;

        cmbPredictionType.DataSource = PredictionTypes;
        cmbPredictionType.DisplayMember = "Name";
        cmbPredictionType.ValueMember = "Id";

      }
      catch
      {
        // ignore
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      // Basic validation
      if (cmbMatch.SelectedItem == null)
      {
        MessageBox.Show("Odaberite utakmicu.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (cmbPredictionType.SelectedItem == null)
      {
        MessageBox.Show("Odaberite tip prognoze.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      if (string.IsNullOrWhiteSpace(txtUserName.Text))
      {
        MessageBox.Show("Unesite korisničko ime.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      DateTime createdAt = DateTime.Now;
      DateTime.TryParse(txtCreatedAt.Text.Trim(), out createdAt);

      var matchItem = cmbMatch.SelectedItem as MatchItem;
      Match selectedMatch = matchItem?.Match;
      if (selectedMatch == null)
      {
        MessageBox.Show("Neispravan odabir utakmice.", "Greška", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return;
      }

      var predType = cmbPredictionType.SelectedItem as PredictionType;

      var p = editing ?? new Prediction();
      p.MatchId = selectedMatch.Id;
      p.PredictionTypeId = predType.Id;
      p.UserName = txtUserName.Text.Trim();
      p.CreatedAt = createdAt;

      Result = p;
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    // small wrapper for display in combo
    internal class MatchItem
    {
      public Match Match { get; set; }
      public int Id => Match.Id;
      public string Display { get; set; }
      public override string ToString() => Display;
    }
  }
}
