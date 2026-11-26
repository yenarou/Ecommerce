import { useState } from 'react'
import { register } from '../api/auth'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import GoogleButton from './GoogleButton.jsx'
import { localePrefix, t } from '../i18n'
import '../styles/Auth.css'

export default function RegisterForm({ locale = 'es' }) {
  const root = localePrefix(locale)
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
      setError(t(locale, 'auth.passwordLengthError'))
      return
    }
    if (form.password !== form.confirm) {
      setError(t(locale, 'auth.passwordMismatch'))
      return
    }

    setLoading(true)
    try {
      const user = await register(form.username, form.email, form.password)
      setUser(user)
      await useCartStore.getState().loadFromServer()
      window.location.href = `${root}/`
    } catch (err) {
      setError(err.message || t(locale, 'auth.signUpError'))
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">{t(locale, 'auth.createAccount')}</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="username">{t(locale, 'auth.username')}</label>
          <input id="username" name="username" type="text" required autoComplete="username" value={form.username} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="email">{t(locale, 'auth.email')}</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">{t(locale, 'auth.password')}</label>
          <input id="password" name="password" type="password" required autoComplete="new-password" value={form.password} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="confirm">{t(locale, 'auth.confirmPassword')}</label>
          <input id="confirm" name="confirm" type="password" required autoComplete="new-password" value={form.confirm} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? t(locale, 'auth.creatingAccount') : t(locale, 'auth.createAccount')}
        </button>
      </form>

      <p className="auth-page__divider">{t(locale, 'auth.or')}</p>
      <GoogleButton locale={locale} />

      <p className="auth-page__switch">
        {t(locale, 'auth.hasAccount')} <a href={`${root}/login`}>{t(locale, 'auth.signInAction')}</a>
      </p>
    </div>
  )
}