import { clientsClaim } from '@vite-pwa/workbox-swkit/core'
import { cleanupOutdatedCaches, matchPrecache, precacheAndRoute } from '@vite-pwa/workbox-swkit/precaching'
import { registerRoute, setCatchHandler } from '@vite-pwa/workbox-swkit/routing'
import { ExpirationPlugin } from '@vite-pwa/workbox-swkit/expiration'
import { CacheFirst, NetworkFirst, NetworkOnly } from '@vite-pwa/workbox-swkit/strategies'

precacheAndRoute(self.__WB_MANIFEST)
cleanupOutdatedCaches()

const storePagePath = /^\/(?:(?:en|zh-CN)\/)?(?:$|carrito\/?$|categoria\/[^/]+\/?$|products\/[^/]+\/?$)/
const isSameOrigin = ({ url }) => url.origin === self.location.origin
const isNavigation = ({ request }) => request.mode === 'navigate'
const appAssetPath = /\.(?:css|js|mjs|woff2?)$/i

async function cacheOfflineEntryPoints() {
  const cache = await caches.open('catalog-pages')

  for (const pathname of ['/', '/carrito', '/en/', '/en/carrito', '/zh-CN/', '/zh-CN/carrito']) {
    const request = new Request(new URL(pathname, self.location.origin), {
      credentials: 'same-origin',
    })

    try {
      const response = await fetch(request)
      if (!response.ok) {
        console.warn(`No se pudo guardar ${pathname} para uso sin conexión (HTTP ${response.status}).`)
        continue
      }
      await cache.put(request, response)
    } catch (error) {
      console.warn(`No se pudo guardar ${pathname} para uso sin conexión.`, error)
    }
  }
}

async function preservePrecachedAppAssets() {
  try {
    const appAssets = await caches.open('app-assets')
    const cacheNames = await caches.keys()

    // Product HTML can outlive a deployment, so keep its hashed CSS/JS before Workbox prunes old precache entries.
    for (const cacheName of cacheNames) {
      if (!cacheName.startsWith('workbox-precache-v2-')) continue

      const precache = await caches.open(cacheName)
      for (const request of await precache.keys()) {
        const url = new URL(request.url)
        if (url.origin !== self.location.origin || !appAssetPath.test(url.pathname)) continue

        const response = await precache.match(request)
        if (!response) continue

        url.searchParams.delete('__WB_REVISION__')
        try {
          await appAssets.put(new Request(url, { credentials: 'same-origin' }), response)
        } catch (error) {
          console.warn(`No se pudo conservar el recurso ${url.pathname} para uso sin conexión.`, error)
        }
      }
    }

    const keys = await appAssets.keys()
    const expiredKeys = keys.slice(0, Math.max(0, keys.length - 200))
    await Promise.all(expiredKeys.map((request) => appAssets.delete(request)))
  } catch (error) {
    console.warn('No se pudieron conservar los estilos y scripts de páginas guardadas.', error)
  }
}

self.addEventListener('install', (event) => {
  event.waitUntil(Promise.all([
    cacheOfflineEntryPoints(),
    preservePrecachedAppAssets(),
  ]))
})

registerRoute(
  (request) => isSameOrigin(request) && isNavigation(request) && storePagePath.test(request.url.pathname),
  new NetworkFirst({
    cacheName: 'catalog-pages',
    plugins: [new ExpirationPlugin({ maxEntries: 50, maxAgeSeconds: 7 * 24 * 60 * 60 })],
  }),
)

registerRoute(
  ({ request, url }) => request.method === 'GET'
    && url.origin === self.location.origin
    && ['font', 'script', 'style'].includes(request.destination),
  new CacheFirst({
    cacheName: 'app-assets',
    plugins: [new ExpirationPlugin({ maxEntries: 200, maxAgeSeconds: 30 * 24 * 60 * 60 })],
  }),
)

registerRoute(
  (request) => isSameOrigin(request) && isNavigation(request),
  new NetworkOnly(),
)

registerRoute(
  ({ request }) => request.method === 'GET' && request.destination === 'image',
  new CacheFirst({
    cacheName: 'static-assets',
    plugins: [new ExpirationPlugin({ maxEntries: 100, maxAgeSeconds: 30 * 24 * 60 * 60 })],
  }),
)

setCatchHandler(async ({ request }) => {
  if (request.mode === 'navigate') {
    return (await matchPrecache('/offline.html')) ?? Response.error()
  }
  return Response.error()
})

self.skipWaiting()
clientsClaim()
