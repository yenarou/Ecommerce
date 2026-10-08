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

    loadFromServer().then(
      () => setError(''),
      (loadError) =>
        setError(loadError.message || 'No se pudo cargar el carrito de tu cuenta.'),
    )
  }, [token, loadFromServer])

  if (!error) return null

  return (
    <p role="alert" className="cart-load-error">
      No se pudo cargar el carrito guardado: {error}
    </p>
  )
}
