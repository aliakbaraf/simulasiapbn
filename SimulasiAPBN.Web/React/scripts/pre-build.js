/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
const fs = require("fs")
const path = require("path")

const rootPath = path.join(__dirname, "..")
const buildDirPath = path.join(rootPath, "build")

const main = () => {
	process.stdout.write("Executing pre-build script... ")

	if (fs.existsSync(buildDirPath)) {
		fs.rmdirSync(buildDirPath, { recursive: true })
	}

	process.stdout.write("[DONE]\n")
}

main()
