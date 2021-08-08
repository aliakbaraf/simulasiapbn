/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import "react-app-polyfill/stable"

import React from "react"
import ReactDOM from "react-dom"
import { BrowserRouter as ReactRouter } from "react-router-dom"
import { Provider } from "react-redux"
import { PersistGate } from "redux-persist/integration/react"
import { Toaster } from "react-hot-toast"

import environment from "@common/libraries/environment"
import figlet from "@common/libraries/figlet"
import monitoring from "@common/libraries/monitoring"
import ErrorBoundary from "@components/ErrorBoundary"
import store, { persistor } from "@flow/store"
import DefaultErrorScreen from "@screens/Error/DefaultErrorScreen"

import Application from "./Application"
import * as serviceWorkerRegistration from "./serviceWorkerRegistration"
import "./styles/global.css"

figlet.showBanner()
monitoring.initialize()

ReactDOM.render(
	<Provider store={store}>
		<PersistGate persistor={persistor}>
			<ErrorBoundary component={DefaultErrorScreen}>
				<ReactRouter>
					<Application />
				</ReactRouter>
				<Toaster position="top-center" />
			</ErrorBoundary>
		</PersistGate>
	</Provider>,
	document.querySelector("#application")
)

// If you want your application to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://cra.link/PWA
if (environment.isProduction()) {
	serviceWorkerRegistration.register()
} else {
	serviceWorkerRegistration.unregister()
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
monitoring.reportWebVitals()
