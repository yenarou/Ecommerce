import React, { useEffect, useMemo, useState } from 'react'
import { loginAdmin } from '../api/auth'
import { clearSession, getSession, getToken, saveSession } from '../lib/auth'
import Dashboard from './Dashboard.jsx'
import Products from './Products.jsx'
import Categories from './Categories.jsx'
import Orders from './Orders.jsx'

const links = [
  ['dashboard', 'Dashboard'],
  ['products', 'Productos'],
  ['categories', 'Categorías'],
  ['orders', 'Órdenes'],
]

function Login({ onLogin }) {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function submit(e) {
    e.preventDefault(); setError(''); setLoading(true)
    try { const data = await loginAdmin(email, password); saveSession(data); onLogin(data) }
    catch (err) { setError(err.message) }
    finally { setLoading(false) }
  }

  return <div className="login-page"><div className="login-card">
    <h1>Panel de administracion</h1><p className="muted">Inicia sesión para administrar los productos y asi</p>
    {error && <div className="alert">{error}</div>}
    <form className="form" onSubmit={submit}>
      <div className="field">
        <label>Correo</label>
        <input type="email" value={email} onChange={e=>setEmail(e.target.value)} required />
      </div>
      <div className="field">
        <label>Contraseña</label>
        <input type="password" value={password} onChange={e=>setPassword(e.target.value)} required />
      </div>
      <button className="btn" disabled={loading}>{loading ? 'Entrando...' : 'Iniciar sesión'}</button>
    </form>
  </div></div>
}

export default function AdminApp(){
  const [session,setSession]=useState(null)
  const [section,setSection]=useState('dashboard')
  useEffect(()=>{ if(getToken()) setSession(getSession() || {token:getToken()}) },[])

  const current = useMemo(()=>links.find(x=>x[0]===section)?.[1] || 'Dashboard',[section])
  if(!session) return <Login onLogin={setSession}/>
  function logout(){ clearSession(); setSession(null) }
  return <div className="admin-shell">
    <aside className="sidebar"><div className="brand">Ecommerce <span>Admin</span></div><nav className="nav">{links.map(([id,label])=><a href={`#${id}`} className={section===id?'active':''} onClick={e=>{e.preventDefault();setSection(id)}} key={id}>{label}</a>)}</nav></aside>
    <main className="main"><div className="mobile-nav">{links.map(([id,label])=><button className="btn secondary" onClick={()=>setSection(id)} key={id}>{label}</button>)}</div>
      <div className="topbar"><h1>{current}</h1><button className="btn secondary" onClick={logout}>Cerrar sesión</button></div>
      {section==='dashboard' && <Dashboard/>}{section==='products' && <Products/>}{section==='categories' && <Categories/>}{section==='orders' && <Orders/>}
    </main>
  </div>
}
