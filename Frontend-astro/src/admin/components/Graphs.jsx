import React, { useEffect, useMemo, useState } from 'react'
import {
  ResponsiveContainer, LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip,
  BarChart, Bar, PieChart, Pie, Cell, Legend,
} from 'recharts'
import { getAdminOrders, getAdminProducts } from '../api/admin'
import { STATUS_LABELS } from '../lib/config'

const STATUS_COLORS = {
  Pending: '#f59e0b',
  Confirmed: '#3b82f6',
  Shipped: '#8b5cf6',
  Delivered: '#10b981',
  Cancelled: '#ef4444',
}

const money = (n) =>
  Number(n).toLocaleString('es-MX', { style: 'currency', currency: 'MXN' })

function Empty() {
  return <div className="empty">Sin datos todavía.</div>
}

export default function Dashboard() {
  const [orders, setOrders] = useState([])
  const [products, setProducts] = useState([])
  const [error, setError] = useState('')

  useEffect(() => {
    async function load() {
      try {
        setOrders(await getAdminOrders())
        setProducts(await getAdminProducts())
      } catch (e) {
        setError(e.message)
      }
    }
    load()
  }, [])

  const stats = useMemo(() => {

    const valid = orders.filter((o) => o.status !== 'Cancelled')
    const revenue = valid.reduce((s, o) => s + Number(o.total), 0)

    const byDay = {}
    valid.forEach((o) => {
      const day = o.createdAt.slice(0, 10)
      byDay[day] = (byDay[day] || 0) + Number(o.total)
    })
    const sales = Object.entries(byDay)
      .sort(([a], [b]) => a.localeCompare(b))
      .map(([day, total]) => ({ day, total }))

    const byStatus = {}
    orders.forEach((o) => {
      byStatus[o.status] = (byStatus[o.status] || 0) + 1
    })
    const statuses = Object.entries(byStatus).map(([key, value]) => ({ key, name: STATUS_LABELS[key] || key, value }))

    const byProduct = {}
    valid.forEach((o) =>
      o.items?.forEach((i) => {
        byProduct[i.name] = (byProduct[i.name] || 0) + i.quantity
      })
    )
    const top = Object.entries(byProduct)
      .map(([name, qty]) => ({ name, qty }))
      .sort((a, b) => b.qty - a.qty)
      .slice(0, 5)

    return {
      revenue,
      count: orders.length,
      avg: valid.length ? revenue / valid.length : 0,
      pending: byStatus.Pending || 0,
      sales,
      statuses,
      top,
    }
  }, [orders])

  const lowStock = useMemo(
    () =>
      [...products]
        .sort((a, b) => a.stock - b.stock)
        .slice(0, 6)
        .map((p) => ({ name: p.name, stock: p.stock })),
    [products]
  )

  return (
    <div className="grid">
      {error && <div className="alert">{error}</div>}

      <div className="grid grid-2">
        <div className="card"><small>Ventas</small><h2>{money(stats.revenue)}</h2></div>
        <div className="card"><small>Órdenes</small><h2>{stats.count}</h2></div>
        <div className="card"><small>Ticket promedio</small><h2>{money(stats.avg)}</h2></div>
        <div className="card"><small>Pendientes</small><h2>{stats.pending}</h2></div>
      </div>

      <div className="card">
        <h3>Ventas por día</h3>
        {stats.sales.length ? (
          <div style={{ height: 280 }}>
            <ResponsiveContainer>
              <LineChart data={stats.sales}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="day" />
                <YAxis />
                <Tooltip formatter={(v) => money(v)} />
                <Line type="monotone" dataKey="total" stroke="#3b82f6" strokeWidth={2} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        ) : <Empty />}
      </div>

      <div className="grid grid-2">
        <div className="card">
          <h3>Productos más vendidos</h3>
          {stats.top.length ? (
            <div style={{ height: 280 }}>
              <ResponsiveContainer>
                <BarChart data={stats.top} layout="vertical" margin={{ left: 20 }}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis type="number" allowDecimals={false} />
                  <YAxis type="category" dataKey="name" width={120} />
                  <Tooltip />
                  <Bar dataKey="qty" name="Unidades" fill="#b3b910" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          ) : <Empty />}
        </div>

        <div className="card">
          <h3>Órdenes por estado</h3>
          {stats.statuses.length ? (
            <div style={{ height: 280 }}>
              <ResponsiveContainer>
                <PieChart>
                  <Pie data={stats.statuses} dataKey="value" nameKey="name" innerRadius={55} outerRadius={90}>
                    {stats.statuses.map((s) => (
                      <Cell key={s.key} fill={STATUS_COLORS[s.key] || '#94a3b8'} />
                    ))}
                  </Pie>
                  <Tooltip />
                  <Legend />
                </PieChart>
              </ResponsiveContainer>
            </div>
          ) : <Empty />}
        </div>
      </div>

      <div className="card">
        <h3>Productos con menos stock</h3>
        {lowStock.length ? (
          <div style={{ height: 280 }}>
            <ResponsiveContainer>
              <BarChart data={lowStock}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis allowDecimals={false} />
                <Tooltip />
                <Bar dataKey="stock" name="Stock" fill="#f59e0b" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        ) : <Empty />}
      </div>
    </div>
  )
}