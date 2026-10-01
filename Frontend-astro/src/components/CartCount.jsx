import { useCart } from '../stores/cart'

export default function CartCount() {
  const { totals } = useCart()
  return <span className="topbar__cart-count">{totals.itemCount}</span>
}