using System;
using System.Windows.Forms;

namespace PredictionAppApiClient
{
  public partial class LoginForm : Form
  {
    public LoginForm()
    {
      InitializeComponent();
    }
    public ApiClient ApiClient { get; set; }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
      // For scaffolding: accept any non-empty credentials
      if (!string.IsNullOrWhiteSpace(txtUsername.Text) && !string.IsNullOrWhiteSpace(txtPassword.Text))
      {
        ApiClient = new ApiClient(Properties.Settings.Default.apiUrl);
        var response = await ApiClient.LoginAsync(
          $"{Properties.Settings.Default.apiUrl}user/login",
          txtUsername.Text,
          txtPassword.Text);
        if (response.StatusCode == System.Net.HttpStatusCode.OK) 
        {           
          this.DialogResult = DialogResult.OK;
          this.Close();
        }
        else
        {
          MessageBox.Show("Login failed. Please check your credentials.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }
      else
      {
        MessageBox.Show("Please enter username and password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
      }
    }
  }
}