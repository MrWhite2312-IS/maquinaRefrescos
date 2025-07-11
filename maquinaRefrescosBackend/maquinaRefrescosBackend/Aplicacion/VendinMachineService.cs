using maquinaRefrescosBackend.Models;

namespace maquinaRefrescosBackend.Aplicacion
{
    public class VendinMachineService : IVendingMachineService
    {
        public Bebida CocaCola { get; set; } = new Bebida { Nombre = "Coca Cola", Cantidad = 10, Precio = 800 };
        public Bebida Pepsi { get; set; } = new Bebida { Nombre = "Pepsi", Cantidad = 8, Precio = 750 };
        public Bebida Fanta { get; set; } = new Bebida { Nombre = "Fanta", Cantidad = 10, Precio = 950 };
        public Bebida Sprite { get; set; } = new Bebida { Nombre = "Sprite", Cantidad = 15, Precio = 975 };

        public Moneda Quinientos { get; set; } = new Moneda { Valor = 500, Cantidad = 20 };
        public Moneda cien { get; set; } = new Moneda { Valor = 100, Cantidad = 30 };
        public Moneda cincuenta { get; set; } = new Moneda { Valor = 50, Cantidad = 50 };
        public Moneda veinticinco { get; set; } = new Moneda { Valor = 25, Cantidad = 25 };



        public double DevolverMonto(List<Bebida> Bebidas)
        {
            return Bebidas.Sum(b => (double)b.Precio * b.Cantidad);
        }

        public List<Bebida> BebidasDisponibles()
        {
            return new List<Bebida> { CocaCola, Pepsi, Fanta, Sprite };
        }

        public bool ValidarVuelto(double vuelto)
        {
            double vueltoMaximo = Quinientos.Valor * Quinientos.Cantidad +
                                     cien.Valor * cien.Cantidad +
                                     cincuenta.Valor * cincuenta.Cantidad +
                                     veinticinco.Valor * veinticinco.Cantidad;
            if (vuelto > vueltoMaximo)
            {
                throw new Exception("El vuelto solicitado es mayor al disponible en la máquina.");
            }
            return true;
        }
        public List<Moneda> RestarMonedasDisponibles(double Vuelto)
        {
            var monedasEntregadas = new List<Moneda>();
            double restante = Vuelto;


            var monedas = new List<Moneda> { Quinientos, cien, cincuenta, veinticinco }
                .OrderByDescending(m => m.Valor);

            foreach (var moneda in monedas)
            {
                int cantidadAEntregar = 0;
                while (restante >= moneda.Valor && moneda.Cantidad > 0)
                {
                    restante -= moneda.Valor;
                    moneda.Cantidad--;
                    cantidadAEntregar++;
                }
                if (cantidadAEntregar > 0)
                {
                    monedasEntregadas.Add(new Moneda { Valor = moneda.Valor, Cantidad = cantidadAEntregar });
                }
            }
            if (restante > 0)
            {

                foreach (var entregada in monedasEntregadas)
                {
                    var original = monedas.First(m => m.Valor == entregada.Valor);
                    original.Cantidad += entregada.Cantidad;
                }
                return new List<Moneda>();
            }

            return monedasEntregadas;
        }

        public List<Moneda> CalcularVuelto(List<Moneda> Monedas, double montoOrden)
        {
            double montoIngresado = Monedas.Sum(m => m.Valor * m.Cantidad);
            if (montoIngresado < montoOrden)
            {
                throw new Exception("El monto de las monedas es insuficiente para cubrir el costo de la orden.");
            }
            else if (montoIngresado > montoOrden)
            {

                double vuelto = montoIngresado - montoOrden;
                ValidarVuelto(vuelto);
                List<Moneda> monedasVuelto = RestarMonedasDisponibles(vuelto);

                return monedasVuelto;

            }
            else
            {
                return new List<Moneda>();

            }

        }

        public List<Moneda> ComprarBebidas(List<Bebida> Bebidas, List<Moneda> Monedas)
        {
            double montoOrden = DevolverMonto(Bebidas);
            List<Moneda> monedasVuelto = CalcularVuelto(Monedas, montoOrden);
            foreach (var bebida in Bebidas)
            {
                var bebidaDisponible = BebidasDisponibles().FirstOrDefault(b => b.Nombre == bebida.Nombre);
                if (bebidaDisponible == null || bebidaDisponible.Cantidad < bebida.Cantidad)
                {
                    throw new Exception($"No hay suficiente {bebida.Nombre} disponible.");
                }
                bebidaDisponible.Cantidad -= bebida.Cantidad;
            }
            return monedasVuelto;
        }
    }
}
