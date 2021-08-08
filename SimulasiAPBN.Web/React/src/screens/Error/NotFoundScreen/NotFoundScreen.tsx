/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect } from "react"
import { useSelector } from "react-redux"
import { useHistory } from "react-router-dom"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import ApplicationLogo from "@components/ApplicationLogo"
import Button from "@components/Button"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"

import "./NotFoundScreen.scoped.css"

type NotFoundScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>
const NotFoundScreen: React.FC<NotFoundScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)

	const onBackButtonClicked = () => {
		history.goBack()
	}

	useEffect(() => {
		htmlDocument.setTitle("Tidak Ditemukan")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	return (
		<DefaultLayout className="screen" screenType={ScreenType.Unknown}>
			<ApplicationLogo />
			<div className="content">
				<p className="header">Terjadi Kesalahan!</p>
				<p className="description">Halaman yang kamu tuju tidak ditemukan.</p>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onBackButtonClicked}
				>
					Kembali
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default NotFoundScreen
