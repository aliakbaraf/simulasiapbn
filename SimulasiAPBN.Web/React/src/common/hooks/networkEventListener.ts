/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { useEffect } from "react"
import { useDispatch, useSelector } from "react-redux"

import RootState from "@flow/types"
import { setNetworkOnLine } from "@flow/slices/common"

type NetworkEventListenerHook = () => boolean

const useNetworkEventListener: NetworkEventListenerHook = () => {
	const dispatch = useDispatch()
	const networkOnLine = useSelector<RootState, boolean>(state => state.common.networkOnLine)

	const onNetworkChanged = (event: Event) => {
		switch (event.type) {
			case "offline":
				dispatch(setNetworkOnLine(false))
				break
			case "online":
				dispatch(setNetworkOnLine())
				break
		}
	}

	useEffect(() => {
		window.addEventListener("offline", onNetworkChanged)
		window.addEventListener("online", onNetworkChanged)
		return () => {
			window.removeEventListener("offline", onNetworkChanged)
			window.removeEventListener("online", onNetworkChanged)
		}
	}, [])

	return networkOnLine
}

export default useNetworkEventListener
