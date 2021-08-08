/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { Redirect, Route, Switch } from "react-router-dom"

import RootRoutes from "@screens/routes"

const LandingScreen = React.lazy(() => import("@screens/Misc/LandingScreen"))
const EducationRouter = React.lazy(() => import("@screens/Education/EducationRouter"))
const GameRouter = React.lazy(() => import("@screens/Game/GameRouter"))
const NotFoundScreen = React.lazy(() => import("@screens/Error/NotFoundScreen"))

const RootRouter: React.FC = () => {
	return (
		<Switch>
			<Redirect exact from={"/index.html"} to={RootRoutes.IndexScreen} />
			<Redirect exact from={RootRoutes.IndexScreen} to={RootRoutes.LandingScreen} />
			<Route exact path={RootRoutes.LandingScreen} component={LandingScreen} />
			<Route path={RootRoutes.EducationScreen} component={EducationRouter} />
			<Route path={RootRoutes.GameScreen} component={GameRouter} />
			<Route component={NotFoundScreen} />
		</Switch>
	)
}

export default RootRouter
