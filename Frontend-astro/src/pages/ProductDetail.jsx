import { useState, useEffect } from 'react'
import { fetchProductById } from '../api/catalog'
import { useCart } from '../stores/cart'
import '../styles/ProductDetail.css'

export default function ProductDetail({id}) {
  const { addItem } = useCart()

  const [product, setProduct] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const [quantity, setQuantity] = useState(1)
  const [customizationId, setCustomizationId] = useState('none')
  const [justAdded, setJustAdded] = useState(false)

  useEffect(() => {
    let active = true
    setIsLoading(true)
    setProduct(null)
    setJustAdded(false)
    setQuantity(1)
    setCustomizationId('none')

    fetchProductById(id)
      .then((data) => { if (active) setProduct(data) })
      .finally(() => { if (active) setIsLoading(false) })

    return () => { active = false }
  }, [id])

  if (isLoading) {
    return <div className="container product-detail"><p>Cargando...</p></div>
  }

  if (!product) {
    return (
      <div className="container product-detail">
        <p>No encontramos esa pieza.</p>
        <a href="/" className="btn btn-primary">Volver al inicio</a>
      </div>
    )
  }

  const category = product.category
  const customization =
    customizationId === 'none' ? null : product.customizationOptions.find((c) => c.id === customizationId)
  const unitPrice = product.price + (customization?.additional_price ?? 0)

  function handleQuantityChange(next) {
    const clamped = Math.max(1, Math.min(product.stock, next))
    setQuantity(clamped)
  }

  function handleAddToCart() {
    addItem(product, customization, quantity)
    setJustAdded(true)
  }

  return (
    <div className="container product-detail">
      <nav className="product-detail__crumbs" aria-label="Ruta de navegación">
        <a href="/">Inicio</a> <span aria-hidden="true">/</span>{' '}
        {category && <a href={`/categoria/${category.slug}`}>{category.name}</a>}{' '}
        <span aria-hidden="true">/</span> {product.name}
      </nav>

      <div className="product-detail__layout">
        <div className="product-detail__gallery">
          <div className="product-detail__main-image">
            <img src={product.image_url} alt={product.name} />
          </div>
          <div className="product-detail__thumb">
            <img src={product.image_url_alt} alt="" />
          </div>
        </div>

        <div className="product-detail__info">
          <h1 className="product-detail__name">{product.name}</h1>
          <p className="price product-detail__price">${unitPrice.toFixed(2)} MXN</p>
          <p className="product-detail__description">{product.description}</p>

          <dl className="product-detail__facts">
            <div>
              <dt>Tiempo de tejido</dt>
              <dd>{product.days_to_make} días</dd>
            </div>
            <div>
              <dt>Disponibilidad</dt>
              <dd>{product.stock > 0 ? `${product.stock} piezas` : 'Agotado'}</dd>
            </div>
          </dl>

          <hr className="stitch-divider" />

          <div className="product-detail__field">
            <label htmlFor="customization">Personalización</label>
            <select id="customization" value={customizationId} onChange={(e) => setCustomizationId(e.target.value)}>
              <option value="none">Sin personalización</option>
              {product.customizationOptions.map((opt) => (
                <option key={opt.id} value={opt.id}>
                  {opt.description}
                  {opt.additional_price > 0 ? ` (+$${opt.additional_price.toFixed(2)})` : ''}
                </option>
              ))}
            </select>
          </div>

          <div className="product-detail__field">
            <label htmlFor="quantity">Cantidad</label>
            <div className="quantity-picker">
              <button type="button" onClick={() => handleQuantityChange(quantity - 1)} disabled={quantity <= 1} aria-label="Disminuir cantidad">−</button>
              <input id="quantity" type="number" min="1" max={product.stock} value={quantity} onChange={(e) => handleQuantityChange(Number(e.target.value))} />
              <button type="button" onClick={() => handleQuantityChange(quantity + 1)} disabled={quantity >= product.stock} aria-label="Aumentar cantidad">+</button>
            </div>
          </div>

          <div className="product-detail__actions">
            <button className="btn btn-primary" onClick={handleAddToCart} disabled={product.stock === 0}>
              Agregar al carrito — ${(unitPrice * quantity).toFixed(2)}
            </button>
          </div>

          {justAdded && (
            <>
              <p className="product-detail__confirm">Se agregó al carrito.</p>
              <br></br>
              <a href="/carrito" className="btn"> Ir al carrito </a>
              {/*<button className="btn" onClick={() => navigate('/carrito')}>Ir al carrito</button>*/}
            </>
          )}
        </div>
      </div>
    </div>
  )
}