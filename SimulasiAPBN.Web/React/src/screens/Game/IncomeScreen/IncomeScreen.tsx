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
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import SimulationSession from "@core/models/SimulationSession"
import { isIncomeProgressDone } from "@common/libraries/clientSimulationProgress"
import RootState from "@flow/types"
import {
	SimulationProgress,
	SimulationEconomic,
	SimulationTotalEconomic,
} from "@flow/types/simulation"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import GamesRoutes from "@screens/Game/routes"
import { toCurrency } from "@common/libraries/locale"

import "./IncomeScreen.scoped.css"

type IncomeCardProps = ComponentProps<
	{
		done?: boolean
		onClick: React.MouseEventHandler<HTMLDivElement>
	},
	React.AllHTMLAttributes<HTMLDivElement>
	>

const IncomeCard: React.FC<IncomeCardProps> = props => {
	const isDone = props.done === true

	return (
		<div className={`card${isDone ? " card-done" : ""}`} onClick={props.onClick}>
			<div className="card-title">{props.children}</div>
			<div className="card-status">{isDone ? <FiCheckSquare /> : <FiSquare />}</div>
		</div>
	)
}

type IncomeScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const IncomeScreen: React.FC<IncomeScreenProps> = () => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const history = useHistory()
	const handleError = useErrorHandler(dispatch)

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const progress = useSelector<RootState, SimulationProgress>(state => state.simulation.progress)
	const expenditures = useSelector<RootState, SimulationEconomic>(state => state.simulation.economicMacros)
	const totalExpenditure = useSelector<RootState, SimulationTotalEconomic>(
		state => state.simulation.totalEconomicMacro
	)
	/* Event Handlers */
	
	const onEconomicMacroButtonClicked: React.MouseEventHandler = () => {
		history.push(GamesRoutes.EconomicMacroScreen)
	}

	const onPreviewResultButtonClicked: React.MouseEventHandler = () => {
		if (!isIncomeProgressDone(progress)) {
			handleError.showToast(
				`Kamu belum menyelesaikan rancangan pendapatan negara ${formatter.getStateBudgetName(session?.state_budget)}.`
			)
			return
		}

		history.push(GamesRoutes.SummaryIncomeScreen)
	}
	/* End of Event Handlers */

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<div className="header">
				<span className="title">Rancangan {formatter.getStateBudgetName(session?.state_budget)}</span>
				<span className="sub-title">versi {session?.name || ""}</span>
			</div>
			<div className={`card-list${expenditures.all.length <= 0 ? " hidden" : ""}`}>
				{expenditures.economicMacro.length <= 0 ? null : (
					<IncomeCard onClick={onEconomicMacroButtonClicked} done={progress.economicMacroDone}>
						<span>Pendapatan Negara</span>
						{!progress.economicMacroDone ? null : (
							<span className="text-sm font-normal p-1 border border-green-500 rounded">
								{toCurrency(totalExpenditure.economicMacro)} T
							</span>
						)}
					</IncomeCard>
				)}
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					size={SizeVariant.Large}
					onClick={onPreviewResultButtonClicked}
				>
					Lanjut
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default IncomeScreen
