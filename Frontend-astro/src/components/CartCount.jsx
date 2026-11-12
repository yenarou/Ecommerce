import { useCart } from '../stores/cart'

export default function CartCount() {
  const { totals } = useCart()
  return <span className="topbar__cart-count" aria-live="polite" aria-atomic="true">{totals.itemCount}</span>
}