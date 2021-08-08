/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import { FiArrowLeft } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import Image from "@components/Image"
import SimulationSession from "@core/models/SimulationSession"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import EducationRoutes from "@screens/Education/routes"
import GamesRoutes from "@screens/Game/routes"

import FunDistributionDarkImage from "@assets/webp/fun-distribution-dark.webp"
import FunDistributionDarkFallbackImage from "@assets/png/fun-distribution-dark.png"
import FunAllocationDarkImage from "@assets/webp/fun-allocation-dark.webp"
import FunAllocationDarkFallbackImage from "@assets/png/fun-allocation-dark.png"
import FunStabilityDarkImage from "@assets/webp/fun-stability-dark.webp"
import FunStabilityDarkFallbackImage from "@assets/png/fun-stability-dark.png"
import FunDistributionLightImage from "@assets/webp/fun-distribution-light.webp"
import FunDistributionLightFallbackImage from "@assets/png/fun-distribution-light.png"
import FunAllocationLightImage from "@assets/webp/fun-allocation-light.webp"
import FunAllocationLightFallbackImage from "@assets/png/fun-allocation-light.png"
import FunStabilityLightImage from "@assets/webp/fun-stability-light.webp"
import FunStabilityLightFallbackImage from "@assets/png/fun-stability-light.png"
import "./FunctionsScreen.scoped.css"

type FunctionsScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const FunctionsScreen: React.FC<FunctionsScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const [funDistributionImage, setFunDistributionImage] = useState<string>("")
	const [funDistributionFallbackImage, setFunDistributionFallbackImage] = useState<string>("")
	const [funAllocationImage, setFunAllocationImage] = useState<string>("")
	const [funAllocationFallbackImage, setFunAllocationFallbackImage] = useState<string>("")
	const [funStabilityImage, setFunStabilityImage] = useState<string>("")
	const [funStabilityFallbackImage, setFunStabilityFallbackImage] = useState<string>("")

	const onBackButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.BasicTheoryScreen)
	}

	const onNextButtonClicked: React.MouseEventHandler = () => {
		history.push(GamesRoutes.IndexScreen)
	}

	useEffect(() => {
		htmlDocument.setTitle("Fungsi APBN")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		setFunDistributionImage(darkMode ? FunDistributionDarkImage : FunDistributionLightImage)
		setFunDistributionFallbackImage(darkMode ? FunDistributionDarkFallbackImage : FunDistributionLightFallbackImage)
		setFunAllocationImage(darkMode ? FunAllocationDarkImage : FunAllocationLightImage)
		setFunAllocationFallbackImage(darkMode ? FunAllocationDarkFallbackImage : FunAllocationLightFallbackImage)
		setFunStabilityImage(darkMode ? FunStabilityDarkImage : FunStabilityLightImage)
		setFunStabilityFallbackImage(darkMode ? FunStabilityDarkFallbackImage : FunStabilityLightFallbackImage)
	}, [darkMode])

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Education}>
			<div className="content-wrapper">
				<div className="banner">
					<p>
						APBN adalah instrumen atau alat untuk mencapai tujuan bernegara. APBN memiliki tiga fungsi utama
						yaitu fungsi distribusi, fungsi alokasi, dan fungsi stabilitas.
					</p>
				</div>
				<div className="content">
					<div className="card-wrapper">
						<div className="function-card">
							<Image
								className="function-card-image"
								source={funDistributionImage}
								fallback={funDistributionFallbackImage}
								alt="Fungsi Distribusi"
							/>
							<header className="function-card-header">
								<span className="function-title">Fungsi Distribusi</span>
							</header>
						</div>
					</div>
					<div className="card-wrapper">
						<div className="function-card">
							<Image
								className="function-card-image"
								source={funAllocationImage}
								fallback={funAllocationFallbackImage}
								alt="Fungsi Alokasi"
							/>
							<header className="function-card-header">
								<span className="function-title">Fungsi Alokasi</span>
							</header>
						</div>
					</div>
					<div className="card-wrapper">
						<div className="function-card">
							<Image
								className="function-card-image"
								source={funStabilityImage}
								fallback={funStabilityFallbackImage}
								alt="Fungsi Stabilitas"
							/>
							<header className="function-card-header">
								<span className="function-title">Fungsi Stabilitas</span>
							</header>
						</div>
					</div>
				</div>
			</div>
			<div className="click-to-action">
				<Button color={ColorVariant.PrimaryAlternate} icon={FiArrowLeft} onClick={onBackButtonClicked} />
				<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
					{typeof session === "undefined" || session === null ? "Mulai Simulasi" : "Ke Simulasi"}
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default FunctionsScreen
