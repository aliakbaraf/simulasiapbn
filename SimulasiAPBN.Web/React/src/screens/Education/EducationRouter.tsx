/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { Redirect, Route, Switch } from "react-router-dom"

import EducationRoutes from "@screens/Education/routes"

const IntroductionScreen = React.lazy(() => import("@screens/Education/IntroductionScreen"))
const RulesScreen = React.lazy(() => import("@screens/Education/RulesScreen"))
const BasicTheoryScreen = React.lazy(() => import("@screens/Education/BasicTheoryScreen"))
const FunctionsScreen = React.lazy(() => import("@screens/Education/FunctionsScreen"))
const NotFoundScreen = React.lazy(() => import("@screens/Error/NotFoundScreen"))

const EducationRouter: React.FC = () => {
	return (
		<Switch>
			<Redirect exact from={EducationRoutes.IndexScreen} to={EducationRoutes.IntroductionScreen} />
			<Route path={EducationRoutes.IntroductionScreen} component={IntroductionScreen} />
			<Route path={EducationRoutes.RulesScreen} component={RulesScreen} />
			<Route path={EducationRoutes.BasicTheoryScreen} component={BasicTheoryScreen} />
			<Route path={EducationRoutes.FunctionsScreen} component={FunctionsScreen} />
			<Route component={NotFoundScreen} />
		</Switch>
	)
}

export default EducationRouter
