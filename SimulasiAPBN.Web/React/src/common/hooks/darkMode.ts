/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { useEffect, useState } from "react"
import { useDispatch } from "react-redux"

import { setDarkMode } from "@flow/slices/common"

type DarkModeHook = () => MediaQueryList | null

const useDarkMode: DarkModeHook = () => {
	const dispatch = useDispatch()
	const [mediaQueryList, setMediaQueryList] = useState<MediaQueryList | null>(null)

	const onMediaMatches = (matches: boolean) => {
		dispatch(setDarkMode(matches))
	}

	const onMediaQueryListChanged = (event: MediaQueryListEvent) => {
		onMediaMatches(event.matches)
	}

	useEffect(() => {
		if ("matchMedia" in window) {
			setMediaQueryList(window.matchMedia("(prefers-color-scheme: dark)"))
		}
	}, [])

	useEffect(() => {
		if (mediaQueryList !== null) {
			onMediaMatches(mediaQueryList.matches)
			if ("addEventListener" in mediaQueryList) {
				mediaQueryList.addEventListener("change", onMediaQueryListChanged)
			} else {
				// eslint-disable-next-line @typescript-eslint/ban-ts-comment
				// @ts-ignore
				// noinspection JSDeprecatedSymbols
				mediaQueryList.addListener(onMediaQueryListChanged)
			}
		}
		return () => {
			if (mediaQueryList !== null) {
				if ("removeEventListener" in mediaQueryList) {
					mediaQueryList.removeEventListener("change", onMediaQueryListChanged)
				} else {
					// eslint-disable-next-line @typescript-eslint/ban-ts-comment
					// @ts-ignore
					// noinspection JSDeprecatedSymbols
					mediaQueryList.removeListener(onMediaQueryListChanged)
				}
			}
		}
	}, [mediaQueryList])

	return mediaQueryList
}

export default useDarkMode
