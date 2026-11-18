// @ts-check
import { defineConfig } from 'astro/config'
import react from '@astrojs/react'
import node from '@astrojs/node'
import netlify from '@astrojs/netlify'
import { AstroPWAIntegration as AstroPWA } from '@vite-pwa/astro'

// Netlify define NETLIFY=true durante su build; en Docker se usa el adaptador de Node
const onNetlify = Boolean(process.env.NETLIFY)

export default defineConfig({
  output: 'server',
  adapter: onNetlify ? netlify() : node({ mode: 'standalone' }),
  build: { inlineStylesheets: 'always' },
  integrations: [
    react(),
    AstroPWA({
      registerType: 'autoUpdate',
      injectRegister: false, // el registro se hace a mano en el layout
      includeAssets: ['favicon.svg'],
      manifest: {
        id: '/',
        name: 'Hilo y Alma',
        short_name: 'Hilo y Alma',
        description: 'Taller de crochet en Guadalajara: figuras, llaveros y decoración tejidos a mano.',
        lang: 'es-MX',
        start_url: '/',
        scope: '/',
        display: 'standalone',
        theme_color: '#8b3a3a',
        background_color: '#efe6d6',
        icons: [
          { src: '/icons/icon-192.png', sizes: '192x192', type: 'image/png', purpose: 'any' },
          { src: '/icons/icon-512.png', sizes: '512x512', type: 'image/png', purpose: 'any' },
          { src: '/icons/icon-maskable-512.png', sizes: '512x512', type: 'image/png', purpose: 'maskable' },
        ],
      },
      workbox: {
        navigateFallback: null,
        globPatterns: ['**/*.{css,js,svg,ico,woff2}', 'icons/*.png'],
        runtimeCaching: [
          {
            urlPattern: ({ request }) => request.mode === 'navigate',
            handler: 'NetworkFirst',
            options: { cacheName: 'pages', networkTimeoutSeconds: 3 },
          },
        ],
      },
    }),
  ],
})



/* @ts-check
import { defineConfig } from 'astro/config';

import react from '@astrojs/react';
import node from '@astrojs/node';

// https://astro.build/config
export default defineConfig({
  output: 'server',
  adapter: node({ mode: 'standalone' }),
  integrations: [react()],
  build: { inlineStylesheets: 'always' },
});*/