import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useCart } from '../context/CartContext'
import { createOrder } from '../api/catalog'
import './Checkout.css'

const initialForm = {
  fullName: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  paymentMethod: 'card',
}

export default function Checkout() {
  const { items, totals, clearCart } = useCart()
  const navigate = useNavigate()
  const [form, setForm] = useState(initialForm)
  const [placed, setPlaced] = useState(false)

  function handleChange(e) {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  async function handleSubmit(e) {
    e.preventDefault()
    const userId = '11111111-1111-1111-1111-111111111111' // usuario demo del seed, hasta que tengas login
    const orderItems = items.map((it) => ({
    productId: it.product_id,
    customizationId: it.customization_id,
    quantity: it.quantity,
  }))
  await createOrder(userId, orderItems)
  setPlaced(true)
  clearCart()
  }

  if (items.length === 0 && !placed) {
    return (
      <div className="container checkout-page">
        <p>Tu carrito está vacío.</p>
        <Link to="/" className="btn btn-primary">
          Ver colecciones
        </Link>
      </div>
    )
  }

  if (placed) {
    return (
      <div className="container checkout-page checkout-page--confirm">
        <h1>¡Pedido recibido!</h1>
        <p>
          Te escribiremos a <strong>{form.email || 'tu correo'}</strong> para confirmar tiempos de
          tejido y entrega.
        </p>
        <button className="btn btn-primary" onClick={() => navigate('/')}>
          Volver al inicio
        </button>
      </div>
    )
  }

  return (
    <div className="container checkout-page">
      <h1 className="checkout-page__title">Checkout</h1>

      <div className="checkout-page__layout">
        <form className="checkout-form" onSubmit={handleSubmit}>
          <h2>Datos de envío</h2>

          <div className="checkout-form__field">
            <label htmlFor="fullName">Nombre completo</label>
            <input id="fullName" name="fullName" required value={form.fullName} onChange={handleChange} />
          </div>

          <div className="checkout-form__row">
            <div className="checkout-form__field">
              <label htmlFor="email">Correo</label>
              <input
                id="email"
                name="email"
                type="email"
                required
                value={form.email}
                onChange={handleChange}
              />
            </div>
            <div className="checkout-form__field">
              <label htmlFor="phone">Teléfono</label>
              <input id="phone" name="phone" required value={form.phone} onChange={handleChange} />
            </div>
          </div>

          <div className="checkout-form__field">
            <label htmlFor="address">Dirección</label>
            <input id="address" name="address" required value={form.address} onChange={handleChange} />
          </div>

          <div className="checkout-form__field">
            <label htmlFor="city">Ciudad</label>
            <input id="city" name="city" required value={form.city} onChange={handleChange} />
          </div>

          <h2>Pago</h2>
          <div className="checkout-form__field">
            <label htmlFor="paymentMethod">Método de pago</label>
            <select id="paymentMethod" name="paymentMethod" value={form.paymentMethod} onChange={handleChange}>
              <option value="card">Tarjeta</option>
              <option value="transfer">Transferencia</option>
              <option value="cash">Efectivo contra entrega</option>
            </select>
          </div>

          <button type="submit" className="btn btn-primary checkout-form__submit">
            Confirmar pedido — ${totals.subtotal.toFixed(2)} MXN
          </button>
        </form>

        <aside className="checkout-summary stitch-border">
          <h2 className="checkout-summary__title">Tu pedido</h2>
          <ul className="checkout-summary__list">
            {items.map((item) => {
              const unitPrice = item.product.price + (item.customization?.additional_price ?? 0)
              return (
                <li key={item.id}>
                  <span>
                    {item.product.name} × {item.quantity}
                    {item.customization && (
                      <span className="checkout-summary__custom"> · {item.customization.description}</span>
                    )}
                    {item.customization_text && (
                      <span className="checkout-summary__custom"> · "{item.customization_text}"</span>
                    )}
                  </span>
                  <span>${(unitPrice * item.quantity).toFixed(2)}</span>
                </li>
              )
            })}
          </ul>
          <div className="checkout-summary__total">
            <span>Total</span>
            <span>${totals.subtotal.toFixed(2)} MXN</span>
          </div>
        </aside>
      </div>
    </div>
  )
}
