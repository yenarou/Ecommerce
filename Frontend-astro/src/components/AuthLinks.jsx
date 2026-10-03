import { useEffect } from 'react'
import { useAuth } from '../stores/auth'
import { getMe } from '../api/auth'

export default function AuthLinks() {
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)

  useEffect(() => {
    if (!user?.token) return
    getMe(user.token)
      .then((me) => {
        if (me === null) logout()
      })
      .catch(() => {})
  }, [])

  if (!user) {
    return (
      <div className="topbar__auth">
        <a href="/login" className="topbar__nav-link">Iniciar sesión</a>
        <a href="/registro" className="topbar__nav-link">Registrarse</a>
      </div>
    )
  }

  return (
    <div className="topbar__auth">
      <span className="topbar__user">Hola, {user.username}</span>
      <button type="button" className="topbar__logout" onClick={logout}>Salir</button>
    </div>
  )
}