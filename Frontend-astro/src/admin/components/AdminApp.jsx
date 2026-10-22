import React, { useEffect, useMemo, useRef, useState } from 'react'
import { useAuth } from '../../stores/auth'
import { useCartStore } from '../../stores/cart'
import Dashboard from './Dashboard.jsx'
import Products from './Products.jsx'
import Categories from './Categories.jsx'
import Orders from './Orders.jsx'
import Graphs from './Graphs.jsx'
import Inventory from './Inventory.jsx'

const links = [
  ['dashboard', 'Dashboard'],
  ['products', 'Productos'],
  ['categories', 'Categorías'],
  ['orders', 'Órdenes'],
  ['graphs', 'Gráficas'],
  ['inventory', 'Inventario'],
]

export default function AdminApp() {
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)
  const clearCart = useCartStore((s) => s.clearCart)
  const [section, setSection] = useState('dashboard')
  const saliendo = useRef(false)

  const esAdmin = user?.rol === 'Admin'

  useEffect(() => {
    if (saliendo.current) return
    if (!user) window.location.href = '/login'
    else if (!esAdmin) window.location.href = '/'
  }, [user, esAdmin])

  const current = useMemo(() => links.find((x) => x[0] === section)?.[1] || 'Dashboard', [section])

  if (!esAdmin) return null

  function handleLogout() {
    saliendo.current = true
    logout()
    clearCart()
    window.location.href = '/'
  }

  return (
    <div className="admin-shell">
      <aside className="sidebar">
        <div className="brand">Hilo y Alma <span> <br></br>Panel del admin</span></div>
        <nav className="nav">
          {links.map(([id, label]) => (
            <a href={`#${id}`} className={section === id ? 'active' : ''} onClick={(e) => { e.preventDefault(); setSection(id) }} key={id}>{label}</a>
          ))}
        </nav>
      </aside>
      <main className="main">
        <div className="mobile-nav">
          {links.map(([id, label]) => (
            <button className="btn secondary" onClick={() => setSection(id)} key={id}>{label}</button>
          ))}
        </div>
        <div className="topbar">
          <h1>{current}</h1>
          <a href="/" className="btn secondary">Volver a la tienda</a>
          <button className="btn secondary" onClick={handleLogout}>Cerrar sesión</button>
        </div>
        {section === 'dashboard' && <Dashboard />}
        {section === 'products' && <Products />}
        {section === 'categories' && <Categories />}
        {section === 'orders' && <Orders />}
        {section === 'graphs' && <Graphs />}
        {section === 'inventory' && <Inventory />}
      </main>
    </div>
  )
}