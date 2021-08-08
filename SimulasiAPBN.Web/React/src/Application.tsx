/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, Suspense } from "react"
import { useDispatch, useSelector } from "react-redux"

import useDarkMode from "@common/hooks/darkMode"
import useErrorHandler from "@common/hooks/errorHandler"
import useNetworkEventListener from "@common/hooks/networkEventListener"
import { useService } from "@common/hooks/services"
import SimulationSession from "@core/models/SimulationSession"
import { setApplicationStarted, setLoading, setNetworkLoading } from "@flow/slices/common"
import {
	setContents,
	setDeficitLaw,
	setDeficitThreshold,
	setGrossDomesticProduct,
	setRules,
} from "@flow/slices/dynamic-metadata"
import RootState from "@flow/types"

import OfflineScreen from "@screens/Error/OfflineScreen"
import ContinueSessionScreen from "@screens/Misc/ContinueSessionScreen"
import LoadingScreen from "@screens/Misc/LoadingScreen"
import SuspenseFallbackScreen from "@screens/Misc/SuspenseFallbackScreen"

const RootRouter = React.lazy(() => import("@screens/RootRouter"))

const Application: React.FC = () => {
	useDarkMode()
	const dispatch = useDispatch()
	const handleError = useErrorHandler(dispatch)
	const networkOnLine = useNetworkEventListener()
	const service = useService()

	const applicationStarted = useSelector<RootState, boolean>(state => state.common.applicationStarted)
	const displayContinueSession = useSelector<RootState, boolean>(state => state.common.displayContinueSession)
	const loading = useSelector<RootState, boolean | string>(state => state.common.loading)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)

	// This function will only run on first time load
	const startApplication = async () => {
		if (applicationStarted || !networkOnLine) {
			return
		}

		try {
			dispatch(setLoading("Memulai aplikasi..."))
			dispatch(setNetworkLoading())

			const [contents, deficitLaw, deficitThreshold, grossDomesticProduct, rules] = await Promise.all([
				service.Simulation.getWebContents(),
				service.Simulation.getConfigDeficitLaw(),
				service.Simulation.getConfigDeficitThreshold(),
				service.Simulation.getConfigGrossDomesticProduct(),
				service.Simulation.getRules(),
			])
			dispatch(setContents(contents))
			dispatch(setDeficitLaw(deficitLaw))
			dispatch(setDeficitThreshold(deficitThreshold))
			dispatch(setGrossDomesticProduct(grossDomesticProduct))
			dispatch(setRules(rules))

			dispatch(setApplicationStarted())
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setNetworkLoading(false))
			dispatch(setLoading(false))
		}
	}

	useEffect(() => {
		startApplication().then()
	}, [networkOnLine])

	if (loading !== false) {
		return <LoadingScreen />
	}

	if (!networkOnLine) {
		return <OfflineScreen />
	}

	if (displayContinueSession && !(typeof session === "undefined" || session === null)) {
		return <ContinueSessionScreen />
	}

	return <Suspense fallback={<SuspenseFallbackScreen />}>
		<RootRouter />
	</Suspense>
}

export default Application
