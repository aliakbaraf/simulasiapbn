/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { AxiosResponse } from "axios"
import EngineResponse from "./EngineResponse"

type EngineExecutor<TModel> = (...args: any[]) => Promise<AxiosResponse<EngineResponse<TModel>>>
export default EngineExecutor
