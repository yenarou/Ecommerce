import { useEffect, useState } from 'react'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'

export default function CartBootstrap() {
  const token = useAuth((state) => state.user?.token)
  const loadFromServer = useCartStore((state) => state.loadFromServer)
  const [error, setError] = useState('')

  useEffect(() => {
    if (!token) {
      setError('')
      return
    }

    let active = true
    const loadCart = () => {
      if (!navigator.onLine) {
        setError('')
        return
      }

      loadFromServer().then(
        () => {
          if (active) setError('')
        },
        (loadError) => {
          if (active) {
            setError(
              navigator.onLine
                ? loadError.message || 'No se pudo cargar el carrito de tu cuenta.'
                : '',
            )
          }
        },
      )
    }
    const handleOffline = () => setError('')

    window.addEventListener('online', loadCart)
    window.addEventListener('offline', handleOffline)
    loadCart()

    return () => {
      active = false
      window.removeEventListener('online', loadCart)
      window.removeEventListener('offline', handleOffline)
    }
  }, [token, loadFromServer])

  if (!error) return null

  return (
    <p role="alert" className="cart-load-error">
      No se pudo cargar el carrito guardado: {error}
    </p>
  )
}
