/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import { name, description, version, author, homepage } from "@root/package.json"

const config = {
	application: {
		name: name,
		description: description,
		version: version,
		author: {
			name: author.name,
			url: author.url,
		},
		homepage: homepage,
	},
	nodeEnv: process.env.NODE_ENV as "production" | "development" | "test",
	publicUrl: process.env.PUBLIC_URL,
	googleTagManagerID: process.env.REACT_APP_GOOGLE_TAG_MANAGER_ID,
	sentryDSN: process.env.REACT_APP_SENTRY_DSN,
}

export default config
