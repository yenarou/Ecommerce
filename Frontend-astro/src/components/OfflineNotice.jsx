import { useEffect, useState } from 'react'
import { localePrefix, t } from '../i18n'
import '../styles/OfflineNotice.css'

export default function OfflineNotice({ locale = 'es' }) {
  const root = localePrefix(locale)
  const [isOffline, setIsOffline] = useState(false)

  useEffect(() => {
    const updateConnection = () => setIsOffline(!navigator.onLine)
    updateConnection()
    window.addEventListener('online', updateConnection)
    window.addEventListener('offline', updateConnection)

    return () => {
      window.removeEventListener('online', updateConnection)
      window.removeEventListener('offline', updateConnection)
    }
  }, [])

  if (!isOffline) return null

  return (
    <aside className="offline-notice" role="status" aria-live="polite">
      <div className="offline-notice__content">
        <div>
          <p>
            <strong>{t(locale, 'offline.title')}</strong> {t(locale, 'offline.description')}
          </p>
          <p className="offline-notice__detail">
            {t(locale, 'offline.details')}
          </p>
        </div>
        <a href={`${root}/carrito`}>{t(locale, 'offline.openCart')}</a>
      </div>
    </aside>
  )
}
