import sharp from 'sharp'
import { mkdir } from 'node:fs/promises'

const SOURCE = 'src/assets/logo.png'
const BRAND_BG = '#efe6d6' //fondo del manifest y tokens
const TRANSPARENT = { r: 0, g: 0, b: 0, alpha: 0 }

await mkdir('public/icons', { recursive: true })

async function makeIcon({ size, logoScale, out }) {
  const inner = Math.round(size * logoScale)
  const logo = await sharp(SOURCE)
    .resize(inner, inner, { fit: 'contain', background: TRANSPARENT })
    .toBuffer()

  await sharp({ create: { width: size, height: size, channels: 4, background: BRAND_BG } })
    .composite([{ input: logo, gravity: 'center' }])
    .png()
    .toFile(out)
}

await makeIcon({ size: 192, logoScale: 0.9, out: 'public/icons/icon-192.png' })
await makeIcon({ size: 512, logoScale: 0.9, out: 'public/icons/icon-512.png' })
//sabe que formato
await makeIcon({ size: 512, logoScale: 0.8, out: 'public/icons/icon-maskable-512.png' })

console.log('Iconos generados en public/icons')