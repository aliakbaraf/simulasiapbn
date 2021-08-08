/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useEffect, useRef, useState } from "react"
import { useHistory } from "react-router-dom"
import { useDispatch, useSelector } from "react-redux"
import { toast } from "react-hot-toast"
import { FiHelpCircle, FiInfo, FiSave } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import useFormatter from "@common/hooks/formatter"
import { useService } from "@common/hooks/services"
import ComponentProps from "@common/libraries/component"
import { toCurrency } from "@common/libraries/locale"
import Button from "@components/Button"
import EconomicMacroItem from "@components/EconomicMacroItem"
import BudgetType from "@core/enums/BudgetType"
import SimulationSession from "@core/models/SimulationSession"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import StateBudget from "@core/models/StateBudget"
import RootState from "@flow/types"
import { setNetworkLoading } from "@flow/slices/common"
import {
	setClientSimulation,
	setEconomicMacroClientProgress,
} from "@flow/slices/simulation"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationSlide.css"
import "./styles/AllocationSlide.scoped.css"

type EconomicMacroSlideProps = ComponentProps<
	{
		budgetType: BudgetType
		economicMacros: SimulationEconomicMacro[]
		totalEconomicMacro: number
		onSwitchToInformationSlide: (economicMacros: SimulationEconomicMacro[], totalEconomicMacro: number) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const EconomicMacroSlide: React.FC<EconomicMacroSlideProps> = props => {
	let budgetTypeName = ""
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const service = useService()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const stateBudget = useSelector<RootState, StateBudget>(state => state.simulation.stateBudget)

	const totalEconomicMacroRefs = {
		formattedValue: useRef<HTMLSpanElement>(null),
		value: useRef<HTMLSpanElement>(null),
	}
	const economicMacroRefs: React.RefObject<HTMLInputElement>[] = []
	const [typedEconomicMacros] = useState<SimulationEconomicMacro[]>(props.economicMacros)
	const [typedTotalEconomicMacro] = useState<number>(props.totalEconomicMacro)

	/* Event Handlers */
	const onSwitchToInformationSlide: React.MouseEventHandler = () => {
		const [newEconomicMacros, newTotalEconomicMacro] = getCurrentTypedEconomicMacro()
		props.onSwitchToInformationSlide(newEconomicMacros, newTotalEconomicMacro)
	}

	const onEconomicMacroItemValueChange = (previousValue: number, value: number,
		threshold: number, thresholdValue:number) => {
		if (!totalEconomicMacroRefs.value.current) {
			return
		}

		const changeValue = ((value - previousValue) * thresholdValue) / threshold / 1000
		const currentTotalEconomicMacro = parseFloat(totalEconomicMacroRefs.value.current.innerText)
		
		onTotalEconomicMacroChange(currentTotalEconomicMacro + changeValue)
	}

	const onTotalEconomicMacroChange = (newTotalEconomicMacro: number) => {
		if (!totalEconomicMacroRefs.value.current || !totalEconomicMacroRefs.formattedValue.current) {
			return
		}
		totalEconomicMacroRefs.value.current.innerText = newTotalEconomicMacro.toString()
		totalEconomicMacroRefs.formattedValue.current.innerText = `${toCurrency(newTotalEconomicMacro)} T`

	}

	const onSaveButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		saveEconomicMacros().then()
	}
	/* End of Event Handlers */

	/* Functions */
	const loadData = () => {
		if (props.budgetType === BudgetType.EconomicMacro) {
			budgetTypeName = "Ekonomi Makro"
		}
	}

	const getCurrentTypedEconomicMacro: () => [SimulationEconomicMacro[], number] = () => {
		const newEconomicMacros: SimulationEconomicMacro[] = []
		for (const [index, economicMacro] of typedEconomicMacros.entries()) {
			const newTotalAllocationString = economicMacroRefs[index]?.current?.value
			if (!newTotalAllocationString) {
				continue
			}

			const newTotalAllocation = parseFloat(newTotalAllocationString)
			newEconomicMacros.push({ ...economicMacro, used_value: newTotalAllocation })
		}

		const newTotalEconomicMacroString = totalEconomicMacroRefs.value.current?.innerText
		if (!newTotalEconomicMacroString) {
			return [newEconomicMacros, typedTotalEconomicMacro]
		}

		const newTotalEconomicMacro = parseFloat(newTotalEconomicMacroString)
		return [newEconomicMacros, newTotalEconomicMacro]
	}

	const saveEconomicMacros = async () => {
		if (
			typeof session === "undefined" ||
			session === null ||
			economicMacroRefs.length <= 0 ||
			economicMacroRefs.length !== typedEconomicMacros.length
		) {
			console.log("A", economicMacroRefs.length, typedEconomicMacros.length)
			return
		}

		try {
			const [newEconomicMacros, newTotalEconomicMacro] = getCurrentTypedEconomicMacro()

			if (newEconomicMacros.length > 0) {
				console.log("B")
				for (const newEconomicMacro of newEconomicMacros) {
					if (newEconomicMacro.used_value <= 0) {
						handleError.showToast(
							`Asumsi ${newEconomicMacro.economic_macro.name} ` + "minimal 0."
						)
						return
					}
				}

				dispatch(setNetworkLoading())

				const clientSimulation = await service.Simulation.updateEconomicMacro(session.id, newEconomicMacros, newTotalEconomicMacro)
				dispatch(setClientSimulation(clientSimulation))
			}
			if (props.budgetType === BudgetType.EconomicMacro) {
				dispatch(setEconomicMacroClientProgress(true))
			}
			toast.success(`Asumsi ${budgetTypeName} berhasil disimpan.`)
			history.push(GamesRoutes.IncomeScreen)
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setNetworkLoading(false))
		}
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		loadData()
	}, [])

	useEffect(() => {
		onTotalEconomicMacroChange(typedTotalEconomicMacro)
	}, [totalEconomicMacroRefs.formattedValue.current, totalEconomicMacroRefs.value.current])
	/* End of Effects */

	return (
		<div className="content">
			<div className="allocation-content">
				<div className="header">
					<span className="title">{budgetTypeName}</span>
					<span className="help-info">
						Ketuk tanda&nbsp;
						<FiHelpCircle className="inline-flex text-primary dark:text-secondary" />
						&nbsp;untuk mengetahui informasi ekonomi makro pada{" "}
						{formatter.getStateBudgetName(stateBudget)}.
					</span>
				</div>
				<div className="allocation-list">
					{typedEconomicMacros.map((economicMacro, index) => {
						const ref = useRef<HTMLInputElement>(null)

						useEffect(() => {
							economicMacroRefs[index] = ref
						})

						return (
							<EconomicMacroItem
								key={index}
								budgetTypeName={budgetTypeName}
								simulationEconomicMacro={economicMacro}
								stateBudget={stateBudget}
								ref={ref}
								onValueChange={onEconomicMacroItemValueChange}
							/>
						)
					})}
				</div>
			</div>
			<div className="bar bottom-bar">
				<div className="bar-left">
					<Button
						color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
						disabled={networkLoading}
						icon={FiInfo}
						size={SizeVariant.Small}
						onClick={onSwitchToInformationSlide}
					/>
					<span className="hidden" ref={totalEconomicMacroRefs.value}>
						0
					</span>
					<span className="ml-2 font-bold" ref={totalEconomicMacroRefs.formattedValue}>
						{toCurrency(0)} T
					</span>
				</div>
				<div className="bar-right">
					<Button
						color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
						disabled={networkLoading}
						icon={FiSave}
						rightIcon
						size={SizeVariant.Small}
						onClick={onSaveButtonClicked}
					>
						SIMPAN
					</Button>
				</div>
			</div>
		</div>
	)
}

export default React.memo<EconomicMacroSlideProps>(EconomicMacroSlide)
