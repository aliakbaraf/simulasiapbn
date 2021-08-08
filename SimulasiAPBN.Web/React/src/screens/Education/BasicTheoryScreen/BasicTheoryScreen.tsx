/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import { FiArrowLeft } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import Drawer from "@components/Drawer"
import Image from "@components/Image"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import EducationRoutes from "@screens/Education/routes"

import BasicTheoryDarkImage from "@assets/webp/basic-theory-dark.webp"
import BasicTheoryDarkFallbackImage from "@assets/png/basic-theory-dark.png"
import BasicTheoryLightImage from "@assets/webp/basic-theory-light.webp"
import BasicTheoryLightFallbackImage from "@assets/png/basic-theory-light.png"
import "./BasicTheoryScreen.scoped.css"

enum BasicTheory {
	Inactive = -1,
	PendapatanNegaraDanHibah,
	BelanjaNegara,
	KeseimbanganPrimer,
	PembiayaanAnggaran,
	AsumsiDasarEkonomiMakro,
}

type BasicTheoryScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const BasicTheoryScreen: React.FC<BasicTheoryScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const [activeTheory, setActiveTheory] = useState<BasicTheory>(BasicTheory.Inactive)
	const [basicTheoryImage, setBasicTheoryImage] = useState<string>("")
	const [basicTheoryFallbackImage, setBasicTheoryFallbackImage] = useState<string>("")

	const onBackButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.RulesScreen)
	}

	const onNextButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.FunctionsScreen)
	}

	const onOpenDrawer = (theory: BasicTheory) => {
		setActiveTheory(theory)
	}

	const onCloseDrawer = () => {
		setActiveTheory(BasicTheory.Inactive)
	}

	useEffect(() => {
		htmlDocument.setTitle("Teori Dasar")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		setBasicTheoryImage(darkMode ? BasicTheoryDarkImage : BasicTheoryLightImage)
		setBasicTheoryFallbackImage(darkMode ? BasicTheoryDarkFallbackImage : BasicTheoryLightFallbackImage)
	}, [darkMode])

	const PendapatanNegaraDanHibah: React.FC = () => (
		<Drawer title="Pendapatan Negara dan Hibah" onCloseDrawer={onCloseDrawer}>
			<p>
				<strong>Pendapatan Negara</strong> adalah hak Pemerintah Pusat yang diakui sebagai penambahan nilai
				kekayaan bersih. Pendapatan Negara terdiri atas Penerimaan Perpajakan, Penerimaan Negara Bukan Pajak dan
				Penerimaan Hibah.
			</p>
			<ol className="list-decimal pl-6">
				<li>
					Penerimaan Perpajakan adalah semua penerimaan yang terdiri dari Pajak Dalam Negeri dan Pajak
					Perdagangan Internasional. Yang termasuk Pajak Dalam Negeri adalah semua penerimaan Negara yang
					berasal dari pajak penghasilan, pajak pertambahan nilai barang dan jasa, pajak penjualan atas barang
					mewah, dan lain-lain. Pajak perdagangan Internasional adalah semua penerimaan Negara yang berasal
					dari bea masuk dan pajak/pungutan ekspor.
				</li>
				<li>
					Penerimaan Negara Bukan Pajak --yang selanjutnya disingkat PNBP-- adalah penerimaan Pemerintah Pusat
					yang diterima dalam bentuk penerimaan dari sumber daya alam (SDA), pendapatan Badan Usaha Milik
					Negara (BUMN), serta pendapatan Badan Layanan Umum (BLU).
				</li>
			</ol>
			<br />
			<p>
				<strong>Hibah</strong> merupakan seluruh penerimaan Negara baik dalam bentuk devisa dan/atau devisa yang
				dirupiahkan, Rupiah, jasa dan/atau surat berharga yang diperoleh dari pemberian hibah yang tidak perlu
				dibayar kembali dan tidak mengikat, baik yang berasal dari dalam negeri maupun dari luar negeri.
			</p>
		</Drawer>
	)

	const BelanjaNegara: React.FC = () => (
		<Drawer title="Belanja Negara" onCloseDrawer={onCloseDrawer}>
			<p>
				<strong>Belanja Negara</strong> adalah kewajiban Pemerintah Pusat yang diakui sebagai pengurangan nilai
				kekayaan bersih yang terdiri atas Belanja Pemerintah Pusat dan Transfer ke Daerah dan Dana Desa.
			</p>
		</Drawer>
	)

	const KeseimbanganPrimer: React.FC = () => (
		<Drawer title="Keseimbangan Primer" onCloseDrawer={onCloseDrawer}>
			<p>
				Mengembangkan kemampuan Pemerintah membayar pokok dan bunga utang dengan menggunakan pendapatan Negara.
				Apabila keseimbangan primer negatif, maka Pemerintah harus berhutang. Sebaliknya, jika keseimbangan
				primer positif, maka Pemerintah bisa menggunakannya untuk membayar pokok hutang.
			</p>
		</Drawer>
	)

	const PembiayaanAnggaran: React.FC = () => (
		<Drawer title="Pembiayaan Anggaran" onCloseDrawer={onCloseDrawer}>
			<p>
				<strong>Pembiayaan Anggaran</strong> adalah setiap penerimaan yang perlu dibayar kembali dan/atau
				pengeluaran yang akan diterima kembali, baik pada tahun anggaran yang bersangkutan maupun tahun-tahun
				anggaran berikutnya.
			</p>
		</Drawer>
	)

	const AsumsiDasarEkonomiMakro: React.FC = () => (
		<Drawer title="Asumsi Dasar Ekonomi Makro" onCloseDrawer={onCloseDrawer}>
			<p>
				<strong>Asumsi Dasar Ekonomi Makro</strong> adalah indikator utama ekonomi makro yang digunakan sebagai
				acuan dalam menyusun berbagai komponen postur APBN. Asumsi dasar ekonomi makro terdiri dari:
			</p>
			<div className="macro-assumption-item">Pertumbuhan Ekonomi</div>
			<div className="macro-assumption-item">Inflasi</div>
			<div className="macro-assumption-item">Nilai Tukar Rupiah</div>
			<div className="macro-assumption-item">Suku Bunga Surat Perbendaharaan Negara (SPN)</div>
			<div className="macro-assumption-item">Harga Minyak</div>
			<div className="macro-assumption-item">Lifting Minyak</div>
			<div className="macro-assumption-item">Lifting Gas</div>
		</Drawer>
	)

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Education}>
			<div className="content-wrapper">
				<div className="side-content">
					<div className="image-wrapper">
						<Image
							className="basic-theory-image"
							source={basicTheoryImage}
							fallback={basicTheoryFallbackImage}
							alt="Teori Dasar"
						/>
					</div>
					<div className="click-to-action-md">
						<Button
							color={ColorVariant.PrimaryAlternate}
							icon={FiArrowLeft}
							onClick={onBackButtonClicked}
						/>
						<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
							Lanjut
						</Button>
					</div>
				</div>
				<div className="main-content">
					<p className="header">Pahami 5 hal ini sebelum kamu merancang APBN!</p>
					<p className="description">Ketuk pada salah satu kotak di bawah untuk mempelajari lebih lanjut.</p>
					<div className="theory-list">
						<div className="theory-item" onClick={() => onOpenDrawer(BasicTheory.PendapatanNegaraDanHibah)}>
							Pendapatan Negara dan Hibah
						</div>
						<div className="theory-item" onClick={() => onOpenDrawer(BasicTheory.BelanjaNegara)}>
							Belanja Negara
						</div>
						<div className="theory-item" onClick={() => onOpenDrawer(BasicTheory.KeseimbanganPrimer)}>
							Keseimbangan Primer
						</div>
						<div className="theory-item" onClick={() => onOpenDrawer(BasicTheory.PembiayaanAnggaran)}>
							Pembiayaan Anggaran
						</div>
						<div className="theory-item" onClick={() => onOpenDrawer(BasicTheory.AsumsiDasarEkonomiMakro)}>
							Asumsi Dasar Ekonomi Makro
						</div>
					</div>
				</div>
			</div>
			<div className="click-to-action">
				<Button color={ColorVariant.PrimaryAlternate} icon={FiArrowLeft} onClick={onBackButtonClicked} />
				<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
					Lanjut
				</Button>
			</div>

			{activeTheory === BasicTheory.PendapatanNegaraDanHibah ? <PendapatanNegaraDanHibah /> : null}
			{activeTheory === BasicTheory.BelanjaNegara ? <BelanjaNegara /> : null}
			{activeTheory === BasicTheory.KeseimbanganPrimer ? <KeseimbanganPrimer /> : null}
			{activeTheory === BasicTheory.PembiayaanAnggaran ? <PembiayaanAnggaran /> : null}
			{activeTheory === BasicTheory.AsumsiDasarEkonomiMakro ? <AsumsiDasarEkonomiMakro /> : null}
		</DefaultLayout>
	)
}

export default BasicTheoryScreen
