using System.ComponentModel.DataAnnotations;

namespace Front.Models.Respuestas
{
    public class UsuarioFront
    {
        public int ID { get; set; }
        public string DNI { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string CaracteristicaTelefono { get; set; }
        public string NumeroTelefono { get; set; }

        public string Localidad { get; set; }
        public string Direccion { get; set; }

        [Display(Name = "Rol")]
        public string RolUsuarioDescripcion { get; set; }
        public DateTime FechaContratoIngreso { get; set; }
    }
}