/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import Guid from "@core/types/Guid"

export const showPublication = (sessionId: Guid) => {
	const baseElement = document.getElementsByTagName("base")[0]
	const baseUrl = baseElement.href.endsWith("/") ? baseElement.href : baseElement.href + "/"
	const urlSearchParams = new URLSearchParams()
	urlSearchParams.append("sessionId", sessionId)
	window.location.href = `${baseUrl}publication?${urlSearchParams.toString()}`
}
