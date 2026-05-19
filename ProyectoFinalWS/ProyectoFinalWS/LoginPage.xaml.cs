namespace ProyectoFinalWS
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void btnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text) || string.IsNullOrEmpty(txtContrasena.Text))
            {
                lblError.Text = "Por favor ingresa usuario y contraseña";
                return;
            }

            btnLogin.IsEnabled = false;
            btnLogin.Text = "Verificando...";
            lblError.Text = "";

            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                bool acceso = datos.ValidarLogin(txtUsuario.Text, txtContrasena.Text);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (acceso)
                    {
                        Application.Current!.Windows[0].Page = new AppShell();
                    }
                    else
                    {
                        lblError.Text = "Usuario o contraseña incorrectos";
                        btnLogin.IsEnabled = true;
                        btnLogin.Text = "Ingresar";
                    }
                });
            });
        }
    }
}