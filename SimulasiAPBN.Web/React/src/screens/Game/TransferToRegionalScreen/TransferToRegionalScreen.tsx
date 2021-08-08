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

import "./TransferToRegionalScreen.scoped.css"
import BudgetType from "@core/enums/BudgetType"

const BudgetTypeName = "Transfer ke Daerah dan Dana Desa"
enum Slide {
	InformationSlide,
	AllocationSlide,
}
type TransferToRegionalScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const TransferToRegionalScreen: React.FC<TransferToRegionalScreenProps> = () => {
	const dispatch = useDispatch()
	const htmlDocument = useHTMLDocument()

	const expenditures = useSelector<RootState, SimulationExpenditure>(state => state.simulation.expenditures)
	const totalExpenditure = useSelector<RootState, SimulationTotalExpenditure>(
		state => state.simulation.totalExpenditure
	)

	const [allExpenditures, setAllExpenditures] = useState<SimulationStateExpenditure[]>(expenditures.all)
	const [transferToRegionalExpenditures, setTransferToRegionalExpenditures] = useState<SimulationStateExpenditure[]>(
		expenditures.transferToRegional
	)
	const [slide, setSlide] = useState<Slide>(Slide.InformationSlide)
	const [totalAllExpenditures, setTotalAllExpenditures] = useState<number>(totalExpenditure.all)
	const [totalTransferToRegionalExpenditure, setTotalTransferToRegionalExpenditure] = useState<number>(
		totalExpenditure.transferToRegional
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
		const newTotalAllExpenditure = totalAllExpenditures + (totalExpenditure - totalTransferToRegionalExpenditure)

		setSlide(Slide.InformationSlide)
		setAllExpenditures(newAllExpenditures)
		setTransferToRegionalExpenditures(expenditures)
		setTotalAllExpenditures(newTotalAllExpenditure)
		setTotalTransferToRegionalExpenditure(totalExpenditure)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadTransferToRegionalData = () => {
		if (transferToRegionalExpenditures.length <= 0) {
			return
		}
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		htmlDocument.setTitle(BudgetTypeName)

		dispatch(setLoading(`Menyiapkan data ${BudgetTypeName}...`))
		loadTransferToRegionalData()
		dispatch(setLoading(false))
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		loadTransferToRegionalData()
	}, [transferToRegionalExpenditures])
	/* End of Effects */

	const Content: React.FC = () => {
		switch (slide) {
			case Slide.AllocationSlide:
				return (
					<ExpenditureAllocationSlide
						budgetType={BudgetType.TransferToRegionalExpenditure}
						expenditures={transferToRegionalExpenditures}
						totalExpenditure={totalTransferToRegionalExpenditure}
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
						totalTypedExpenditure={totalTransferToRegionalExpenditure}
						onSwitchToAllocationSlide={onSwitchToAllocationSlide}
					>
						<strong>{BudgetTypeName}</strong> digunakan untuk mendanai beberapa urusan pemerintahan yang
						menjadi kewenangan daerah berdasarkan ketentuan yang diatur dalam peraturan perundang-undangan.
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

export default TransferToRegionalScreen
