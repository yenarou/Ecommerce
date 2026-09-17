import './Hero.css'
import heroImg2 from '/images/heroImg2.png'

export default function Hero() {
  return (
    <section className="hero">
      <div className="hero__text">
        <p className="hero__eyebrow">Taller de crochet · Guadalajara</p>
        <h1 className="hero__title">
          Pequeñas piezas,
          <br />
          hechas para quedarse.
        </h1>
        <p className="hero__subtitle">
          Figuras, llaveros y decoración tejidos a mano, con tiempo y atención
          al detalle. Elige una pieza o hazla tuya.
        </p>
      </div>
      <div className="hero__image" aria-hidden="true">
        <img
          src={heroImg2}
          alt=""
        />
      </div>
    </section>
  )
}
