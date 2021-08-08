/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import TagManager, { TagManagerArgs } from "react-gtm-module"
import * as Sentry from "@sentry/browser"
import { BrowserOptions } from "@sentry/browser"
import { Integrations } from "@sentry/tracing"
import { ReportHandler } from "web-vitals"

import config from "@common/libraries/config"
import environment from "@common/libraries/environment"

export const initialize = () => {
	initializeGoogleTagManager()
	initializeSentry()
}

export const initializeGoogleTagManager = () => {
	if (!config.googleTagManagerID) {
		return
	}

	const tagManagerArgs: TagManagerArgs = {
		gtmId: config.googleTagManagerID,
	}

	TagManager.initialize(tagManagerArgs)
}

export const initializeSentry = () => {
	if (!config.sentryDSN) {
		return
	}

	const browserOptions: BrowserOptions = {
		dsn: config.sentryDSN,
		autoSessionTracking: true,
		release: `${config.application.name}@${config.application.version}`,
		integrations: [new Integrations.BrowserTracing()],
		tracesSampleRate: environment.isProduction() ? 0.4 : 1.0,
		environment: config.nodeEnv,
	}

	Sentry.init(browserOptions)
}

export const reportError = (error: Error) => {
	return Sentry.captureException(error)
}

export const reportWebVitals = (onPerfEntry?: ReportHandler) => {
	if (onPerfEntry && typeof onPerfEntry === "function") {
		import("web-vitals").then(({ getCLS, getFID, getFCP, getLCP, getTTFB }) => {
			getCLS(onPerfEntry)
			getFID(onPerfEntry)
			getFCP(onPerfEntry)
			getLCP(onPerfEntry)
			getTTFB(onPerfEntry)
		})
	}
}

export default { initialize, reportError, reportWebVitals }
