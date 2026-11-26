import { useEffect } from 'react'
import { useCart } from '../stores/cart'
import { localePrefix, t } from '../i18n'
import '../styles/Checkout.css'

const paymentMessageIds = {
  exito: {
    title: 'payment.successTitle',
    text: 'payment.successText',
  },
  pendiente: {
    title: 'payment.pendingTitle',
    text: 'payment.pendingText',
  },
  error: {
    title: 'payment.errorTitle',
    text: 'payment.errorText',
  },
}

const reasonMessageIds = [
  ['insufficient_amount', 'payment.insufficientAmount'],
  ['bad_filled_card_data', 'payment.badCardData'],
  ['security_code', 'payment.securityCode'],
  ['bad_filled_date', 'payment.badDate'],
  ['expiration', 'payment.badDate'],
  ['card_number', 'payment.cardNumber'],
  ['call_for_authorize', 'payment.authorize'],
  ['card_disabled', 'payment.disabledCard'],
  ['high_risk', 'payment.highRisk'],
]

function reasonMessageId(code) {
  const normalizedCode = (code || '').toLowerCase()
  return reasonMessageIds.find(([reason]) => normalizedCode.includes(reason))?.[1] ?? 'payment.bankDeclined'
}

export default function PagoResultado({ estado, locale = 'es' }) {
  const root = localePrefix(locale)
  const { clearCart } = useCart()
  const { title, text } = paymentMessageIds[estado]

  const params = new URLSearchParams(window.location.search)
  const referencia = params.get('ref')
  const ticket = params.get('ticket')
  const ticketSeguro = ticket && ticket.startsWith('https://') ? ticket : null
  const esClabe = !!referencia && /^\d{18}$/.test(referencia)
  const motivo = params.get('motivo')
  const motivoTexto = estado === 'error' ? t(locale, reasonMessageId(motivo)) : null
  console.log('PagoResultado', { estado, referencia, ticket, ticketSeguro, esClabe, motivo, motivoTexto })

  useEffect(() => {
    if (estado !== 'error') clearCart()
  }, [])

  return (
    <div className="container checkout-page checkout-page--confirm">
      <h1>{t(locale, title)}</h1>
      {motivoTexto && <p><strong>{motivoTexto}</strong></p>}
      <p>{t(locale, text)}</p>

      {estado === 'pendiente' && (referencia || ticketSeguro) && (
        <div className="pago-datos">
          {referencia && (
            <p>
              <strong>{t(locale, esClabe ? 'payment.clabe' : 'payment.reference')}</strong>{' '}
              {referencia}
            </p>
          )}
          {ticketSeguro && (
            <p>
              <a href={ticketSeguro} target="_blank" rel="noopener noreferrer">
                {t(locale, 'payment.instructions')}
              </a>
            </p>
          )}
        </div>
      )}

      <a href={estado === 'error' ? `${root}/carrito` : `${root}/`} className="btn btn-primary">
        {t(locale, estado === 'error' ? 'payment.returnCart' : 'payment.returnHome')}
      </a>
    </div>
  )
}
