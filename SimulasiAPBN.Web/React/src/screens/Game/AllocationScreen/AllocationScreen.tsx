/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import { FiCheckSquare, FiSquare } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import useFormatter from "@common/hooks/formatter"
import { isAllClientProgressDone } from "@common/libraries/clientSimulationProgress"
import ComponentProps from "@common/libraries/component"
import { toCurrency } from "@common/libraries/locale"
import Button from "@components/Button"
import SimulationSession from "@core/models/SimulationSession"
import RootState from "@flow/types"
import {
	SimulationExpenditure,
	SimulationProgress,
	SimulationSpecialPolicy,
	SimulationTotalExpenditure,
} from "@flow/types/simulation"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import GamesRoutes from "@screens/Game/routes"

import "./AllocationScreen.scoped.css"

type AllocationCardProps = ComponentProps<
	{
		done?: boolean
		onClick: React.MouseEventHandler<HTMLDivElement>
	},
	React.AllHTMLAttributes<HTMLDivElement>
>

const AllocationCard: React.FC<AllocationCardProps> = props => {
	const isDone = props.done === true

	return (
		<div className={`card${isDone ? " card-done" : ""}`} onClick={props.onClick}>
			<div className="card-title">{props.children}</div>
			<div className="card-status">{isDone ? <FiCheckSquare /> : <FiSquare />}</div>
		</div>
	)
}

type AllocationScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const AllocationScreen: React.FC<AllocationScreenProps> = () => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const expenditures = useSelector<RootState, SimulationExpenditure>(state => state.simulation.expenditures)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const progress = useSelector<RootState, SimulationProgress>(state => state.simulation.progress)
	const totalExpenditure = useSelector<RootState, SimulationTotalExpenditure>(
		state => state.simulation.totalExpenditure
	)
	const specialPolicies = useSelector<RootState, SimulationSpecialPolicy[]>(state => state.simulation.specialPolicies)

	/* Event Handlers */
	const onCentralGovernmentButtonClicked: React.MouseEventHandler = () => {
		history.push(GamesRoutes.CentralGovernmentScreen)
	}

	const onTransferToRegionalButtonClicked: React.MouseEventHandler = () => {
		history.push(GamesRoutes.TransferToRegionalScreen)
	}

	const onSpecialPolicyButtonClicked = (specialPolicy: SimulationSpecialPolicy) => {
		const handler: React.MouseEventHandler = () => {
			history.push(`${GamesRoutes.SpecialPolicyScreen}/${specialPolicy.id}`)
		}
		return handler
	}

	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.IncomeScreen)
	}

	const onPreviewResultButtonClicked: React.MouseEventHandler = () => {
		if (!isAllClientProgressDone(progress)) {
			handleError.showToast(
				`Kamu belum menyelesaikan rancangan ${formatter.getStateBudgetName(session?.state_budget)}.`
			)
			return
		}

		history.push(GamesRoutes.SummaryScreen)
	}
	/* End of Event Handlers */

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<div className="header">
				<span className="title">Rancangan {formatter.getStateBudgetName(session?.state_budget)}</span>
				<span className="sub-title">versi {session?.name || ""}</span>
			</div>
			<div className={`card-list${expenditures.all.length <= 0 ? " hidden" : ""}`}>
				{expenditures.centralGovernment.length <= 0 ? null : (
					<AllocationCard onClick={onCentralGovernmentButtonClicked} done={progress.centralGovernmentDone}>
						<span>Belanja Pemerintah Pusat</span>
						{!progress.centralGovernmentDone ? null : (
							<span className="text-sm font-normal p-1 border border-green-500 rounded">
								{toCurrency(totalExpenditure.centralGovernment)} T
							</span>
						)}
					</AllocationCard>
				)}
				{expenditures.transferToRegional.length <= 0 ? null : (
					<AllocationCard onClick={onTransferToRegionalButtonClicked} done={progress.transferToRegionalDone}>
						<span>Transfer ke Daerah dan Dana Desa</span>
						{!progress.transferToRegionalDone ? null : (
							<span className="text-sm font-normal p-1 border border-green-500 rounded">
								{toCurrency(totalExpenditure.transferToRegional)} T
							</span>
						)}
					</AllocationCard>
				)}
			</div>
			<div className={`card-list${specialPolicies.length <= 0 ? " hidden" : ""}`}>
				{specialPolicies.map((specialPolicy, index) => (
					<AllocationCard
						key={index}
						onClick={onSpecialPolicyButtonClicked(specialPolicy)}
						done={progress.specialPoliciesDone[specialPolicy.id]}
					>
						<span className="text-xs font-normal italic p-1 rounded bg-gray-300 dark:bg-gray-900">
							Kebijakan Khusus
						</span>
						<span>{specialPolicy.name}</span>
					</AllocationCard>
				))}
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					size={SizeVariant.Large}
					onClick={onBackButtonClicked}
				>
					Kembali
				</Button>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					size={SizeVariant.Large}
					onClick={onPreviewResultButtonClicked}
				>
					Pratinjau Hasil
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default AllocationScreen
