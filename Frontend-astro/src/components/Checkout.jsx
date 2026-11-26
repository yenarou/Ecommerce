import { useEffect, useState } from 'react'
import { useCart } from '../stores/cart'
import { updateCart } from '../api/cart'
import { createOrder, payOrder } from '../api/orders'
import { initMercadoPago, CardPayment } from '@mercadopago/sdk-react'
import { getSavedAddresses, saveAddress, removeAddress } from '../lib/savedAddresses'
import '../styles/Checkout.css'
import { useAuth } from '../stores/auth'
import { getMe } from '../api/auth'
import { localePrefix, t } from '../i18n'

if (import.meta.env.PUBLIC_MP_PUBLIC_KEY) {
  const paymentLocale = typeof document !== 'undefined' && document.documentElement.lang === 'en' ? 'en-US' : 'es-MX'
  initMercadoPago(import.meta.env.PUBLIC_MP_PUBLIC_KEY, { locale: paymentLocale })
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

export default function Checkout({ locale = 'es' }) {
  const root = localePrefix(locale)
  const offlineCheckoutMessage = t(locale, 'checkout.offline')
  const { items, totals, clearCart } = useCart()
  const [form, setForm] = useState({ ...initialForm, country: t(locale, 'checkout.defaultCountry') })
  const [placed, setPlaced] = useState(false)
  const [orderInfo, setOrderInfo] = useState(null)
  const [isOnline, setIsOnline] = useState(true)
  const [checkoutMessage, setCheckoutMessage] = useState('')
  const user = useAuth((s) => s.user)
  const logout = useAuth((s) => s.logout)

  //direccion en cache
  const userId = user?.id || user?.email || null
  const [savedAddresses, setSavedAddresses] = useState([])
  const [addressChoice, setAddressChoice] = useState('new')
  const [saveForLater, setSaveForLater] = useState(true)

  useEffect(() => {
    const saved = getSavedAddresses(userId)
    setSavedAddresses(saved)
    setAddressChoice(saved.length ? saved[0].id : 'new')
  }, [userId])

  useEffect(() => {
    const updateConnection = () => {
      const online = navigator.onLine
      setIsOnline(online)
      setCheckoutMessage(online ? '' : offlineCheckoutMessage)
    }
    updateConnection()
    window.addEventListener('online', updateConnection)
    window.addEventListener('offline', updateConnection)

    return () => {
      window.removeEventListener('online', updateConnection)
      window.removeEventListener('offline', updateConnection)
    }
  }, [])

  function handleRemoveAddress(id){
    removeAddress(userId, id)
    const saved = getSavedAddresses(userId)
    setSavedAddresses(saved)
    if(addressChoice === id) setAddressChoice(saved.length ? saved[0].id : 'new')
  }
  //direccion en cache
  
  function handleChange(e) {
    //mejor manejo de phone con clean, revisar cambios del jsx (se quito el oninput)
    const { name, value } = e.target
    const clean = name === 'phone' ? value.replace(/\D/g, ''): value
    setForm((prev) => ({ ...prev, [name]: clean }))
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
      window.location.href = `${root}/pago/exito`
      return
    }
    if (result.status === 'failed') {
      const motivo = result.statusDetail ? `?motivo=${encodeURIComponent(result.statusDetail)}` : ''
      window.location.href = `${root}/pago/error${motivo}`
      return
    }
    const q = new URLSearchParams()
    if (result.reference) q.set('ref', result.reference)
    if (result.ticketUrl) q.set('ticket', result.ticketUrl)
    window.location.href = `${root}/pago/pendiente?${q.toString()}`
  }

  async function handleCardSubmit(formData, additionalData) {
    if (!navigator.onLine) {
      setCheckoutMessage(offlineCheckoutMessage)
      return
    }

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
      if (!navigator.onLine) {
        setCheckoutMessage(offlineCheckoutMessage)
        return
      }
      alert(err.message || t(locale, 'checkout.paymentError'))
      throw err
    }
  }

  async function handleSubmit(e) {
    e.preventDefault()

    if (!navigator.onLine) {
      setCheckoutMessage(offlineCheckoutMessage)
      return
    }
    setCheckoutMessage('')

    if (!user) {
      alert(t(locale, 'checkout.signInRequired'))
      window.location.href = `${root}/login`
      return
    }

    const sinStock = items.find((it) => it.quantity > it.product.stock)
    if (sinStock) {
      alert(t(locale, 'checkout.insufficientStock', { name: sinStock.product.name }))
      return
    }

    try {
      //direccion en cache
      const selectedSaved = savedAddresses.find((a) => a.id === addressChoice)
      const address = selectedSaved ? {
        street: selectedSaved.street,
        city: selectedSaved.city,
        state: selectedSaved.state,
        zipCode: selectedSaved.zipCode,
        country: selectedSaved.country,
      } : {
        street: form.address,
        city: form.city,
        state: form.state,
        zipCode: form.zipCode,
        country: form.country,
      }

      await updateCart(items)
      const response = await createOrder(address)
      const orderId = response.createOrder.id

      if(selectedSaved || saveForLater) saveAddress(userId, address)

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
      if (!navigator.onLine) {
        setCheckoutMessage(offlineCheckoutMessage)
        return
      }
      alert(err.message || t(locale, 'checkout.createOrderError'))
    }
  }
  if (!user && !placed) {
    return (
      <div className="container checkout-page">
        <p>{t(locale, 'checkout.signInPrompt')}</p>
        <a href={`${root}/login`} className="btn btn-primary">{t(locale, 'checkout.signIn')}</a>
      </div>
    )
  }

  if (items.length === 0 && !placed) {
    return (
      <div className="container checkout-page">
        <p>{t(locale, 'checkout.emptyCart')}</p>
        <a href={`${root}/`} className="btn btn-primary">
          {t(locale, 'checkout.browse')}
        </a>
      </div>
    )
  }

  if (placed) {
    return (
      <div className="container checkout-page checkout-page--confirm">
        <h1>{t(locale, 'checkout.orderReceived')}</h1>
        <p>
          {t(locale, 'checkout.emailIntro')} <strong>{form.email || t(locale, 'checkout.yourEmail')}</strong> {t(locale, 'checkout.emailOutro')}
        </p>
        <button className="btn btn-primary" onClick={() => window.location.href = `${root}/`}>
          {t(locale, 'checkout.returnHome')}
        </button>
      </div>
    )
  }

  if (orderInfo) {
    return (
      <div className="container checkout-page">
        <h1 className="checkout-page__title">{t(locale, 'checkout.cardTitle')}</h1>
        <p>{t(locale, 'checkout.totalDue')} ${orderInfo.total.toFixed(2)} MXN</p>
        {!isOnline ? (
          <p className="checkout-form__offline-message" role="status">{offlineCheckoutMessage}</p>
        ) : (
          <CardPayment
            initialization={{ amount: orderInfo.total, payer: { email: form.email } }}
            onSubmit={handleCardSubmit}
            onError={(e) => console.error(e)}
          />
        )}
      </div>
    )
  }

  return (
    <div className="container checkout-page">
      <h1 className="checkout-page__title">{t(locale, 'checkout.title')}</h1>

      <div className="checkout-page__layout">
        <form className="checkout-form" onSubmit={handleSubmit}>
          <h2>{t(locale, 'checkout.shippingDetails')}</h2>

          <div className="checkout-form__field">
            <label htmlFor="fullName">{t(locale, 'checkout.fullName')}</label>
            <input type="text" id="fullName" name="fullName" required value={form.fullName} onChange={handleChange} />
          </div>

          <div className="checkout-form__row">
            <div className="checkout-form__field">
              <label htmlFor="email">{t(locale, 'checkout.email')}</label>
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
              <label htmlFor="phone">{t(locale, 'checkout.phone')}</label>
              <input type="tel" maxLength="10" minLength="10" pattern="[0-9]{10}" placeholder={t(locale, 'checkout.phoneExample')} id="phone" name="phone" required value={form.phone} onChange={handleChange} />
            </div>
          </div>




          {savedAddresses.length > 0 && (
            <div className="checkout-form__field">
              <label>{t(locale, 'checkout.shippingAddress')}</label>

              {savedAddresses.map((a) => (
                <div key={a.id} className="saved-address">
                  <label className="liveAid">
                    <input className="mfn" type="radio" name="addressChoice" checked={addressChoice === a.id} onChange={() => setAddressChoice(a.id)}/>{' '}
                    {a.street}, {a.city}, {a.state}, {a.zipCode}, {a.country}
                  </label>
                  <button type="button" className="saved-address__remove" onClick={() => handleRemoveAddress(a.id)}>{t(locale, 'checkout.remove')}</button>
                </div>
              ))}

              <div className="saved-address">
                <label className="liveAid">
                  <input className="mfn" type="radio" name="addressChoice" checked={addressChoice === 'new'} onChange={() => setAddressChoice('new')}/>{' '}
                  {t(locale, 'checkout.enterNewAddress')}
                </label>
              </div>

            </div>
          )}

          {addressChoice === 'new' && (
            <>
              <div className="checkout-form__field">
                <label htmlFor="address">{t(locale, 'checkout.streetAddress')}</label>
                <input type="text" autoComplete="address-line1" placeholder={t(locale, 'checkout.streetExample')} id="address" name="address" required value={form.address} onChange={handleChange} />
              </div>

              <div className="checkout-form__field">
                <label htmlFor="city">{t(locale, 'checkout.city')}</label>
                <input type="text" id="city" autoComplete="address-level2" placeholder={t(locale, 'checkout.cityExample')} name="city" required value={form.city} onChange={handleChange} />
              </div>

              <div className="checkout-form__row">
                <div className="checkout-form__field">
                <label htmlFor="state">{t(locale, 'checkout.state')}</label>
                <input type="text" id="state" name="state" required value={form.state} onChange={handleChange} />
              </div>

              <div className="checkout-form__field">
                <label htmlFor="zipCode">{t(locale, 'checkout.zipCode')}</label>
                <input type="text" id="zipCode" name="zipCode" required value={form.zipCode} onChange={handleChange} />
              </div>
              </div>

              <div className="checkout-form__field">
                <label htmlFor="country">{t(locale, 'checkout.country')}</label>
                <input type="text" id="country" name="country" required value={form.country} onChange={handleChange} />
              </div>

              <label className="checkout-form__check">
                <input type="checkbox" checked={saveForLater} onChange={(e) => setSaveForLater(e.target.checked)}/>{' '}
                {t(locale, 'checkout.saveAddress')}
              </label>
            </>
          )}



          <h2>{t(locale, 'checkout.payment')}</h2>
          <div className="checkout-form__field">
            <label htmlFor="paymentMethod">{t(locale, 'checkout.paymentMethod')}</label>
            <select id="paymentMethod" name="paymentMethod" value={form.paymentMethod} onChange={handleChange}>
              <option value="card">{t(locale, 'checkout.card')}</option>
              <option value="spei">{t(locale, 'checkout.spei')}</option>
              <option value="oxxo">{t(locale, 'checkout.oxxo')}</option>
            </select>
          </div>

          <button type="submit" className="btn btn-primary checkout-form__submit" disabled={!isOnline}>
            {t(locale, 'checkout.placeOrder')} = ${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}
          </button>
          {checkoutMessage && (
            <p className="checkout-form__offline-message" role="status">{checkoutMessage}</p>
          )}
        </form>

        <aside className="checkout-summary stitch-border">
          <h2 className="checkout-summary__title">{t(locale, 'checkout.yourOrder')}</h2>
          <ul className="checkout-summary__list">
            {items.map((item) => {
              const unitPrice = item.product.price
              return (
                <li key={item.id}>
                  <span>
                    {item.product.name} × {item.quantity}
                    {(item.customization_text || item.wrap) && (
                      <span className="checkout-summary__custom">
                        {item.customization_text && ` · ${t(locale, 'checkout.personalization', { text: item.customization_text })}`}
                        {item.wrap && ` · ${t(locale, 'checkout.giftWrapping')}`}
                      </span>
                    )}
                  </span>
                  <span>${(unitPrice * item.quantity).toFixed(2)}</span>
                </li>
              )
            })}
          </ul>
          <div className="checkout-summary__total">
            <span>{t(locale, 'checkout.total')}</span>
            <span>${totals.subtotal.toFixed(2)} {items[0]?.product.currency || 'MXN'}</span>
          </div>
        </aside>
      </div>
    </div>
  )
}
