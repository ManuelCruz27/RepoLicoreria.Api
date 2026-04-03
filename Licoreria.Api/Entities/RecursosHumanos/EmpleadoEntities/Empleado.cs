using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Entities.RecursosHumanos.EmpleadoEntities
{
    [Table("Gestion_Empleados")]
    public class Empleado
    {
        [Key]
        public int EmpleadoID { get; set; }

        public string? Nombre { get; set; }
        public string? Apellido_Paterno { get; set; }
        public string? Apellido_Materno { get; set; }

        public string? Sexo { get; set; }

        public string? Correo { get; set; }

        // OJO: en tu tabla es bigint, aquí lo guardamos como long?
        public long? Celular { get; set; }

        public string? CURP { get; set; }  // (parece que tu columna se llama CURT)
        public string? RFC { get; set; }

        public string? Direccion { get; set; }

        // Si vas a agregar más columnas (colonia, municipio, etc.)
        public string? NExterior { get; set; }
        public string? NInterior { get; set; }

        public string? Colonia { get; set; }
        public string? Municipio { get; set; }
        public string? Estado { get; set; }

        public string? CodigoPostal { get; set; }
    }
}
