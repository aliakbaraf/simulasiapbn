/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useEffect } from "react"
import { useHistory } from "react-router-dom"
import { useSelector } from "react-redux"

import ColorVariant from "@common/enum/ColorVariant"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import { toCurrency } from "@common/libraries/locale"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import RootState from "@flow/types"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/InformationSlide.scoped.css"

type EconomicMacroInformationSlideProps = ComponentProps<
	{
		budgetTypeName: string
		economicMacros: SimulationEconomicMacro[]
		totalEconomicMacro: number
		onSwitchToAllocationSlide: () => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const EconomicMacroInformationSlide: React.FC<EconomicMacroInformationSlideProps> = props => {
	const { budgetTypeName, economicMacros, totalEconomicMacro, onSwitchToAllocationSlide } = props
	const history = useHistory()
	
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	
	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.IncomeScreen)
	}

	const loadData = () => {
		if (economicMacros.length <= 0) {
			return
		}
	}

	useEffect(() => {
		loadData()
	}, [])

	useEffect(() => {
		loadData()
	}, [economicMacros])

	return (
		<div className="content information-content">
			<p className="header">{budgetTypeName}</p>
			<p className="description">{props.children}</p>
			<div className="expenditure-information">
				<span className="value">{toCurrency(totalEconomicMacro)} T</span>
				<span className="description font-bold">Total Pendapatan Negara</span>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onBackButtonClicked}
				>
					Kembali
				</Button>
				&nbsp;&nbsp;&nbsp;&nbsp;
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					onClick={onSwitchToAllocationSlide} 
				>
					Atur Asumsi
				</Button>
			</div>
		</div>
	)
}

export default EconomicMacroInformationSlide
