import { useState } from 'react'
import { getGoogleUrl } from '../api/auth'

export default function GoogleButton() {
  const [error, setError] = useState('')

  async function handleClick() {
    setError('')
    try {
      const url = await getGoogleUrl()
      if (!url) {
        setError('El inicio con Google aún no está disponible.')
        return
      }
      window.location.href = url
    } catch (err) {
      setError(err.message)
    }
  }

  return (
    <>
      <button type="button" className="btn auth-google" onClick={handleClick}>
        Continuar con Google
      </button>
      {error && <p className="auth-form__error" role="alert">{error}</p>}
    </>
  )
}