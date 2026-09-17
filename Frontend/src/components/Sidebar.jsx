import { Link } from 'react-router-dom'
import { categories } from '../data/mockData'
import './Sidebar.css'

export default function Sidebar() {
  return (
    <aside className="sidebar" aria-label="Filtros y colecciones">
      <div className="sidebar__block">
        <h2 className="sidebar__title">Colecciones</h2>
        <ul className="sidebar__list">
          {categories.map((cat) => (
            <li key={cat.slug}>
              <Link to={`/categoria/${cat.slug}`} className="sidebar__link">
                {cat.name}
              </Link>
            </li>
          ))}
        </ul>
      </div>

    </aside>
  )
}
