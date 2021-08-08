/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useSelector } from "react-redux"
import { useHistory } from "react-router-dom"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import RootRoutes from "@screens/routes"

import "./LandingScreen.scoped.css"

const ApplicationLogo = React.lazy(() => import("@components/ApplicationLogo"))

type LandingScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const LandingScreen: React.FC<LandingScreenProps> = () => {
	const history = useHistory()
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const landingText = useSelector<RootState, string[]>(state => state.dynamicMetadata.landingText)

	const onStartButtonClicked: React.MouseEventHandler = () => {
		history.push(RootRoutes.EducationScreen)
	}

	return (
		<DefaultLayout className="screen" screenType={ScreenType.Unknown}>
			<ApplicationLogo />
			<div className="content">
				{landingText.map((text, index) => (
					<h4 className="landing-text" key={index}>
						{text}
					</h4>
				))}
			</div>
			<div className="click-to-action">
				<Button
					onClick={onStartButtonClicked}
					color={darkMode ? ColorVariant.Primary : ColorVariant.Secondary}
					size={SizeVariant.Large}
				>
					<strong>MULAI</strong>
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default LandingScreen
