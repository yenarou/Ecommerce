import React, { useEffect, useState } from 'react'
import { getAdminProducts, getStockMovements, adjustProductStock } from '../api/admin'

const LOW_STOCK = 3

const REASON_LABELS = {
  Initial: 'Inventario inicial',
  Sale: 'Venta',
  Purchase: 'Compra a proveedor',
  Adjustment: 'Ajuste manual',
  Cancellation: 'Cancelación de orden',
}

function StockBadge({ stock }) {
  const low = stock <= LOW_STOCK
  return (
    <span
      style={{
        padding: '2px 10px',
        borderRadius: 999,
        fontWeight: 600,
        fontSize: 13,
        background: low ? '#fee2e2' : '#d1fae5',
        color: low ? '#991b1b' : '#065f46',
      }}
    >
      {stock}
    </span>
  )
}

export default function Inventory() {
  const [products, setProducts] = useState([])
  const [movements, setMovements] = useState([])
  const [filter, setFilter] = useState('')
  const [error, setError] = useState('')

  async function load() {
    try {
      setError('')
      setProducts(await getAdminProducts())
      setMovements(await getStockMovements(filter || null, 100))
    } catch (e) {
      setError(e.message)
    }
  }

  useEffect(() => {
    load()
  }, [filter])

  async function adjust(p) {
    const input = prompt(
      `Ajustar stock de "${p.name}" (actual: ${p.stock}).\nEscribe un número positivo para sumar o negativo para restar:`
    )
    if (input === null) return

    const qty = Number.parseInt(input, 10)
    if (!Number.isInteger(qty) || qty === 0) {
      setError('Escribe un número entero distinto de cero.')
      return
    }

    try {
      await adjustProductStock(p.id, qty)
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  return (
    <div className="grid">
      {error && <div className="alert">{error}</div>}

      <div className="card table-wrap">
        <h3>Existencias</h3>
        <table className="table">
          <thead>
            <tr>
              <th>Producto</th>
              <th>Categoría</th>
              <th>Stock</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {[...products]
              .sort((a, b) => a.stock - b.stock)
              .map((p) => (
                <tr key={p.id}>
                  <td>{p.name}</td>
                  <td>{p.categoryName}</td>
                  <td><StockBadge stock={p.stock} /></td>
                  <td>
                    <button className="btn secondary" onClick={() => adjust(p)}>
                      Ajustar
                    </button>
                  </td>
                </tr>
              ))}
          </tbody>
        </table>
        {!products.length && <div className="empty">No hay productos.</div>}
      </div>

      <div className="card table-wrap">
        <div className="actions">
          <h3 style={{ flex: 1 }}>Historial de movimientos</h3>
          <select value={filter} onChange={(e) => setFilter(e.target.value)}>
            <option className="hammersmithOdeon" value="">Todos los productos</option>
            {products.map((p) => (
              <option key={p.id} value={p.id}>{p.name}</option>
            ))}
          </select>
        </div>
        <table className="table">
          <thead>
            <tr>
              <th>Fecha</th>
              <th>Producto</th>
              <th>Cantidad</th>
              <th>Motivo</th>
              <th>Usuario</th>
            </tr>
          </thead>
          <tbody>
            {movements.map((m) => (
              <tr key={m.id}>
                <td>{new Date(m.createdAt).toLocaleString('es-MX')}</td>
                <td>{m.productName}</td>
                <td style={{ fontWeight: 600, color: m.quantity > 0 ? '#065f46' : '#991b1b' }}>
                  {m.quantity > 0 ? `+${m.quantity}` : m.quantity}
                </td>
                <td>{REASON_LABELS[m.reason] || m.reason}</td>
                <td>{m.createdBy}</td>
              </tr>
            ))}
          </tbody>
        </table>
        {!movements.length && <div className="empty">Sin movimientos todavía.</div>}
      </div>
    </div>
  )
}