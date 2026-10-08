import { useEffect } from 'react'
import { useAuth } from '../stores/auth'
import { useCartStore } from '../stores/cart'

export default function GoogleCallback() {
    useEffect(() => {
        const completeLogin = async () => {
            const params = new URLSearchParams(window.location.search)

            const hashParams = new URLSearchParams(
                window.location.hash.substring(1)
            )

            const userId = params.get('userId')
            const username = params.get('username')
            const token = hashParams.get('token')

            if (!userId || !username || !token) {
                window.location.href = '/login?error=google_auth'
                return
            }

            useAuth.getState().setUser({
                userId,
                username,
                token
            })

            try {
                await useCartStore.getState().loadFromServer()
            } catch (error) {
                console.error('No se pudo cargar el carrito de la cuenta:', error)
            }

            window.location.href = '/'
        }

        void completeLogin()
    }, [])

    return <p>Iniciando sesión...</p>
}