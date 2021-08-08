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
import ExpenditureInformationSlide from "@components/ExpenditureInformationSlide"
import ExpenditureAllocationSlide from "@components/ExpenditureAllocationSlide"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import { setLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import { SimulationExpenditure, SimulationTotalExpenditure } from "@flow/types/simulation"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"

import "./CentralGovernmentScreen.scoped.css"
import BudgetType from "@core/enums/BudgetType"

const BudgetTypeName = "Belanja Pemerintah Pusat"
enum Slide {
	InformationSlide,
	AllocationSlide,
}
type CentralGovernmentScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const CentralGovernmentScreen: React.FC<CentralGovernmentScreenProps> = () => {
	const dispatch = useDispatch()
	const htmlDocument = useHTMLDocument()

	const expenditures = useSelector<RootState, SimulationExpenditure>(state => state.simulation.expenditures)
	const totalExpenditure = useSelector<RootState, SimulationTotalExpenditure>(
		state => state.simulation.totalExpenditure
	)

	const [allExpenditures, setAllExpenditures] = useState<SimulationStateExpenditure[]>(expenditures.all)
	const [centralGovernmentExpenditures, setCentralGovernmentExpenditures] = useState<SimulationStateExpenditure[]>(
		expenditures.centralGovernment
	)
	const [functions, setFunctions] = useState<string[]>([])
	const [slide, setSlide] = useState<Slide>(Slide.InformationSlide)
	const [totalAllExpenditures, setTotalAllExpenditures] = useState<number>(totalExpenditure.all)
	const [totalCentralGovernmentExpenditure, setTotalCentralGovernmentExpenditure] = useState<number>(
		totalExpenditure.centralGovernment
	)

	/* Event Handlers */
	const onSwitchToAllocationSlide = () => {
		setSlide(Slide.AllocationSlide)
	}

	const onSwitchToInformationSlide = (expenditures: SimulationStateExpenditure[], totalExpenditure: number) => {
		const newAllExpenditures = allExpenditures.map(expenditure => {
			const newExpenditure = expenditures.find(entity => entity.id === expenditure.id)
			return newExpenditure ? newExpenditure : expenditure
		})
		const newTotalAllExpenditure = totalAllExpenditures + (totalExpenditure - totalCentralGovernmentExpenditure)

		setSlide(Slide.InformationSlide)
		setAllExpenditures(newAllExpenditures)
		setCentralGovernmentExpenditures(expenditures)
		setTotalAllExpenditures(newTotalAllExpenditure)
		setTotalCentralGovernmentExpenditure(totalExpenditure)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadCentralGovernmentData = () => {
		if (centralGovernmentExpenditures.length <= 0) {
			return
		}

		const centralGovernmentFunctions = centralGovernmentExpenditures.map((expenditure, index) => {
			const budgetFunction = expenditure.state_expenditure.budget.function
			return index !== expenditures.centralGovernment.length - 1
				? `fungsi ${budgetFunction}`
				: `dan fungsi ${budgetFunction}`
		})

		setFunctions(centralGovernmentFunctions)
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		htmlDocument.setTitle(BudgetTypeName)

		dispatch(setLoading(`Menyiapkan data ${BudgetTypeName}...`))
		loadCentralGovernmentData()
		dispatch(setLoading(false))
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		loadCentralGovernmentData()
	}, [centralGovernmentExpenditures])
	/* End of Effects */

	const Content: React.FC = () => {
		switch (slide) {
			case Slide.AllocationSlide:
				return (
					<ExpenditureAllocationSlide
						budgetType={BudgetType.CentralGovernmentExpenditure}
						expenditures={centralGovernmentExpenditures}
						totalExpenditure={totalCentralGovernmentExpenditure}
						onSwitchToInformationSlide={onSwitchToInformationSlide}
					/>
				)
			case Slide.InformationSlide:
			default:
				return (
					<ExpenditureInformationSlide
						budgetTypeName={BudgetTypeName}
						expenditures={allExpenditures}
						totalExpenditure={totalAllExpenditures}
						totalTypedExpenditure={totalCentralGovernmentExpenditure}
						onSwitchToAllocationSlide={onSwitchToAllocationSlide}
					>
						<strong>{BudgetTypeName}</strong> menurut fungsi adalah belanja Pemerintah Pusat yang digunakan
						untuk menjalankan {functions.join(", ")}.
					</ExpenditureInformationSlide>
				)
		}
	}

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<Content />
		</DefaultLayout>
	)
}

export default CentralGovernmentScreen
