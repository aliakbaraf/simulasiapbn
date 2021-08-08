/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useHistory } from "react-router-dom"
import { useSelector } from "react-redux"
import { FiArrowLeft } from "react-icons/fi"
import toast from "react-hot-toast"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import Image from "@components/Image"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import EducationRoutes from "@screens/Education/routes"

import RulesDarkImage from "@assets/webp/rules-dark.webp"
import RulesDarkFallbackImage from "@assets/png/rules-dark.png"
import RulesLightImage from "@assets/webp/rules-light.webp"
import RulesLightFallbackImage from "@assets/png/rules-light.png"
import "./RulesScreen.scoped.css"

type RulesScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const RulesScreen: React.FC<RulesScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const rules = useSelector<RootState, string[]>(state => state.dynamicMetadata.rules)
	const [rulesImage, setRulesImage] = useState<string>("")
	const [rulesFallbackImage, setRulesFallbackImage] = useState<string>("")

	const onBackButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.IntroductionScreen)
	}

	const onNextButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.BasicTheoryScreen)
	}

	useEffect(() => {
		htmlDocument.setTitle("Ketentuan")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		setRulesImage(darkMode ? RulesDarkImage : RulesLightImage)
		setRulesFallbackImage(darkMode ? RulesDarkFallbackImage : RulesLightFallbackImage)
	}, [darkMode])

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Education}>
			<div className="content-wrapper">
				<div className="side-content">
					<div className="image-wrapper">
						<Image
							className="rules-image"
							source={rulesImage}
							fallback={rulesFallbackImage}
							alt="Ketentuan"
						/>
					</div>
					<div className="click-to-action-md">
						<Button
							color={ColorVariant.PrimaryAlternate}
							icon={FiArrowLeft}
							onClick={onBackButtonClicked}
						/>
						<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
							Lanjut
						</Button>
					</div>
				</div>
				<div className="main-content">
					<p className="header">Simulasi Penyusunan APBN</p>
					<p className="description">
						Melalui simulasi ini kamu dapat mengatur kebijakan belanja negara sesuai preferensimu. Dalam
						menyusun APBN terdapat beberapa ketentuan yang harus dicermati.
					</p>
					<div className="rule-list">
						{rules.map((rule, index) => {
							const isEven = index % 2 === 0
							return (
								<div
									className={`rule-item rule-item-${isEven ? "even" : "odd"}`}
									key={index}
									onClick={() => toast(rule)}
								>
									<div className="rule-item-number">{index + 1}</div>
									<span className="rule-item-text">{rule}</span>
								</div>
							)
						})}
					</div>
				</div>
			</div>
			<div className="click-to-action">
				<Button color={ColorVariant.PrimaryAlternate} icon={FiArrowLeft} onClick={onBackButtonClicked} />
				<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
					Lanjut
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default RulesScreen
