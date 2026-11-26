import { useEffect } from 'react'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'
import { getMe } from '../api/auth'
import { localePrefix, t } from '../i18n'

export default function AuthLinks({ locale = 'es' }) {
  const root = localePrefix(locale)
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
        <a href={`${root}/login`} className="topbar__nav-link">{t(locale, 'auth.signIn')}</a>
        <a href={`${root}/ordenes`} className="topbar__nav-link">{t(locale, 'auth.myOrders')}</a>
        <a href={`${root}/registro`} className="topbar__nav-link">{t(locale, 'auth.signUp')}</a>
      </div>
    )
  }

  return (
    <div className="topbar__auth">
      <span className="topbar__user">{t(locale, 'auth.hello', { name: user.username })}</span>
      <a href={`${root}/ordenes`} className="topbar__nav-link">{t(locale, 'auth.myOrders')}</a>
      {user?.rol === 'Admin' && (
        <a href="/admin" className="topbar__nav-link">{t(locale, 'auth.admin')}</a>
        )}
      <button type="button" className="topbar__logout" onClick={handleLogout}>{t(locale, 'auth.signOut')}</button>
    </div>
  )
}