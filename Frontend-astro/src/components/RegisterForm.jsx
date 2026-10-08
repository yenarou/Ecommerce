import { useState } from 'react'
import { register } from '../api/auth'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import GoogleButton from './GoogleButton.jsx'
import '../styles/Auth.css'

export default function RegisterForm() {
  const setUser = useAuth((s) => s.setUser)
  const [form, setForm] = useState({ username: '', email: '', password: '', confirm: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  function handleChange(e) {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  async function handleSubmit(e) {
    e.preventDefault()
    setError('')

    if (form.password.length < 6) {
      setError('La contraseña debe tener al menos 6 caracteres.')
      return
    }
    if (form.password !== form.confirm) {
      setError('Las contraseñas no coinciden.')
      return
    }

    setLoading(true)
    try {
      const user = await register(form.username, form.email, form.password)
      setUser(user)
      await useCartStore.getState().loadFromServer()
      window.location.href = '/'
    } catch (err) {
      setError(err.message || 'No se pudo crear la cuenta')
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">Crear cuenta</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="username">Nombre de usuario</label>
          <input id="username" name="username" type="text" required autoComplete="username" value={form.username} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="email">Correo</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">Contraseña</label>
          <input id="password" name="password" type="password" required autoComplete="new-password" value={form.password} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="confirm">Confirmar contraseña</label>
          <input id="confirm" name="confirm" type="password" required autoComplete="new-password" value={form.confirm} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? 'Creando cuenta...' : 'Crear cuenta'}
        </button>
      </form>

      <p className="auth-page__divider">o</p>
      <GoogleButton />

      <p className="auth-page__switch">
        ¿Ya tienes cuenta? <a href="/login">Inicia sesión</a>
      </p>
    </div>
  )
}