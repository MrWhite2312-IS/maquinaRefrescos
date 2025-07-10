using maquinaRefrescosBackend.Models;

namespace maquinaRefrescosBackend.Aplicacion
{
    public interface IVendingMachineService
    {
        public double DevolverMonto(List<Bebida> Bebidas);
        public bool ValidarMonedas(List<Moneda> Monedas);
        public double CalcularVuelto(List<Moneda> Monedas, double MontoTotal);
        public Factura ComprarBebidas(List<Bebida> Bebidas, List<Moneda> Monedas);

    }
}
