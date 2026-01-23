namespace PredictionAppApiClient
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.lblUser = new System.Windows.Forms.Label();
      this.lblPass = new System.Windows.Forms.Label();
      this.btnLogin = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txtUsername
      // 
      this.txtUsername.Location = new System.Drawing.Point(82, 19);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.Size = new System.Drawing.Size(151, 20);
      this.txtUsername.TabIndex = 0;
      this.txtUsername.Text = "user@gmail.com";
      // 
      // txtPassword
      // 
      this.txtPassword.Location = new System.Drawing.Point(82, 52);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(151, 20);
      this.txtPassword.TabIndex = 1;
      this.txtPassword.Text = "password";
      // 
      // lblUser
      // 
      this.lblUser.AutoSize = true;
      this.lblUser.Location = new System.Drawing.Point(20, 22);
      this.lblUser.Name = "lblUser";
      this.lblUser.Size = new System.Drawing.Size(35, 13);
      this.lblUser.TabIndex = 2;
      this.lblUser.Text = "Email:";
      // 
      // lblPass
      // 
      this.lblPass.AutoSize = true;
      this.lblPass.Location = new System.Drawing.Point(20, 55);
      this.lblPass.Name = "lblPass";
      this.lblPass.Size = new System.Drawing.Size(56, 13);
      this.lblPass.TabIndex = 3;
      this.lblPass.Text = "Password:";
      // 
      // btnLogin
      // 
      this.btnLogin.Location = new System.Drawing.Point(169, 87);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.Size = new System.Drawing.Size(64, 20);
      this.btnLogin.TabIndex = 4;
      this.btnLogin.Text = "Login";
      this.btnLogin.UseVisualStyleBackColor = true;
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // LoginForm
      // 
      this.AcceptButton = this.btnLogin;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(252, 125);
      this.Controls.Add(this.btnLogin);
      this.Controls.Add(this.lblPass);
      this.Controls.Add(this.lblUser);
      this.Controls.Add(this.txtPassword);
      this.Controls.Add(this.txtUsername);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "LoginForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Login";
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblUser;
        private System.Windows.Forms.Label lblPass;
        private System.Windows.Forms.Button btnLogin;
    }
}