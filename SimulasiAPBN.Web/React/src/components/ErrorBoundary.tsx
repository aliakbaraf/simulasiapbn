/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes } from "react"
import { connect, MapDispatchToProps, MapStateToProps } from "react-redux"
import { toast } from "react-hot-toast"

import ComponentProps from "@common/libraries/component"
import RootState from "@flow/types"
import { CommonError } from "@flow/types/common"
import { setError } from "@flow/slices/common"
import monitoring from "@common/libraries/monitoring"
import EngineErrorCode from "@core/services/engine/EngineErrorCode"
import EngineRunnerError, { isEngineRunnerError } from "@core/services/engine/EngineRunnerError"
import ClientSimulation from "@core/types/ClientSimulation"
import { setClientSimulation } from "@flow/slices/simulation"
import environment from "@common/libraries/environment"

type ErrorBoundaryProps = ComponentProps<{
	component: React.FC<ComponentProps<{ error?: CommonError }, AllHTMLAttributes<HTMLDivElement>>>
}>
type ErrorBoundaryState = {
	error?: Error
	showErrorScreen: boolean
	supportId?: string
}

class ErrorBoundary extends React.Component<CombinedProps, ErrorBoundaryState> {
	constructor(props: CombinedProps) {
		super(props)
		this.state = {
			...this.state,
			showErrorScreen: false,
		}
	}

	private onErrorPropsChanged() {
		this.setState({ ...this.state, error: this.props.error as Error | undefined })
	}

	private onErrorStateChanged() {
		if (typeof this.state.error === "undefined") {
			this.setState({ ...this.state, showErrorScreen: false })
			return
		}

		const showErrorScreen = (
			error: Error,
			useSupportId: boolean | string = true,
			sanitizedOnProduction: boolean = true
		) => {
			error = sanitizedOnProduction && environment.isProduction() ? new Error() : error
			const supportId =
				typeof useSupportId === "string" && useSupportId
					? useSupportId
					: typeof useSupportId === "boolean" && useSupportId
					? monitoring.reportError(error)
					: void 0
			this.setState({ ...this.state, error, supportId, showErrorScreen: true })
		}

		const showErrorToast = (error: Error) => {
			toast.error(error.message)
		}

		if (!isEngineRunnerError(this.state.error)) {
			return showErrorScreen(this.state.error)
		}

		const engineRunnerError = this.state.error as EngineRunnerError
		switch (engineRunnerError.code) {
			// Method: TOAST
			case EngineErrorCode.DataNotFound:
			case EngineErrorCode.DataValidationFailed:
			case EngineErrorCode.RequiredDataNotProvided:
				return showErrorToast(engineRunnerError)

			// Method: ERROR SCREEN, without support ID
			case EngineErrorCode.GenericClientError:
			case EngineErrorCode.GenericServerError:
			case EngineErrorCode.ApplicationNotInstalled:
			case EngineErrorCode.NoStateBudgetData:
			case EngineErrorCode.NotAuthenticated:
				return showErrorScreen(engineRunnerError, engineRunnerError.supportId || false, false)

			// Method: CUSTOM
			case EngineErrorCode.SessionHasBeenCompleted:
			case EngineErrorCode.SessionKeyNotProvided:
			case EngineErrorCode.InvalidSessionKey:
				this.props.setClientSimulation()
				return showErrorScreen(engineRunnerError, engineRunnerError.supportId || false, false)

			// Method: ERROR SCREEN, with support ID
			default:
				return showErrorScreen(engineRunnerError, engineRunnerError.supportId || false)
		}
	}

	public static getDerivedStateFromError(error: Error) {
		return { error }
	}

	public componentDidCatch(error: Error) {
		this.setState({ ...this.state, error })
	}

	public componentDidUpdate(prevProps: Readonly<CombinedProps>, prevState: Readonly<ErrorBoundaryState>) {
		if (this.props.error !== prevProps.error) {
			return this.onErrorPropsChanged()
		}

		if (this.state.error !== prevState.error) {
			return this.onErrorStateChanged()
		}
	}

	public render() {
		if (this.state.showErrorScreen && typeof this.state.error !== "undefined") {
			const Component = this.props.component
			const error: CommonError = { message: this.state.error.message, supportId: this.state.supportId }
			return <Component error={error} />
		}

		return this.props.children
	}
}

type DispatchProps = {
	setClientSimulation: (clientSimulation?: ClientSimulation) => void
	setError: (error: CommonError) => void
}
const mapDispatchToProps: MapDispatchToProps<DispatchProps, ErrorBoundaryProps> = { setClientSimulation, setError }

type StateProps = {
	error?: CommonError
}
const mapStateToProps: MapStateToProps<StateProps, ErrorBoundaryProps, RootState> = state => {
	return {
		error: state.common.error,
	}
}

type CombinedProps = ErrorBoundaryProps & DispatchProps & StateProps

export default connect(mapStateToProps, mapDispatchToProps)(ErrorBoundary)
