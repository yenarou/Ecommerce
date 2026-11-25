import { useEffect, useState } from 'react'
import { useTranslations } from '../i18n/ui.js'

export default function NotifyButton({ locale = 'es' }) {
  const t = useTranslations(locale)
  const [permiso, setPermiso] = useState(null)

  useEffect(() => {
    if ('Notification' in window) setPermiso(Notification.permission)
  }, [])

  if (permiso !== 'default') return null

  async function activar() {
    const resultado = await Notification.requestPermission()
    setPermiso(resultado)
    if (resultado === 'granted') {
      const reg = await navigator.serviceWorker.ready
      reg.showNotification('Hilo y Alma', {
        body: t('pwa.avisosOk'),
        icon: '/icons/icon-192.png',
      })
    }
  }

  return (
    <button type="button" className="notify-btn" onClick={activar}>
      {t('pwa.avisos')}
    </button>
  )
}