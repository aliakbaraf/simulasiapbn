/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useDispatch } from "react-redux"
import { useHistory } from "react-router-dom"

import ColorVariant from "@common/enum/ColorVariant"
import ComponentProps from "@common/libraries/component"
import ApplicationLogo from "@components/ApplicationLogo"
import Button from "@components/Button"
import { setNetworkOnLine } from "@flow/slices/common"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"

import "./OfflineScreen.scoped.css"

type OfflineScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const OfflineScreen: React.FC<OfflineScreenProps> = () => {
	const dispatch = useDispatch()
	const history = useHistory()

	const onTryAgainButtonClicked: React.MouseEventHandler = () => {
		if ("navigator" in window) {
			dispatch(setNetworkOnLine(window.navigator.onLine))
			return
		}

		history.push(history.location.pathname)
	}

	return (
		<DefaultLayout className="screen" screenType={ScreenType.Unknown}>
			<ApplicationLogo />
			<div className="content">
				<p className="description">Kamu sedang dalam kondisi luring. Pastikan kamu terhubung ke Internet.</p>
			</div>
			<div className="click-to-action">
				<Button color={ColorVariant.SecondaryAlternate} onClick={onTryAgainButtonClicked}>
					Coba Lagi
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default OfflineScreen
