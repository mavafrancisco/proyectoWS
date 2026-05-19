using Microcharts;
using SkiaSharp;

namespace ProyectoFinalWS
{
    public partial class InscripcionesPage : ContentPage
    {
        List<clsPrograma> listaProgramas = new List<clsPrograma>();

        public InscripcionesPage()
        {
            InitializeComponent();
            CargarProgramas();
        }

        private async void CargarProgramas()
        {
            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                listaProgramas = datos.ObtenerProgramasActivos();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var p in listaProgramas)
                        pickerPrograma.Items.Add($"{p.nombre} ({p.tipo})");
                });
            });
        }

        private void OnProgramaChanged(object sender, EventArgs e)
        {
            if (pickerPrograma.SelectedIndex < 0) return;
            int idPrograma = listaProgramas[pickerPrograma.SelectedIndex].id_programa;
            CargarInscripciones(idPrograma);
        }

        private async void CargarInscripciones(int idPrograma)
        {
            chartBarras.Chart = null;

            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                List<clsInscripcionGrupo> lista = datos.ObtenerInscripcionesPorPrograma(idPrograma);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (lista.Count == 0)
                    {
                        lblTotalGrupos.Text = "0";
                        lblOcupacionPromedio.Text = "0%";
                        lblSubtituloBarras.Text = "Sin datos para este programa";
                        borderRecomendacion.IsVisible = false;
                        return;
                    }

                    lblTotalGrupos.Text = lista.Count.ToString();
                    decimal ocupProm = lista.Average(g => g.porcentaje_ocupacion);
                    lblOcupacionPromedio.Text = $"{ocupProm:N1}%";

                    if (ocupProm >= 80)
                    {
                        lblSemaforoOcupacion.Text = "🟢 Óptimo";
                        lblSemaforoOcupacion.TextColor = Color.FromArgb("#085041");
                    }
                    else if (ocupProm >= 50)
                    {
                        lblSemaforoOcupacion.Text = "🟡 Aceptable";
                        lblSemaforoOcupacion.TextColor = Color.FromArgb("#854F0B");
                    }
                    else
                    {
                        lblSemaforoOcupacion.Text = "🔴 Crítico";
                        lblSemaforoOcupacion.TextColor = Color.FromArgb("#A32D2D");
                    }

                    var ultimo = lista.Last();
                    lblSubtituloBarras.Text = $"Último grupo: {ultimo.total_inscritos} inscritos de {ultimo.cupo_maximo} cupos";

                    var entries = lista.Select((g, i) =>
                    {
                        bool esUltimo = i == lista.Count - 1;
                        return new ChartEntry(g.total_inscritos)
                        {
                            Label = DateTime.Parse(g.fecha_inicio).ToString("MMM yy"),
                            ValueLabel = g.total_inscritos.ToString(),
                            Color = esUltimo ? SKColor.Parse("#5B0000") : SKColor.Parse("#9C8442"),
                            TextColor = SKColor.Parse("#5B0000"),
                        };
                    }).ToList();

                    chartBarras.Chart = new BarChart
                    {
                        Entries = entries,
                        LabelTextSize = 28,
                        ValueLabelTextSize = 28,
                        BackgroundColor = SKColors.White,
                    };

                    colecGrupos.ItemsSource = lista
                        .Select(g => new
                        {
                            fecha_inicio = DateTime.Parse(g.fecha_inicio).ToString("MMM yyyy"),
                            total_inscritos = g.total_inscritos,
                            egresados = g.egresados,
                            bajas = g.bajas,
                            porcentaje_ocupacion = g.porcentaje_ocupacion
                        }).ToList();

                    string nombrePrograma = listaProgramas[pickerPrograma.SelectedIndex].nombre;
                    string recomendacion = "";
                    string colorRec = "#085041";
                    string bgRec = "#EAF9EF";
                    string strokeRec = "#9FE1CB";

                    if (lista.Count >= 2)
                    {
                        int primero = lista.First().total_inscritos;
                        int ultimoVal = lista.Last().total_inscritos;
                        decimal ocupActual = lista.Last().porcentaje_ocupacion;
                        int diferencia = ultimoVal - primero;
                        decimal cambio = primero > 0 ? (diferencia * 100m / primero) : 0;

                        if (cambio >= 20 && ocupActual >= 80)
                        {
                            recomendacion = $"✅ {nombrePrograma.Split('-')[0].Trim()} muestra una tendencia de crecimiento sólida " +
                                            $"({cambio:N0}% más inscritos vs el primer grupo). " +
                                            "Se recomienda abrir un segundo grupo por período para atender " +
                                            "la demanda y considerar versiones especializadas del programa.";
                            colorRec = "#085041"; bgRec = "#EAF9EF"; strokeRec = "#9FE1CB";
                        }
                        else if (cambio >= 0 && ocupActual >= 50)
                        {
                            recomendacion = $"🟡 {nombrePrograma.Split('-')[0].Trim()} mantiene una demanda estable. " +
                                            "Se recomienda reforzar la estrategia de marketing digital, " +
                                            "ofrecer descuentos por inscripción anticipada y solicitar " +
                                            "testimonios de egresados para atraer nuevos alumnos.";
                            colorRec = "#854F0B"; bgRec = "#FAEEDA"; strokeRec = "#D4B483";
                        }
                        else if (cambio < 0 && ocupActual >= 30)
                        {
                            recomendacion = $"⚠️ {nombrePrograma.Split('-')[0].Trim()} presenta una caída del {Math.Abs(cambio):N0}% " +
                                            "en inscripciones. Se recomienda revisar y actualizar el temario, " +
                                            "evaluar si el horario o modalidad son adecuados para el público objetivo " +
                                            "y comparar la oferta con instituciones competidoras.";
                            colorRec = "#854F0B"; bgRec = "#FAEEDA"; strokeRec = "#D4B483";
                        }
                        else
                        {
                            recomendacion = $"🔴 {nombrePrograma.Split('-')[0].Trim()} presenta una caída crítica del {Math.Abs(cambio):N0}% " +
                                            $"y una ocupación actual del {ocupActual:N1}%. " +
                                            "Con el nivel actual de inscritos el programa genera pérdidas operativas. " +
                                            "Se recomienda suspender temporalmente el programa, rediseñar " +
                                            "completamente el contenido o descontinuarlo si no mejora en el siguiente período.";
                            colorRec = "#791F1F"; bgRec = "#FCEBEB"; strokeRec = "#F7C1C1";
                        }
                    }

                    borderRecomendacion.IsVisible = true;
                    borderRecomendacion.Stroke = new SolidColorBrush(Color.FromArgb(strokeRec));
                    borderRecomendacion.BackgroundColor = Color.FromArgb(bgRec);
                    lblRecomendacion.Text = recomendacion;
                    lblRecomendacion.TextColor = Color.FromArgb(colorRec);
                });
            });
        }
    }
}