import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { fetchCategoryBySlug } from '../api/catalog'
import ProductCard from '../components/ProductCard'
import ProductCardSkeleton from '../components/ProductCardSkeleton'
import Sidebar from '../components/Sidebar'
import './CategoryDetail.css'

export default function CategoryDetail() {
  const { slug } = useParams()
  const [category, setCategory] = useState(null)
  const [isLoading, setIsLoading] = useState(true)
  const products = category?.products ?? []

  useEffect(() => {
  setIsLoading(true)
  fetchCategoryBySlug(slug)
    .then(setCategory)
    .catch((err) => console.error('Error cargando categoría:', err))
    .finally(() => setIsLoading(false))
}, [slug])

  if (!category) {
    return (
      <div className="container category-detail">
        <p>No encontramos esa colección.</p>
        <Link to="/" className="btn btn-primary">
          Volver al inicio
        </Link>
      </div>
    )
  }

  return (
    <div className="container category-detail">
      <nav className="category-detail__crumbs" aria-label="Ruta de navegación">
        <Link to="/">Inicio</Link> <span aria-hidden="true">/</span> {category.name}
      </nav>

      <div className="category-detail__layout">
        <Sidebar />
        <main className="category-detail__main">
          <h1 className="category-detail__title">{category.name}</h1>
          <p className="category-detail__description">{category.description}</p>

          <hr className="stitch-divider" />

          {isLoading ? (
            <div className="category-detail__grid">
              {Array.from({ length: 4 }).map((_, i) => (
                <ProductCardSkeleton key={i} />
              ))}
            </div>
          ) : products.length === 0 ? (
            <p className="category-detail__empty">
              Todavía no hay piezas publicadas en esta colección.
            </p>
          ) : (
            <div className="category-detail__grid">
              {products.map((product) => (
                <ProductCard key={product.id} product={product} />
              ))}
            </div>
          )}
        </main>
      </div>
    </div>
  )
}