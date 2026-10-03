const CLIENT_ID = import.meta.env.PUBLIC_GOOGLE_CLIENT_ID

export default function GoogleButton() {
  function handleClick() {
    if (!CLIENT_ID) {
      alert('Falta configurar PUBLIC_GOOGLE_CLIENT_ID en el .env')
      return
    }
    const params = new URLSearchParams({
      client_id: CLIENT_ID,
      redirect_uri: `${window.location.origin}/auth/google`,
      response_type: 'code',
      scope: 'openid email profile',
    })
    window.location.href = `https://accounts.google.com/o/oauth2/v2/auth?${params}`
  }

  return (
    <button type="button" className="btn auth-google" onClick={handleClick}>
      Continuar con Google
    </button>
  )
}