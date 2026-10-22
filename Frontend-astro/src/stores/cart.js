import { useMemo } from 'react'
import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { getActiveCart, updateCart } from '../api/cart'
import { useAuth } from './auth'

const CART_ID = 'cart-local-demo'
let activeCartRequest = null

function makeId() {
  return `local-${Date.now()}-${Math.random().toString(16).slice(2)}`
}

export const useCartStore = create(
  persist(
    (set, get) => ({
      items: [],
      loadError: null,

      loadFromServer: async () => {
        const token = useAuth.getState().user?.token
        if (!token) return

        if (activeCartRequest?.token === token) {
          return activeCartRequest.promise
        }

        const promise = getActiveCart(token)
          .then(({ activeCart }) => {
            if (useAuth.getState().user?.token !== token) return

            const items = (activeCart?.items ?? []).map((item) => ({
              id: item.id,
              cart_id: activeCart.id,
              product_id: item.product.id,
              customization_id: null,
              customization_text: item.customization?.description ?? '',
              wrap: Boolean(item.customization?.isWrap),
              quantity: item.quantity,
              product: {
                ...item.product,
                category: {
                  id: item.product.categoryId,
                  name: item.product.categoryName,
                },
              },
            }))

            // mezcla, navegador con servidor
            const clave = (it) =>
              `${it.product_id}|${(it.customization_text ?? '').trim()}|${Boolean(it.wrap)}`
            const locales = get().items
            const mezclados = items.map((it) => ({ ...it }))
            let hayCambios = false

            for (const local of locales) {
              const existente = mezclados.find((it) => clave(it) === clave(local))
              if (!existente) {
                mezclados.push(local)
                hayCambios = true
              } else if (local.quantity > existente.quantity) {
                existente.quantity = local.quantity
                hayCambios = true
              }
            }

            set({ items: mezclados, loadError: null })
            if (hayCambios) {
              updateCart(mezclados).catch((e) =>
                console.error('No se pudo guardar el carrito:', e),
              )
            }
          })
          .catch((error) => {
            if (useAuth.getState().user?.token === token) {
              set({ loadError: error.message || 'No se pudo cargar el carrito.' })
            }
            throw error
          })
          .finally(() => {
            if (activeCartRequest?.promise === promise) {
              activeCartRequest = null
            }
          })

        activeCartRequest = { token, promise }
        return promise
      },

      addItem: (product, quantity, customizationText, wrap) =>
        set((state) => {
          const text = customizationText.trim()
          const existing = state.items.find(
            (it) => it.product_id === product.id &&
            it.customization_text === text &&
            Boolean(it.wrap) === wrap)
            
            const inCart = state.items
            .filter((it) => it.product_id === product.id)
            .reduce((sum, it) => sum + it.quantity, 0)
            const available = product.stock - inCart
            const toAdd = Math.min(quantity, available)
            if (toAdd <= 0) return state 
          
            if (existing) {
              return { items: state.items.map((it) =>
                it.id === existing.id ? { ...it, quantity: it.quantity + toAdd } : it) }
              }
              return { items: [...state.items, {
                id: makeId(), cart_id: CART_ID, product_id: product.id,
                customization_id: null, customization_text: text, wrap,
                quantity: toAdd, product }] }
              }),

      updateQuantity: (itemId, quantity) =>
        set((state) => {
          const item = state.items.find((it) => it.id === itemId)
          if (!item) return state
          const others = state.items
          .filter((it) => it.product_id === item.product_id && it.id !== itemId)
          .reduce((sum, it) => sum + it.quantity, 0)
          const max = item.product.stock - others
          const safe = Math.min(quantity, max)
          return { items: state.items
            .map((it) => (it.id === itemId ? { ...it, quantity: safe } : it))
            .filter((it) => it.quantity > 0) }
          }),

      updateProductStock: (productId, stock) =>
        set((state) => ({
          items: state.items.map((it) =>
            it.product_id === productId
              ? { ...it, product: { ...it.product, stock } }
              : it),
        })),

      removeItem: (itemId) =>
        set((state) => ({ items: state.items.filter((it) => it.id !== itemId) })),

      clearCart: () => set({ items: [] }),

      sync: async () => {
        const state = useCartStore.getState()
        const user = useAuth.getState().user
        if (user && user.token) {
          try {
            await updateCart(state.items)
            console.log('Carrito sincronizado con el servidor')
          } catch (err) {
            console.error('Error al sincronizar carrito:', err)
          }
        }
      }
    }),
    {
      name: 'cart',
      partialize: (state) => ({ items: state.items }),
    }
  )
)

export function useCart() {
  const items = useCartStore((s) => s.items)
  const addItem = useCartStore((s) => s.addItem)
  const updateQuantity = useCartStore((s) => s.updateQuantity)
  const removeItem = useCartStore((s) => s.removeItem)
  const clearCart = useCartStore((s) => s.clearCart)
  const updateProductStock = useCartStore((s) => s.updateProductStock)
  const sync = useCartStore((s) => s.sync)
  const loadError = useCartStore((s) => s.loadError)

  const totals = useMemo(() => {
    const subtotal = items.reduce((sum, it) => {
      return sum + it.product.price * it.quantity
    }, 0)
    const itemCount = items.reduce((sum, it) => sum + it.quantity, 0)
    return { subtotal, itemCount }
  }, [items])

  return { cartId: CART_ID, items, addItem, updateQuantity, removeItem, clearCart, updateProductStock, sync, loadError, totals }
}