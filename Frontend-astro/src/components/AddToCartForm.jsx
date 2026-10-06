import { useState } from 'react'
import { useCart } from '../stores/cart'

export default function AddToCartForm({ product }) {
    const { addItem, items } = useCart()

  const [quantity, setQuantity] = useState(1)
  const [customizationId, setCustomizationId] = useState('none')
  const [justAdded, setJustAdded] = useState(false)

    const inCart = items
    .filter((it) => it.product_id === product.id)
    .reduce((sum, it) => sum + it.quantity, 0)
    const remaining = product.stock - inCart

  const options = product.customizationOptions ?? []
  const customization =
    customizationId === 'none' ? null : options.find((c) => c.id === customizationId)
  const unitPrice = product.price + (customization?.additional_price ?? 0)

  function handleQuantityChange(next) {
    const clamped = Math.max(1, Math.min(remaining, next))
    setQuantity(clamped)
  }

  function handleAddToCart() {
    addItem(product, customization, quantity)
    setJustAdded(true)
  }

  return (
    <>
      <div className="product-detail__field">
        <label htmlFor="customization">Personalización</label>
        <select id="customization" value={customizationId} onChange={(e) => setCustomizationId(e.target.value)}>
          <option value="none">Sin personalización</option>
          {options.map((opt) => (
            <option key={opt.id} value={opt.id}>
              {opt.description}
              {opt.additional_price > 0 ? ` (+$${opt.additional_price.toFixed(2)})` : ''}
            </option>
          ))}
        </select>
      </div>

      <div className="product-detail__field">
        <label htmlFor="quantity">Cantidad</label>
        <div className="quantity-picker">
          <button type="button" onClick={() => handleQuantityChange(quantity - 1)} disabled={quantity <= 1} aria-label="Disminuir cantidad">−</button>
          <input id="quantity" type="number" min="1" max={remaining} value={quantity} onChange={(e) => handleQuantityChange(Number(e.target.value))} />
          <button type="button" onClick={() => handleQuantityChange(quantity + 1)} disabled={quantity >= remaining} aria-label="Aumentar cantidad">+</button>
        </div>
      </div>

      <div className="product-detail__actions">
        <button className="btn btn-primary" onClick={handleAddToCart} disabled={product.stock === 0}>
          Agregar al carrito = ${(unitPrice * quantity).toFixed(2)}
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