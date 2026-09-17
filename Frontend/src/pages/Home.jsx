import Sidebar from '../components/Sidebar'
import Hero from '../components/Hero'
import ProductCard from '../components/ProductCard'
import ProductCardSkeleton from '../components/ProductCardSkeleton'
import { fetchProducts } from '../api/catalog'
import { useState, useEffect } from 'react'
import './Home.css'

export default function Home() {
  const [featured, setFeatured] = useState([])
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
  fetchProducts({ page: 1, pageSize: 6 })
    .then((res) => setFeatured(res?.items ?? []))
    .catch((err) => console.error('Error cargando productos:', err))
    .finally(() => setIsLoading(false))
}, [])

  return (
    <div className="container home">
      <Hero />

      <div className="home__layout">
        <Sidebar />

        <main className="home__main">
          <h2 className="home__section-title">Piezas destacadas</h2>
          <div className="home__grid">
            {isLoading ? Array.from({length: featured.length}).map((_, i) => (
              <ProductCardSkeleton key={i}/>)) : featured.map((product) => <ProductCard key={product.id} product={product}/>)}
          </div>
        </main>
      </div>

      <hr className="stitch-divider" />

      <section className="home__context">
        <div>
          <h2>Hecho por encargo</h2>
          <p>
            Cada figura empieza a tejerse cuando confirmas tu
            pedido. Por eso el tiempo de entrega varía según la pieza y las
            personalizaciones que elijas, lo verás reflejado en cada producto.
          </p>
        </div>
        <div>
          <h2>Personaliza tu producto</h2>
          <p>
            Desde la página de cada producto puedes escribir una indicación de
            personalización y elegir un empaque de regalo antes de pasar al carrito.
          </p>
        </div>
      </section>
    </div>
  )
}
