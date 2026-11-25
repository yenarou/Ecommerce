const DB_NAME = 'hiloyalma-outbox'
const STORE = 'cola'

const MUTATION = `
  mutation UpdateCart($request: CartRequestInput!) {
    updateCart(request: $request)
  }
`

function abrirDB() {
  return new Promise((ok, fail) => {
    const req = indexedDB.open(DB_NAME, 1)
    req.onupgradeneeded = () => req.result.createObjectStore(STORE, { keyPath: 'id' })
    req.onsuccess = () => ok(req.result)
    req.onerror = () => fail(req.error)
  })
}

export async function encolarCarrito({ url, token, items }) {
  const body = JSON.stringify({
    query: MUTATION,
    variables: {
      request: {
        items: items.map((item) => ({
          productId: item.product_id,
          quantity: item.quantity,
          customization: {
            personalizationDescription: item.customization_text?.trim() || null,
            wrap: Boolean(item.wrap),
          },
        })),
      },
    },
  })

  const db = await abrirDB()
  await new Promise((ok, fail) => {
    const tx = db.transaction(STORE, 'readwrite')
    tx.objectStore(STORE).put({ id: 'carrito', url, token, body, fecha: Date.now() })
    tx.oncomplete = ok
    tx.onerror = () => fail(tx.error)
  })

  const reg = await navigator.serviceWorker?.ready
  if (reg && 'sync' in reg) {
    await reg.sync.register('cola-pendientes')
    return true
  }
  return false // navegador sin Background Sync
}
export async function pedirVaciado() {
  const reg = await navigator.serviceWorker?.ready
  reg?.active?.postMessage('vaciar-cola')
}

if (typeof window !== 'undefined') {
  window.addEventListener('online', () => pedirVaciado())
  pedirVaciado()
}