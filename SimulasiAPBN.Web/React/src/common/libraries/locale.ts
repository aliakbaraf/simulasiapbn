/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
export const toLocale = (value: number) =>
	new Intl.NumberFormat("id-ID", {
		style: "decimal",
		maximumFractionDigits: 2,
	}).format(value)

export const toCurrency = (value: number) => "Rp" + toLocale(value)
