import { useState } from 'react'
import { login } from '../api/auth'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import GoogleButton from './GoogleButton.jsx'
import { localePrefix, t } from '../i18n'
import '../styles/Auth.css'

export default function LoginForm({ locale = 'es' }) {
  const root = localePrefix(locale)
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
      window.location.href = user.rol === 'Admin' ? '/admin' : `${root}/`
    } catch (err) {
      setError(err.message || t(locale, 'auth.signInError'))
      setLoading(false)
    }
  }

  return (
    <div className="container auth-page">
      <h1 className="auth-page__title">{t(locale, 'auth.signIn')}</h1>

      <form className="auth-form" onSubmit={handleSubmit}>
        <div className="auth-form__field">
          <label htmlFor="email">{t(locale, 'auth.email')}</label>
          <input id="email" name="email" type="email" required autoComplete="email" value={form.email} onChange={handleChange} />
        </div>
        <div className="auth-form__field">
          <label htmlFor="password">{t(locale, 'auth.password')}</label>
          <input id="password" name="password" type="password" required autoComplete="current-password" value={form.password} onChange={handleChange} />
        </div>

        {error && <p className="auth-form__error" role="alert">{error}</p>}

        <button type="submit" className="btn btn-primary auth-form__submit" disabled={loading}>
          {loading ? t(locale, 'auth.signingIn') : t(locale, 'auth.enter')}
        </button>
      </form>

      <p className="auth-page__divider">{t(locale, 'auth.or')}</p>
      <GoogleButton locale={locale} />

      <p className="auth-page__switch">
        {t(locale, 'auth.noAccount')} <a href={`${root}/registro`}>{t(locale, 'auth.signUpAction')}</a>
      </p>
    </div>
  )
}