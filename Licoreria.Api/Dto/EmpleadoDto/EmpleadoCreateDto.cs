using System.ComponentModel.DataAnnotations.Schema;

namespace Licoreria.Api.Dto.EmpleadoDto
{
    public class EmpleadoCreateDto
    {
            public string Nombre { get; set; } = "";
            public string ApellidoPaterno { get; set; } = "";
            public string ApellidoMaterno { get; set; } = "";
            public string Sexo { get; set; } = "";
            public string CorreoElectronico { get; set; } = "";
            public string NumeroCelular { get; set; } = "";

            public string CURP { get; set; } = "";
            public string RFC { get; set; } = "";

            public string Direccion { get; set; } = "";
            public string NExterior { get; set; } = "";
            public string NInterior { get; set; } = "";
            public string Colonia { get; set; } = "";
            public string Municipio { get; set; } = "";
            public string Estado { get; set; } = "";
            public string CodigoPostal { get; set; } = "";




    }
}
