namespace ProyectoFinalWS
{
    public partial class PublicidadPage : ContentPage
    {
        readonly List<string> nombresCampanas = new List<string>
        {
            "Facebook Enero 2024",
            "Facebook Abril 2024",
            "Facebook Julio 2024",
            "Facebook Octubre 2024",
            "Facebook Enero 2025",
            "Instagram Marzo 2024",
            "Instagram Junio 2024",
            "Instagram Septiembre 2024",
            "Instagram Marzo 2025"
        };

        readonly List<dynamic> datosCampanas = new List<dynamic>
        {
            new { canal = "Facebook", inversion = 500, alcance = 1200, solicitudes = 48,  inscritos = 10 },
            new { canal = "Facebook", inversion = 500, alcance = 980,  solicitudes = 39,  inscritos = 8  },
            new { canal = "Facebook", inversion = 500, alcance = 1100, solicitudes = 55,  inscritos = 12 },
            new { canal = "Facebook", inversion = 500, alcance = 1050, solicitudes = 42,  inscritos = 9  },
            new { canal = "Facebook", inversion = 500, alcance = 1000, solicitudes = 50,  inscritos = 12 },
            new { canal = "Instagram", inversion = 400, alcance = 800, solicitudes = 96,  inscritos = 20 },
            new { canal = "Instagram", inversion = 400, alcance = 850, solicitudes = 102, inscritos = 22 },
            new { canal = "Instagram", inversion = 400, alcance = 900, solicitudes = 108, inscritos = 24 },
            new { canal = "Instagram", inversion = 400, alcance = 820, solicitudes = 98,  inscritos = 21 },
        };

        // Promedios precalculados
        readonly double promedioConvFacebook = 5.2;  // promedio tasa interés FB
        readonly double promedioConvInstagram = 12.1; // promedio tasa interés IG
        readonly double promedioInscFacebook = 10.2; // promedio inscritos FB
        readonly double promedioInscInstagram = 21.8; // promedio inscritos IG

        public PublicidadPage()
        {
            InitializeComponent();
            foreach (var n in nombresCampanas)
                pickerCampana.Items.Add(n);
            CargarComparativaCanales();
        }

        private void CargarComparativaCanales()
        {
            colecCanales.ItemsSource = new List<object>
            {
                new {
                    canal           = "📘 Facebook (promedio)",
                    conversion      = "5.2% 🔴",
                    colorConversion = "#A32D2D",
                    progreso        = 0.052,
                    colorBarra      = "#E24B4A",
                    detalle         = "1,066 alcance → 47 solicitudes → 10 inscritos promedio"
                },
                new {
                    canal           = "📸 Instagram (promedio)",
                    conversion      = "12.1% 🟡",
                    colorConversion = "#854F0B",
                    progreso        = 0.121,
                    colorBarra      = "#9C8442",
                    detalle         = "843 alcance → 101 solicitudes → 22 inscritos promedio"
                }
            };
        }

        private void OnCampanaChanged(object sender, EventArgs e)
        {
            if (pickerCampana.SelectedIndex < 0) return;

            var camp = datosCampanas[pickerCampana.SelectedIndex];

            int alcance = camp.alcance;
            int solicitudes = camp.solicitudes;
            int inscritos = camp.inscritos;
            int inversion = camp.inversion;
            string canal = camp.canal;


            lblAlcance.Text = alcance.ToString("N0");
            lblInversion.Text = $"${inversion:N0}";


            lblEmbudoAlcance.Text = alcance.ToString("N0");
            lblEmbudoSolicitudes.Text = solicitudes.ToString();
            lblEmbudoInscritos.Text = inscritos.ToString();


            double tasaInteres = solicitudes * 100.0 / alcance;
            double tasaConversion = inscritos * 100.0 / solicitudes;
            double costoAlumno = inversion * 1.0 / inscritos;

            
            lblEtiqueta1.Text = tasaInteres < 10
                ? $"⚠️ Solo el {tasaInteres:N1}% sigue"
                : $"✅ El {tasaInteres:N1}% sigue";

            lblEtiqueta2.Text = $"El {tasaConversion:N1}% se inscribe";
            lblSubSolicitudes.Text = $"De cada {alcance:N0} solo {solicitudes} preguntan";
            lblSubInscritos.Text = $"De cada {alcance:N0} visitas, solo {inscritos} se inscriben";

            
            string emojiInteres = tasaInteres < 5 ? "🔴" : tasaInteres < 15 ? "🟡" : "🟢";
            string emojiConversion = tasaConversion < 15 ? "🟡" : "🟢";
            string textoInteres, textoConversion, textoCosto;

            if (tasaInteres < 5)
                textoInteres = $"{tasaInteres:N1}% {emojiInteres} — Muy bajo. De cada 100 personas, menos de 5 preguntan.";
            else if (tasaInteres < 15)
                textoInteres = $"{tasaInteres:N1}% {emojiInteres} — Aceptable. De cada 100 personas, {tasaInteres:N0} preguntan.";
            else
                textoInteres = $"{tasaInteres:N1}% {emojiInteres} — Excelente. El anuncio genera mucho interés.";

            if (tasaConversion < 15)
                textoConversion = $"{tasaConversion:N1}% {emojiConversion} — De cada 100 que preguntan, {tasaConversion:N0} se inscriben.";
            else
                textoConversion = $"{tasaConversion:N1}% {emojiConversion} — Buena conversión de interesados a inscritos.";

            if (costoAlumno < 30)
                textoCosto = $"${costoAlumno:N2} por alumno 🟢 — Muy eficiente.";
            else if (costoAlumno < 60)
                textoCosto = $"${costoAlumno:N2} por alumno 🟡 — Aceptable, pero se puede mejorar.";
            else
                textoCosto = $"${costoAlumno:N2} por alumno 🔴 — Alto costo de captación, revisar estrategia.";

            lblTasaInteres.Text = textoInteres;
            lblTasaConversion.Text = textoConversion;
            lblCostoAlumno.Text = textoCosto;

            progInteres.Progress = Math.Min(tasaInteres / 100, 1);
            progConversion.Progress = Math.Min(tasaConversion / 100, 1);

            
            string canalContrario = canal == "Facebook" ? "Instagram" : "Facebook";
            double promedioEste = canal == "Facebook" ? promedioConvFacebook : promedioConvInstagram;
            double promedioOtro = canal == "Facebook" ? promedioConvInstagram : promedioConvFacebook;
            double inscEste = canal == "Facebook" ? promedioInscFacebook : promedioInscInstagram;
            double inscOtro = canal == "Facebook" ? promedioInscInstagram : promedioInscFacebook;

            
            string mensajeCanal;
            string colorMensaje;

            if (tasaInteres > promedioOtro)
            {
                
                double ventaja = tasaInteres / promedioOtro;
                mensajeCanal = $"✅ Esta campaña de {canal} ({tasaInteres:N1}%) supera el promedio " +
                               $"de {canalContrario} ({promedioOtro:N1}%). " +
                               $"Es {ventaja:N1}x más efectiva que el canal alternativo. " +
                               $"Se recomienda mantener la inversión en {canal} y replicar " +
                               $"el contenido de este anuncio en futuras campañas.";
                colorMensaje = "#085041";
            }
            else
            {
                
                double ventajaOtro = promedioOtro / tasaInteres;
                double extraInscritos = inscOtro - inscritos;
                mensajeCanal = $"⚠️ Esta campaña de {canal} ({tasaInteres:N1}%) está por debajo " +
                               $"del promedio de {canalContrario} ({promedioOtro:N1}%). " +
                               $"{canalContrario} es {ventajaOtro:N1}x más efectivo y genera " +
                               $"en promedio {extraInscritos:N0} inscritos más por campaña. " +
                               $"Se recomienda redirigir parte del presupuesto a {canalContrario}.";
                colorMensaje = "#791F1F";
            }

            
            colecCanales.ItemsSource = new List<object>
            {
                new {
                    canal           = $"📘 Facebook",
                    conversion      = canal == "Facebook"
                        ? $"{tasaInteres:N1}% (esta campaña)"
                        : $"{promedioConvFacebook:N1}% (promedio)",
                    colorConversion = canal == "Facebook" && tasaInteres >= promedioConvInstagram
                        ? "#085041" : "#A32D2D",
                    progreso        = canal == "Facebook"
                        ? Math.Min(tasaInteres / 100, 1)
                        : promedioConvFacebook / 100,
                    colorBarra      = "#E24B4A",
                    detalle         = canal == "Facebook"
                        ? $"Esta campaña: {alcance:N0} alcance → {solicitudes} solicitudes → {inscritos} inscritos"
                        : $"Promedio: 1,066 alcance → 47 solicitudes → 10 inscritos"
                },
                new {
                    canal           = $"📸 Instagram",
                    conversion      = canal == "Instagram"
                        ? $"{tasaInteres:N1}% (esta campaña)"
                        : $"{promedioConvInstagram:N1}% (promedio)",
                    colorConversion = canal == "Instagram" && tasaInteres >= promedioConvFacebook
                        ? "#085041" : "#854F0B",
                    progreso        = canal == "Instagram"
                        ? Math.Min(tasaInteres / 100, 1)
                        : promedioConvInstagram / 100,
                    colorBarra      = "#9C8442",
                    detalle         = canal == "Instagram"
                        ? $"Esta campaña: {alcance:N0} alcance → {solicitudes} solicitudes → {inscritos} inscritos"
                        : $"Promedio: 843 alcance → 101 solicitudes → 22 inscritos"
                }
            };


            borderAlertaPublicidad.IsVisible = true;
            lblAlertaPublicidad.Text = mensajeCanal;
            lblAlertaPublicidad.TextColor = Color.FromArgb(colorMensaje);
            borderAlertaPublicidad.BackgroundColor = Color.FromArgb(
                colorMensaje == "#085041" ? "#EAF9EF" : "#FCEBEB");
            borderAlertaPublicidad.Stroke = new SolidColorBrush(Color.FromArgb(
                colorMensaje == "#085041" ? "#9FE1CB" : "#F7C1C1"));


            if (tasaInteres < 10)
            {
                borderAlertaPublicidad.IsVisible = true;
            }
        }
    }
}