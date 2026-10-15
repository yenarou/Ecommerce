# FrontendAdmin

Panel administrativo separado del Frontend-astro de la tienda.

## Requisitos
- Node.js 22+
- Backend ECommerce ejecutándose
- GraphQL disponible en `http://localhost:14001/graphql`

## Instalación
```bash
npm install
npm run dev
```

Panel: http://localhost:15175

## Variables de entorno
Copia `.env.example` como `.env` si necesitas cambiar las URLs.

## Flujo
- `/admin` inicia el panel.
- `/admin/login` inicia sesión con el endpoint existente `/api/v1/auth/login`.
- Token JWT se guarda en `localStorage`.
- Las operaciones GraphQL se concentran en `src/api/admin.js`.
- El token se manda como `Authorization: Bearer <token>`.

## Importante
El frontend espera las mutaciones administrativas con los nombres documentados en `src/api/admin.js`.
Si en el backend cambiaste el nombre de una mutación o sus argumentos, solamente debes modificar ese archivo; no necesitas tocar las pantallas.
