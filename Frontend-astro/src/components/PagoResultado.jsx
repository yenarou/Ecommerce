import { useEffect } from 'react'
import { useCart } from '../stores/cart'
import { useTranslations, rutaLocal } from '../i18n/ui.js'
import '../styles/Checkout.css'

const CLAVES_MOTIVO = [
  'insufficient_amount',
  'bad_filled_card_data',
  'security_code',
  'bad_filled_date',
  'expiration',
  'card_number',
  'call_for_authorize',
  'card_disabled',
  'high_risk',
]

export default function PagoResultado({ estado, locale = 'es' }) {
  const t = useTranslations(locale)
  const { clearCart } = useCart()

  const prefijo = estado === 'exito' ? 'pago.exito' : estado === 'pendiente' ? 'pago.pend' : 'pago.err'
  const titulo = t(`${prefijo}.titulo`)
  const texto = t(`${prefijo}.texto`)

  function textoMotivo(codigo) {
    const c = (codigo || '').toLowerCase()
    const clave = CLAVES_MOTIVO.find((k) => c.includes(k))
    return t(clave ? `pago.m.${clave}` : 'pago.m.default')
  }

  const params = new URLSearchParams(window.location.search)
  const referencia = params.get('ref')
  const ticket = params.get('ticket')
  const ticketSeguro = ticket && ticket.startsWith('https://') ? ticket : null
  const esClabe = !!referencia && /^\d{18}$/.test(referencia)
  const motivo = params.get('motivo')
  const motivoTexto = estado === 'error' ? textoMotivo(motivo) : null

  useEffect(() => {
    if (estado !== 'error') clearCart()
  }, [])

  return (
    <div className="container checkout-page checkout-page--confirm">
      <h1>{titulo}</h1>
      {motivoTexto && <p><strong>{motivoTexto}</strong></p>}
      <p>{texto}</p>

      {estado === 'pendiente' && (referencia || ticketSeguro) && (
        <div className="pago-datos">
          {referencia && (
            <p>
              <strong>{esClabe ? t('pago.clabe') : t('pago.ref')}</strong>{' '}
              {referencia}
            </p>
          )}
          {ticketSeguro && (
            <p>
              <a href={ticketSeguro} target="_blank" rel="noopener noreferrer">
                {t('pago.instrucciones')}
              </a>
            </p>
          )}
        </div>
      )}

      <a href={estado === 'error' ? rutaLocal(locale, '/carrito') : rutaLocal(locale, '/')} className="btn btn-primary">
        {estado === 'error' ? t('pago.volverCarrito') : t('chk.volver')}
      </a>
    </div>
  )
}