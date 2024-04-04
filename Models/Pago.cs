namespace PruebaTecnicaDIAN.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime Fecha { get; set; }
        public string Nombre { get; set; }
        public string TipoIdentificacion { get; set; }
        public string NumeroIdentificacion { get; set; }
        public decimal Monto  { get; set; }
    }
}
