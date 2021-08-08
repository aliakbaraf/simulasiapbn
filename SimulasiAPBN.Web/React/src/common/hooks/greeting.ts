/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { useEffect, useState } from "react"

type GreetingHook = () => string

const useGreeting: GreetingHook = () => {
	const [greeting, setGreeting] = useState<string>("Halo")

	const renewGreeting = () => {
		const now = new Date().getHours()
		if (now <= 10) {
			setGreeting("Selamat Pagi")
		} else if (now <= 15) {
			setGreeting("Selamat Siang")
		} else if (now <= 17) {
			setGreeting("Selamat Petang")
		} else {
			setGreeting("Selamat Malam")
		}
	}

	useEffect(() => {
		renewGreeting()
		const interval = setInterval(renewGreeting, 60 * 1000)
		return () => {
			clearInterval(interval)
		}
	}, [])

	return greeting
}

export default useGreeting
