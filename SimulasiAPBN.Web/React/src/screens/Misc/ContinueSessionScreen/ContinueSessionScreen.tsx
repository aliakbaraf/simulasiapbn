/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"

import ColorVariant from "@common/enum/ColorVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import useGreeting from "@common/hooks/greeting"
import { useService } from "@common/hooks/services"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import SimulationSession from "@core/models/SimulationSession"
import { setClientSimulation } from "@flow/slices/simulation"
import { setDisplayContinueSession, setLoading, setNetworkLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import RootRoutes from "@screens/routes"

import "./ContinueSessionScreen.scoped.css"

type ContinueSessionScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const ContinueSessionScreen: React.FC<ContinueSessionScreenProps> = () => {
	const dispatch = useDispatch()
	const greeting = useGreeting()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const service = useService()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)

	const onResetButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		resetSimulation().then()
	}

	const onContinueButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		continueSimulation().then()
	}

	const resetSimulation = async () => {
		if (typeof session === "undefined" || session === null) {
			return
		}

		try {
			dispatch(setLoading(`Mengakhiri sesi simulasi...`))
			dispatch(setNetworkLoading())

			await service.Simulation.finishSession(session.id)

			history.push(RootRoutes.LandingScreen)
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setClientSimulation())
			dispatch(setDisplayContinueSession(false))
			dispatch(setNetworkLoading(false))
			dispatch(setLoading(false))
		}
	}

	const continueSimulation = async () => {
		if (typeof session === "undefined" || session === null) {
			return
		}

		try {
			dispatch(setLoading(`Mengambil sesi simulasi untuk ${session.name}...`))
			dispatch(setNetworkLoading())

			const clientSimulation = await service.Simulation.getSession(session.id)
			dispatch(setClientSimulation(clientSimulation))

			const routeStartWithEducation = history.location.pathname.startsWith(RootRoutes.EducationScreen)
			const routeStartWithGame = history.location.pathname.startsWith(RootRoutes.GameScreen)
			if (!routeStartWithEducation && !routeStartWithGame) {
				history.push(RootRoutes.GameScreen)
			}
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setDisplayContinueSession(false))
			dispatch(setNetworkLoading(false))
			dispatch(setLoading(false))
		}
	}
	return (
		<DefaultLayout className="screen" screenType={ScreenType.Unknown}>
			<div className="content">
				<p className="greeting">
					{greeting}, {session?.name || ""}!
				</p>
				<p className="description">Terdapat rancangan APBN yang belum kamu selesaikan.</p>
				<p className="prompt">Lanjutkan sesi sebelumnya?</p>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onResetButtonClicked}
				>
					Rancang Baru
				</Button>
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					onClick={onContinueButtonClicked}
				>
					Lanjutkan
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default ContinueSessionScreen
