/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { Dispatch } from "@reduxjs/toolkit"
import { toast } from "react-hot-toast"

import { setError } from "@flow/slices/common"

type ErrorHandler = (error: Error, processor?: "default" | "toast") => void
type ErrorHandlerShowToast = (message: string) => void
type ErrorHandlerHook = (dispatch: Dispatch) => ErrorHandler & { showToast: ErrorHandlerShowToast }

const useErrorHandler: ErrorHandlerHook = (dispatch: Dispatch) => {
	const errorHandler: ErrorHandler = (error, processor = "default") => {
		if (processor === "toast") {
			toast.error(error.message)
			return
		}

		dispatch(setError(error))
	}

	;(errorHandler as any).showToast = (message: string) => {
		toast.error(message)
	}

	return errorHandler as ErrorHandler & { showToast: ErrorHandlerShowToast }
}

export default useErrorHandler
