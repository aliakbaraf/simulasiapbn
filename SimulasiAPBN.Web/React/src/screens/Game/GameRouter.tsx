/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useSelector } from "react-redux"
import { Redirect, Route, Switch } from "react-router-dom"

import SimulationSession from "@core/models/SimulationSession"
import RootState from "@flow/types"

import GamesRoutes from "@screens/Game/routes"

const StartScreen = React.lazy(() => import("@screens/Game/StartScreen"))
const AllocationScreen = React.lazy(() => import("@screens/Game/AllocationScreen"))
const IncomeScreen = React.lazy(() => import("@screens/Game/IncomeScreen"))
const EconomicMacroScreen = React.lazy(() => import("@screens/Game/EconomicMacroScreen"))
const CentralGovernmentScreen = React.lazy(() => import("@screens/Game/CentralGovernmentScreen"))
const TransferToRegionalScreen = React.lazy(() => import("@screens/Game/TransferToRegionalScreen"))
const SpecialPolicyScreen = React.lazy(() => import("@screens/Game/SpecialPolicyScreen"))
const SummaryScreen = React.lazy(() => import("@screens/Game/SummaryScreen"))
const SummaryIncomeScreen = React.lazy(() => import("@screens/Game/SummaryIncomeScreen"))
const NotFoundScreen = React.lazy(() => import("@screens/Error/NotFoundScreen"))

const GameRouter = () => {
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)

	if (typeof session === "undefined" || session === null) {
		return <StartScreen />
	}

	return (
		<Switch>
			<Redirect exact from={GamesRoutes.IndexScreen} to={GamesRoutes.IncomeScreen} />
			<Redirect from={GamesRoutes.StartScreen} to={GamesRoutes.IncomeScreen} />
			<Route path={GamesRoutes.IncomeScreen} component={IncomeScreen} />
			<Route path={GamesRoutes.SummaryIncomeScreen} component={SummaryIncomeScreen} />
			<Route path={GamesRoutes.EconomicMacroScreen} component={EconomicMacroScreen} />
			<Route path={GamesRoutes.AllocationScreen} component={AllocationScreen} />
			<Route path={GamesRoutes.CentralGovernmentScreen} component={CentralGovernmentScreen} />
			<Route path={GamesRoutes.TransferToRegionalScreen} component={TransferToRegionalScreen} />
			<Route path={`${GamesRoutes.SpecialPolicyScreen}/:specialPolicyId`} component={SpecialPolicyScreen} />
			<Route path={GamesRoutes.SummaryScreen} component={SummaryScreen} />
			<Route component={NotFoundScreen} />
		</Switch>
	)
}

export default GameRouter
