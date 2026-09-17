import { createContext, useContext, useMemo, useState, useCallback } from 'react'

const CartContext = createContext(null)

function makeId() {
  return `local-${Date.now()}-${Math.random().toString(16).slice(2)}`
}

export function CartProvider({ children }) {
  //cartid lusgo viene del backend
  const [cartId] = useState('cart-local-demo')
  const [items, setItems] = useState([])

  const addItem = useCallback((product, customization, quantity, customizationText = '') => {
    const normalizedCustomizationText = customization ? customizationText.trim() : ''
    setItems((prev) => {
      const existing = prev.find(
        (it) => (
          it.product_id === product.id &&
          it.customization_id === (customization?.id ?? null) &&
          it.customization_text === normalizedCustomizationText
        )
      )
      if (existing) {
        return prev.map((it) =>
          it.id === existing.id ? { ...it, quantity: it.quantity + quantity } : it
        )
      }
      return [
        ...prev,
        {
          id: makeId(),
          cart_id: cartId,
          product_id: product.id,
          customization_id: customization?.id ?? null,
          customization_text: normalizedCustomizationText,
          quantity,
          product,
          customization: customization ?? null,
        },
      ]
    })
  }, [cartId])

  const updateQuantity = useCallback((itemId, quantity) => {
    setItems((prev) =>
      prev
        .map((it) => (it.id === itemId ? { ...it, quantity } : it))
        .filter((it) => it.quantity > 0)
    )
  }, [])

  const removeItem = useCallback((itemId) => {
    setItems((prev) => prev.filter((it) => it.id !== itemId))
  }, [])

  const clearCart = useCallback(() => setItems([]), [])

  const totals = useMemo(() => {
    const subtotal = items.reduce((sum, it) => {
      const unit = it.product.price + (it.customization?.additional_price ?? 0)
      return sum + unit * it.quantity
    }, 0)
    const itemCount = items.reduce((sum, it) => sum + it.quantity, 0)
    return { subtotal, itemCount }
  }, [items])

  const value = { cartId, items, addItem, updateQuantity, removeItem, clearCart, totals }

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>
}

export function useCart() {
  const ctx = useContext(CartContext)
  if (!ctx) throw new Error('useCart debe usarse dentro de <CartProvider>')
  return ctx
}
