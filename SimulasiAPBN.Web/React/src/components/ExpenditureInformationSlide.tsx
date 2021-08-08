/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useEffect, useState } from "react"
import { useHistory } from "react-router-dom"
import { useSelector } from "react-redux"

import ColorVariant from "@common/enum/ColorVariant"
import ComponentProps from "@common/libraries/component"
import { toCurrency, toLocale } from "@common/libraries/locale"
import Button from "@components/Button"
import Allocation from "@core/models/Allocation"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import RootState from "@flow/types"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/InformationSlide.scoped.css"

type AllocationValue = { allocation: Allocation; minValue: number; value: number }
type ExpenditureInformationSlideProps = ComponentProps<
	{
		budgetTypeName: string
		expenditures: SimulationStateExpenditure[]
		totalExpenditure: number
		totalTypedExpenditure: number
		onSwitchToAllocationSlide: () => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const ExpenditureInformationSlide: React.FC<ExpenditureInformationSlideProps> = props => {
	const { budgetTypeName, expenditures, totalExpenditure, totalTypedExpenditure, onSwitchToAllocationSlide } = props
	const history = useHistory()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const mandatoryAllocations = useSelector<RootState, Allocation[]>(state =>
		state.simulation.allocations.filter(allocation => allocation.is_mandatory)
	)

	const [allocationValues, setAllocationValues] = useState<AllocationValue[]>([])

	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.AllocationScreen)
	}

	const loadData = () => {
		if (expenditures.length <= 0) {
			return
		}

		const newAllocationValues = mandatoryAllocations.map(allocation => ({ allocation, minValue: 0, value: 0 }))
		for (const expenditure of expenditures) {
			const stateExpenditureAllocations = expenditure.state_expenditure.state_expenditure_allocations
			const totalAllocation = expenditure.total_allocation
			for (const stateExpenditureAllocation of stateExpenditureAllocations) {
				if (!stateExpenditureAllocation.allocation.is_mandatory) {
					continue
				}

				const allocationValue = totalAllocation * (stateExpenditureAllocation.percentage / 100)
				const allocationValueIndex = newAllocationValues.findIndex(
					value => value.allocation.id === stateExpenditureAllocation.allocation_id
				)
				newAllocationValues[allocationValueIndex].value += allocationValue
			}
		}
		for (const [index, allocationValue] of newAllocationValues.entries()) {
			const mandatoryThreshold = (allocationValue.allocation.mandatory_threshold || 0) / 100
			newAllocationValues[index].minValue = mandatoryThreshold * totalExpenditure
		}

		setAllocationValues(newAllocationValues)
	}

	useEffect(() => {
		loadData()
	}, [])

	useEffect(() => {
		loadData()
	}, [expenditures])

	return (
		<div className="content information-content">
			<p className="header">{budgetTypeName}</p>
			<p className="description">{props.children}</p>
			<div className="expenditure-information">
				<span className="value">{toCurrency(totalExpenditure)} T</span>
				<span className="description font-bold">Total Belanja Negara</span>
			</div>
			<div className="expenditure-information">
				<span className="value">{toCurrency(totalTypedExpenditure)} T</span>
				<span className="description font-bold">Total {budgetTypeName}</span>
			</div>
			{allocationValues.map((allocationValue, index) => (
				<div key={index} className="expenditure-information">
					<span className="value">{toCurrency(allocationValue.value)} T</span>
					<span className="description font-bold">Alokasi {allocationValue.allocation.name}</span>
					<span className="description hidden sm:block">
						minimal {toCurrency(allocationValue.minValue)} T (
						{toLocale(allocationValue.allocation.mandatory_threshold || 0)}% dari Total Belanja Negara)
					</span>
					<span className="description block sm:hidden">
						minimal {toCurrency(allocationValue.minValue)} T
					</span>
					<span className="description block sm:hidden">
						({toLocale(allocationValue.allocation.mandatory_threshold || 0)}% dari Total Belanja Negara)
					</span>
				</div>
			))}
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
					Atur Anggaran
				</Button>
			</div>
		</div>
	)
}

export default ExpenditureInformationSlide
