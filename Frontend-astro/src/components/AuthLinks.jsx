import { useAuth } from '../stores/auth'

export default function AuthLinks() {
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)

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