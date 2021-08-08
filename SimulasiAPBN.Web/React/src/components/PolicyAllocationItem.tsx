/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import Slider from "react-rangeslider"

import useErrorHandler from "@common/hooks/errorHandler"
import ComponentProps from "@common/libraries/component"
import { toCurrency } from "@common/libraries/locale"
import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import RootState from "@flow/types"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationItem.scoped.css"

type PolicyAllocationItemProps = ComponentProps<
	{
		policyAllocation: SimulationSpecialPolicyAllocation
		onValueChange: (previousValue: number, value: number) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const renderComponent = (
	props: React.PropsWithChildren<PolicyAllocationItemProps>,
	ref: React.ForwardedRef<HTMLInputElement>
) => {
	const dispatch = useDispatch()
	const handleError = useErrorHandler(dispatch)

	const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)

	const { ...simulationSpecialPolicyAllocation } = props.policyAllocation
	const { ...specialPolicyAllocation } = simulationSpecialPolicyAllocation.special_policy_allocation
	const maxValue = specialPolicyAllocation.total_allocation * specialPolicyAllocation.simulation_maximum_multiplier
	const name = specialPolicyAllocation.allocation.name
	const totalAllocation = parseFloat(simulationSpecialPolicyAllocation.total_allocation.toFixed(2))

	const [value, setValue] = useState<number>(totalAllocation)

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

	return (
		<div className="allocation-item">
			<div className="header">
				<span>Alokasi {name}</span>
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
		</div>
	)
}

const PolicyAllocationItem = React.forwardRef<HTMLInputElement, PolicyAllocationItemProps>(renderComponent)

export default React.memo(PolicyAllocationItem)
