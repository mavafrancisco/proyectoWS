using Microcharts;
using SkiaSharp;

namespace ProyectoFinalWS
{
    public partial class BenchmarkingPage : ContentPage
    {
        readonly List<string> competidores = new List<string>
        {
            "Instituto de Salud del Norte",
            "Centro Educativo Médico de Occidente",
            "Escuela Superior de Enfermería Sinaloa"
        };

        readonly List<int> indices = new List<int> { 78, 65, 82 };

        readonly List<string> descripcionesIndice = new List<string>
        {
            "LAMB es competitiva en precio y programas exclusivos, pero tiene brechas en oferta médica",
            "LAMB supera en variedad de diplomados pero el competidor tiene mayor presencia regional",
            "LAMB es líder en precio y programas de enfermería, ventaja competitiva sólida"
        };

        readonly List<List<object>> ventajasPorComp = new List<List<object>>
        {
            new List<object>
            {
                new { programa = "Tutoría Tanatológica (UNAM)", detalle = "Único con aval UNAM en tanatología en la región" },
                new { programa = "FPTT — Formación en Tanatología", detalle = "El competidor no ofrece formación profesional en esta área" },
                new { programa = "Precio más competitivo", detalle = "LAMB cobra hasta 15% menos en diplomados equivalentes" },
            },
            new List<object>
            {
                new { programa = "Mayor variedad de diplomados", detalle = "LAMB ofrece 9 diplomados vs 4 del competidor" },
                new { programa = "Doble aval (UNAM + La Salle)", detalle = "El competidor solo trabaja con un aval institucional" },
                new { programa = "Precios más accesibles en cursos", detalle = "$800 vs $900 del competidor, 11% más económico" },
            },
            new List<object>
            {
                new { programa = "Especialización en enfermería", detalle = "LAMB tiene 4 diplomados de enfermería vs 1 del competidor" },
                new { programa = "Aval UNAM en programas clave", detalle = "El competidor no cuenta con aval UNAM" },
                new { programa = "Trayectoria y experiencia", detalle = "LAMB tiene más grupos históricos y egresados certificados" },
            }
        };

        readonly List<List<object>> comparativaPorComp = new List<List<object>>
                {
                    new List<object>
                    {
                        new { programa = "Administración en Enfermería", lamb = "✅", competidor = "✅" },
                        new { programa = "Liderazgo en Salud",           lamb = "✅", competidor = "✅" },
                        new { programa = "Tutoría Tanatológica",         lamb = "✅", competidor = "❌" },
                        new { programa = "FPTT Tanatología",             lamb = "✅", competidor = "❌" },
                        new { programa = "Urgencias Médicas",            lamb = "❌", competidor = "✅" },
                        new { programa = "Nutrición Clínica",            lamb = "❌", competidor = "✅" },
                        new { programa = "Inteligencia Emocional",       lamb = "✅", competidor = "✅" },
                    },
                    new List<object>
                    {
                        new { programa = "Gestión de Enfermería",        lamb = "✅", competidor = "✅" },
                        new { programa = "Tutoría y Tanatología",        lamb = "✅", competidor = "✅" },
                        new { programa = "Inteligencia Emocional",       lamb = "✅", competidor = "✅" },
                        new { programa = "Psicología Clínica",           lamb = "❌", competidor = "✅" },
                        new { programa = "ASLAE",                        lamb = "✅", competidor = "❌" },
                        new { programa = "ALGSS",                        lamb = "✅", competidor = "❌" },
                    },
                    new List<object>
                    {
                        new { programa = "Administración Hospitalaria",  lamb = "✅", competidor = "✅" },
                        new { programa = "Calidad en Salud",             lamb = "✅", competidor = "✅" },
                        new { programa = "Liderazgo de Equipos",         lamb = "✅", competidor = "✅" },
                        new { programa = "Medicina Preventiva",          lamb = "❌", competidor = "✅" },
                        new { programa = "AGSE",                         lamb = "✅", competidor = "❌" },
                        new { programa = "Tanatología",                  lamb = "✅", competidor = "❌" },
                    }
                };


                        readonly List<float[]> datosLamb = new List<float[]>
                {
                    new float[] { 85, 75, 90, 80, 65 }, // vs competidor 1
                    new float[] { 85, 90, 90, 80, 65 }, // vs competidor 2
                    new float[] { 85, 88, 90, 85, 65 }, // vs competidor 3
                };

                        readonly List<float[]> datosComp = new List<float[]>
                {
                    new float[] { 70, 65, 60, 70, 80 }, // competidor 1
                    new float[] { 75, 55, 65, 75, 85 }, // competidor 2
                    new float[] { 80, 60, 55, 65, 75 }, // competidor 3
                };

        readonly string[] dimensiones = { "Precio", "Variedad", "Avales", "Experiencia", "Alcance" };
        readonly List<List<object>> brechasPorComp = new List<List<object>>
        {
            new List<object>
            {
                new {
                    programa = "Urgencias Médicas",
                    descripcion = "El competidor lo ofrece con alta demanda en el sector salud",
                    demanda = "Alta demanda",
                    colorDemanda = "#085041",
                    sugerencia = "💡 Recomendación: Evaluar implementar este programa en el siguiente ciclo. Requiere un instructor con especialidad en urgencias y puede generar ingresos adicionales de $48,000 por grupo."
                },
                new {
                    programa = "Nutrición Clínica",
                    descripcion = "Área en crecimiento, el competidor ya lo tiene posicionado",
                    demanda = "Media demanda",
                    colorDemanda = "#854F0B",
                    sugerencia = "💡 Recomendación: Considerar a mediano plazo. Realizar una encuesta de interés entre los egresados antes de invertir en el programa."
                },
            },
            new List<object>
            {
                new {
                    programa = "Psicología Clínica",
                    descripcion = "Alta demanda en el sector salud, el competidor lo tiene bien posicionado",
                    demanda = "Alta demanda",
                    colorDemanda = "#085041",
                    sugerencia = "💡 Recomendación: Alta prioridad. LAMB podría diferenciarse con un enfoque en psicología para profesionales de enfermería, un nicho no explotado en la región."
                },
            },
            new List<object>
            {
                new {
                    programa = "Medicina Preventiva",
                    descripcion = "Programa complementario con creciente interés institucional",
                    demanda = "Media demanda",
                    colorDemanda = "#854F0B",
                    sugerencia = "💡 Recomendación: Evaluar en el mediano plazo. Puede implementarse como curso corto ($800) antes de convertirlo en diplomado completo."
                },
            }
        };

        readonly List<List<object>> preciosPorComp = new List<List<object>>
        {
            new List<object>
            {
                new { programa = "Administración en Enfermería", precioLamb = "$4,800", precioComp = "$5,200", progresoLamb = 0.70, progresoComp = 0.85, diferencia = "✅ LAMB es 8% más económico", colorDif = "#085041" },
                new { programa = "Liderazgo en Salud",           precioLamb = "$4,800", precioComp = "$4,500", progresoLamb = 0.75, progresoComp = 0.70, diferencia = "⚠️ Competidor es 6% más económico", colorDif = "#854F0B" },
                new { programa = "Inteligencia Emocional",       precioLamb = "$800",   precioComp = "$950",   progresoLamb = 0.55, progresoComp = 0.70, diferencia = "✅ LAMB es 16% más económico", colorDif = "#085041" },
            },
            new List<object>
            {
                new { programa = "Gestión de Enfermería",        precioLamb = "$4,800", precioComp = "$5,500", progresoLamb = 0.65, progresoComp = 0.80, diferencia = "✅ LAMB es 13% más económico", colorDif = "#085041" },
                new { programa = "Inteligencia Emocional",       precioLamb = "$800",   precioComp = "$900",   progresoLamb = 0.60, progresoComp = 0.70, diferencia = "✅ LAMB es 11% más económico", colorDif = "#085041" },
            },
            new List<object>
            {
                new { programa = "Administración Hospitalaria",  precioLamb = "$4,800", precioComp = "$5,000", progresoLamb = 0.70, progresoComp = 0.75, diferencia = "✅ LAMB es 4% más económico", colorDif = "#085041" },
                new { programa = "Calidad en Salud",             precioLamb = "$800",   precioComp = "$850",   progresoLamb = 0.65, progresoComp = 0.70, diferencia = "✅ LAMB es 6% más económico", colorDif = "#085041" },
            }
        };

        public BenchmarkingPage()
        {
            InitializeComponent();
            foreach (var c in competidores)
                pickerCompetidor.Items.Add(c);
        }

        private void OnCompetidorChanged(object sender, EventArgs e)
        {
            if (pickerCompetidor.SelectedIndex < 0) return;
            int idx = pickerCompetidor.SelectedIndex;

            
            int indice = indices[idx];
            lblIndice.Text = $"{indice}/100";
            lblDescripcionIndice.Text = descripcionesIndice[idx];
            lblSemaforoIndice.Text = indice >= 75 ? "🟢" : indice >= 50 ? "🟡" : "🔴";

            
            borderIndice.IsVisible = true;
            borderComparativa.IsVisible = true;
            borderVentajas.IsVisible = true;
            borderBrechas.IsVisible = true;
            borderPrecios.IsVisible = true;

            
            colecComparativa.ItemsSource = comparativaPorComp[idx];
            colecVentajas.ItemsSource = ventajasPorComp[idx];
            colecBrechas.ItemsSource = brechasPorComp[idx];
            colecPrecios.ItemsSource = preciosPorComp[idx];
        }
    }
}