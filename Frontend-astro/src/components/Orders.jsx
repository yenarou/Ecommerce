import { useEffect, useState } from 'react'
import { getOrders } from '../api/orders'
import { useAuth } from '../stores/auth'
import { localePrefix, localizeProduct, t } from '../i18n'
import '../styles/Orders.css'

const knownStatuses = new Set(['Pending', 'Confirmed', 'Shipped', 'Delivered', 'Cancelled'])

function getStatusValue(status) {
  return typeof status === 'string' ? status : status?.value
}

function formatDate(value, locale) {
  return new Date(value).toLocaleDateString(locale, {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
  })
}

export default function Orders({ locale = 'es' }) {
  const root = localePrefix(locale)
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
        if (active) setError(requestError.message || t(locale, 'orders.loadError'))
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
      <h1 className="orders-page__title">{t(locale, 'orders.title')}</h1>

      {!token ? (
        <div className="orders-page__message">
          <p>{t(locale, 'orders.signInPrompt')}</p>
          <a className="btn btn-primary" href={`${root}/login`}>{t(locale, 'orders.signIn')}</a>
        </div>
      ) : loading ? (
        <p className="orders-page__message" role="status">{t(locale, 'orders.loading')}</p>
      ) : error ? (
        <div className="orders-page__message orders-page__message--error" role="alert">
          <p>{error}</p>
        </div>
      ) : orders.length === 0 ? (
        <div className="orders-page__message">
          <p>{t(locale, 'orders.empty')}</p>
          <a className="btn btn-primary" href={`${root}/`}>{t(locale, 'orders.browse')}</a>
        </div>
      ) : (
        <div className="orders-list">
          {orders.map((order) => {
            const status = getStatusValue(order.status)

            return (
              <article className="order-card stitch-border" key={order.id}>
                <header className="order-card__header">
                  <div>
                    <h2>{t(locale, 'orders.order')} <span>#{order.id.slice(0, 8)}</span></h2>
                    <p>{t(locale, 'orders.placedOn')} {formatDate(order.createdAt, locale)}</p>
                  </div>
                  <span className={`order-card__status order-card__status--${status?.toLowerCase() || 'unknown'}`}>
                    {knownStatuses.has(status) ? t(locale, `orders.status.${status}`) : status || t(locale, 'orders.unknownStatus')}
                  </span>
                </header>

                <ul className="order-card__items">
                  {order.items.map((item, index) => {
                    const image = item.product?.images?.[0]
                    const displayProduct = localizeProduct(item.product, locale)

                    return (
                      <li className="order-item" key={item.id}>
                        {image ? (
                          <img
                            className="order-item__image"
                            src={image.url}
                            alt={displayProduct.name}
                          />
                        ) : (
                          <div className="order-item__image order-item__image--empty" aria-hidden="true">
                            {t(locale, 'orders.noImage')}
                          </div>
                        )}
                        <div className="order-item__product">
                          <h3>{displayProduct?.name || t(locale, 'orders.article', { number: index + 1 })}</h3>
                          {item.product && (
                            <p className="price">
                              ${Number(item.product.price).toFixed(2)} {item.product.currency} {t(locale, 'orders.each')}
                            </p>
                          )}
                          {item.customization?.description && (
                            <p>{t(locale, 'orders.personalization', { text: item.customization.description })}</p>
                          )}
                          {item.customization?.isWrap && <p>{t(locale, 'orders.giftWrap')}</p>}
                        </div>
                        <div className="order-item__details">
                          <span>{t(locale, 'orders.quantity')}: {item.quantity}</span>
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
                  <h3>{t(locale, 'orders.deliveryAddress')}</h3>
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
