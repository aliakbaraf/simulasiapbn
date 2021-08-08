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
import Button from "@components/Button"
import Drawer from "@components/Drawer"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import StateBudget from "@core/models/StateBudget"
import RootState from "@flow/types"
import { toCurrency } from "@common/libraries/locale"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationItem.scoped.css"

type EconomicMacroItemProps = ComponentProps<
	{
		budgetTypeName: string
		simulationEconomicMacro: SimulationEconomicMacro
		stateBudget?: StateBudget
		onValueChange: (previousValue: number, value: number, threshold: number, thresholdValue: number) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const renderComponent = (
	props: React.PropsWithChildren<EconomicMacroItemProps>,
	ref: React.ForwardedRef<HTMLInputElement>
) => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const handleError = useErrorHandler(dispatch)

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)

	const { economic_macro, used_value } = props.simulationEconomicMacro
	const maxValue = economic_macro.maximum_value
	const minValue = economic_macro.minimum_value
	const name = economic_macro.name
	const threshold = economic_macro.threshold
	const thresholdValue = economic_macro.threshold_value
	const economicMacros = (economic_macro.economic_macros || [])
	const economicDescription = economic_macro.description || ""
	const [showHelpDrawer, setShowHelpDrawer] = useState<boolean>(false)
	const [value, setValue] = useState<number>(used_value)

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
		if (newValue < minValue) {
			handleError.showToast(`Nilai asumsi ${name} minimum ${minValue}`)
			return
		}
		if (newValue > maxValue) {
			handleError.showToast(`Nilai asumsi ${name} maksimum ${maxValue}`)
			return
		}

		if (typeof props.onValueChange === "function") {
			props.onValueChange(value, newValue, threshold, thresholdValue)
		}

		setValue(newValue)
	}

	const onSliderChange = (newValue: number) => {
		if (networkLoading) {
			return
		}

		if (typeof props.onValueChange === "function") {
			props.onValueChange(value, newValue, threshold, thresholdValue)
		}
		setValue(parseFloat(newValue.toFixed(2)))
	}

	const HelpDrawer: React.FC = () => (
		<Drawer title={name} onCloseDrawer={onCloseHelpDrawer}>
			<p className="font-semibold mb-3">
				Asumsi Ekonomi {name} pada {props.budgetTypeName}{" "}
				{formatter.getStateBudgetName(props.stateBudget)} adalah {" "}
				{props.simulationEconomicMacro.economic_macro.default_value} {" "}
				{props.simulationEconomicMacro.economic_macro.unit_desc}{". "}
				Dan tiap {props.simulationEconomicMacro.economic_macro.threshold}{" "}
				{props.simulationEconomicMacro.economic_macro.unit_desc}{" "}
				memiliki nilai {toCurrency(props.simulationEconomicMacro.economic_macro.threshold_value)} M
			</p>
			{economicMacros.length <= 0 ? null : (
				<div className="mb-3">
					<span>
						Sesuai data {formatter.getStateBudgetName(props.stateBudget)}, Anggaran {name} memiliki
						asumsi ekonomi makro sebagai berikut.
					</span>
					<ol className="list-disc list-outside pl-5">
						{economicMacros.map((economicMacro, index) => (
							<li key={index}>
								{economicMacro.name} {economicMacro.unit_desc}
							</li>
						))}
					</ol>
				</div>
			)}
			{!economicDescription ? null : <p className="mb-3">{economicDescription}</p>}
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
						min={minValue}
						max={maxValue}
						step={threshold}
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
						step={threshold}
						value={value}
						onChange={onInputChange}
					/>
				</div>
			</div>
			{showHelpDrawer ? <HelpDrawer /> : null}
		</div>
	)
}

const EconomicMacroItem = React.forwardRef<HTMLInputElement, EconomicMacroItemProps>(renderComponent)

export default React.memo(EconomicMacroItem)
