using Microcharts;
using SkiaSharp;

namespace ProyectoFinalWS
{
    public partial class DashboardPage : ContentPage
    {
        public DashboardPage()
        {
            InitializeComponent();
            CargarDashboard();
        }

        private async void CargarDashboard()
        {
            try
            {


                await Task.Run(() =>
                {
                    clsDatos datos = new clsDatos();
                    clsDashboard dash = datos.ObtenerDashboard();

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        lblIngresosMes.Text = $"${dash.ingresosMes:N0}";
                        lblComparacionMes.Text = $"vs ${dash.ingresosMesAnterior:N0} en 2024";

                        if (dash.ingresosMesAnterior > 0)
                        {
                            decimal variacion = ((dash.ingresosMes - dash.ingresosMesAnterior)
                                                / dash.ingresosMesAnterior) * 100;
                            lblPorcentajeMes.Text = variacion >= 0
                                ? $"+{variacion:N1}%" : $"{variacion:N1}%";
                            lblSemaforoMes.Text = variacion >= 0 ? "🟢" : "🔴";
                        }
                        else
                        {
                            lblComparacionMes.Text = "Sin datos del año anterior";
                            lblSemaforoMes.Text = "⚪";
                        }

                        lblAlumnos.Text = dash.alumnosActivos.ToString();
                        lblProgramas.Text = dash.programasActivos.ToString();
                        lblConstancias.Text = dash.constanciasAnio.ToString();

                        int totalInscritos2025 = dash.alumnosActivos + dash.totalBajas;

                        lblDesercion.Text = dash.totalBajas.ToString();

                        if (dash.tasaDesercion < 10)
                        {
                            lblSemaforoDesercion.Text = $"🟢 de {totalInscritos2025} inscritos";
                            lblSemaforoDesercion.TextColor = Color.FromArgb("#085041");
                        }
                        else if (dash.tasaDesercion < 25)
                        {
                            lblSemaforoDesercion.Text = $"🟡 de {totalInscritos2025} inscritos";
                            lblSemaforoDesercion.TextColor = Color.FromArgb("#854F0B");
                        }
                        else
                        {
                            lblSemaforoDesercion.Text = $"🔴 de {totalInscritos2025} inscritos";
                            lblSemaforoDesercion.TextColor = Color.FromArgb("#A32D2D");
                        }

                        lblSemaforoAlumnos.Text = dash.alumnosActivos > 0
                            ? $"🟢 {dash.alumnosActivos} activos"
                            : "⚪ Sin alumnos";

                        lblSubtituloGrafica.Text = $"Total: {dash.alumnosActivos} alumnos activos";
                        lblDiplomados.Text = dash.alumnosDiplomados.ToString();
                        lblCursos.Text = dash.alumnosCursos.ToString();

                        var entries = new List<ChartEntry>
                        {
                             new ChartEntry(dash.alumnosDiplomados)
                            {
                              Label      = "Diplomados",
                              ValueLabel = dash.alumnosDiplomados.ToString(),
                                 Color      = SKColor.Parse("#5B0000"),
                                 },
                                 new ChartEntry(dash.alumnosCursos)
                            {
                                Label      = "Cursos",
                                ValueLabel = dash.alumnosCursos.ToString(),
                                Color      = SKColor.Parse("#9C8442"),
                            }
                        };

                        chartDonut.Chart = new DonutChart
                        {
                            Entries = entries,
                            LabelTextSize = 35,
                            BackgroundColor = SKColors.Transparent,
                        };
                    });
                });
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"No se pudo cargar el dashboard: {ex.Message}", "OK");
            }
        }
    }

}