/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import EngineErrorCode from "./EngineErrorCode"

interface EngineError {
	code: EngineErrorCode
	reason: string
	support_id?: string
}

export default EngineError
