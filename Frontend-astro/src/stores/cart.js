import { useMemo } from 'react'
import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { updateCart } from '../api/cart'
import { useAuth } from './auth'

const CART_ID = 'cart-local-demo'

function makeId() {
  return `local-${Date.now()}-${Math.random().toString(16).slice(2)}`
}

export const useCartStore = create(
  persist(
    (set) => ({
      items: [],

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
    { name: 'cart' }
  )
)

export function useCart() {
  const items = useCartStore((s) => s.items)
  const addItem = useCartStore((s) => s.addItem)
  const updateQuantity = useCartStore((s) => s.updateQuantity)
  const removeItem = useCartStore((s) => s.removeItem)
  const clearCart = useCartStore((s) => s.clearCart)
  const sync = useCartStore((s) => s.sync)

  const totals = useMemo(() => {
    const subtotal = items.reduce((sum, it) => {
      return sum + it.product.price * it.quantity
    }, 0)
    const itemCount = items.reduce((sum, it) => sum + it.quantity, 0)
    return { subtotal, itemCount }
  }, [items])

  return { cartId: CART_ID, items, addItem, updateQuantity, removeItem, clearCart, sync, totals }
}