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

export default function PagoResultado({ estado }) {
  const { clearCart } = useCart()
  const { titulo, texto } = TEXTOS[estado]

  useEffect(() => {
    if (estado !== 'error') clearCart()
  }, [])

  return (
    <div className="container checkout-page checkout-page--confirm">
      <h1>{titulo}</h1>
      <p>{texto}</p>
      <a href={estado === 'error' ? '/carrito' : '/'} className="btn btn-primary">
        {estado === 'error' ? 'Volver al carrito' : 'Volver al inicio'}
      </a>
    </div>
  )
}