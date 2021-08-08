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
import { FiInfo, FiSave } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import { useService } from "@common/hooks/services"
import ComponentProps from "@common/libraries/component"
import { toCurrency } from "@common/libraries/locale"
import Button from "@components/Button"
import PolicyAllocationItem from "@components/PolicyAllocationItem"
import SimulationSession from "@core/models/SimulationSession"
import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import { setNetworkLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import { SimulationSpecialPolicy } from "@flow/types/simulation"
import { setSpecialPolicyClientProgress, setClientSimulation } from "@flow/slices/simulation"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/AllocationSlide.css"
import "./styles/AllocationSlide.scoped.css"

type PolicyAllocationSlideProps = ComponentProps<
	{
		policyAllocations: SimulationSpecialPolicyAllocation[]
		specialPolicy?: SimulationSpecialPolicy
		totalPolicyAllocation: number
		onSwitchToInformationSlide: (
			policyAllocations: SimulationSpecialPolicyAllocation[],
			totalPolicyAllocation: number
		) => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const PolicyAllocationSlide: React.FC<PolicyAllocationSlideProps> = props => {
	const dispatch = useDispatch()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const service = useService()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const networkLoading = useSelector<RootState, boolean>(state => state.common.networkLoading)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)

	const totalPolicyAllocationRefs = {
		formattedValue: useRef<HTMLSpanElement>(null),
		value: useRef<HTMLSpanElement>(null),
	}
	const policyAllocationRefs: React.RefObject<HTMLInputElement>[] = []
	const [policyAllocations] = useState<SimulationSpecialPolicyAllocation[]>(props.policyAllocations)
	const [totalPolicyAllocation] = useState<number>(props.totalPolicyAllocation)

	/* Event Handlers */
	const onSwitchToInformationSlide: React.MouseEventHandler = () => {
		const [newPolicyAllocations, newTotalPolicyAllocation] = getCurrentPolicyAllocations()
		props.onSwitchToInformationSlide(newPolicyAllocations, newTotalPolicyAllocation)
	}

	const onPolicyAllocationItemValueChange = (previousValue: number, value: number) => {
		if (!totalPolicyAllocationRefs.value.current) {
			return
		}

		const changeValue = value - previousValue
		const currentTotalExpenditure = parseFloat(totalPolicyAllocationRefs.value.current.innerText)
		onTotalPolicyAllocationChange(currentTotalExpenditure + changeValue)
	}

	const onTotalPolicyAllocationChange = (newTotalExpenditure: number) => {
		if (!totalPolicyAllocationRefs.value.current || !totalPolicyAllocationRefs.formattedValue.current) {
			return
		}

		totalPolicyAllocationRefs.value.current.innerText = newTotalExpenditure.toString()
		totalPolicyAllocationRefs.formattedValue.current.innerText = `${toCurrency(newTotalExpenditure)} T`
	}

	const onSaveButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		savePolicyAllocations().then()
	}
	/* End of Event Handlers */

	/* Functions */
	const getCurrentPolicyAllocations: () => [SimulationSpecialPolicyAllocation[], number] = () => {
		const newPolicyAllocations: SimulationSpecialPolicyAllocation[] = []
		for (const [index, policyAllocation] of policyAllocations.entries()) {
			const newTotalAllocationString = policyAllocationRefs[index]?.current?.value
			if (!newTotalAllocationString) {
				continue
			}

			const newTotalAllocation = parseFloat(newTotalAllocationString)
			newPolicyAllocations.push({ ...policyAllocation, total_allocation: newTotalAllocation })
		}

		const newTotalPolicyAllocationString = totalPolicyAllocationRefs.value.current?.innerText
		if (!newTotalPolicyAllocationString) {
			return [newPolicyAllocations, totalPolicyAllocation]
		}

		const newTotalPolicyAllocation = parseFloat(newTotalPolicyAllocationString)
		return [newPolicyAllocations, newTotalPolicyAllocation]
	}

	const savePolicyAllocations = async () => {
		if (
			typeof session === "undefined" ||
			session === null ||
			policyAllocationRefs.length <= 0 ||
			policyAllocationRefs.length !== policyAllocations.length
		) {
			return
		}

		try {
			const [newPolicyAllocations] = getCurrentPolicyAllocations()

			if (newPolicyAllocations.length > 0) {
				for (const newPolicyAllocation of newPolicyAllocations) {
					if (newPolicyAllocation.total_allocation <= 0) {
						handleError.showToast(
							`Alokasi ${newPolicyAllocation.special_policy_allocation.allocation.name} ` +
								"harus lebih dari Rp0 T."
						)
						return
					}
				}

				dispatch(setNetworkLoading())

				const clientSimulation = await service.Simulation.updateSpecialPolicyAllocation(
					session.id,
					newPolicyAllocations
				)
				dispatch(setClientSimulation(clientSimulation))
			}

			dispatch(setSpecialPolicyClientProgress(props.specialPolicy?.id || "", true))
			toast.success(`Alokasi ${props.specialPolicy?.name || "Kebijakan Khusus"} berhasil disimpan.`)
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
		onTotalPolicyAllocationChange(totalPolicyAllocation)
	}, [totalPolicyAllocationRefs.formattedValue.current, totalPolicyAllocationRefs.value.current])
	/* End of Effects */

	return (
		<div className="content">
			<div className="allocation-content">
				<div className="header">
					<span className="title">{props.specialPolicy?.name || "Kebijakan Khusus"}</span>
					<span className="currency-info">Angka-angka berikut dalam triliun Rupiah.</span>
				</div>
				<div className="allocation-list">
					{policyAllocations.map((policyAllocation, index) => {
						const ref = useRef<HTMLInputElement>(null)

						useEffect(() => {
							policyAllocationRefs[index] = ref
						}, [])

						return (
							<PolicyAllocationItem
								key={index}
								policyAllocation={policyAllocation}
								ref={ref}
								onValueChange={onPolicyAllocationItemValueChange}
							/>
						)
					})}
				</div>
			</div>
			<div className="bar bottom-bar">
				<div className="bar-left">
					<Button
						disabled={networkLoading}
						color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
						icon={FiInfo}
						size={SizeVariant.Small}
						onClick={onSwitchToInformationSlide}
					/>
					<span className="hidden" ref={totalPolicyAllocationRefs.value}>
						0
					</span>
					<span className="ml-2 font-bold" ref={totalPolicyAllocationRefs.formattedValue}>
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

export default React.memo<PolicyAllocationSlideProps>(PolicyAllocationSlide)
