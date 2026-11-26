import { clientsClaim } from '@vite-pwa/workbox-swkit/core'
import { cleanupOutdatedCaches, matchPrecache, precacheAndRoute } from '@vite-pwa/workbox-swkit/precaching'
import { registerRoute, setCatchHandler } from '@vite-pwa/workbox-swkit/routing'
import { ExpirationPlugin } from '@vite-pwa/workbox-swkit/expiration'
import { CacheFirst, NetworkFirst, NetworkOnly } from '@vite-pwa/workbox-swkit/strategies'

precacheAndRoute(self.__WB_MANIFEST)
cleanupOutdatedCaches()

const storePagePath = /^\/(?:$|carrito\/?$|categoria\/[^/]+\/?$|products\/[^/]+\/?$)/
const isSameOrigin = ({ url }) => url.origin === self.location.origin
const isNavigation = ({ request }) => request.mode === 'navigate'

async function cacheOfflineEntryPoints() {
  const cache = await caches.open('catalog-pages')

  for (const pathname of ['/', '/carrito']) {
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

self.addEventListener('install', (event) => {
  event.waitUntil(cacheOfflineEntryPoints())
})

registerRoute(
  (request) => isSameOrigin(request) && isNavigation(request) && storePagePath.test(request.url.pathname),
  new NetworkFirst({
    cacheName: 'catalog-pages',
    networkTimeoutSeconds: 3,
    plugins: [new ExpirationPlugin({ maxEntries: 50, maxAgeSeconds: 7 * 24 * 60 * 60 })],
  }),
)

registerRoute(
  (request) => isSameOrigin(request) && isNavigation(request),
  new NetworkOnly(),
)

registerRoute(
  ({ request, url }) => request.method === 'GET' && (
    request.destination === 'image'
    || (
      url.origin === self.location.origin
      && ['font', 'script', 'style'].includes(request.destination)
    )
  ),
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
