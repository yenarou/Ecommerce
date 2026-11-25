import { useState } from 'react'
import { login } from '../api/auth'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import GoogleButton from './GoogleButton.jsx'
import { useTranslations, rutaLocal } from '../i18n/ui.js'
import '../styles/Auth.css'

export default function LoginForm({ locale = 'es' }) {
  const t = useTranslations(locale)
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
      await useCartStore.getState().loadFromServer()
      window.location.href = user.rol === 'Admin' ? '/admin' : rutaLocal(locale, '/')
    } catch (err) {
      setError(err.message || t('au.errLogin'))
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">{t('au.login.titulo')}</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="email">{t('au.correo')}</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">{t('au.password')}</label>
          <input id="password" name="password" type="password" required autoComplete="current-password" value={form.password} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? t('au.entrando') : t('au.entrar')}
        </button>
      </form>

      <p className="auth-page__divider">{t('au.o')}</p>
      <GoogleButton locale={locale} />

      <p className="auth-page__switch">
        {t('au.sinCuenta')} <a href={rutaLocal(locale, '/registro')}>{t('au.registrate')}</a>
      </p>
    </div>
  )
}