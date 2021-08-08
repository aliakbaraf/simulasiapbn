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
import ExpenditureAllocationItem from "@components/ExpenditureAllocationItem"
import BudgetType from "@core/enums/BudgetType"
import SimulationSession from "@core/models/SimulationSession"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import StateBudget from "@core/models/StateBudget"
import RootState from "@flow/types"
import { setNetworkLoading } from "@flow/slices/common"
import {
	setCentralGovernmentClientProgress,
	setClientSimulation,
	setTransferToRegionalClientProgress,
} from "@flow/slices/simulation"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationSlide.css"
import "./styles/AllocationSlide.scoped.css"

type ExpenditureAllocationSlideProps = ComponentProps<
	{
		budgetType: BudgetType
		expenditures: SimulationStateExpenditure[]
		totalExpenditure: number
		onSwitchToInformationSlide: (expenditures: SimulationStateExpenditure[], totalExpenditure: number) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const ExpenditureAllocationSlide: React.FC<ExpenditureAllocationSlideProps> = props => {
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

	const totalExpenditureRefs = {
		formattedValue: useRef<HTMLSpanElement>(null),
		value: useRef<HTMLSpanElement>(null),
	}
	const expenditureRefs: React.RefObject<HTMLInputElement>[] = []
	const [typedExpenditures] = useState<SimulationStateExpenditure[]>(props.expenditures)
	const [typedTotalExpenditure] = useState<number>(props.totalExpenditure)

	/* Event Handlers */
	const onSwitchToInformationSlide: React.MouseEventHandler = () => {
		const [newExpenditures, newTotalExpenditure] = getCurrentTypedExpenditure()
		props.onSwitchToInformationSlide(newExpenditures, newTotalExpenditure)
	}

	const onExpenditureAllocationItemValueChange = (previousValue: number, value: number) => {
		if (!totalExpenditureRefs.value.current) {
			return
		}

		const changeValue = value - previousValue
		const currentTotalExpenditure = parseFloat(totalExpenditureRefs.value.current.innerText)
		onTotalExpenditureChange(currentTotalExpenditure + changeValue)
	}

	const onTotalExpenditureChange = (newTotalExpenditure: number) => {
		if (!totalExpenditureRefs.value.current || !totalExpenditureRefs.formattedValue.current) {
			return
		}

		totalExpenditureRefs.value.current.innerText = newTotalExpenditure.toString()
		totalExpenditureRefs.formattedValue.current.innerText = `${toCurrency(newTotalExpenditure)} T`
	}

	const onSaveButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		saveExpenditures().then()
	}
	/* End of Event Handlers */

	/* Functions */
	const loadData = () => {
		if (props.budgetType === BudgetType.CentralGovernmentExpenditure) {
			budgetTypeName = "Belanja Pemerintah Pusat"
		}
		if (props.budgetType === BudgetType.TransferToRegionalExpenditure) {
			budgetTypeName = "Transfer ke Daerah dan Dana Desa"
		}
	}

	const getCurrentTypedExpenditure: () => [SimulationStateExpenditure[], number] = () => {
		const newExpenditures: SimulationStateExpenditure[] = []
		for (const [index, expenditure] of typedExpenditures.entries()) {
			const newTotalAllocationString = expenditureRefs[index]?.current?.value
			if (!newTotalAllocationString) {
				continue
			}

			const newTotalAllocation = parseFloat(newTotalAllocationString)
			newExpenditures.push({ ...expenditure, total_allocation: newTotalAllocation })
		}

		const newTotalExpenditureString = totalExpenditureRefs.value.current?.innerText
		if (!newTotalExpenditureString) {
			return [newExpenditures, typedTotalExpenditure]
		}

		const newTotalExpenditure = parseFloat(newTotalExpenditureString)
		return [newExpenditures, newTotalExpenditure]
	}

	const saveExpenditures = async () => {
		if (
			typeof session === "undefined" ||
			session === null ||
			expenditureRefs.length <= 0 ||
			expenditureRefs.length !== typedExpenditures.length
		) {
			console.log("A", expenditureRefs.length, typedExpenditures.length)
			return
		}

		try {
			const [newExpenditures] = getCurrentTypedExpenditure()

			if (newExpenditures.length > 0) {
				console.log("B")
				for (const newExpenditure of newExpenditures) {
					if (newExpenditure.total_allocation <= 0) {
						handleError.showToast(
							`Anggaran ${newExpenditure.state_expenditure.budget.function} ` + "harus lebih dari Rp0 T."
						)
						return
					}
				}

				dispatch(setNetworkLoading())

				const clientSimulation = await service.Simulation.updateStateExpenditure(session.id, newExpenditures)
				dispatch(setClientSimulation(clientSimulation))
			}

			if (props.budgetType === BudgetType.CentralGovernmentExpenditure) {
				dispatch(setCentralGovernmentClientProgress(true))
			}
			if (props.budgetType === BudgetType.TransferToRegionalExpenditure) {
				dispatch(setTransferToRegionalClientProgress(true))
			}
			toast.success(`Anggaran ${budgetTypeName} berhasil disimpan.`)
			history.push(GamesRoutes.AllocationScreen)
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
		onTotalExpenditureChange(typedTotalExpenditure)
	}, [totalExpenditureRefs.formattedValue.current, totalExpenditureRefs.value.current])
	/* End of Effects */

	return (
		<div className="content">
			<div className="allocation-content">
				<div className="header">
					<span className="title">{budgetTypeName}</span>
					<span className="help-info">
						Ketuk tanda&nbsp;
						<FiHelpCircle className="inline-flex text-primary dark:text-secondary" />
						&nbsp;untuk mengetahui informasi fungsi anggaran dan alokasi fungsi anggaran pada{" "}
						{formatter.getStateBudgetName(stateBudget)}.
					</span>
					<span className="currency-info">Angka-angka berikut dalam triliun Rupiah.</span>
				</div>
				<div className="allocation-list">
					{typedExpenditures.map((expenditure, index) => {
						const ref = useRef<HTMLInputElement>(null)

						useEffect(() => {
							expenditureRefs[index] = ref
						})

						return (
							<ExpenditureAllocationItem
								key={index}
								budgetTypeName={budgetTypeName}
								expenditure={expenditure}
								stateBudget={stateBudget}
								ref={ref}
								onValueChange={onExpenditureAllocationItemValueChange}
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
					<span className="hidden" ref={totalExpenditureRefs.value}>
						0
					</span>
					<span className="ml-2 font-bold" ref={totalExpenditureRefs.formattedValue}>
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

export default React.memo<ExpenditureAllocationSlideProps>(ExpenditureAllocationSlide)
