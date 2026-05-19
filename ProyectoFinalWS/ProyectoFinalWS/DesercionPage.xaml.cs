using Microcharts;
using SkiaSharp;

namespace ProyectoFinalWS
{
    public partial class DesercionPage : ContentPage
    {
        List<clsPrograma> listaProgramas = new List<clsPrograma>();

        readonly string[] meses = {
            "Ene","Feb","Mar","Abr","May","Jun",
            "Jul","Ago","Sep","Oct","Nov","Dic"
        };

        public DesercionPage()
        {
            InitializeComponent();
            CargarFiltros();
        }

        private async void CargarFiltros()
        {           
            int anioActual = DateTime.Now.Year;
            for (int a = anioActual; a >= anioActual - 3; a--)
                pickerAnio.Items.Add(a.ToString());

            
            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                listaProgramas = datos.ObtenerProgramasActivos();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var p in listaProgramas)
                        pickerPrograma.Items.Add(p.nombre.Length > 40
                            ? p.nombre.Substring(0, 40) + "..." : p.nombre);
                });
            });
        }

        private void OnFiltroChanged(object sender, EventArgs e)
        {
            if (pickerAnio.SelectedIndex < 0 || pickerPrograma.SelectedIndex < 0)
                return;

            int anio = int.Parse(pickerAnio.Items[pickerAnio.SelectedIndex]);
            int idPrograma = listaProgramas[pickerPrograma.SelectedIndex].id_programa;
            string nombrePrograma = listaProgramas[pickerPrograma.SelectedIndex].nombre;

            CargarDesercion(idPrograma, anio, nombrePrograma);
        }

        private async void CargarDesercion(int idPrograma, int anio, string nombrePrograma)
        {
            chartBarras.Chart = null;
            borderRecomendacion.IsVisible = false;

            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                List<clsBajaProgramaMes> bajasMes = datos.ObtenerBajasPorPrograma(idPrograma, anio);
                List<clsMotivoBaja> motivos = datos.ObtenerMotivosBajasPorPrograma(idPrograma, anio);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (bajasMes.Count == 0)
                    {
                        lblTotalBajas.Text = "0";
                        lblMesCritico.Text = "Sin bajas";
                        lblBajasMesCritico.Text = "No hubo bajas este año";
                        lblSubtituloGrafica.Text = "Sin bajas registradas para este programa y año";
                        colecMotivos.ItemsSource = null;
                        return;
                    }

                    
                    int totalBajas = bajasMes.Sum(b => b.bajas);
                    lblTotalBajas.Text = totalBajas.ToString();

                    var mesCritico = bajasMes.OrderByDescending(b => b.bajas).First();
                    lblMesCritico.Text = meses[mesCritico.mes - 1];
                    lblBajasMesCritico.Text = $"{mesCritico.bajas} bajas ese mes";
                    lblSubtituloGrafica.Text = $"Total {totalBajas} bajas en {anio}";

                    
                    var entries = bajasMes.Select(b =>
                    {
                        bool esCritico = b.mes == mesCritico.mes;
                        return new ChartEntry(b.bajas)
                        {
                            Label = meses[b.mes - 1],
                            ValueLabel = b.bajas.ToString(),
                            Color = esCritico
                                ? SKColor.Parse("#5B0000")
                                : SKColor.Parse("#9C8442"),
                            TextColor = SKColor.Parse("#5B0000"),
                        };
                    }).ToList();

                    chartBarras.Chart = new BarChart
                    {
                        Entries = entries,
                        LabelTextSize = 30,
                        ValueLabelTextSize = 28,
                        BackgroundColor = SKColors.White,
                    };

                    
                    if (motivos.Count > 0)
                    {
                        decimal maxPorc = motivos.Max(m => m.porcentaje);
                        colecMotivos.ItemsSource = motivos.Select(m => new
                        {
                            motivo = m.motivo,
                            cantidad = m.cantidad,
                            porcentaje = m.porcentaje,
                            progreso = maxPorc > 0
                                ? (double)(m.porcentaje / maxPorc) : 0
                        }).ToList();
                    }

                    
                    string motivoPrincipal = motivos.Count > 0
                        ? motivos.First().motivo : "";
                    string recomendacion = GenerarRecomendacion(
                        mesCritico.mes, motivoPrincipal,
                        totalBajas, nombrePrograma);

                    borderRecomendacion.IsVisible = true;
                    lblRecomendacion.Text = recomendacion;
                });
            });
        }

        private string GenerarRecomendacion(int mesCritico, string motivoPrincipal,
            int totalBajas, string nombrePrograma)
        {
            string nombreCorto = nombrePrograma.Contains("-")
                ? nombrePrograma.Split('-')[0].Trim()
                : nombrePrograma.Split(' ').First();

            
            string recMes = "";
            if (mesCritico == 1 || mesCritico == 2)
            {
                recMes = "Las bajas se concentran en enero-febrero, típicamente por el impacto " +
                         "económico de las fiestas de fin de año. Se recomienda: ofrecer " +
                         "plan de pagos diferido para inscripciones de enero, con primer pago " +
                         "en febrero, y lanzar promociones de inicio de año con descuento del 10%.";
            }
            else if (mesCritico == 7 || mesCritico == 8)
            {
                recMes = "Las bajas aumentan en julio-agosto, periodo de cambios laborales y " +
                         "vacacionales. Se recomienda: ofrecer modalidad asíncrona flexible " +
                         "para que los alumnos avancen a su ritmo durante el verano y no " +
                         "abandonen el programa por compromisos laborales o de viaje.";
            }
            else if (mesCritico == 4 || mesCritico == 5)
            {
                recMes = "Las bajas se concentran en abril-mayo. Se recomienda implementar " +
                         "sesiones de seguimiento a mitad del programa para detectar alumnos " +
                         "en riesgo antes de que decidan darse de baja.";
            }
            else
            {
                recMes = $"Las bajas se concentran en {new[] { "", "Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" }[mesCritico]}. " +
                         "Se recomienda reforzar el acompañamiento académico en ese período.";
            }

           
            string recMotivo = "";
            if (motivoPrincipal.Contains("económic"))
            {
                recMotivo = "El principal motivo es económico. Considera implementar un " +
                            "programa de becas parciales o convenios de pago en mensualidades " +
                            "para retener a alumnos con dificultades financieras.";
            }
            else if (motivoPrincipal.Contains("trabajo"))
            {
                recMotivo = "El cambio de trabajo es el principal motivo. Refuerza la " +
                            "flexibilidad del programa con clases grabadas y acceso " +
                            "asíncrono para que los alumnos no abandonen por compromisos laborales.";
            }
            else if (motivoPrincipal.Contains("interés"))
            {
                recMotivo = $"La falta de interés indica que el contenido de {nombreCorto} " +
                            "puede estar desactualizado o no cumple las expectativas. " +
                            "Se recomienda revisar el temario, incluir casos prácticos " +
                            "reales y actualizar los materiales didácticos.";
            }
            else
            {
                recMotivo = "Se recomienda implementar encuestas de satisfacción al inicio " +
                            "y mitad del programa para identificar problemas antes de que " +
                            "el alumno decida abandonar.";
            }

            return $"📅 {recMes}\n\n💰 {recMotivo}";
        }
    }
}