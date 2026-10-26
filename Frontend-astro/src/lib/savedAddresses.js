const PREFIX = 'checkout_addresses'
const MAX_SAVED = 5
const FIELDS = ['street', 'city', 'state', 'zipCode', 'country']

const keyFor = (userId) => `${PREFIX}:${userId || 'guest'}`
const norm = (v) => (v ?? '').toString().trim().toLowerCase()
const newId = () => Date.now().toString(36) + Math.random().toString(36).slice(2, 8)

function sameAddress(a, b) {
  return FIELDS.every((f) => norm(a[f]) === norm(b[f]))
}

export function getSavedAddresses(userId) {
  try {
    const raw = localStorage.getItem(keyFor(userId))
    const list = raw ? JSON.parse(raw) : []
    return Array.isArray(list) ? list : []
  } catch {
    return []
  }
}

//guardar direccion
export function saveAddress(userId, address) {
  try {
    const clean = {}
    FIELDS.forEach((f) => {
      clean[f] = (address[f] ?? '').toString().trim()
    })

    const rest = getSavedAddresses(userId).filter((a) => !sameAddress(a, clean))
    const list = [{ id: newId(), ...clean }, ...rest].slice(0, MAX_SAVED)
    localStorage.setItem(keyFor(userId), JSON.stringify(list))
  } catch {
    //se lleno
  }
}

export function removeAddress(userId, id) {
  try {
    const list = getSavedAddresses(userId).filter((a) => a.id !== id)
    localStorage.setItem(keyFor(userId), JSON.stringify(list))
  } catch {
    //se ignora
  }
}