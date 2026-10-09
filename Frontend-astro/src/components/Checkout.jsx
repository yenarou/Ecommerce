import { useState } from 'react'
import { useCart } from '../stores/cart'
import { updateCart } from '../api/cart'
import { createOrder, payOrder } from '../api/orders'
import { initMercadoPago, CardPayment } from '@mercadopago/sdk-react'
import '../styles/Checkout.css'
import { useAuth } from '../stores/auth'
import { getMe } from '../api/auth'

if (import.meta.env.PUBLIC_MP_PUBLIC_KEY) {
  initMercadoPago(import.meta.env.PUBLIC_MP_PUBLIC_KEY, { locale: 'es-MX' })
}

const initialForm = {
  fullName: '',
  email: '',
  phone: '',
  address: '',
  city: '',
  state: '',
  zipCode: '',
  country: 'México',
  paymentMethod: 'card',
}

export default function Checkout() {
  const { items, totals, clearCart } = useCart()
  const [form, setForm] = useState(initialForm)
  const [placed, setPlaced] = useState(false)
  const [orderInfo, setOrderInfo] = useState(null)
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)

  function handleChange(e) {
    const { name, value } = e.target
    setForm((prev) => ({ ...prev, [name]: value }))
  }

  function buildPayer() {
    const parts = form.fullName.trim().split(/\s+/)
    return {
      email: form.email,
      firstName: parts[0],
      lastName: parts.slice(1).join(' ') || parts[0],
    }
  }

  function goToResult(result) {
    if (result.paid) {
      window.location.href = '/pago/exito'
      return
    }
    if (result.status === 'failed') {
      window.location.href = '/pago/error'
      return
    }
    const q = new URLSearchParams()
    if (result.reference) q.set('ref', result.reference)
    if (result.ticketUrl) q.set('ticket', result.ticketUrl)
    window.location.href = `/pago/pendiente?${q.toString()}`
  }

  async function handleCardSubmit(formData, additionalData) {
    try {
      const res = await payOrder({
        orderId: orderInfo.id,
        paymentOption: 'card',
        payer: {
          ...buildPayer(),
          identificationType: formData.payer?.identification?.type,
          identificationNumber: formData.payer?.identification?.number,
        },
        card: {
          token: formData.token,
          paymentMethodId: formData.payment_method_id,
          paymentMethodType: additionalData?.paymentTypeId ?? 'credit_card',
          installments: formData.installments,
        },
      })
      goToResult(res.payOrder)
    } catch (err) {
      alert(err.message || 'No se pudo procesar el pago')
      throw err
    }
  }

  async function handleSubmit(e) {
    e.preventDefault()

    if (!user) {
      alert('Debes iniciar sesión para realizar un pedido')
      window.location.href = '/login'
      return
    }

    const sinStock = items.find((it) => it.quantity > it.product.stock)
    if (sinStock) {
      alert(`No hay suficiente stock de "${sinStock.product.name}"`)
      return
    }

    try {
      const address = {
        street: form.address,
        city: form.city,
        state: form.state,
        zipCode: form.zipCode,
        country: form.country
      }

      await updateCart(items)
      const response = await createOrder(address)
      const orderId = response.createOrder.id

      if (form.paymentMethod === 'card') {
        setOrderInfo({ id: orderId, total: Number(response.createOrder.total) })
        return
      }

      const res = await payOrder({
        orderId,
        paymentOption: form.paymentMethod,
        payer: buildPayer(),
      })
      goToResult(res.payOrder)

    } catch (err) {
      alert(err.message || 'Error al crear el pedido')
    }
  }
  if (!user && !placed) {
    return (
      <div className="container checkout-page">
        <p>Para confirmar tu pedido necesitas iniciar sesión.</p>
        <a href="/login" className="btn btn-primary">Iniciar sesión</a>
      </div>
    )
  }

  if (items.length === 0 && !placed) {
    return (
      <div className="container checkout-page">
        <p>Tu carrito está vacío.</p>
        <a href="/" className="btn btn-primary">
          Ver colecciones
        </a>
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
        <button className="btn btn-primary" onClick={() => window.location.href = '/'}>
          Volver al inicio
        </button>
      </div>
    )
  }

  if (orderInfo) {
    return (
      <div className="container checkout-page">
        <h1 className="checkout-page__title">Pago con tarjeta</h1>
        <p>Total a pagar: ${orderInfo.total.toFixed(2)} MXN</p>
        <CardPayment
          initialization={{ amount: orderInfo.total, payer: { email: form.email } }}
          onSubmit={handleCardSubmit}
          onError={(e) => console.error(e)}
        />
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
            <input type="text" id="fullName" name="fullName" required value={form.fullName} onChange={handleChange} />
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
              <input type="tel" maxLength="10" minLength="10" pattern="[0-9]{10}" placeholder="Ej: 3312345678" oninput="this.value = this.value.replace(/[^0-9]/g, '')" id="phone" name="phone" required value={form.phone} onChange={handleChange} />
            </div>
          </div>

          <div className="checkout-form__field">
            <label htmlFor="address">Dirección</label>
            <input type="text" autocomplete="address-line1" placeholder="Av. Juárez 123" id="address" name="address" required value={form.address} onChange={handleChange} />
          </div>

          <div className="checkout-form__field">
            <label htmlFor="city">Ciudad</label>
            <input type="text" id="city" autocomplete="address-level2" placeholder="Ej: Guadalajara" name="city" required value={form.city} onChange={handleChange} />
          </div>

          <div className="checkout-form__row">
            <div className="checkout-form__field">
              <label htmlFor="state">Estado</label>
              <input type="text" id="state" name="state" required value={form.state} onChange={handleChange} />
            </div>
            <div className="checkout-form__field">
              <label htmlFor="zipCode">Código Postal</label>
              <input type="text" id="zipCode" name="zipCode" required value={form.zipCode} onChange={handleChange} />
            </div>
          </div>

          <div className="checkout-form__field">
            <label htmlFor="country">País</label>
            <input type="text" id="country" name="country" required value={form.country} onChange={handleChange} />
          </div>

          <h2>Pago</h2>
          <div className="checkout-form__field">
            <label htmlFor="paymentMethod">Método de pago</label>
            <select id="paymentMethod" name="paymentMethod" value={form.paymentMethod} onChange={handleChange}>
              <option value="card">Tarjeta de crédito o débito</option>
              <option value="spei">Transferencia SPEI</option>
              <option value="oxxo">Efectivo en OXXO</option>
            </select>
          </div>

          <button type="submit" className="btn btn-primary checkout-form__submit">
            Confirmar pedido = ${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}
          </button>
        </form>

        <aside className="checkout-summary stitch-border">
          <h2 className="checkout-summary__title">Tu pedido</h2>
          <ul className="checkout-summary__list">
            {items.map((item) => {
              const unitPrice = item.product.price
              return (
                <li key={item.id}>
                  <span>
                    {item.product.name} × {item.quantity}
                    {(item.customization_text || item.wrap) && (
                      <span className="checkout-summary__custom">
                        {item.customization_text && ` · Personalización: ${item.customization_text}`}
                        {item.wrap && ' · Envolver para regalo'}
                      </span>
                    )}
                  </span>
                  <span>${(unitPrice * item.quantity).toFixed(2)}</span>
                </li>
              )
            })}
          </ul>
          <div className="checkout-summary__total">
            <span>Total</span>
            <span>${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}</span>
          </div>
        </aside>
      </div>
    </div>
  )
}
