/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import Engine from "@core/services/engine/Engine"
import Services from "@core/services"

export const useEngine = () => new Engine()

export const useService = () => new Services()

export default { useEngine, useService }
