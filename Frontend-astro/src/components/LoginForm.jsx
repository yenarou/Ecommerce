import { useState } from 'react'
import { login } from '../api/auth'
import { useAuth } from '../stores/auth'
import GoogleButton from './GoogleButton.jsx'
import '../styles/Auth.css'

export default function LoginForm() {
  const setUser = useAuth((s) => s.setUser)
  const [form, setForm] = useState({ email: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  function handleChange(e) {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const user = await login(form.email, form.password)
      setUser(user)
      window.location.href = '/'
    } catch (err) {
      setError(err.message || 'No se pudo iniciar sesión')
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">Iniciar sesión</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="email">Correo</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">Contraseña</label>
          <input id="password" name="password" type="password" required autoComplete="current-password" value={form.password} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? 'Entrando...' : 'Entrar'}
        </button>
      </form>

      <p className="auth-page__divider">o</p>
      <GoogleButton />

      <p className="auth-page__switch">
        ¿No tienes cuenta? <a href="/registro">Regístrate</a>
      </p>
    </div>
  )
}