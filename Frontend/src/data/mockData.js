// borra esto ya que tengas backend, esto es simulacion de productos
import shyguy from '/images/shyguy.png'
import mario from '/images/mario.png'
import marioycapi from '/images/marioycapi.png'
import snoopy from '/images/snoopy.png'
import snoop from '/images/snoop.png'
import sunflower from '/images/sunflower.png'
import flores from '/images/flores.png'
import mandalas from '/images/mandalas.png'

export const categories = [
  {
    slug: 'figuras',
    name: 'Figuras',
    description: 'Figuras de peluche tejidas, ideales para regalo o colección.',
  },
  {
    slug: 'llaveros',
    name: 'Llaveros',
    description: 'Piezas pequeñas para mochila, bolsa o llaves.',
  },
  {
    slug: 'decoracion',
    name: 'Decoración',
    description: 'Piezas para repisa, escritorio o pared.',
  },
]

export const products = [
  {
    id: 'p-001',
    category: 'figuras',
    name: 'Shy-Guy',
    description:
      'Es un personaje vergon tejido a mano de 15cm de alto.',
    price: 320.0,
    stock: 5,
    image_url: shyguy,
    image_url_alt: shyguy,
    days_to_make: 5,
  },
  {
    id: 'p-002',
    category: 'figuras',
    name: 'Mario',
    description:
      'Fontanero pendejo, le roban a la princesa cada 2 dias, mete buenos vergazos.',
    price: 310.0,
    stock: 3,
    image_url: marioycapi,
    image_url_alt: mario,
    days_to_make: 4,
  },
  {
    id: 'p-003',
    category: 'llaveros',
    name: 'Snoop Dogg',
    description: 'Perro blanco y negro con los ojos cerrados porque ojos que no ven, corazon que no siente. Rapea durisimo.',
    price: 95.0,
    stock: 20,
    image_url: snoopy,
    image_url_alt: snoop,
    days_to_make: 1,
  },
  {
    id: 'p-004',
    category: 'llaveros',
    name: 'Flor del sol',
    description: 'Flor pedorra que se parece al sol.',
    price: 110.0,
    stock: 0,
    image_url: sunflower,
    image_url_alt: snoop,
    days_to_make: 1,
  },
  {
    id: 'p-005',
    category: 'decoracion',
    name: 'Ramo de flores',
    description: 'Flores de crochet.',
    price: 560.0,
    stock: 2,
    image_url: flores,
    image_url_alt: flores,
    days_to_make: 6,
  },
  {
    id: 'p-006',
    category: 'decoracion',
    name: 'Mandalas colgantes',
    description: 'Decoracion de mandalas para colgar en la pared o ventana no se.',
    price: 310.0,
    stock: 9,
    image_url: mandalas,
    image_url_alt: snoop,
    days_to_make: 3,
  },
]

//opciones aplicables a productos
export const customizationOptions = [
  {
    id: 'c-001',
    description: 'Cambiar color base',
    image_url: 'https://images.unsplash.com/photo-1520903920243-7ce9d02d3a08?auto=format&fit=crop&w=200&q=60',
    additional_price: 0,
  },
  {
    id: 'c-002',
    description: 'Bordar iniciales',
    image_url: 'https://images.unsplash.com/photo-1519241047957-be31d7379a5d?auto=format&fit=crop&w=200&q=60',
    additional_price: 60,
  },
  {
    id: 'c-003',
    description: 'Agregar moño o accesorio',
    image_url: 'https://images.unsplash.com/photo-1517705008128-361805f42e07?auto=format&fit=crop&w=200&q=60',
    additional_price: 45,
  },
  {
    id: 'c-004',
    description: 'Empaque de regalo',
    image_url: 'https://images.unsplash.com/photo-1549465220-1a8b9238cd48?auto=format&fit=crop&w=200&q=60',
    additional_price: 35,
  },
]

export function getProductsByCategory(slug) {
  return products.filter((p) => p.category === slug)
}

export function getProductById(id) {
  return products.find((p) => p.id === id)
}

export function getCategoryBySlug(slug) {
  return categories.find((c) => c.slug === slug)
}
