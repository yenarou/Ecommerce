import { useEffect } from 'react'
import { useCart } from '../stores/cart'
import '../styles/Checkout.css'

const TEXTOS = {
  exito: {
    titulo: '¡Pago recibido!',
    texto: 'Tu pedido quedó pagado. Te escribiremos para confirmar tiempos de tejido y entrega.',
  },
  pendiente: {
    titulo: 'Pago pendiente',
    texto: 'Tu pedido está reservado. Cuando se acredite tu pago (en OXXO o por SPEI) lo marcaremos como pagado.',
  },
  error: {
    titulo: 'No se pudo completar el pago',
    texto: 'No se realizó ningún cargo. Puedes intentarlo de nuevo desde tu carrito.',
  },
}

const MOTIVOS = [
  ['insufficient_amount', 'Tu tarjeta no tiene saldo suficiente.'],
  ['bad_filled_card_data', 'Algún dato de tu tarjeta es incorrecto. Revisa el número, la fecha y el código de seguridad.'],
  ['security_code', 'El código de seguridad es incorrecto.'],
  ['bad_filled_date', 'La fecha de vencimiento es incorrecta.'],
  ['expiration', 'La fecha de vencimiento es incorrecta.'],
  ['card_number', 'El número de tarjeta es incorrecto.'],
  ['call_for_authorize', 'Debes autorizar el pago llamando a tu banco.'],
  ['card_disabled', 'Tu tarjeta está inactiva. Llama a tu banco para activarla.'],
  ['high_risk', 'El pago fue rechazado por seguridad. Prueba con otra tarjeta.'],
]

function textoMotivo(codigo) {
  const c = (codigo || '').toLowerCase()
  const hallado = MOTIVOS.find(([clave]) => c.includes(clave))
  return hallado ? hallado[1] : 'Tu banco rechazó el pago. Intenta con otra tarjeta.'
}

export default function PagoResultado({ estado }) {
  const { clearCart } = useCart()
    const { titulo, texto } = TEXTOS[estado]

  const params = new URLSearchParams(window.location.search)
  const referencia = params.get('ref')
  const ticket = params.get('ticket')
  const ticketSeguro = ticket && ticket.startsWith('https://') ? ticket : null
  const esClabe = !!referencia && /^\d{18}$/.test(referencia)
  const motivo = params.get('motivo')
  const motivoTexto = estado === 'error' ? textoMotivo(motivo) : null
  console.log('PagoResultado', { estado, referencia, ticket, ticketSeguro, esClabe, motivo, motivoTexto })

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
              <strong>{esClabe ? 'CLABE para tu transferencia:' : 'Referencia de pago:'}</strong>{' '}
              {referencia}
            </p>
          )}
          {ticketSeguro && (
            <p>
              <a href={ticketSeguro} target="_blank" rel="noopener noreferrer">
                Ver instrucciones de pago
              </a>
            </p>
          )}
        </div>
      )}

      <a href={estado === 'error' ? '/carrito' : '/'} className="btn btn-primary">
        {estado === 'error' ? 'Volver al carrito' : 'Volver al inicio'}
      </a>
    </div>
  )
}