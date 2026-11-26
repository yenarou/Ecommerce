import { getRelativeLocaleUrl } from 'astro:i18n'
import { useEffect } from 'react'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import { getMe } from '../api/auth'
import { useTranslations } from '../i18n/ui.js'

export default function AuthLinks({ locale = 'es' }) {
  const t = useTranslations(locale)
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)
  const clearCart = useCartStore((s) => s.clearCart)

  function handleLogout() {
    logout()
    clearCart()
  }

  useEffect(() => {
    if (!user?.token) return
    getMe(user.token)
      .then((me) => {
        if (me === null) handleLogout()
      })
      .catch(() => {})
  }, [])

  if (!user) {
    return (
      <div className="topbar__auth">
        <a href={getRelativeLocaleUrl(locale, '/login')} className="topbar__nav-link">{t('auth.login')}</a>
        <a href={getRelativeLocaleUrl(locale, '/ordenes')} className="topbar__nav-link">{t('auth.ordenes')}</a>
        <a href={getRelativeLocaleUrl(locale, '/registro')} className="topbar__nav-link">{t('auth.registro')}</a>
      </div>
    )
  }

  return (
    <div className="topbar__auth">
      <span className="topbar__user">{t('auth.hola')} {user.username}</span>
      <a href={getRelativeLocaleUrl(locale, '/ordenes')} className="topbar__nav-link">{t('auth.ordenes')}</a>
      {user?.rol === 'Admin' && (
        <a href="/admin" className="topbar__nav-link">{t('auth.admin')}</a>
      )}
      <button type="button" className="topbar__logout" onClick={handleLogout}>{t('auth.salir')}</button>
    </div>
  )
}