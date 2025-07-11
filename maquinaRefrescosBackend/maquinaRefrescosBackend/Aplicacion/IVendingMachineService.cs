using maquinaRefrescosBackend.Models;

namespace maquinaRefrescosBackend.Aplicacion
{
    public interface IVendingMachineService
    {
        public Bebida CocaCola { get; set; }
        public Bebida Pepsi { get; set; }
        public Bebida Fanta { get; set; }
        public Bebida Sprite { get; set; }

        public Moneda Quinientos { get; set; }
        public Moneda cien { get; set; }

        public Moneda cincuenta { get; set; }
        public Moneda veinticinco { get; set; }

        public List<Bebida> BebidasDisponibles();
        public double DevolverMonto(List<Bebida> Bebidas);
        public bool ValidarVuelto(double vuelto);
        public List<Moneda> RestarMonedasDisponibles(double Vuelto);
        public List<Moneda> CalcularVuelto(List<Moneda> Monedas, double montoOrden);
        public List<Moneda> ComprarBebidas(List<Bebida> Bebidas, List<Moneda> Monedas);
        public List<Moneda> monedasParaVuelto();

    }
}
