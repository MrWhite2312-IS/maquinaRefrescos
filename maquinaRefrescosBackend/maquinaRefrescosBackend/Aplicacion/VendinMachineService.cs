using maquinaRefrescosBackend.Models;

namespace maquinaRefrescosBackend.Aplicacion
{
    public class VendinMachineService : IVendingMachineService
    {
        public Bebida CocaCola { get; set; } = new Bebida { Nombre = "Coca Cola", Cantidad = 10, Precio = 800 };
        public Bebida Pepsi { get; set; } = new Bebida { Nombre = "Pepsi", Cantidad = 8, Precio = 750 };
        public Bebida Fanta { get; set; } = new Bebida { Nombre = "Fanta", Cantidad = 10, Precio = 950 };
        public Bebida Sprite { get; set; } = new Bebida { Nombre = "Sprite", Cantidad = 15, Precio = 975 };

        public double DevolverMonto(List<Bebida> Bebidas)
        {
            return Bebidas.Sum(b => (double)b.Precio * b.Cantidad);
        }

    }
}
