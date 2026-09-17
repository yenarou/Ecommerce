import { Link } from 'react-router-dom'
import './ProductCard.css'

export default function ProductCard({ product }) {
  return (
    <Link to={`/producto/${product.id}`} className="product-card">
      <div className="product-card__image">
        <img src={product.image_url} alt={product.name} />
        {product.stock <= 3 && product.stock > 0 && <span className="product-card__tag">Últimas piezas</span>}
        {product.stock == 0 && <span className="product-card__tag">No disponible</span>}
      </div>
      <div className="product-card__body">
        <h3 className="product-card__name">{product.name}</h3>
        <p className="product-card__meta">{product.days_to_make} días de tejido</p>
        <p className="price">${product.price.toFixed(2)} MXN</p>
      </div>
    </Link>
  )
}
