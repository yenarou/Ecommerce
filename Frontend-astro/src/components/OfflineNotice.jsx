import { useEffect, useState } from 'react'
import '../styles/OfflineNotice.css'

export default function OfflineNotice() {
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
            <strong>Estás sin conexión.</strong> Puedes consultar las piezas que ya visitaste y
            seguir usando el carrito guardado en este dispositivo.
          </p>
          <p className="offline-notice__detail">
            El precio y las existencias se confirmarán al volver la conexión. No se procesan
            pedidos ni pagos sin internet.
          </p>
        </div>
        <a href="/carrito">Abrir carrito</a>
      </div>
    </aside>
  )
}
