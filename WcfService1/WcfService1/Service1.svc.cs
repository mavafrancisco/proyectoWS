using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace WcfService1
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Service1" en el código, en svc y en el archivo de configuración.
    // NOTE: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Service1.svc o Service1.svc.cs en el Explorador de soluciones e inicie la depuración.
    public class Service1 : IService1
    {
        string strconn = @"workstation id=bdWSmacias.mssql.somee.com;packet size=4096;user id=FranciscoMacias_SQLLogin_1;pwd=up1zulubg4;data source=bdWSmacias.mssql.somee.com;persist security info=False;initial catalog=bdWSmacias;TrustServerCertificate=True";
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string ObtenerDatos()
        {

            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = "SELECT * FROM datosuno";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }

                    }

                }
            }
            catch (Exception ex)
            {

                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            

            return res;
        }


        public string ObtenerEstado(int idEquipo)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = "SELECT * FROM datosdos WHERE idEquipo = @idEquipo";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idEquipo", idEquipo);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerDatosGraficas()
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = "SELECT nombre, superBowls, division, anioFundacion FROM DatosUno";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ValidarLogin(string usuario, string contrasena)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Usuarios WHERE usuario = @usuario AND contrasena = @contrasena";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@usuario", usuario);
                        cmd.Parameters.AddWithValue("@contrasena", contrasena);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerResumenDashboard()
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    (SELECT COUNT(*) FROM INSCRIPCION WHERE estado = 'activo') AS alumnosActivos,
                    
                    (SELECT COUNT(DISTINCT g.id_programa) 
                     FROM GRUPO g 
                     INNER JOIN INSCRIPCION i ON g.id_grupo = i.id_grupo
                     WHERE i.estado = 'activo') AS programasActivos,
                    
                    (SELECT COUNT(*) FROM CONSTANCIA) AS constanciasAnio,
                    
                    (SELECT ISNULL(SUM(monto), 0) 
                    FROM PAGO_ALUMNO 
                    WHERE estatus_pago = 'pagado'
                    AND YEAR(fecha) = 2025) AS ingresosMes,
                    
                    (SELECT ISNULL(SUM(monto), 0) 
                    FROM PAGO_ALUMNO 
                    WHERE estatus_pago = 'pagado'
                    AND YEAR(fecha) = 2024) AS ingresosMesAnterior,
                    
                    (SELECT COUNT(*) FROM INSCRIPCION WHERE estado = 'activo' 
                     AND id_grupo IN (
                         SELECT id_grupo FROM GRUPO WHERE id_programa IN (
                             SELECT id_programa FROM PROGRAMA WHERE tipo = 'diplomado'
                         )
                     )) AS alumnosDiplomados,
                    
                    (SELECT COUNT(*) FROM INSCRIPCION WHERE estado = 'activo' 
                     AND id_grupo IN (
                         SELECT id_grupo FROM GRUPO WHERE id_programa IN (
                             SELECT id_programa FROM PROGRAMA WHERE tipo = 'curso'
                         )
                     )) AS alumnosCursos,
                    
                    (SELECT COUNT(*) FROM INSCRIPCION WHERE estado = 'activo' 
                     AND id_grupo IN (
                         SELECT id_grupo FROM GRUPO WHERE id_programa IN (
                             SELECT id_programa FROM PROGRAMA WHERE tipo = 'taller'
                         )
                     )) AS alumnosTalleres,
                    
                    (SELECT CAST(COUNT(*) * 100.0 / NULLIF(
                        (SELECT COUNT(*) FROM INSCRIPCION), 0
                     ) AS DECIMAL(5,2)) 
                     FROM BAJA) AS tasaDesercion,

                    (SELECT COUNT(*) FROM BAJA 
                    WHERE YEAR(fecha_baja) = 2025) AS totalBajas";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adap.Fill(dt);
                        res = JsonConvert.SerializeObject(dt, Formatting.None);
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerProgramasActivos()
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT DISTINCT p.id_programa, p.nombre, p.tipo
                FROM PROGRAMA p
                INNER JOIN GRUPO g ON p.id_programa = g.id_programa
                WHERE g.fecha_fin >= DATEADD(YEAR, -2, GETDATE())
                ORDER BY p.tipo, p.nombre";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adap.Fill(dt);
                        res = JsonConvert.SerializeObject(dt, Formatting.None);
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerInscripcionesPorPrograma(int idPrograma)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    g.id_grupo,
                    g.fecha_inicio,
                    g.fecha_fin,
                    g.cupo_maximo,
                    COUNT(i.id_inscripcion) AS total_inscritos,
                    SUM(CASE WHEN i.estado = 'egresado' THEN 1 ELSE 0 END) AS egresados,
                    SUM(CASE WHEN i.estado = 'baja' THEN 1 ELSE 0 END) AS bajas,
                    SUM(CASE WHEN i.estado = 'activo' THEN 1 ELSE 0 END) AS activos,
                    CAST(COUNT(i.id_inscripcion) * 100.0 / g.cupo_maximo AS DECIMAL(5,2)) AS porcentaje_ocupacion
                FROM GRUPO g
                LEFT JOIN INSCRIPCION i ON g.id_grupo = i.id_grupo
                WHERE g.id_programa = @idPrograma
                GROUP BY g.id_grupo, g.fecha_inicio, g.fecha_fin, g.cupo_maximo
                ORDER BY g.fecha_inicio";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPrograma", idPrograma);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerResumenFinanzas(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT
                    ISNULL(SUM(CASE WHEN estatus_pago = 'pagado' THEN monto ELSE 0 END), 0) AS cobrado,
                    ISNULL(SUM(CASE WHEN estatus_pago = 'pendiente' THEN monto ELSE 0 END), 0) AS pendiente,
                    ISNULL(SUM(CASE WHEN estatus_pago = 'vencido' THEN monto ELSE 0 END), 0) AS vencido,
                    ISNULL(SUM(monto), 0) AS totalFacturado
                FROM PAGO_ALUMNO
                WHERE YEAR(fecha) = @anio";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerEgresosPorAnio(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT ISNULL(SUM(monto), 0) AS totalEgresos
                FROM PAGO_PROFESOR
                WHERE YEAR(fecha_pago) = @anio";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerFinanzasPorAnio()
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    anio,
                    ISNULL(SUM(ingresos), 0) AS ingresos,
                    ISNULL(SUM(egresos), 0) AS egresos
                FROM (
                    SELECT YEAR(fecha) AS anio, SUM(monto) AS ingresos, 0 AS egresos
                    FROM PAGO_ALUMNO WHERE estatus_pago = 'pagado'
                    GROUP BY YEAR(fecha)
                    UNION ALL
                    SELECT YEAR(fecha_pago) AS anio, 0 AS ingresos, SUM(monto) AS egresos
                    FROM PAGO_PROFESOR
                    GROUP BY YEAR(fecha_pago)
                ) t
                GROUP BY anio
                ORDER BY anio";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adap.Fill(dt);
                        res = JsonConvert.SerializeObject(dt, Formatting.None);
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }


        public string ObtenerResumenDesercion(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT
                    COUNT(b.id_baja) AS totalBajas,
                    (SELECT COUNT(*) FROM INSCRIPCION WHERE YEAR(fecha_inscripcion) = @anio) AS totalInscritos,
                    CAST(COUNT(b.id_baja) * 100.0 / NULLIF((SELECT COUNT(*) FROM INSCRIPCION WHERE YEAR(fecha_inscripcion) = @anio), 0) AS DECIMAL(5,2)) AS tasaDesercion,
                    SUM(CASE WHEN DATEDIFF(DAY, i.fecha_inscripcion, b.fecha_baja) <= 14 THEN 1 ELSE 0 END) AS bajasTemprana
                FROM BAJA b
                INNER JOIN INSCRIPCION i ON b.id_inscripcion = i.id_inscripcion
                WHERE YEAR(b.fecha_baja) = @anio";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerMotivosBajas(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    ISNULL(CAST(motivo AS VARCHAR(500)), 'Sin motivo especificado') AS motivo,
                    COUNT(*) AS cantidad,
                    CAST(COUNT(*) * 100.0 / NULLIF((SELECT COUNT(*) FROM BAJA WHERE YEAR(fecha_baja) = @anio), 0) AS DECIMAL(5,2)) AS porcentaje
                FROM BAJA
                WHERE YEAR(fecha_baja) = @anio
                GROUP BY CAST(motivo AS VARCHAR(500))
                ORDER BY cantidad DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerBajasPorMes(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    MONTH(fecha_baja) AS mes,
                    COUNT(*) AS bajas
                FROM BAJA
                WHERE YEAR(fecha_baja) = @anio
                GROUP BY MONTH(fecha_baja)
                ORDER BY mes";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerIngresosPorTipo(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    p.tipo,
                    ISNULL(SUM(pa.monto), 0) AS ingresos,
                    COUNT(DISTINCT i.id_inscripcion) AS totalAlumnos
                FROM PAGO_ALUMNO pa
                INNER JOIN INSCRIPCION i ON pa.id_inscripcion = i.id_inscripcion
                INNER JOIN GRUPO g ON i.id_grupo = g.id_grupo
                INNER JOIN PROGRAMA p ON g.id_programa = p.id_programa
                WHERE pa.estatus_pago = 'pagado'
                AND YEAR(pa.fecha) = @anio
                GROUP BY p.tipo";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerTopProgramasRentables(int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT TOP 5
                    p.nombre,
                    p.tipo,
                    ISNULL(SUM(pa.monto), 0) AS ingresos,
                    ISNULL((SELECT SUM(pp.monto) FROM PAGO_PROFESOR pp 
                            INNER JOIN GRUPO gg ON pp.id_grupo = gg.id_grupo
                            WHERE gg.id_programa = p.id_programa
                            AND YEAR(pp.fecha_pago) = @anio), 0) AS egresos,
                    ISNULL(SUM(pa.monto), 0) - 
                    ISNULL((SELECT SUM(pp.monto) FROM PAGO_PROFESOR pp 
                            INNER JOIN GRUPO gg ON pp.id_grupo = gg.id_grupo
                            WHERE gg.id_programa = p.id_programa
                            AND YEAR(pp.fecha_pago) = @anio), 0) AS ganancia
                FROM PAGO_ALUMNO pa
                INNER JOIN INSCRIPCION i ON pa.id_inscripcion = i.id_inscripcion
                INNER JOIN GRUPO g ON i.id_grupo = g.id_grupo
                INNER JOIN PROGRAMA p ON g.id_programa = p.id_programa
                WHERE pa.estatus_pago = 'pagado'
                AND YEAR(pa.fecha) = @anio
                GROUP BY p.id_programa, p.nombre, p.tipo
                ORDER BY ganancia DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerBajasPorPrograma(int idPrograma, int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    MONTH(b.fecha_baja) AS mes,
                    COUNT(*) AS bajas
                FROM BAJA b
                INNER JOIN INSCRIPCION i ON b.id_inscripcion = i.id_inscripcion
                INNER JOIN GRUPO g ON i.id_grupo = g.id_grupo
                WHERE g.id_programa = @idPrograma
                AND YEAR(b.fecha_baja) = @anio
                GROUP BY MONTH(b.fecha_baja)
                ORDER BY mes";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPrograma", idPrograma);
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }

        public string ObtenerMotivosBajasPorPrograma(int idPrograma, int anio)
        {
            string res = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(strconn))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    ISNULL(CAST(b.motivo AS VARCHAR(500)), 'Sin motivo') AS motivo,
                    COUNT(*) AS cantidad,
                    CAST(COUNT(*) * 100.0 / NULLIF(
                        (SELECT COUNT(*) FROM BAJA bb
                         INNER JOIN INSCRIPCION ii ON bb.id_inscripcion = ii.id_inscripcion
                         INNER JOIN GRUPO gg ON ii.id_grupo = gg.id_grupo
                         WHERE gg.id_programa = @idPrograma
                         AND YEAR(bb.fecha_baja) = @anio), 0)
                    AS DECIMAL(5,2)) AS porcentaje
                FROM BAJA b
                INNER JOIN INSCRIPCION i ON b.id_inscripcion = i.id_inscripcion
                INNER JOIN GRUPO g ON i.id_grupo = g.id_grupo
                WHERE g.id_programa = @idPrograma
                AND YEAR(b.fecha_baja) = @anio
                GROUP BY CAST(b.motivo AS VARCHAR(500))
                ORDER BY cantidad DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPrograma", idPrograma);
                        cmd.Parameters.AddWithValue("@anio", anio);
                        using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adap.Fill(dt);
                            res = JsonConvert.SerializeObject(dt, Formatting.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                res = JsonConvert.SerializeObject($"Error... {ex.Message}");
            }
            return res;
        }


        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
