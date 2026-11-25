import { useState } from 'react'
import { register } from '../api/auth'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import GoogleButton from './GoogleButton.jsx'
import { useTranslations, rutaLocal } from '../i18n/ui.js'
import '../styles/Auth.css'

export default function RegisterForm({ locale = 'es' }) {
  const t = useTranslations(locale)
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
      setError(t('au.errPassCorta'))
      return
    }
    if (form.password !== form.confirm) {
      setError(t('au.errPassNo'))
      return
    }

    setLoading(true)
    try {
      const user = await register(form.username, form.email, form.password)
      setUser(user)
      await useCartStore.getState().loadFromServer()
      window.location.href = rutaLocal(locale, '/')
    } catch (err) {
      setError(err.message || t('au.errReg'))
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">{t('au.reg.titulo')}</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="username">{t('au.usuario')}</label>
          <input id="username" name="username" type="text" required autoComplete="username" value={form.username} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="email">{t('au.correo')}</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">{t('au.password')}</label>
          <input id="password" name="password" type="password" required autoComplete="new-password" value={form.password} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="confirm">{t('au.confirmar')}</label>
          <input id="confirm" name="confirm" type="password" required autoComplete="new-password" value={form.confirm} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? t('au.creando') : t('au.reg.titulo')}
        </button>
      </form>

      <p className="auth-page__divider">{t('au.o')}</p>
      <GoogleButton locale={locale} />

      <p className="auth-page__switch">
        {t('au.yaCuenta')} <a href={rutaLocal(locale, '/login')}>{t('au.iniciaSesion')}</a>
      </p>
    </div>
  )
}