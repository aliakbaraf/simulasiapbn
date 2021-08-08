/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
class EngineUrl {
	private readonly baseUrl = "/engine"
	private readonly endpoint: string

	constructor(endpoint: string) {
		this.endpoint = endpoint
	}

	public toString(): string {
		return this.baseUrl + this.endpoint
	}
}

export default EngineUrl
