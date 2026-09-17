import './ProductCardSkeleton.css'

export default function ProductCardSkeleton() {
  return (
    <div className="product-card-skeleton" aria-hidden="true">
      <div className="product-card-skeleton__image" />
      <div className="product-card-skeleton__body">
        <div className="product-card-skeleton__line product-card-skeleton__line--title" />
        <div className="product-card-skeleton__line product-card-skeleton__line--meta" />
        <div className="product-card-skeleton__line product-card-skeleton__line--price" />
      </div>
    </div>
  )
}