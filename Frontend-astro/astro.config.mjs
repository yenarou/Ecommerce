// @ts-check
import { defineConfig } from 'astro/config';

import react from '@astrojs/react';
import node from '@astrojs/node';
import { AstroPWAIntegration } from '@vite-pwa/astro';

// https://astro.build/config
export default defineConfig({
  site: 'https://www.hilo-yalma.com',
  output: 'server',
  adapter: node({ mode: 'standalone' }),
  integrations: [
    react(),
    AstroPWAIntegration({
      registerType: 'autoUpdate',
      generateSW: {
        navigateFallback: null
      },
      manifest: {
        name: 'Hilo y Alma',
        short_name: 'Hilo y Alma',
        description: 'Taller de crochet con piezas hechas a mano.',
        lang: 'es-MX',
        start_url: '/',
        scope: '/',
        display: 'standalone',
        theme_color: '#8b3a3a',
        background_color: '#efe6d6',
        icons: [
          {
            src: '/icons/pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png',
            purpose: 'any'
          },
          {
            src: '/icons/pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'any'
          },
          {
            src: '/icons/pwa-maskable-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable'
          }
        ]
      }
    })
  ]
});