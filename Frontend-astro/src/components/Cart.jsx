import { useCart } from '../stores/cart'
import '../styles/Cart.css'

export default function Cart() {
  const { items, updateQuantity, removeItem, totals, sync } = useCart()

  return (
    <div className="container cart-page">
      <h1 className="cart-page__title">Tu carrito</h1>

      {items.length === 0 ? (
        <div className="cart-page__empty">
          <p>Todavía no agregas ninguna pieza.</p>
          <a href="/" className="btn btn-primary">
            Ver colecciones
          </a>
        </div>
      ) : (
        <div className="cart-page__layout">
          <ul className="cart-list">
            {items.map((item) => {
              const unitPrice = item.product.price
              return (
                <li key={item.id} className="cart-item">
                  <img className="cart-item__image" src={item.product.images[0]?.url} alt={item.product.name} />
                  <div className="cart-item__info">
                    <p className="cart-item__name">{item.product.name}</p>
                    <p className="cart-item__customization">
                      Personalización: {item.customization_text}
                      {item.wrap && ' · Envolver para regalo'}
                    </p>
                    <p className="price cart-item__price">${unitPrice.toFixed(2)} {item.product.currency} c/u</p>
                  </div>

                  <div className="quantity-picker cart-item__quantity">
                    <button
                      type="button"
                      onClick={() => {
                        updateQuantity(item.id, item.quantity - 1)
                        sync()
                      }} disabled={item.quantity <= 1}
                      aria-label="Disminuir cantidad"
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
                      aria-label="Aumentar cantidad"
                    >
                      +
                    </button>
                  </div>

                  <p className="cart-item__subtotal">${(unitPrice * item.quantity).toFixed(2)}</p>

                  <button type="button" className="cart-item__remove" onClick={() => {
                    removeItem(item.id)
                    sync()
                  }}>
                    Quitar
                  </button>
                </li>
              )
            })}
          </ul>

          <aside className="cart-summary stitch-border">
            <h2 className="cart-summary__title">Resumen</h2>
            <div className="cart-summary__row">
              <span>Piezas</span>
              <span>{totals.itemCount}</span>
            </div>
            <div className="cart-summary__row cart-summary__row--total">
              <span>Subtotal</span>
              <span>${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}</span>
            </div>
            <p className="cart-summary__note">Envío y tiempos de entrega se confirman en el checkout.</p>
            <button className="btn btn-primary cart-summary__cta" onClick={() => window.location.href = '/checkout'}>
              Continuar al checkout
            </button>
          </aside>
        </div>
      )}
    </div>
  )
}
