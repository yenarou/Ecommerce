import { Outlet } from 'react-router-dom'
import TopBar from './TopBar'
import Footer from './Footer'

export default function Layout() {
  return (
    <>
      <TopBar />
      <Outlet />
      <Footer />
    </>
  )
}
