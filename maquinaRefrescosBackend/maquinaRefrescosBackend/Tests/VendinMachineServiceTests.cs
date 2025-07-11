using NUnit.Framework;
using System;
using maquinaRefrescosBackend.Aplicacion;
using maquinaRefrescosBackend.Models;


namespace maquinaRefrescosBackend.Tests
{
    [TestFixture]
    public class VendinMachineServiceTests
    {
        private VendinMachineService GetService()
        {
            return new VendinMachineService();
        }

        [Test]
        public void DevolverMonto_ReturnsCorrectAmount()
        {
            var service = GetService();
            var bebidas = new List<Bebida>
            {
                new Bebida { Nombre = "Coca Cola", Cantidad = 2, Precio = 800 },
                new Bebida { Nombre = "Pepsi", Cantidad = 1, Precio = 750 }
            };
            var monto = service.DevolverMonto(bebidas);
            Assert.AreEqual(2350, monto);
        }

        [Test]
        public void BebidasDisponibles_ReturnsAllBebidas()
        {
            var service = GetService();
            var disponibles = service.BebidasDisponibles();
            Assert.IsTrue(disponibles.Any(b => b.Nombre == "Coca Cola"));
            Assert.IsTrue(disponibles.Any(b => b.Nombre == "Pepsi"));
            Assert.IsTrue(disponibles.Any(b => b.Nombre == "Fanta"));
            Assert.IsTrue(disponibles.Any(b => b.Nombre == "Sprite"));
        }

        [Test]
        public void ValidarVuelto_ThrowsException_WhenVueltoIsTooHigh()
        {
            var service = GetService();
            Assert.Throws<Exception>(() => service.ValidarVuelto(999999));
        }

        [Test]
        public void ValidarVuelto_ReturnsTrue_WhenVueltoIsValid()
        {
            var service = GetService();
            var maxVuelto = service.Quinientos.Valor * service.Quinientos.Cantidad +
                            service.cien.Valor * service.cien.Cantidad +
                            service.cincuenta.Valor * service.cincuenta.Cantidad +
                            service.veinticinco.Valor * service.veinticinco.Cantidad;
            Assert.IsTrue(service.ValidarVuelto(maxVuelto - 1));
        }

        [Test]
        public void RestarMonedasDisponibles_ReturnsCorrectChange()
        {
            var service = GetService();
            var result = service.RestarMonedasDisponibles(575);
            Assert.IsTrue(result.Sum(m => m.Valor * m.Cantidad) == 575);
        }

        [Test]
        public void RestarMonedasDisponibles_ReturnsEmpty_WhenChangeNotPossible()
        {
            var service = GetService();
            var result = service.RestarMonedasDisponibles(999999);
            Assert.IsEmpty(result);
        }

        [Test]
        public void CalcularVuelto_ThrowsException_WhenInsufficientFunds()
        {
            var service = GetService();
            var monedas = new List<Moneda> { new Moneda { Valor = 500, Cantidad = 1 } };
            Assert.Throws<Exception>(() => service.CalcularVuelto(monedas, 1000));
        }

        [Test]
        public void CalcularVuelto_ReturnsChange_WhenOverpaid()
        {
            var service = GetService();
            var monedas = new List<Moneda> { new Moneda { Valor = 500, Cantidad = 3 } };
            var result = service.CalcularVuelto(monedas, 800);
            Assert.IsTrue(result.Sum(m => m.Valor * m.Cantidad) == 700);
        }

        [Test]
        public void CalcularVuelto_ReturnsEmpty_WhenExactPayment()
        {
            var service = GetService();
            var monedas = new List<Moneda> { new Moneda { Valor = 500, Cantidad = 2 } };
            var result = service.CalcularVuelto(monedas, 1000);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ComprarBebidas_ThrowsException_WhenNotEnoughStock()
        {
            var service = GetService();
            var bebidas = new List<Bebida> { new Bebida { Nombre = "Coca Cola", Cantidad = 999, Precio = 800 } };
            var monedas = new List<Moneda> { new Moneda { Valor = 500, Cantidad = 100 } };
            Assert.Throws<Exception>(() => service.ComprarBebidas(bebidas, monedas));
        }

        [Test]
        public void ComprarBebidas_DecreasesStockAndReturnsChange()
        {
            var service = GetService();
            var bebidas = new List<Bebida> { new Bebida { Nombre = "Coca Cola", Cantidad = 1, Precio = 800 } };
            var monedas = new List<Moneda> { new Moneda { Valor = 500, Cantidad = 2 } };
            var initialStock = service.CocaCola.Cantidad;
            var result = service.ComprarBebidas(bebidas, monedas);
            Assert.IsTrue(result.Sum(m => m.Valor * m.Cantidad) == 200);
            Assert.AreEqual(initialStock - 1, service.CocaCola.Cantidad);
        }
    }
}
