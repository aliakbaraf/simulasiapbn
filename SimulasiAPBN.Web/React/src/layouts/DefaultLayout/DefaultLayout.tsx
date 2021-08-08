/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import mergeProps from "merge-props"
import { FiMenu } from "react-icons/fi"

import RootRoutes from "@screens/routes"
import ColorVariant from "@common/enum/ColorVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import { useService } from "@common/hooks/services"
import ComponentProps from "@common/libraries/component"
import { showPublication } from "@common/libraries/publication"
import Button from "@components/Button"
import DarkModeSwitch from "@components/DarkModeSwitch"
import Image from "@components/Image"
import MinistryLogo from "@components/MinistryLogo"
import Navigation from "@components/Navigation"
import SimulationSession from "@core/models/SimulationSession"
import { setClientSimulation } from "@flow/slices/simulation"
import { setDisplayContinueSession, setLoading, setNetworkLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import BaseLayout from "@layouts/BaseLayout"
import { ScreenType } from "@layouts/DefaultLayout"

import BasicLoadingSpinnerVector from "@assets/svg/basic-loading-spinner.svg"
import "./DefaultLayout.css"

type DefaultLayoutProps = ComponentProps<
	{
		fixedNavigation?: boolean
		screenType: ScreenType
	},
	React.AllHTMLAttributes<HTMLDivElement>
>

const DefaultLayout: React.FC<DefaultLayoutProps> = props => {
	const { fixedNavigation, screenType, ...pureProps } = props
	const { children, ...otherProps } = mergeProps({}, pureProps)

	const dispatch = useDispatch()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const service = useService()

	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const [showMenu, setShowMenu] = useState<boolean>(false)
	const [showEndSessionPrompt, setShowEndSessionPrompt] = useState<boolean>(false)

	const onCloseMenuButtonClicked: React.MouseEventHandler = () => {
		setShowMenu(false)
	}

	const onOpenMenuButtonClicked: React.MouseEventHandler = () => {
		setShowMenu(true)
	}

	const onLearnStateBudgetButtonClicked: React.MouseEventHandler = () => {
		history.push(RootRoutes.EducationScreen)
	}

	const onToSimulationButtonClicked: React.MouseEventHandler = () => {
		history.push(RootRoutes.GameScreen)
	}

	const onCloseEndSimulationPromptButtonClicked: React.MouseEventHandler = () => {
		setShowEndSessionPrompt(false)
	}

	const onOpenEndSimulationPromptButtonClicked: React.MouseEventHandler = () => {
		setShowMenu(false)
		setShowEndSessionPrompt(true)
	}

	const onEndSimulationButtonClicked: React.MouseEventHandler = () => {
		endSimulationSession().then()
	}

	const endSimulationSession = async () => {
		if (typeof session === "undefined" || session === null) {
			return
		}

		try {
			dispatch(setLoading(`Mengakhiri sesi simulasi...`))
			dispatch(setNetworkLoading())

			const finishedSession = await service.Simulation.finishSession(session.id)
			dispatch(setClientSimulation())

			showPublication(finishedSession?.session.id || "")
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setDisplayContinueSession(false))
		}
	}

	const NavigationAction: React.FC = () => {
		const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)
		const showMenuButton =
			screenType === ScreenType.Game ||
			(screenType === ScreenType.Education && typeof session !== "undefined" && session !== null)
		return (
			<>
				{!networkLoading ? null : (
					<span className="mr-4">
						<Image
							className="w-5 animate-spin-slow"
							source={BasicLoadingSpinnerVector}
							fallback={BasicLoadingSpinnerVector}
							type="image/svg+xml"
							alt={"Berkomunikasi dengan server..."}
						/>
					</span>
				)}
				<DarkModeSwitch />
				{showMenuButton ? (
					<Button
						color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
						icon={FiMenu}
						onClick={onOpenMenuButtonClicked}
					/>
				) : null}
			</>
		)
	}

	const MenuContainer: React.FC = () => (
		<>
			<div className="menu-container">
				<div className="card">
					<div className="header">
						{typeof session !== "undefined" && session !== null ? (
							<>
								<p className="title">{session.name}</p>
								<span className="sub-title">MENTERI KEUANGAN</span>
							</>
						) : (
							<MinistryLogo withTitle />
						)}
					</div>
					<div className="content">
						{screenType !== ScreenType.Unknown ? (
							<Button
								color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
								onClick={onLearnStateBudgetButtonClicked}
							>
								PELAJARI APBN
							</Button>
						) : null}
						{typeof session !== "undefined" && session !== null ? (
							<>
								{screenType === ScreenType.Education ? (
									<Button
										color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
										onClick={onToSimulationButtonClicked}
									>
										KE SIMULASI
									</Button>
								) : null}
								<Button
									color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
									onClick={onOpenEndSimulationPromptButtonClicked}
								>
									AKHIRI SIMULASI
								</Button>
							</>
						) : null}
						<Button
							color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
							onClick={onCloseMenuButtonClicked}
						>
							KEMBALI
						</Button>
					</div>
				</div>
			</div>
			<div className="floating-background" />
		</>
	)

	const EndSessionPromptContainer: React.FC = () => (
		<>
			<div className="end-session-prompt-container">
				<div className="card">
					<div className="header">
						<span className="title">Akhiri Simulasi</span>
					</div>
					<div className="content">
						<p className="description">Apakah kamu yakin ingin mengakhiri sesi simulasi ini?</p>
					</div>
					<div className="click-to-action">
						<Button
							color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
							onClick={onCloseEndSimulationPromptButtonClicked}
						>
							Kembali
						</Button>
						<Button
							color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
							onClick={onEndSimulationButtonClicked}
						>
							AKHIRI
						</Button>
					</div>
				</div>
			</div>
			<div className="floating-background" />
		</>
	)

	return (
		<BaseLayout>
			<Navigation fixed={fixedNavigation || false}>
				<NavigationAction />
			</Navigation>
			<div className="layout-content">
				<div {...otherProps}>{children}</div>
			</div>
			{showMenu ? <MenuContainer /> : null}
			{showEndSessionPrompt ? <EndSessionPromptContainer /> : null}
		</BaseLayout>
	)
}

export default DefaultLayout
