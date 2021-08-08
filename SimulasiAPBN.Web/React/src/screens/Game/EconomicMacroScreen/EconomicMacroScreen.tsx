/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useDispatch, useSelector } from "react-redux"

import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import EconomicMacroInformationSlide from "@components/EconomicMacroInformationSlide"
import EconomicMacroSlide from "@components/EconomicMacroSlide"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import { setLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import { SimulationEconomic, SimulationTotalEconomic } from "@flow/types/simulation"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"

import BudgetType from "@core/enums/BudgetType"

import "./EconomicMacroScreen.scoped.css"

const BudgetTypeName = "Ekonomi Makro"

enum Slide {
	InformationSlide,
	AllocationSlide,
}

type EconomicMacroScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const EconomicMacroScreen: React.FC<EconomicMacroScreenProps> = () => {
	const dispatch = useDispatch()
	const htmlDocument = useHTMLDocument()

	const economicMacros = useSelector<RootState, SimulationEconomic>(state => state.simulation.economicMacros)
	const totalEconomic = useSelector<RootState, SimulationTotalEconomic>(
		state => state.simulation.totalEconomicMacro
	)
	const [allEconomicMacros, setAllEconomicMacros] = useState<SimulationEconomicMacro[]>(economicMacros.all)
	const [economicMacroExpenditures, setEconomicMacroExpenditures] = useState<SimulationEconomicMacro[]>(
		economicMacros.economicMacro
	)

	const [totalEconomicExpenditure, setTotalEconomicExpenditure] = useState<number>(
		totalEconomic.economicMacro
	)
	const [functions, setFunctions] = useState<string[]>([])
	const [slide, setSlide] = useState<Slide>(Slide.InformationSlide)
		

	/* Event Handlers */
	const onSwitchToAllocationSlide = () => {
		setSlide(Slide.AllocationSlide)
	}

	const onSwitchToInformationSlide = (economicMacros: SimulationEconomicMacro[]) => {
		setSlide(Slide.InformationSlide)
		setAllEconomicMacros(economicMacros)
		setEconomicMacroExpenditures(economicMacros)
		setTotalEconomicExpenditure(totalEconomicExpenditure)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadEconomicMacroData = () => {
		if (economicMacroExpenditures.length <= 0) {
			return
		}

		const economicMacroFunctions = economicMacroExpenditures.map((economicMacro, index) => {
			const economicFunction = economicMacro.economic_macro.name
			return index !== economicMacros.economicMacro.length - 1
				? `${economicFunction}`
				: `dan ${economicFunction}`
		})

		setFunctions(economicMacroFunctions)
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		htmlDocument.setTitle(BudgetTypeName)

		dispatch(setLoading(`Menyiapkan data ${BudgetTypeName}...`))
		loadEconomicMacroData()
		dispatch(setLoading(false))
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		loadEconomicMacroData()
	}, [economicMacroExpenditures])
	/* End of Effects */

	const Content: React.FC = () => {
		switch (slide) {
			case Slide.AllocationSlide:
				return (
					<EconomicMacroSlide
						budgetType={BudgetType.EconomicMacro}
						economicMacros={economicMacroExpenditures}
						totalEconomicMacro={totalEconomicExpenditure}
						onSwitchToInformationSlide={onSwitchToInformationSlide}
					/>
				)
			case Slide.InformationSlide:
			default:
				return (
					<EconomicMacroInformationSlide
						budgetTypeName={BudgetTypeName}
						economicMacros={allEconomicMacros}
						totalEconomicMacro={totalEconomicExpenditure}
						onSwitchToAllocationSlide={onSwitchToAllocationSlide}
					>
						<strong>{BudgetTypeName}</strong> adalah parameter yang menyusun
						pendapatan negara yang terdiri dari {functions.join(", ")}.
					</EconomicMacroInformationSlide>
				)
		}
	}

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<Content />
		</DefaultLayout>
	)
}
export default EconomicMacroScreen
