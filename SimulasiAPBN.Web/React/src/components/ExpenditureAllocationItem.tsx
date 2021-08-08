/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import Slider from "react-rangeslider"
import { FiHelpCircle } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import ComponentProps from "@common/libraries/component"
import useFormatter from "@common/hooks/formatter"
import { toCurrency } from "@common/libraries/locale"
import Button from "@components/Button"
import Drawer from "@components/Drawer"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import StateBudget from "@core/models/StateBudget"
import RootState from "@flow/types"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationItem.scoped.css"

type ExpenditureAllocationItemProps = ComponentProps<
	{
		budgetTypeName: string
		expenditure: SimulationStateExpenditure
		stateBudget?: StateBudget
		onValueChange: (previousValue: number, value: number) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const renderComponent = (
	props: React.PropsWithChildren<ExpenditureAllocationItemProps>,
	ref: React.ForwardedRef<HTMLInputElement>
) => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const handleError = useErrorHandler(dispatch)

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)

	const { state_expenditure, total_allocation } = props.expenditure
	const budgetDescription = state_expenditure.budget.description || ""
	const budgetTargets = state_expenditure.budget.budget_targets || []
	const maxValue = state_expenditure.total_allocation * state_expenditure.simulation_maximum_multiplier
	const name = state_expenditure.budget.function
	const stateExpenditureAllocations = (state_expenditure.state_expenditure_allocations || []).filter(
		expenditureAllocation => expenditureAllocation.percentage > 0
	)
	const totalAllocation = parseFloat(total_allocation.toFixed(2))

	const [showHelpDrawer, setShowHelpDrawer] = useState<boolean>(false)
	const [value, setValue] = useState<number>(totalAllocation)

	const onShowHelpDrawer = () => {
		setShowHelpDrawer(true)
	}

	const onCloseHelpDrawer = () => {
		setShowHelpDrawer(false)
	}

	const onInputChange: React.ChangeEventHandler<HTMLInputElement> = event => {
		if (networkLoading) {
			return
		}

		const newStringValue = event.target.value
		if (!newStringValue) {
			return
		}
		if (!newStringValue.trim()) {
			return
		}

		const newValue = parseFloat(newStringValue)
		if (isNaN(newValue)) {
			return
		}
		if (newValue < 0) {
			handleError.showToast(`Nilai alokasi ${name} minimum Rp0`)
			return
		}
		if (newValue > maxValue) {
			handleError.showToast(`Nilai alokasi ${name} maksimum ${toCurrency(maxValue)} T`)
			return
		}

		if (typeof props.onValueChange === "function") {
			props.onValueChange(value, newValue)
		}

		setValue(newValue)
	}

	const onSliderChange = (newValue: number) => {
		if (networkLoading) {
			return
		}

		if (typeof props.onValueChange === "function") {
			props.onValueChange(value, newValue)
		}
		setValue(parseFloat(newValue.toFixed(2)))
	}

	const HelpDrawer: React.FC = () => (
		<Drawer title={name} onCloseDrawer={onCloseHelpDrawer}>
			<p className="font-semibold mb-3">
				Alokasi fungsi anggaran {name} pada {props.budgetTypeName}{" "}
				{formatter.getStateBudgetName(props.stateBudget)} adalah sebesar{" "}
				{toCurrency(state_expenditure.total_allocation)} T.
			</p>
			{stateExpenditureAllocations.length <= 0 ? null : (
				<div className="mb-3">
					<span>
						Sesuai data {formatter.getStateBudgetName(props.stateBudget)}, fungsi anggaran {name} memiliki
						alokasi-alokasi sebagai berikut.
					</span>
					<ol className="list-disc list-outside pl-5">
						{stateExpenditureAllocations.map((expenditureAllocation, index) => (
							<li key={index}>
								Alokasi {expenditureAllocation.allocation.name}
								{expenditureAllocation.allocation.is_mandatory ? " (bersifat wajib)" : ""} sebesar{" "}
								{expenditureAllocation.percentage}% dari total alokasi fungsi anggaran.
							</li>
						))}
					</ol>
				</div>
			)}
			{!budgetDescription ? null : <p className="mb-3">{budgetDescription}</p>}
			{budgetTargets.length <= 0 ? null : (
				<div>
					<span>Sasaran:</span>
					<ol className="list-decimal list-outside pl-6">
						{budgetTargets.map((target, index) => {
							let endString = ";"
							if (index === budgetTargets.length - 1) {
								endString = "."
							} else if (index === budgetTargets.length - 2) {
								endString = "; dan"
							}
							return (
								<li key={index}>
									{target.description}
									{endString}
								</li>
							)
						})}
					</ol>
				</div>
			)}
		</Drawer>
	)

	return (
		<div className="allocation-item">
			<div className="header">
				<span>{name}</span>&nbsp;
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					icon={FiHelpCircle}
					noOutline
					size={SizeVariant.Small}
					onClick={onShowHelpDrawer}
				/>
				<span />
			</div>
			<div className="body">
				<div className={`slider-container${networkLoading ? " disabled" : ""}`}>
					<Slider
						className="slider"
						min={0}
						max={maxValue}
						step={0.01}
						tooltip={false}
						value={value}
						onChange={onSliderChange}
					/>
				</div>
				<div className="input-container">
					<input
						className="input"
						ref={ref}
						disabled={networkLoading}
						type="number"
						step={0.01}
						value={value}
						onChange={onInputChange}
					/>
				</div>
			</div>
			{showHelpDrawer ? <HelpDrawer /> : null}
		</div>
	)
}

const ExpenditureAllocationItem = React.forwardRef<HTMLInputElement, ExpenditureAllocationItemProps>(renderComponent)

export default React.memo(ExpenditureAllocationItem)
