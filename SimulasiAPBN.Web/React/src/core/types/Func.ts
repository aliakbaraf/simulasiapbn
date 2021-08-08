/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
type Func<TResult = void, TParam0 = void, TParam1 = void, TParam2 = void, TParam3 = void, TParam4 = void> = (
	param0: TParam0,
	param1: TParam1,
	param2: TParam2,
	param3: TParam3,
	param4: TParam4
) => TResult

export default Func
