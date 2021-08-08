/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useEffect, useState } from "react"
import { useDispatch, useSelector } from "react-redux"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import ApplicationLogo from "@components/ApplicationLogo"
import Button from "@components/Button"
import { clearError } from "@flow/slices/common"
import RootState from "@flow/types"
import { CommonError } from "@flow/types/common"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import RootRoutes from "@screens/routes"

import "./DefaultErrorScreen.scoped.css"

const ErrorMessage: React.FC = ({ children }) => <p className="description">{children}</p>

const TechnicalSupport: React.FC = ({ children }) => (
	<>
		<p className="support-text">
			Kamu dapat menghubungi tim teknis kami dengan menyertakan kode penanganan berikut.
		</p>
		<span className="support-id">{children}</span>
	</>
)

type DefaultErrorScreenProps = ComponentProps<
	{
		error?: CommonError
	},
	AllHTMLAttributes<HTMLDivElement>
>

const DefaultErrorScreen: React.FC<DefaultErrorScreenProps> = ({ error }) => {
	const dispatch = useDispatch()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const fromErrorHandler = useSelector<RootState, boolean>(state => typeof state.common.error !== "undefined")
	const [showErrorMessage, setShowErrorMessage] = useState<boolean>(false)
	const [showTechnicalSupport, setShowTechnicalSupport] = useState<boolean>(false)

	const onGameLobbyButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		dispatch(clearError())
		if (!fromErrorHandler) {
			window.location.href = RootRoutes.IndexScreen
		}
	}

	useEffect(() => {
		htmlDocument.setTitle("Kesalahan")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		if (typeof error === "undefined") {
			return
		}

		setShowErrorMessage(typeof error.message !== "undefined" && error.message.length > 0)
		setShowTechnicalSupport(typeof error.supportId !== "undefined" && error.supportId.length > 0)
	}, [error])

	return (
		<DefaultLayout className="screen" screenType={ScreenType.Unknown}>
			<ApplicationLogo />
			<div className="content">
				<p className="header">Terjadi Kesalahan!</p>
				{showErrorMessage ? <ErrorMessage>{error?.message || ""}</ErrorMessage> : null}
				{showTechnicalSupport ? <TechnicalSupport>{error?.supportId || ""}</TechnicalSupport> : null}
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onGameLobbyButtonClicked}
				>
					Ke Lobi Permainan
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default DefaultErrorScreen
