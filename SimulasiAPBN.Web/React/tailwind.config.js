/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
module.exports = {
	purge: ["./src/**/*.{js,jsx,ts,tsx}", "./public/index.html"],
	darkMode: "class", // or "media" or "class"
	theme: {
		extend: {
			animation: {
				"spin-slow": "spin 2s linear infinite",
			},
			colors: {
				primary: {
					light: "#327EBC",
					DEFAULT: "#005FAC",
					dark: "#004B89",
				},
				secondary: {
					light: "#FCCD59",
					DEFAULT: "#FCB813",
					dark: "#D99B03",
				},
				alternative: {
					light: "#C3F765",
					DEFAULT: "#75AB18",
					dark: "#385702",
				},
				secondAlternative: {
					light: "#FC8551",
					DEFAULT: "#FF5E19",
					dark: "#D94301",
				},
			},
			fontFamily: {
				sans: ["Nunito"],
			},
		},
	},
	variants: {
		extend: {
			backgroundColor: ["disabled"],
			borderColor: ["disabled"],
			textColor: ["disabled"],
		},
	},
	plugins: [require("@tailwindcss/typography")],
}
