using System;
using System.Windows.Forms;


namespace PredictionAppApiClient
{
  internal static class Program
  {
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      ApiClient apiClient;

      using (var login = new LoginForm())
      {
        if (login.ShowDialog() == DialogResult.OK)
        {
          Glavna glavna = new Glavna();
          glavna.ApiClient = login.ApiClient;
          Application.Run(glavna);
        }
      }
    }
  }
}
