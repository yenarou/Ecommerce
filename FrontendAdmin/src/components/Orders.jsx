import React, { useEffect, useState } from 'react'
import { getAdminOrders, updateOrderStatus } from '../api/admin'
import { ORDER_STATUSES } from '../lib/config'

export default function Orders() {
  const [orders, setOrders] = useState([])
  const [error, setError] = useState('')

  async function load() {
    try {
      setOrders(await getAdminOrders())
    } catch (e) {
      setError(e.message)
    }
  }

  useEffect(() => {
    load()
  }, [])

  async function change(o, status) {
    if (status === o.status) return
    try {
      await updateOrderStatus(o.id, status)
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  return (
    <div className="grid">
      {error && <div className="alert">{error}</div>}

      <div className="card table-wrap">
        <table className="table">
          <thead>
            <tr>
              <th>Cliente</th>
              <th>Fecha</th>
              <th>Total</th>
              <th>Estado</th>
              <th>Productos</th>
            </tr>
          </thead>
          <tbody>
            {orders.map((o) => (
              <tr key={o.id}>
                <td>{o.user?.email || 'Sin correo'}</td>
                <td>{new Date(o.createdAt).toLocaleString('es-MX')}</td>
                <td>{o.total}</td>
                <td>
                  <select
                    value={o.status}
                    onChange={(e) => change(o, e.target.value)}
                  >
                    {ORDER_STATUSES.map((s) => (
                      <option key={s}>{s}</option>
                    ))}
                  </select>
                </td>
                <td>
                  {o.items
                    ?.map(
                      (i) => `${i.product?.name || 'Producto'} × ${i.quantity}`
                    )
                    .join(', ')}
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {!orders.length && <div className="empty">No hay órdenes.</div>}
      </div>
    </div>
  )
}