/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import Allocation from "@core/models/Allocation"
import SimulationSession from "@core/models/SimulationSession"
import EconomicMacro from "@core/models/EconomicMacro"

type ClientSimulation = {
	allocations: Allocation[]
	economics: EconomicMacro[]
	session: SimulationSession
}

export default ClientSimulation
