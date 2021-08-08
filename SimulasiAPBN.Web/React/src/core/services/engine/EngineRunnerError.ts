/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import EngineErrorCode from "@core/services/engine/EngineErrorCode"
import EngineResponse from "@core/services/engine/EngineResponse"

class EngineRunnerError<TModel = any> extends Error {
	constructor(message: string, code: EngineErrorCode, supportId?: string) {
		super(message)
		this.code = code
		this.supportId = supportId
	}

	public readonly type: string = EngineRunnerErrorType

	public code: EngineErrorCode

	public supportId?: string

	public response?: EngineResponse<TModel>
}

export const EngineRunnerErrorType = "__ENGINE_RUNNER_ERROR__"

export const isEngineRunnerError = (obj: any) => {
	if (typeof obj === "undefined" || obj === null) {
		return false
	}

	return Object.prototype.hasOwnProperty.call(obj, "type") && obj["type"] === EngineRunnerErrorType
}

export default EngineRunnerError
