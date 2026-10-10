import React, { useEffect, useState } from 'react'
import {
  createProduct,
  deleteProduct,
  getAdminProducts,
  setProductPublication,
  updateProduct,
  updateProductPrice,
  updateProductStock,
  getCategories,
  uploadProductImage,
} from '../api/admin'

function ProductForm({ product, categories, onClose, onSaved }) {
  const [form, setForm] = useState({
    name: product?.name || '',
    description: product?.description || '',
    price: product?.price || 0,
    currency: product?.currency || 'MXN',
    stock: product?.stock || 0,
    categoryId: product?.categoryId || categories[0]?.id || '',
    slug: '',
  })
  const [file, setFile] = useState(null)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState('')

  function set(k, v) {
    setForm((f) => ({ ...f, [k]: v }))
  }

  async function submit(e) {
    e.preventDefault()
    setBusy(true)
    setError('')

    try {
      let saved
      if (product) {
        saved = await updateProduct(product.id, {
          name: form.name,
          description: form.description,
          price: Number(form.price),
          currency: form.currency,
          stock: Number(form.stock),
          categoryId: form.categoryId,
        })
      } else {
        saved = await createProduct({
          name: form.name,
          description: form.description,
          price: Number(form.price),
          currency: form.currency,
          stock: Number(form.stock),
          categoryId: form.categoryId,
          images: [],
        })
      }

      if (file) await uploadProductImage(file)
      onSaved(saved)
    } catch (err) {
      setError(err.message)
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className="modal-backdrop">
      <div className="modal">
        <div className="modal-head">
          <h2>{product ? 'Editar producto' : 'Nuevo producto'}</h2>
          <button className="btn secondary" onClick={onClose}>
            ×
          </button>
        </div>

        {error && <div className="alert">{error}</div>}

        <form className="form" onSubmit={submit}>
          <div className="grid grid-2">
            <div className="field">
              <label>Nombre</label>
              <input
                value={form.name}
                onChange={(e) => set('name', e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label>Categoría</label>
              <select
                value={form.categoryId}
                onChange={(e) => set('categoryId', e.target.value)}
                required
              >
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="field">
            <label>Descripción</label>
            <textarea
              value={form.description}
              onChange={(e) => set('description', e.target.value)}
              required
            />
          </div>

          <div className="grid grid-2">
            <div className="field">
              <label>Precio</label>
              <input
                type="number"
                min="0"
                step="0.01"
                value={form.price}
                onChange={(e) => set('price', e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label>Moneda</label>
              <input
                value={form.currency}
                onChange={(e) => set('currency', e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label>Stock</label>
              <input
                type="number"
                min="0"
                value={form.stock}
                onChange={(e) => set('stock', e.target.value)}
                required
              />
            </div>
            <div className="field">
              <label>Imagen</label>
              <input
                type="file"
                accept="image/*"
                onChange={(e) => setFile(e.target.files?.[0] || null)}
              />
            </div>
          </div>

          <div className="actions">
            <button className="btn" disabled={busy}>
              {busy ? 'Guardando...' : 'Guardar'}
            </button>
            <button type="button" className="btn secondary" onClick={onClose}>
              Cancelar
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default function Products() {
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [edit, setEdit] = useState(null)
  const [show, setShow] = useState(false)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')

  async function load() {
    try {
      setProducts(await getAdminProducts())
      setCategories(await getCategories())
    } catch (e) {
      setError(e.message)
    }
  }

  useEffect(() => {
    load()
  }, [])

  const list = products.filter((p) =>
    p.name.toLowerCase().includes(search.toLowerCase())
  )

  async function remove(p) {
    if (!confirm(`¿Eliminar ${p.name}?`)) return
    try {
      await deleteProduct(p.id)
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  async function stock(p) {
    const v = prompt('Nuevo stock', p.stock)
    if (v === null) return
    try {
      await updateProductStock(p.id, Number(v))
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  async function price(p) {
    const v = prompt('Nuevo precio', p.price)
    if (v === null) return
    try {
      await updateProductPrice(p.id, Number(v), p.currency)
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  async function publish(p) {
    try {
      await setProductPublication(p.id, !p.isPublished)
    } catch {}
    load()
  }

  return (
    <div className="grid">
      <div className="card">
        <div className="actions">
          <button
            className="btn"
            onClick={() => {
              setEdit(null)
              setShow(true)
            }}
          >
            + Nuevo producto
          </button>
          <input
            placeholder="Buscar producto..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            style={{
              padding: '10px',
              border: '1px solid #cbd5e1',
              borderRadius: 9,
              flex: 1,
            }}
          />
        </div>
      </div>

      {error && <div className="alert">{error}</div>}

      <div className="card table-wrap">
        <table className="table">
          <thead>
            <tr>
              <th>Producto</th>
              <th>Categoría</th>
              <th>Precio</th>
              <th>Stock</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {list.map((p) => (
              <tr key={p.id}>
                <td>
                  <div style={{ display: 'flex', gap: 10, alignItems: 'center' }}>
                    {p.images?.[0]?.url ? (
                      <img className="thumb" src={p.images[0].url} alt={p.name} />
                    ) : (
                      <div className="thumb" />
                    )}
                    <span>{p.name}</span>
                  </div>
                </td>
                <td>{p.categoryName}</td>
                <td>
                  {p.price} {p.currency}
                </td>
                <td>{p.stock}</td>
                <td>{p.isPublished === false ? 'Oculto' : 'Publicado'}</td>
                <td>
                  <div className="actions">
                    <button
                      className="btn secondary"
                      onClick={() => {
                        setEdit(p)
                        setShow(true)
                      }}
                    >
                      Editar
                    </button>
                    <button className="btn secondary" onClick={() => price(p)}>
                      Precio
                    </button>
                    <button className="btn secondary" onClick={() => stock(p)}>
                      Stock
                    </button>
                    <button className="btn danger" onClick={() => remove(p)}>
                      Eliminar
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {!list.length && <div className="empty">No hay productos.</div>}
      </div>

      {show && (
        <ProductForm
          product={edit}
          categories={categories}
          onClose={() => setShow(false)}
          onSaved={() => {
            setShow(false)
            load()
          }}
        />
      )}
    </div>
  )
}