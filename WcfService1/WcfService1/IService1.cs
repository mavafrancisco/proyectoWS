using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace WcfService1
{
    // NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de interfaz "IService1" en el código y en el archivo de configuración a la vez.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);


        [OperationContract]
        string ObtenerDatos();

        [OperationContract]                        
        string ObtenerEstado(int idEquipo);

        [OperationContract]
        string ObtenerDatosGraficas();

        [OperationContract]
        string ValidarLogin(string usuario, string contrasena);

        [OperationContract]
        string ObtenerResumenDashboard();

        [OperationContract]
        string ObtenerProgramasActivos();

        [OperationContract]
        string ObtenerInscripcionesPorPrograma(int idPrograma);

        [OperationContract]
        string ObtenerResumenFinanzas(int anio);

        [OperationContract]
        string ObtenerEgresosPorAnio(int anio);

        [OperationContract]
        string ObtenerFinanzasPorAnio();

        [OperationContract]
        string ObtenerResumenDesercion(int anio);

        [OperationContract]
        string ObtenerMotivosBajas(int anio);

        [OperationContract]
        string ObtenerBajasPorMes(int anio);

        [OperationContract]
        string ObtenerIngresosPorTipo(int anio);

        [OperationContract]
        string ObtenerTopProgramasRentables(int anio);

        [OperationContract]
        string ObtenerBajasPorPrograma(int idPrograma, int anio);

        [OperationContract]
        string ObtenerMotivosBajasPorPrograma(int idPrograma, int anio);
        // TODO: agregue aquí sus operaciones de servicio
    }


    // Utilice un contrato de datos, como se ilustra en el ejemplo siguiente, para agregar tipos compuestos a las operaciones de servicio.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
