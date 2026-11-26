import { defineConfig } from 'astro/config';
import react from '@astrojs/react';
import node from '@astrojs/node';
import { AstroPWAIntegration } from '@vite-pwa/astro';

export default defineConfig({
  site: process.env.SITE_URL ?? 'http://localhost:15174',
  output: 'server',
    i18n: {
      defaultLocale: 'es',
      locales: ['es', 'en'],
      fallback: { en: 'es' },
      routing: { prefixDefaultLocale: false, fallbackType: 'rewrite' },
    },
  adapter: node({ mode: 'standalone' }),
  integrations: [
    react(),
    AstroPWAIntegration({
  registerType: 'autoUpdate',
  strategies: 'buildSW',
  filename: 'sw.js',
  manifest: {
    name: 'Hilo y Alma - Figuras de crochet',
    short_name: 'Hilo y Alma',
    description: 'Tienda de figuras de crochet hechas a mano',
    lang: 'es-MX',
    start_url: '/',
    scope: '/',
    display: 'standalone',
    theme_color: '#8b3a3a',
    background_color: '#efe6d6',
    icons: [
      { src: '/icons/icon-192.png', sizes: '192x192', type: 'image/png' },
      { src: '/icons/icon-512.png', sizes: '512x512', type: 'image/png' },
      { src: '/icons/icon-maskable-512.png', sizes: '512x512', type: 'image/png', purpose: 'maskable' },
    ],
  },
  buildSW: {
    swSrc: 'src/sw.js',
    globPatterns: ['_astro/**/*.{js,css}', 'icons/*.png', 'favicon.svg'],
  },
  devOptions: { enabled: false },
}),
  ],
  build: { inlineStylesheets: 'always' },
});