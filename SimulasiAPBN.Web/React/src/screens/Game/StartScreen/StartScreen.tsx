/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useErrorHandler from "@common/hooks/errorHandler"
import useHTMLDocument from "@common/hooks/htmlDocument"
import { useService } from "@common/hooks/services"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import { Modal } from "@components/Modal"
import { setClientSimulation } from "@flow/slices/simulation"
import { setDisplayContinueSession, setLoading, setNetworkLoading } from "@flow/slices/common"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import RootRoutes from "@screens/routes"

import "./StartScreen.scoped.css"

type StartScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const StartScreen: React.FC<StartScreenProps> = () => {
	const dispatch = useDispatch()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const htmlDocument = useHTMLDocument()
	const service = useService()
	const disclaimer = useSelector<RootState, string>(state => state.dynamicMetadata.disclaimer)

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const [name, setName] = useState<string>("")
	const [modal, setModal] = useState(true)
	const [checked, setChecked] = React.useState(false);

	const handleChange = () => {
		setChecked(!checked);
	};

	const toggleModal = () => {
		if (!checked) {
			handleError.showToast("Mohon membaca dan menyetujui disclaimer")
			return
		}

		setModal(!modal)
    }
	const onStartButtonClicked: React.MouseEventHandler = () => {
		startSimulation().then()
	}

	const onNameChanged: React.ChangeEventHandler<HTMLInputElement> = event => {
		setName(event.target.value)
	}

	const onFormSubmit: React.FormEventHandler = event => {
		event.preventDefault()
		startSimulation().then()
	}

	const startSimulation = async () => {
		const sanitizedName = name.trim()

		if (sanitizedName.length === 0) {
			handleError.showToast("Silakan isi nama kamu terlebih dahulu")
			return
		}
		if (sanitizedName.length > 20) {
			handleError.showToast("Panjang nama maksimal 20 karakter")
			return
		}

		try {
			dispatch(setLoading(`Memulai sesi simulasi untuk ${sanitizedName}...`))
			dispatch(setNetworkLoading())

			const session = await service.Simulation.newSession(sanitizedName)
			dispatch(setClientSimulation(session))

			history.push(RootRoutes.GameScreen)
		} catch (error) {
			handleError(error)
		} finally {
			dispatch(setDisplayContinueSession(false))
			dispatch(setNetworkLoading(false))
			dispatch(setLoading(false))
		}
	}

	useEffect(() => {
		htmlDocument.setTitle("Rancang APBN")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	return (
		<DefaultLayout className="screen" screenType={ScreenType.Game}>
			<Modal
				title='Disclaimer'
				isOpen={modal}
			>
				<textarea 
					className="input-text"
					rows={10}
					readOnly={true}
					value={disclaimer}
				/>
				<br /><br />
				<input
					required
					type="checkbox"
					checked={checked}
					onChange={handleChange}
				/>&nbsp;Saya telah membaca dan mengerti
				<br /><br /><br />
				<Button
					size={SizeVariant.Large}
					color={darkMode ? ColorVariant.Primary : ColorVariant.Secondary}
					onClick={toggleModal}
				>
					Setuju
				</Button>
			</Modal>
			
			<div className="header">
				<p className="header-text">Bagaimana APBN versi kamu?</p>
			</div>
			<div className="form-wrapper">
				<form className="form" onSubmit={onFormSubmit}>
					<input
						required
						className="form-input"
						type="text"
						placeholder="Masukan nama kamu..."
						value={name}
						onChange={onNameChanged}
					/>
				</form>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.Primary : ColorVariant.Secondary}
					size={SizeVariant.Large}
					onClick={onStartButtonClicked}
				>
					Rancang APBN
				</Button>
			</div>
		</DefaultLayout>
	)
}

export default StartScreen
