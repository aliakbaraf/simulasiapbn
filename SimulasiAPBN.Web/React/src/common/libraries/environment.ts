/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import config from "@common/libraries/config"

const isDevelopment = () => config.nodeEnv === "development"
const isProduction = () => config.nodeEnv === "production"
const isTest = () => config.nodeEnv === "test"

const environment = { isDevelopment, isProduction, isTest }
export default environment
