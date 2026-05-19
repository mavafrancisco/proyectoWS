using Microcharts;
using SkiaSharp;

namespace ProyectoFinalWS
{
    public partial class FinanzasPage : ContentPage
    {
        public FinanzasPage()
        {
            InitializeComponent();
            CargarAnios();
        }

        private void CargarAnios()
        {
            int anioActual = DateTime.Now.Year;
            for (int a = anioActual; a >= anioActual - 3; a--)
                pickerAnio.Items.Add(a.ToString());
            pickerAnio.SelectedIndex = 0;
        }

        private void OnAnioChanged(object sender, EventArgs e)
        {
            if (pickerAnio.SelectedIndex < 0) return;
            int anio = int.Parse(pickerAnio.Items[pickerAnio.SelectedIndex]);
            CargarFinanzas(anio);
        }

        private async void CargarFinanzas(int anio)
        {
            chartTipo.Chart = null;

            await Task.Run(() =>
            {
                clsDatos datos = new clsDatos();
                clsFinanzas fin = datos.ObtenerFinanzas(anio);
                decimal egresos = datos.ObtenerEgresos(anio);
                List<clsIngresoTipo> ingresosTipo = datos.ObtenerIngresosPorTipo(anio);
                List<clsProgramaRentable> topProg = datos.ObtenerTopProgramas(anio);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    decimal ganancia = fin.cobrado - egresos;
                    decimal margen = fin.cobrado > 0
                        ? (ganancia / fin.cobrado) * 100 : 0;

                    lblGanancia.Text = $"${ganancia:N0}";
                    lblMargen.Text = $"{margen:N1}%";
                    lblSubGanancia.Text = $"Ingresos: ${fin.cobrado:N0}  |  Egresos: ${egresos:N0}";

                    if (margen >= 50)
                    {
                        lblSemaforoGanancia.Text = "🟢";
                    }
                    else if (margen >= 20)
                    {
                        lblSemaforoGanancia.Text = "🟡";
                    }
                    else
                    {
                        lblSemaforoGanancia.Text = "🔴";
                    }

                    if (ingresosTipo.Count > 0)
                    {
                        var entries = new List<ChartEntry>();
                        decimal maxIngreso = ingresosTipo.Max(t => t.ingresos);

                        foreach (var t in ingresosTipo)
                        {
                            SKColor color = t.tipo == "diplomado"
                                ? SKColor.Parse("#5B0000")
                                : t.tipo == "curso"
                                    ? SKColor.Parse("#9C8442")
                                    : SKColor.Parse("#D4B483");

                            entries.Add(new ChartEntry((float)t.ingresos)
                            {
                                Label = t.tipo,
                                ValueLabel = $"${t.ingresos:N0}",
                                Color = color,
                            });

                            
                            if (t.tipo == "diplomado")
                                lblIngDiplomados.Text = $"${t.ingresos:N0}";
                            else if (t.tipo == "curso")
                                lblIngCursos.Text = $"${t.ingresos:N0}";
                        }

                        chartTipo.Chart = new DonutChart
                        {
                            Entries = entries,
                            LabelTextSize = 32,
                            BackgroundColor = SKColors.Transparent,
                        };
                    }

                    
                    if (topProg.Count > 0)
                    {
                        decimal maxGanancia = topProg.Max(p => p.ganancia);
                        colecTop.ItemsSource = topProg.Select(p => new
                        {
                            nombre = p.nombre.Length > 35
                                ? p.nombre.Substring(0, 35) + "..." : p.nombre,
                            tipo = p.tipo,
                            gananciaStr = $"${p.ganancia:N0}",
                            margenStr = p.ingresos > 0
                                ? $"{(p.ganancia / p.ingresos * 100):N1}% margen"
                                : "Sin datos",
                            progreso = maxGanancia > 0
                                ? (double)(p.ganancia / maxGanancia) : 0
                        }).ToList();
                    }

                    
                    if (fin.pendiente > 0 || fin.vencido > 0)
                    {
                        borderCartera.IsVisible = true;
                        decimal totalCartera = fin.pendiente + fin.vencido;

                        lblPendiente.Text = $"${fin.pendiente:N0}";
                        lblVencido.Text = $"${fin.vencido:N0}";

                        progPendiente.Progress = totalCartera > 0
                            ? (double)(fin.pendiente / totalCartera) : 0;
                        progVencido.Progress = totalCartera > 0
                            ? (double)(fin.vencido / totalCartera) : 0;

                        if (fin.vencido > 0)
                        {
                            lblAlertaCartera.Text = "🔴 Cartera vencida";
                            lblRecomendacionCartera.Text =
                                $"Tienes ${fin.vencido:N0} en cartera vencida. " +
                                "Implementar recordatorios automáticos de pago " +
                                "5 días antes del vencimiento para reducir esta cifra.";
                        }
                        else
                        {
                            lblAlertaCartera.Text = "🟡 Pendiente de cobro";
                            lblRecomendacionCartera.Text =
                                $"Tienes ${fin.pendiente:N0} pendientes de cobro. " +
                                "Envía recordatorios a los alumnos con pagos próximos a vencer.";
                        }
                    }
                    else
                    {
                        borderCartera.IsVisible = false;
                    }
                });
            });
        }
    }
}