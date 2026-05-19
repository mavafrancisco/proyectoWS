using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace ProyectoFinalWS
{
    public class clsDashboard
    {
        public int alumnosActivos { get; set; }
        public int programasActivos { get; set; }
        public int constanciasAnio { get; set; }
        public decimal ingresosMes { get; set; }
        public decimal ingresosMesAnterior { get; set; }
        public int alumnosDiplomados { get; set; }
        public int alumnosCursos { get; set; }
        public int alumnosTalleres { get; set; }
        public decimal tasaDesercion { get; set; }
        public int totalBajas { get; set; }
    }

    public class clsPrograma
    {
        public int id_programa { get; set; }
        public string nombre { get; set; }
        public string tipo { get; set; }
    }

    public class clsInscripcionGrupo
    {
        public int id_grupo { get; set; }
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
        public int cupo_maximo { get; set; }
        public int total_inscritos { get; set; }
        public int egresados { get; set; }
        public int bajas { get; set; }
        public int activos { get; set; }
        public decimal porcentaje_ocupacion { get; set; }
    }

    public class clsFinanzas
    {
        public decimal cobrado { get; set; }
        public decimal pendiente { get; set; }
        public decimal vencido { get; set; }
        public decimal totalFacturado { get; set; }
    }

    public class clsFinanzasAnio
    {
        public int anio { get; set; }
        public decimal ingresos { get; set; }
        public decimal egresos { get; set; }
    }

    public class clsDesercion
    {
        public int totalBajas { get; set; }
        public int totalInscritos { get; set; }
        public decimal tasaDesercion { get; set; }
        public int bajasTemprana { get; set; }
    }

    public class clsMotivoBaja
    {
        public string motivo { get; set; }
        public int cantidad { get; set; }
        public decimal porcentaje { get; set; }
    }

    public class clsBajaMes
    {
        public int mes { get; set; }
        public int bajas { get; set; }
    }

    public class clsIngresoTipo
    {
        public string tipo { get; set; }
        public decimal ingresos { get; set; }
        public int totalAlumnos { get; set; }
    }

    public class clsProgramaRentable
    {
        public string nombre { get; set; }
        public string tipo { get; set; }
        public decimal ingresos { get; set; }
        public decimal egresos { get; set; }
        public decimal ganancia { get; set; }
    }

    public class clsBajaProgramaMes
    {
        public int mes { get; set; }
        public int bajas { get; set; }
    }

    public class clsDatos
    {
        public bool ValidarLogin(string usuario, string contrasena)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            string rs = ws.ValidarLogin(usuario, contrasena);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            return dt.Rows.Count > 0;
        }

        public clsDashboard ObtenerDashboard()
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            string rs = ws.ObtenerResumenDashboard();
            System.Diagnostics.Debug.WriteLine($"Dashboard: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            if (dt.Rows.Count == 0) return new clsDashboard();
            DataRow r = dt.Rows[0];
            return new clsDashboard
            {
                alumnosActivos = r["alumnosActivos"] == DBNull.Value ? 0 : Convert.ToInt32(r["alumnosActivos"]),
                programasActivos = r["programasActivos"] == DBNull.Value ? 0 : Convert.ToInt32(r["programasActivos"]),
                constanciasAnio = r["constanciasAnio"] == DBNull.Value ? 0 : Convert.ToInt32(r["constanciasAnio"]),
                ingresosMes = r["ingresosMes"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ingresosMes"]),
                ingresosMesAnterior = r["ingresosMesAnterior"] == DBNull.Value ? 0 : Convert.ToDecimal(r["ingresosMesAnterior"]),
                alumnosDiplomados = r["alumnosDiplomados"] == DBNull.Value ? 0 : Convert.ToInt32(r["alumnosDiplomados"]),
                alumnosCursos = r["alumnosCursos"] == DBNull.Value ? 0 : Convert.ToInt32(r["alumnosCursos"]),
                alumnosTalleres = r["alumnosTalleres"] == DBNull.Value ? 0 : Convert.ToInt32(r["alumnosTalleres"]),
                tasaDesercion = r["tasaDesercion"] == DBNull.Value ? 0 : Convert.ToDecimal(r["tasaDesercion"]),
                totalBajas = r.IsNull("totalBajas") ? 0 : Convert.ToInt32(r["totalBajas"])
            };
        }

        public List<clsPrograma> ObtenerProgramasActivos()
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsPrograma> lista = new List<clsPrograma>();
            string rs = ws.ObtenerProgramasActivos();
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsPrograma
                {
                    id_programa = Convert.ToInt32(rn["id_programa"]),
                    nombre = $"{rn["nombre"]}",
                    tipo = $"{rn["tipo"]}"
                });
            }
            return lista;
        }

        public List<clsInscripcionGrupo> ObtenerInscripcionesPorPrograma(int idPrograma)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsInscripcionGrupo> lista = new List<clsInscripcionGrupo>();
            string rs = ws.ObtenerInscripcionesPorPrograma(idPrograma);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsInscripcionGrupo
                {
                    id_grupo = Convert.ToInt32(rn["id_grupo"]),
                    fecha_inicio = $"{rn["fecha_inicio"]}",
                    fecha_fin = $"{rn["fecha_fin"]}",
                    cupo_maximo = Convert.ToInt32(rn["cupo_maximo"]),
                    total_inscritos = Convert.ToInt32(rn["total_inscritos"]),
                    egresados = Convert.ToInt32(rn["egresados"]),
                    bajas = Convert.ToInt32(rn["bajas"]),
                    activos = Convert.ToInt32(rn["activos"]),
                    porcentaje_ocupacion = Convert.ToDecimal(rn["porcentaje_ocupacion"])
                });
            }
            return lista;
        }

        public clsFinanzas ObtenerFinanzas(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            string rs = ws.ObtenerResumenFinanzas(anio);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            if (dt.Rows.Count == 0) return new clsFinanzas();
            DataRow r = dt.Rows[0];
            return new clsFinanzas
            {
                cobrado = r["cobrado"] == DBNull.Value ? 0 : Convert.ToDecimal(r["cobrado"]),
                pendiente = r["pendiente"] == DBNull.Value ? 0 : Convert.ToDecimal(r["pendiente"]),
                vencido = r["vencido"] == DBNull.Value ? 0 : Convert.ToDecimal(r["vencido"]),
                totalFacturado = r["totalFacturado"] == DBNull.Value ? 0 : Convert.ToDecimal(r["totalFacturado"])
            };
        }

        public List<clsFinanzasAnio> ObtenerFinanzasPorAnio()
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsFinanzasAnio> lista = new List<clsFinanzasAnio>();
            string rs = ws.ObtenerFinanzasPorAnio();
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsFinanzasAnio
                {
                    anio = Convert.ToInt32(rn["anio"]),
                    ingresos = Convert.ToDecimal(rn["ingresos"]),
                    egresos = Convert.ToDecimal(rn["egresos"])
                });
            }
            return lista;
        }

        public decimal ObtenerEgresos(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            string rs = ws.ObtenerEgresosPorAnio(anio);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            if (dt.Rows.Count == 0) return 0;
            return Convert.ToDecimal(dt.Rows[0]["totalEgresos"]);
        }

        public clsDesercion ObtenerDesercion(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            string rs = ws.ObtenerResumenDesercion(anio);
            System.Diagnostics.Debug.WriteLine($"Desercion: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            if (dt.Rows.Count == 0) return new clsDesercion();
            DataRow r = dt.Rows[0];
            return new clsDesercion
            {
                totalBajas = r.IsNull("totalBajas") ? 0 : Convert.ToInt32(r["totalBajas"]),
                totalInscritos = r.IsNull("totalInscritos") ? 0 : Convert.ToInt32(r["totalInscritos"]),
                tasaDesercion = r.IsNull("tasaDesercion") ? 0 : Convert.ToDecimal(r["tasaDesercion"]),
                bajasTemprana = r.IsNull("bajasTemprana") ? 0 : Convert.ToInt32(r["bajasTemprana"])
            };
        }

        public List<clsMotivoBaja> ObtenerMotivosBajas(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsMotivoBaja> lista = new List<clsMotivoBaja>();
            string rs = ws.ObtenerMotivosBajas(anio);
            System.Diagnostics.Debug.WriteLine($"Motivos: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsMotivoBaja
                {
                    motivo = $"{rn["motivo"]}",
                    cantidad = Convert.ToInt32(rn["cantidad"]),
                    porcentaje = Convert.ToDecimal(rn["porcentaje"])
                });
            }
            return lista;
        }

        public List<clsBajaMes> ObtenerBajasPorMes(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsBajaMes> lista = new List<clsBajaMes>();
            string rs = ws.ObtenerBajasPorMes(anio);
            System.Diagnostics.Debug.WriteLine($"BajasMes: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsBajaMes
                {
                    mes = Convert.ToInt32(rn["mes"]),
                    bajas = Convert.ToInt32(rn["bajas"])
                });
            }
            return lista;
        }

        public List<clsIngresoTipo> ObtenerIngresosPorTipo(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsIngresoTipo> lista = new List<clsIngresoTipo>();
            string rs = ws.ObtenerIngresosPorTipo(anio);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsIngresoTipo
                {
                    tipo = $"{rn["tipo"]}",
                    ingresos = rn.IsNull("ingresos") ? 0 : Convert.ToDecimal(rn["ingresos"]),
                    totalAlumnos = rn.IsNull("totalAlumnos") ? 0 : Convert.ToInt32(rn["totalAlumnos"])
                });
            }
            return lista;
        }

        public List<clsProgramaRentable> ObtenerTopProgramas(int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsProgramaRentable> lista = new List<clsProgramaRentable>();
            string rs = ws.ObtenerTopProgramasRentables(anio);
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsProgramaRentable
                {
                    nombre = $"{rn["nombre"]}",
                    tipo = $"{rn["tipo"]}",
                    ingresos = rn.IsNull("ingresos") ? 0 : Convert.ToDecimal(rn["ingresos"]),
                    egresos = rn.IsNull("egresos") ? 0 : Convert.ToDecimal(rn["egresos"]),
                    ganancia = rn.IsNull("ganancia") ? 0 : Convert.ToDecimal(rn["ganancia"])
                });
            }
            return lista;
        }
        public List<clsBajaProgramaMes> ObtenerBajasPorPrograma(int idPrograma, int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsBajaProgramaMes> lista = new List<clsBajaProgramaMes>();
            string rs = ws.ObtenerBajasPorPrograma(idPrograma, anio);
            System.Diagnostics.Debug.WriteLine($"BajasProg: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsBajaProgramaMes
                {
                    mes = Convert.ToInt32(rn["mes"]),
                    bajas = Convert.ToInt32(rn["bajas"])
                });
            }
            return lista;
        }

        public List<clsMotivoBaja> ObtenerMotivosBajasPorPrograma(int idPrograma, int anio)
        {
            wsDatos.Service1Client ws = new wsDatos.Service1Client();
            List<clsMotivoBaja> lista = new List<clsMotivoBaja>();
            string rs = ws.ObtenerMotivosBajasPorPrograma(idPrograma, anio);
            System.Diagnostics.Debug.WriteLine($"MotivosProg: {rs}");
            DataTable dt = JsonConvert.DeserializeObject<DataTable>(rs) ?? new DataTable();
            foreach (DataRow rn in dt.Rows)
            {
                lista.Add(new clsMotivoBaja
                {
                    motivo = $"{rn["motivo"]}",
                    cantidad = Convert.ToInt32(rn["cantidad"]),
                    porcentaje = rn.IsNull("porcentaje") ? 0 : Convert.ToDecimal(rn["porcentaje"])
                });
            }
            return lista;
        }
    }
}
