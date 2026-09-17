import { useState, useEffect } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { fetchProductById } from '../api/catalog'
import { useCart } from '../context/CartContext'
import './ProductDetail.css'

export default function ProductDetail() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { addItem } = useCart()

  const [product, setProduct] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const [quantity, setQuantity] = useState(1)
  const [customizationText, setCustomizationText] = useState('')
  const [giftWrap, setGiftWrap] = useState(false)
  const [justAdded, setJustAdded] = useState(false)

  useEffect(() => {
    let active = true
    setIsLoading(true)
    setProduct(null)
    setJustAdded(false)
    setQuantity(1)
    setCustomizationText('')
    setGiftWrap(false)

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
        <Link to="/" className="btn btn-primary">Volver al inicio</Link>
      </div>
    )
  }

  const category = product.category
  const giftCustomization = product.customizationOptions.find((option) => option.description === 'Empaque de regalo')
  const selectedCustomization = giftWrap ? giftCustomization : null
  const unitPrice = product.price + (selectedCustomization?.additional_price ?? 0)

  function handleQuantityChange(next) {
    const clamped = Math.max(1, Math.min(product.stock, next))
    setQuantity(clamped)
  }

  function handleAddToCart() {
    if (justAdded || product.stock === 0) return
    addItem(product, selectedCustomization, quantity, customizationText)
    setJustAdded(true)
  }

  return (
    <div className="container product-detail">
      <nav className="product-detail__crumbs" aria-label="Ruta de navegación">
        <Link to="/">Inicio</Link> <span aria-hidden="true">/</span>{' '}
        {category && <Link to={`/categoria/${category.slug}`}>{category.name}</Link>}{' '}
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
              <dd>{product.stock > 0 ? `${product.stock} piezas` : 'No disponible'}</dd>
            </div>
          </dl>

          <hr className="stitch-divider" />

          <div className="product-detail__field">
            <label htmlFor="customization-text">Personaliza tu producto</label>
            <textarea
              id="customization-text"
              value={customizationText}
              onChange={(e) => setCustomizationText(e.target.value)}
              maxLength={120}
              rows={3}
              placeholder="Escribe aquí el texto o indicación para tu personalización"
              aria-describedby="customization-text-hint"
            />
            <span id="customization-text-hint" className="product-detail__field-hint">
              Opcional. Máximo 120 caracteres.
            </span>
            <span className="product-detail__customization-note">
              Las piezas personalizadas requieren tiempo adicional de elaboración y pueden tardar más en entregarse.
            </span>
          </div>

          {giftCustomization && (
            <label className="product-detail__checkbox">
              <input
                type="checkbox"
                checked={giftWrap}
                onChange={(e) => setGiftWrap(e.target.checked)}
              />
              <span>
                Empaque de regalo
                <small>+${giftCustomization.additional_price.toFixed(2)} MXN</small>
              </span>
            </label>
          )}

          <div className="product-detail__field">
            <label htmlFor="quantity">Cantidad</label>
            <div className="quantity-picker">
              <button type="button" onClick={() => handleQuantityChange(quantity - 1)} disabled={quantity <= 1} aria-label="Disminuir cantidad">−</button>
              <input id="quantity" type="number" min="1" max={product.stock} value={quantity} onChange={(e) => handleQuantityChange(Number(e.target.value))} />
              <button type="button" onClick={() => handleQuantityChange(quantity + 1)} disabled={quantity >= product.stock} aria-label="Aumentar cantidad">+</button>
            </div>
          </div>

          <div className="product-detail__actions">
            <button
              className={`btn btn-primary${justAdded ? ' product-detail__add-button--added' : ''}`}
              onClick={handleAddToCart}
              disabled={product.stock === 0 || justAdded}
              aria-live="polite"
            >
              {justAdded ? 'Agregado al carrito' : `Agregar al carrito — $${(unitPrice * quantity).toFixed(2)}`}
            </button>
          </div>

          {justAdded && (
            <>
              <p className="product-detail__confirm" role="status">Se agregó al carrito.</p>
              <button
                className="product-detail__cart-link"
                onClick={() => navigate('/carrito')}
              >
                <img src="/images/carrito.png" alt="" />
                <span>Ver mi carrito</span>
                <span className="product-detail__cart-link-arrow" aria-hidden="true">→</span>
              </button>
            </>
          )}
        </div>
      </div>
    </div>
  )
}