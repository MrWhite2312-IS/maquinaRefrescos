<template>
  <div class="vending-machine">
    <h1>Máquina de Refrescos</h1>
    <div class="drinks">
      <h2>Refrescos disponibles</h2>
      <table>
        <thead>
          <tr>
            <th>Nombre</th>
            <th>Precio</th>
            <th>Cantidad disponible</th>
            <th>Cantidad a comprar</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="(bebida, idx) in bebidas" :key="bebida.nombre">
            <td>{{ bebida.nombre }}</td>
            <td>{{ bebida.precio }} colones</td>
            <td>{{ bebida.cantidad }}</td>
            <td>
              <input
                type="number"
                min="0"
                :max="bebida.cantidad"
                v-model.number="compra[idx]"
                @input="actualizarMonto"
              />
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <div class="total">
      <h3>Costo total: {{ montoTotal }} colones</h3>
    </div>
    <div class="payment">
      <h2>Ingrese su pago</h2>
      <div v-for="moneda in monedasDisponibles" :key="moneda.valor">
        <label>
          {{ moneda.label }}:
          <input type="number" min="0" v-model.number="moneda.cantidad" />
        </label>
      </div>
      <div>
        <label>
          Billetes de 1000:
          <input type="number" min="0" v-model.number="billetes1000" />
        </label>
      </div>
    </div>
    <button @click="realizarCompra">Comprar</button>
    <div v-if="mensaje" class="mensaje" :class="{ error: error }">
      <pre>{{ mensaje }}</pre>
    </div>
  </div>
</template>

<script>
import axios from "axios";
export default {
  name: "VendingMachine",
  data() {
    return {
      bebidas: [],
      compra: [],
      montoTotal: 0,
      monedasDisponibles: [
        { valor: 500, cantidad: 0, label: "Monedas de 500" },
        { valor: 100, cantidad: 0, label: "Monedas de 100" },
        { valor: 50, cantidad: 0, label: "Monedas de 50" },
        { valor: 25, cantidad: 0, label: "Monedas de 25" },
      ],
      billetes1000: 0,
      mensaje: "",
      error: false,
    };
  },
  mounted() {
    this.cargarBebidas();
  },
  methods: {
    async cargarBebidas() {
      try {
        const res = await axios.get(
          "https://localhost:7115/api/VendingMachine"
        );
        this.bebidas = res.data;
        this.compra = this.bebidas.map(() => 0);
      } catch (e) {
        this.mensaje = "Error al cargar bebidas.";
        this.error = true;
      }
    },
    async actualizarMonto() {
      const seleccion = this.bebidas
        .map((b, idx) => ({
          nombre: b.nombre,
          cantidad: this.compra[idx],
          precio: b.precio,
        }))
        .filter((b) => b.cantidad > 0);
      if (seleccion.length === 0) {
        this.montoTotal = 0;
        return;
      }
      try {
        const res = await axios.post(
          "https://localhost:7115/api/vendingmachine/DevolverMonto",
          seleccion
        );
        this.montoTotal = res.data;
      } catch (e) {
        this.mensaje = "Error al calcular el monto.";
        this.error = true;
      }
    },
    async realizarCompra() {
      this.mensaje = "";
      this.error = false;
      const bebidasAComprar = this.bebidas
        .map((b, idx) => ({
          nombre: b.nombre,
          cantidad: this.compra[idx],
          precio: b.precio,
        }))
        .filter((b) => b.cantidad > 0);
      if (bebidasAComprar.length === 0) {
        this.mensaje = "Seleccione al menos un refresco.";
        this.error = true;
        return;
      }
      const monedas = this.monedasDisponibles.map((m) => ({
        valor: m.valor,
        cantidad: m.cantidad,
      }));
      if (this.billetes1000 > 0) {
        monedas.push({ valor: 1000, cantidad: this.billetes1000 });
      }
      try {
        const res = await axios.put(
          "https://localhost:7115/api/vendingmachine",
          {
            bebidas: bebidasAComprar,
            monedas,
          }
        );
        if (Array.isArray(res.data) && res.data.length > 0) {
          let totalVuelto = res.data.reduce(
            (acc, m) => acc + m.valor * m.cantidad,
            0
          );
          let desglose = res.data
            .map(
              (m) =>
                `${m.cantidad} moneda${m.cantidad > 1 ? "s" : ""} de ${m.valor}`
            )
            .join("\n");
          this.mensaje = `Su vuelto es de ${totalVuelto} colones.\nDesglose:\n${desglose}`;
        } else {
          this.mensaje = "No hay vuelto que entregar.";
        }
        await this.cargarBebidas();
      } catch (e) {
        if (e.response && e.response.data) {
          this.mensaje =
            typeof e.response.data === "string"
              ? e.response.data
              : e.response.data.title || "Fallo al realizar la compra";
        } else {
          this.mensaje = "Fallo al realizar la compra";
        }
        this.error = true;
        await this.cargarBebidas();
      }
    },
  },
};
</script>

<style scoped>
.vending-machine {
  max-width: 650px;
  margin: 2em auto;
  font-family: "Segoe UI", Arial, sans-serif;
  background: #f8fafc;
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(0, 0, 0, 0.08);
  padding: 2em 2.5em 2em 2.5em;
}
h1 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 1.5em;
  font-size: 2.2em;
  letter-spacing: 1px;
}
.drinks h2,
.payment h2 {
  color: #2980b9;
  margin-bottom: 0.5em;
}
table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 1.5em;
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 2px 8px rgba(44, 62, 80, 0.05);
}
th {
  background: #2980b9;
  color: #fff;
  font-weight: 600;
  padding: 0.7em;
  border: none;
}
td {
  border-bottom: 1px solid #e1e4e8;
  padding: 0.7em;
  text-align: center;
  background: #f9f9f9;
}
tr:last-child td {
  border-bottom: none;
}
input[type="number"] {
  width: 60px;
  padding: 0.3em 0.5em;
  border-radius: 5px;
  border: 1px solid #b2bec3;
  font-size: 1em;
  background: #fff;
  transition: border-color 0.2s;
}
input[type="number"]:focus {
  border-color: #2980b9;
  outline: none;
}
.total {
  text-align: right;
  margin-bottom: 1.5em;
}
.total h3 {
  color: #16a085;
  font-size: 1.3em;
  margin: 0;
}
.payment {
  margin-bottom: 1.5em;
}
.payment label {
  display: inline-block;
  margin-right: 1.5em;
  margin-bottom: 0.7em;
  font-size: 1em;
  color: #34495e;
}
button {
  display: block;
  width: 100%;
  background: linear-gradient(90deg, #16a085 0%, #2980b9 100%);
  color: #fff;
  font-size: 1.2em;
  font-weight: 600;
  border: none;
  border-radius: 8px;
  padding: 0.8em 0;
  margin-top: 1em;
  cursor: pointer;
  box-shadow: 0 2px 8px rgba(44, 62, 80, 0.08);
  transition: background 0.2s;
}
button:hover {
  background: linear-gradient(90deg, #2980b9 0%, #16a085 100%);
}
.mensaje {
  margin-top: 1.5em;
  padding: 1.2em;
  border-radius: 8px;
  background: #e0ffe0;
  color: #222;
  font-size: 1.1em;
  box-shadow: 0 2px 8px rgba(44, 62, 80, 0.05);
}
.mensaje.error {
  background: #ffe0e0;
  color: #a00;
  border: 1px solid #e74c3c;
}
</style>
