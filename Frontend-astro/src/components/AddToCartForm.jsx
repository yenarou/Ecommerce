import { useState } from 'react'
import { useCart } from '../stores/cart'

export default function AddToCartForm({ product }) {
    const { addItem, items, sync } = useCart()

  const [quantity, setQuantity] = useState(1)
    const [personalization, setPersonalization] = useState('')
    const [wrap, setWrap] = useState(false)
    const [justAdded, setJustAdded] = useState(false)

    const inCart = items
    .filter((it) => it.product_id === product.id)
    .reduce((sum, it) => sum + it.quantity, 0)
    const remaining = product.stock - inCart

  function handleQuantityChange(next) {
    const clamped = Math.max(1, Math.min(remaining, next))
    setQuantity(clamped)
  }

  async function handleAddToCart() {
    addItem(product, quantity, personalization, wrap)
    setJustAdded(true)
    await sync()
  }

  return (
    <>
      <div className="product-detail__field">
        <label htmlFor="personalization">Personalización</label>
        <textarea
          id="personalization"
          rows="3"
          maxLength="500"
          required
          value={personalization}
          onChange={(e) => setPersonalization(e.target.value)}
          placeholder="Describe cómo quieres personalizar este producto"
        />
        <span className="product-detail__field-hint">Máximo 500 caracteres.</span>
      </div>

      <label className="product-detail__checkbox" htmlFor="wrap">
        <input
          id="wrap"
          type="checkbox"
          checked={wrap}
          onChange={(e) => setWrap(e.target.checked)}
        />
        <span>Envolver para regalo</span>
      </label>

      <div className="product-detail__field">
        <label htmlFor="quantity">Cantidad</label>
        <div className="quantity-picker">
          <button type="button" onClick={() => handleQuantityChange(quantity - 1)} disabled={quantity <= 1} aria-label="Disminuir cantidad">−</button>
          <input id="quantity" type="number" min="1" max={remaining} value={quantity} onChange={(e) => handleQuantityChange(Number(e.target.value))} />
          <button type="button" onClick={() => handleQuantityChange(quantity + 1)} disabled={quantity >= remaining} aria-label="Aumentar cantidad">+</button>
        </div>
      </div>

      <div className="product-detail__actions">
        <button
          className="btn btn-primary"
          onClick={handleAddToCart}
          disabled={remaining <= 0 || !personalization.trim()}
        >
          Agregar al carrito = ${(product.price * quantity).toFixed(2)}
        </button>
      </div>

      {justAdded && (
        <>
          <p className="product-detail__confirm">Se agregó al carrito.</p>
          <a href="/carrito" className="btn">Ir al carrito</a>
        </>
      )}
    </>
  )
}