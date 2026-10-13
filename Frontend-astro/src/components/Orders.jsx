import { useEffect, useState } from 'react'
import { getOrders } from '../api/orders'
import { useAuth } from '../stores/auth'
import '../styles/Orders.css'

const statusLabels = {
  Pending: 'Pendiente',
  Confirmed: 'Confirmada',
  Shipped: 'Enviada',
  Delivered: 'Entregada',
  Cancelled: 'Cancelada',
}

function getStatusValue(status) {
  return typeof status === 'string' ? status : status?.value
}

function formatDate(value) {
  return new Date(value).toLocaleDateString('es-MX', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

export default function Orders() {
  const token = useAuth((state) => state.user?.token)
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    let active = true

    if (!token) {
      setOrders([])
      setLoading(false)
      setError('')
      return () => {
        active = false
      }
    }

    setLoading(true)
    setError('')
    getOrders(token)
      .then((result) => {
        if (active) setOrders(result)
      })
      .catch((requestError) => {
        if (active) setError(requestError.message || 'No se pudieron cargar tus órdenes.')
      })
      .finally(() => {
        if (active) setLoading(false)
      })

    return () => {
      active = false
    }
  }, [token])

  return (
    <main className="container orders-page">
      <h1 className="orders-page__title">Mis órdenes</h1>

      {!token ? (
        <div className="orders-page__message">
          <p>Inicia sesión para consultar el historial de tus órdenes.</p>
          <a className="btn btn-primary" href="/login">Iniciar sesión</a>
        </div>
      ) : loading ? (
        <p className="orders-page__message" role="status">Cargando tus órdenes...</p>
      ) : error ? (
        <div className="orders-page__message orders-page__message--error" role="alert">
          <p>{error}</p>
        </div>
      ) : orders.length === 0 ? (
        <div className="orders-page__message">
          <p>Aún no tienes órdenes.</p>
          <a className="btn btn-primary" href="/">Explorar productos</a>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map((order) => {
            const status = getStatusValue(order.status)

            return (
              <article className="order-card stitch-border" key={order.id}>
                <header className="order-card__header">
                  <div>
                    <h2>Orden <span>#{order.id.slice(0, 8)}</span></h2>
                    <p>Realizada el {formatDate(order.createdAt)}</p>
                  </div>
                  <span className={`order-card__status order-card__status--${status?.toLowerCase() || 'unknown'}`}>
                    {statusLabels[status] || status || 'Estado desconocido'}
                  </span>
                </header>

                <ul className="order-card__items">
                  {order.items.map((item, index) => {
                    const image = item.product?.images?.[0]

                    return (
                      <li className="order-item" key={item.id}>
                        {image ? (
                          <img
                            className="order-item__image"
                            src={image.url}
                            alt={image.alt || item.product.name}
                          />
                        ) : (
                          <div className="order-item__image order-item__image--empty" aria-hidden="true">
                            Sin imagen
                          </div>
                        )}
                        <div className="order-item__product">
                          <h3>{item.product?.name || `Artículo ${index + 1}`}</h3>
                          {item.product && (
                            <p className="price">
                              ${Number(item.product.price).toFixed(2)} {item.product.currency} c/u
                            </p>
                          )}
                          {item.customization?.description && (
                            <p>Personalización: {item.customization.description}</p>
                          )}
                          {item.customization?.isWrap && <p>Envolver para regalo</p>}
                        </div>
                        <div className="order-item__details">
                          <span>Cantidad: {item.quantity}</span>
                          {item.product && (
                            <span className="price">
                              ${(Number(item.product.price) * item.quantity).toFixed(2)} {item.product.currency}
                            </span>
                          )}
                        </div>
                      </li>
                    )
                  })}
                </ul>

                <footer className="order-card__footer">
                  <h3>Dirección de entrega</h3>
                  <address>
                    {order.address.street}, {order.address.city}, {order.address.state},{' '}
                    {order.address.zipCode}, {order.address.country}
                  </address>
                </footer>
              </article>
            )
          })}
        </div>
      )}
    </main>
  )
}
