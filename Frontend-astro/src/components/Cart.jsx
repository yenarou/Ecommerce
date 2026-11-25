import { useCart } from '../stores/cart'
import { useTranslations, rutaLocal } from '../i18n/ui.js'
import '../styles/Cart.css'

export default function Cart({ locale = 'es' }) {
  const t = useTranslations(locale)
  const { items, updateQuantity, removeItem, totals, sync } = useCart()

  return (
    <div className="container cart-page">
      <h1 className="cart-page__title">{t('cart.titulo')}</h1>

      {items.length === 0 ? (
        <div className="cart-page__empty">
          <p>{t('cart.vacio')}</p>
          <a href={rutaLocal(locale, '/')} className="btn btn-primary">
            {t('cart.verColecciones')}
          </a>
        </div>
      ) : (
        <div className="cart-page__layout">
          <ul className="cart-list">
            {items.map((item) => {
              const unitPrice = item.product.price
              return (
                <li key={item.id} className="cart-item">
                  <img className="cart-item__image" src={item.product.images?.[0]?.url} alt={item.product.name} />
                  <div className="cart-item__info">
                    <p className="cart-item__name">{item.product.name}</p>
                    {(item.customization_text || item.wrap) && (
                      <p className="cart-item__customization">
                        {item.customization_text && `${t('form.personalizacion')}: ${item.customization_text}`}
                        {item.wrap && `${item.customization_text ? ' · ' : ''}${t('form.regalo')}`}
                      </p>
                    )}
                    <p className="price cart-item__price">${unitPrice.toFixed(2)} {item.product.currency} {t('cart.cu')}</p>
                  </div>

                  <div className="quantity-picker cart-item__quantity">
                    <button
                      type="button"
                      onClick={() => {
                        updateQuantity(item.id, item.quantity - 1)
                        sync()
                      }} disabled={item.quantity <= 1}
                      aria-label={t('form.menos')}
                    >
                      −
                    </button>
                    <input
                      type="number"
                      min="1"
                      max={item.product.stock}
                      value={item.quantity}
                      onChange={(e) => {
                        updateQuantity(item.id, Math.max(1, Number(e.target.value)))
                        sync()
                      }}
                    />
                    <button
                      type="button"
                      onClick={() => {
                        updateQuantity(item.id, item.quantity + 1)
                        sync()
                      }}
                      disabled={item.quantity >= item.product.stock}
                      aria-label={t('form.mas')}
                    >
                      +
                    </button>
                  </div>

                  <p className="cart-item__subtotal">${(unitPrice * item.quantity).toFixed(2)}</p>

                  <button type="button" className="cart-item__remove" onClick={() => {
                    removeItem(item.id)
                    sync()
                  }}>
                    {t('cart.quitar')}
                  </button>
                </li>
              )
            })}
          </ul>

          <aside className="cart-summary stitch-border">
            <h2 className="cart-summary__title">{t('cart.resumen')}</h2>
            <div className="cart-summary__row">
              <span>{t('cart.piezas')}</span>
              <span>{totals.itemCount}</span>
            </div>
            <div className="cart-summary__row cart-summary__row--total">
              <span>{t('cart.subtotal')}</span>
              <span>${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}</span>
            </div>
            <p className="cart-summary__note">{t('cart.nota')}</p>
            <button className="btn btn-primary cart-summary__cta" onClick={() => window.location.href = rutaLocal(locale, '/checkout')}>
              {t('cart.continuar')}
            </button>
          </aside>
        </div>
      )}
    </div>
  )
}