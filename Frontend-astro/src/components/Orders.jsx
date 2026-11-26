import { getRelativeLocaleUrl } from 'astro:i18n'
import { useEffect, useState } from 'react'
import { getOrders } from '../api/orders'
import { useAuth } from '../stores/auth'
import { useTranslations, traducirProducto } from '../i18n/ui.js'
import '../styles/Orders.css'

function getStatusValue(status) {
  return typeof status === 'string' ? status : status?.value
}

export default function Orders({ locale = 'es' }) {
  const t = useTranslations(locale)
  const token = useAuth((state) => state.user?.token)
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  function formatDate(value) {
    return new Date(value).toLocaleDateString(locale === 'en' ? 'en-US' : 'es-MX', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    })
  }

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
        if (active) setError(requestError.message || t('ord.errCargar'))
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
      <h1 className="orders-page__title">{t('ord.titulo')}</h1>

      {!token ? (
        <div className="orders-page__message">
          <p>{t('ord.necesitaLogin')}</p>
          <a className="btn btn-primary" href={getRelativeLocaleUrl(locale, '/login')}>{t('auth.login')}</a>
        </div>
      ) : loading ? (
        <p className="orders-page__message" role="status">{t('ord.cargando')}</p>
      ) : error ? (
        <div className="orders-page__message orders-page__message--error" role="alert">
          <p>{error}</p>
        </div>
      ) : orders.length === 0 ? (
        <div className="orders-page__message">
          <p>{t('ord.vacio')}</p>
          <a className="btn btn-primary" href={getRelativeLocaleUrl(locale, '/')}>{t('ord.explorar')}</a>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map((order) => {
            const status = getStatusValue(order.status)
            const estadoTexto = status && t(`ord.st.${status}`) !== `ord.st.${status}`
              ? t(`ord.st.${status}`)
              : (status || t('ord.desconocido'))

            return (
              <article className="order-card stitch-border" key={order.id}>
                <header className="order-card__header">
                  <div>
                    <h2>{t('ord.orden')} <span>#{order.id.slice(0, 8)}</span></h2>
                    <p>{t('ord.realizada')} {formatDate(order.createdAt)}</p>
                  </div>
                  <span className={`order-card__status order-card__status--${status?.toLowerCase() || 'unknown'}`}>
                    {estadoTexto}
                  </span>
                </header>

                <ul className="order-card__items">
                  {order.items.map((item, index) => {
                    const image = item.product?.images?.[0]
                    const producto = traducirProducto(locale, item.product)

                    return (
                      <li className="order-item" key={item.id}>
                        {image ? (
                          <img
                            className="order-item__image"
                            src={image.url}
                            alt={image.alt || producto.name}
                          />
                        ) : (
                          <div className="order-item__image order-item__image--empty" aria-hidden="true">
                            {t('ord.sinImagen')}
                          </div>
                        )}
                        <div className="order-item__product">
                          <h3>{producto?.name || `${t('ord.articulo')} ${index + 1}`}</h3>
                          {item.product && (
                            <p className="price">
                              ${Number(item.product.price).toFixed(2)} {item.product.currency} {t('cart.cu')}
                            </p>
                          )}
                          {item.customization?.description && (
                            <p>{t('form.personalizacion')}: {item.customization.description}</p>
                          )}
                          {item.customization?.isWrap && <p>{t('form.regalo')}</p>}
                        </div>
                        <div className="order-item__details">
                          <span>{t('form.cantidad')}: {item.quantity}</span>
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
                  <h3>{t('ord.direccion')}</h3>
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