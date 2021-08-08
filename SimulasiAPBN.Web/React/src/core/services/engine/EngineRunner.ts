/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import EngineErrorCode from "@core/services/engine/EngineErrorCode"
import EngineExecutor from "@core/services/engine/EngineExecutor"
import EngineRunnerError from "@core/services/engine/EngineRunnerError"

type EngineRunner = <TModel>(executor: EngineExecutor<TModel>, ...args: any[]) => Promise<TModel | undefined>
export default EngineRunner

export const getDefaultEngineRunner: () => EngineRunner = () => {
	return async function <TModel>(executor: EngineExecutor<TModel>, ...args: any[]): Promise<TModel | undefined> {
		if ("navigator" in window && !window.navigator.onLine) {
			throw new Error("Kamu sedang dalam kondisi luring. Pastikan kamu terhubung ke Internet.")
		}

		const executionResult = await executor(...args)
		const response = executionResult.data

		if (typeof response === "undefined") {
			throw new EngineRunnerError("Server tidak memberikan tanggapan.", EngineErrorCode.GenericServerError)
		}
		if (response.success) {
			return response.data
		}

		let error: EngineRunnerError
		if (typeof response.error === "undefined") {
			const message = typeof response.message === "undefined" ? "Terjadi kesalahan." : response.message
			error = new EngineRunnerError(message, EngineErrorCode.GenericClientError)
		} else {
			error = new EngineRunnerError(response.error.reason, response.error.code, response.error.support_id)
		}
		error.response = response
		throw error
	}
}
