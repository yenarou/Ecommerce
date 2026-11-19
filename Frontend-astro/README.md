# Astro Starter Kit: Minimal

```sh
npm create astro@latest -- --template minimal
```

> 🧑‍🚀 **Seasoned astronaut?** Delete this file. Have fun!

## 🚀 Project Structure

Inside of your Astro project, you'll see the following folders and files:

```text
/
├── public/
├── src/
│   └── pages/
│       └── index.astro
└── package.json
```

Astro looks for `.astro` or `.md` files in the `src/pages/` directory. Each page is exposed as a route based on its file name.

There's nothing special about `src/components/`, but that's where we like to put any Astro/React/Vue/Svelte/Preact components.

Any static assets, like images, can be placed in the `public/` directory.

## 🧞 Commands

All commands are run from the root of the project, from a terminal:

| Command                   | Action                                           |
| :------------------------ | :----------------------------------------------- |
| `npm install`             | Installs dependencies                            |
| `npm run dev`             | Starts local dev server at `localhost:4321`      |
| `npm run build`           | Build your production site to `./dist/`          |
| `npm run preview`         | Preview your build locally, before deploying     |
| `npm run astro ...`       | Run CLI commands like `astro add`, `astro check` |
| `npm run astro -- --help` | Get help using the Astro CLI                     |

## 👀 Want to learn more?

Feel free to check [our documentation](https://docs.astro.build) or jump into our [Discord server](https://astro.build/chat).

## PWA instalable

La tienda usa `@vite-pwa/astro` para generar el manifiesto y registrar el service worker. El nombre es **Hilo y Alma** y sus colores salen de los tokens `--color-thread` (`#8b3a3a`) y `--color-paper` (`#efe6d6`) en `src/styles/tokens.css`.

Los iconos de instalación están en `public/icons/`: 192×192, 512×512 y una variante maskable de 512×512. Se generan desde `hilo-y-alma-cat.jpg`, quitando su fondo claro y dejando el gato dentro del área segura central. Con ImageMagick instalado, ejecuta desde esta carpeta:

```sh
magick public/icons/hilo-y-alma-cat.jpg -alpha on -fuzz 5% -fill none -draw 'color 0,0 floodfill' -resize '80%x80%' -gravity center -background '#efe6d6' -extent 192x192 public/icons/pwa-192x192.png
magick public/icons/hilo-y-alma-cat.jpg -alpha on -fuzz 5% -fill none -draw 'color 0,0 floodfill' -resize '80%x80%' -gravity center -background '#efe6d6' -extent 512x512 public/icons/pwa-512x512.png
magick public/icons/hilo-y-alma-cat.jpg -alpha on -fuzz 5% -fill none -draw 'color 0,0 floodfill' -resize '80%x80%' -gravity center -background '#efe6d6' -extent 512x512 public/icons/pwa-maskable-512x512.png
```

Prueba la versión de producción con `npm run build` y `npm run preview`. En Chrome abre DevTools → Application → Manifest para revisar nombre, modo standalone, colores e iconos; después ejecuta Lighthouse con la categoría PWA. Desde Chrome/Android, usa **Instalar Hilo y Alma** o **Agregar a pantalla principal**. La instalación requiere un contexto seguro (HTTPS o `localhost`); la funcionalidad offline completa no forma parte de esta práctica.
