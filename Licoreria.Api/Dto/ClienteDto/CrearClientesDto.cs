namespace Licoreria.Api.Dto.ClienteDto
{
    public class CrearClientesDto
    {
        public string Nombre { get; set; } = "";
        public string ApellidoPaterno { get; set; } = "";
        public string ApellidoMaterno { get; set; } = "";
        public long Celular { get; set; }
        public string Email { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }

        public string Direccion { get; set; } = "";
        public string NumeroExterior { get; set; } = "";
        public string NumeroInterior { get; set; } = "";
        public string Colonia { get; set; } = "";
        public string Municipio { get; set; } = "";
        public string Estado { get; set; } = "";
        public string CodigoPostal { get; set; } = "";
        //public bool Activo { get; set; } = false;
        public int SucursalId { get; set; }

    }
}
