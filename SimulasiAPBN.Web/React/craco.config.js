/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
module.exports = {
	plugins: [
		{
			plugin: require("craco-alias"),
			options: {
				source: "options",
				baseUrl: "./",
				aliases: {
					"@root": "./",
					"@root/*": "./*",
					"@assets": "./src/assets",
					"@assets/*": "./src/assets/*",
					"@common": "./src/common",
					"@common/*": "./src/common/*",
					"@components": "./src/components",
					"@components/*": "./src/components/*",
					"@core": "./src/core",
					"@core/*": "./src/core/*",
					"@flow": "./src/flow",
					"@flow/*": "./src/flow/*",
					"@layouts": "./src/layouts",
					"@layouts/*": "./src/layouts/*",
					"@screens": "./src/screens",
					"@screens/*": "./src/screens/*",
				},
			},
		},
		{ plugin: require("craco-plugin-scoped-css") },
		{
			plugin: require("craco-image-optimizer-plugin"),
			options: {
				mozjpeg: {
					progressive: true,
					quality: 75,
				},
				optipng: {
					enabled: false,
				},
				pngquant: {
					quality: [0.65, 0.9],
					speed: 4,
				},
				gifsicle: {
					interlaced: false,
				},
				webp: {
					quality: 75,
				},
			},
		},
	],
	style: {
		postcss: {
			plugins: [
				require("postcss-import"),
				require("postcss-nested"),
				require("tailwindcss"),
				require("autoprefixer"),
			],
		},
	},
}
