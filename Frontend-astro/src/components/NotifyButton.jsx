import { useEffect, useState } from 'react'

export default function NotifyButton() {
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
        body: 'Avisos activados. Te avisaremos cuando se sincronice tu carrito.',
        icon: '/icons/icon-192.png',
      })
    }
  }

  return (
    <button type="button" className="notify-btn" onClick={activar}>
    Activar avisos
    </button>
  )
}