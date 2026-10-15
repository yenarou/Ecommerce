import React, { useEffect, useState } from 'react'
import {
  createCategory,
  deleteCategory,
  getCategories,
  updateCategory,
} from '../api/admin'

export default function Categories() {
  const [items, setItems] = useState([])
  const [edit, setEdit] = useState(null)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const blank = { slug: '', name: '', description: '' }
  const [form, setForm] = useState(blank)

  async function load() {
    try {
      setItems(await getCategories())
    } catch (e) {
      setError(e.message)
    }
  }

  useEffect(() => {
    load()
  }, [])

  function open(c) {
    setEdit(c || null)
    setForm(
      c ? { slug: c.slug, name: c.name, description: c.description || '' } : blank
    )
  }

  async function save(e) {
    e.preventDefault()
    setBusy(true)
    try {
      if (edit) {
        await updateCategory(edit.id, {
          name: form.name,
          description: form.description,
        })
      } else {
        await createCategory(form)
      }
      open(null)
      load()
    } catch (e) {
      setError(e.message)
    } finally {
      setBusy(false)
    }
  }

  async function remove(c) {
    if (!confirm(`¿Eliminar ${c.name}?`)) return
    try {
      await deleteCategory(c.id)
      load()
    } catch (e) {
      setError(e.message)
    }
  }

  return (
    <div className="grid grid-2">
      <div className="card">
        <h2>{edit ? 'Editar categoría' : 'Nueva categoría'}</h2>
        {error && <div className="alert">{error}</div>}

        <form className="form" onSubmit={save}>
          <div className="field">
            <label>Slug</label>
            <input
              value={form.slug}
              disabled={!!edit}
              onChange={(e) => setForm({ ...form, slug: e.target.value })}
              required
            />
          </div>

          <div className="field">
            <label>Nombre</label>
            <input
              value={form.name}
              onChange={(e) => setForm({ ...form, name: e.target.value })}
              required
            />
          </div>

          <div className="field">
            <label>Descripción</label>
            <textarea
              value={form.description}
              onChange={(e) =>
                setForm({ ...form, description: e.target.value })
              }
            />
          </div>

          <div className="actions">
            <button className="btn" disabled={busy}>
              {busy ? 'Guardando...' : 'Guardar'}
            </button>
            {edit && (
              <button
                type="button"
                className="btn secondary"
                onClick={() => open(null)}
              >
                Cancelar
              </button>
            )}
          </div>
        </form>
      </div>

      <div className="card table-wrap">
        <h2>Categorías</h2>
        <table className="table">
          <thead>
            <tr>
              <th>Nombre</th>
              <th>Slug</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {items.map((c) => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.slug}</td>
                <td>
                  <div className="actions">
                    <button
                      className="btn secondary"
                      onClick={() => open(c)}
                    >
                      Editar
                    </button>
                    <button
                      className="btn danger"
                      onClick={() => remove(c)}
                    >
                      Eliminar
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}