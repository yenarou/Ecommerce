import { Link } from 'react-router-dom'
//import { categories } from '../data/mockData'
import { useCart } from '../context/CartContext'
import './TopBar.css'

export default function TopBar() {
  const { totals } = useCart()

  return (
    <header className="topbar">
      <div className="container topbar__inner">
        <Link to="/" className="topbar__brand">
          <span className="topbar__brand-mark" aria-hidden="true">
            ✳
          </span>
          <span>
            Hilo <em>y</em> Alma
          </span>
        </Link>

        {/* <nav className="topbar__nav" aria-label="Categorías">
          {categories.map((cat) => (
            <Link key={cat.slug} to={`/categoria/${cat.slug}`} className="topbar__nav-link">
              {cat.name}
            </Link>
          ))}
        </nav> */}

        <Link to="/carrito" className="topbar__cart">
          <img className="topbar__cart-image" src="/images/carrito.png" alt="" />
          <span>Carrito</span>
          <span className="topbar__cart-count">{totals.itemCount}</span>
        </Link>
      </div>
    </header>
  )
}
