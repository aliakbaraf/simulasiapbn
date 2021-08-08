/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { useSelector } from "react-redux"
import RootState from "@flow/types"

class HtmlDocument {
	private readonly applicationTitle: string

	public constructor(applicationTitle: string) {
		this.applicationTitle = applicationTitle ?? "Simulasi APBN"
	}

	public setTitle(title: string) {
		if (!title) {
			return
		}

		document.title = `${title} - ${this.applicationTitle}`
	}

	public clearTitle() {
		document.title = this.applicationTitle
	}
}

export const useHTMLDocument = () => {
	const applicationTitle = useSelector<RootState, string>(state => state.dynamicMetadata.applicationTitle)
	return new HtmlDocument(applicationTitle)
}

export default useHTMLDocument
