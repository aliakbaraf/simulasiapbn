/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
/**
 * Enumerator for EngineError.Code with the following rules.
 *  1. Most left digit indicates source of error. 1 = Server, 2 = Client.
 *  2. Second left digit indicates concern group. See below for list of concern group.
 *  3. The last 2 digit indicate an index number.
 * Concern Group
 *  0 = Generic / No Concern
 *  1 = Data Concern
 *  2 = Processing Concern
 *  3 = AAA Concern
 */
enum EngineErrorCode {
	GenericServerError = 1000,
	GenericClientError = 2000,

	ApplicationNotInstalled = 1100,
	NoStateBudgetData = 1101,

	RequiredDataNotProvided = 2100,
	InvalidDataFormat = 2101,
	DataValidationFailed = 2102,
	DataNotFound = 2103,

	SessionHasBeenCompleted = 2200,

	NotAuthenticated = 2300,
	SessionKeyNotProvided = 2301,
	InvalidSessionKey = 2302,
}

export default EngineErrorCode
