//Service Worker de Hilo y Alma

// Versionado de cachés
const VERSION = 'v1';
const MANIFEST = self.__WB_MANIFEST; 

function hash(texto) {
  let h = 0;
  for (let i = 0; i < texto.length; i++) h = (h * 31 + texto.charCodeAt(i)) | 0;
  return (h >>> 0).toString(16);
}

const PRECACHE = `precache-${VERSION}-${hash(JSON.stringify(MANIFEST))}`;
const PAGES = `pages-${VERSION}`;
const ASSETS = `assets-${VERSION}`; 
const CACHES_ACTUALES = [PRECACHE, PAGES, ASSETS];
const OFFLINE_URL = '/offline.html';
const MAX_ASSETS = 80;
const PRIVADAS = ['/carrito', '/checkout', '/admin', '/ordenes', '/pago', '/login', '/registro', '/api'];

self.addEventListener('install', (event) => {
  const urls = MANIFEST.map((e) => new URL(e.url, self.registration.scope).href);
  urls.push(new URL(OFFLINE_URL, self.registration.scope).href);
  const sinRepetir = [...new Set(urls)];

  event.waitUntil(
    caches.open(PRECACHE).then((cache) => cache.addAll(sinRepetir))
  );
  self.skipWaiting();
});

// limpia de cachés viejas y toma de control
self.addEventListener('activate', (event) => {
  event.waitUntil(
    (async () => {
      const nombres = await caches.keys();
      await Promise.all(
        nombres.filter((n) => !CACHES_ACTUALES.includes(n)).map((n) => caches.delete(n))
      );
      await self.clients.claim(); 
    })()
  );
});

// cómo responder
self.addEventListener('fetch', (event) => {
  const { request } = event;
  if (request.method !== 'GET') return;

  const url = new URL(request.url);
  if (!url.protocol.startsWith('http')) return;
  if (url.pathname.startsWith('/graphql')) return;

  if (request.mode === 'navigate') {
    event.respondWith(networkFirst(event, url));
    return;
  }

  if (['style', 'script', 'image', 'font'].includes(request.destination)) {
    event.respondWith(cacheFirst(event));
  }
});

// Estrategia network-first
async function networkFirst(event, url) {
  const { request } = event;
  const privada = PRIVADAS.some((p) => url.pathname.startsWith(p));

  try {
    const respuesta = await fetch(request);
    if (!privada && respuesta.ok && !respuesta.redirected) {
      const copia = respuesta.clone();
      event.waitUntil(caches.open(PAGES).then((c) => c.put(request, copia)));
    }
    return respuesta;
  } catch {
    if (!privada) {
      const guardada = await caches.match(request);
      if (guardada) return guardada;
    }
    return caches.match(OFFLINE_URL);
  }
}

// Estrategia cache-first: assets
async function cacheFirst(event) {
  const { request } = event;
  const guardada = await caches.match(request);
  if (guardada) return guardada;

  try {
    const respuesta = await fetch(request);
    if (respuesta.ok || respuesta.type === 'opaque') {
      const copia = respuesta.clone();
      event.waitUntil(
        caches.open(ASSETS).then(async (c) => {
          await c.put(request, copia);
          await limitar(ASSETS, MAX_ASSETS);
        })
      );
    }
    return respuesta;
  } catch {
    return Response.error();
  }
}

async function limitar(nombre, max) {
  const cache = await caches.open(nombre);
  const claves = await cache.keys();
  if (claves.length > max) {
    await Promise.all(claves.slice(0, claves.length - max).map((k) => cache.delete(k)));
  }
}


//Background Sync + Notificaciones
const DB_NAME = 'hiloyalma-outbox'
const STORE = 'cola'

function abrirDB() {
  return new Promise((ok, fail) => {
    const req = indexedDB.open(DB_NAME, 1)
    req.onupgradeneeded = () => req.result.createObjectStore(STORE, { keyPath: 'id' })
    req.onsuccess = () => ok(req.result)
    req.onerror = () => fail(req.error)
  })
}

async function leerPendiente(id) {
  const db = await abrirDB()
  return new Promise((ok, fail) => {
    const req = db.transaction(STORE).objectStore(STORE).get(id)
    req.onsuccess = () => ok(req.result)
    req.onerror = () => fail(req.error)
  })
}

async function borrarPendiente(id) {
  const db = await abrirDB()
  return new Promise((ok, fail) => {
    const tx = db.transaction(STORE, 'readwrite')
    tx.objectStore(STORE).delete(id)
    tx.oncomplete = ok
    tx.onerror = () => fail(tx.error)
  })
}

async function guardarLog(texto) {
  const db = await abrirDB()
  return new Promise((ok, fail) => {
    const tx = db.transaction(STORE, 'readwrite')
    tx.objectStore(STORE).put({ id: 'diagnostico', texto, fecha: new Date().toISOString() })
    tx.oncomplete = ok
    tx.onerror = () => fail(tx.error)
  })
}

async function vaciarCola() {
  await guardarLog('1. sync recibido')
  try {
    const pendiente = await leerPendiente('carrito')
    if (!pendiente) {
      await guardarLog('2. no hay registro carrito')
      return
    }

    const res = await fetch(pendiente.url, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${pendiente.token}`,
      },
      body: pendiente.body,
    })
    await guardarLog('3. respuesta status ' + res.status)

    if (res.status >= 500) throw new Error('Servidor no disponible, reintentar')

    let json = null
    try { json = await res.json() } catch { /* sin JSON */ }

    await borrarPendiente('carrito')
    await guardarLog('4. registro borrado, permiso notificaciones: ' + Notification.permission)

    if (json && !json.errors && Notification.permission === 'granted') {
      await self.registration.showNotification('Hilo y Alma', {
        body: 'Tu carrito se sincronizó con el servidor.',
        icon: '/icons/icon-192.png',
        badge: '/icons/icon-192.png',
        data: { url: '/carrito' },
      })
      await guardarLog('5. notificación enviada')
    }
  } catch (e) {
    await guardarLog('ERROR: ' + (e && e.message ? e.message : e))
    throw e
  }
}

self.addEventListener('sync', (event) => {
  if (event.tag === 'cola-pendientes') {
    event.waitUntil(vaciarCola())
  }
})

self.addEventListener('notificationclick', (event) => {
  event.notification.close()
  const destino = event.notification.data?.url || '/'
  event.waitUntil(
    self.clients.matchAll({ type: 'window' }).then((ventanas) => {
      const abierta = ventanas.find((v) => v.url.includes(destino))
      return abierta ? abierta.focus() : self.clients.openWindow(destino)
    }),
  )
})
self.addEventListener('message', (event) => {
  if (event.data === 'vaciar-cola') {
    event.waitUntil(vaciarCola())
  }
})