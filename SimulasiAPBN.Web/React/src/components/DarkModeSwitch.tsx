/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useDispatch, useSelector } from "react-redux"
import { FiMoon } from "react-icons/fi"

import Switch from "@components/Switch"
import RootState from "@flow/types"
import { setDarkMode } from "@flow/slices/common"

import "./styles/DarkModeSwitch.scoped.css"

const DarkModeSwitch: React.FC = () => {
	const dispatch = useDispatch()
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)

	const onDarkModeSwitched: React.ChangeEventHandler<HTMLInputElement> = event => {
		dispatch(setDarkMode(event.target.checked))
	}

	return (
		<>
			<label htmlFor="dark-mode-toggle" className="dark-mode-switch-label">
				<FiMoon />
			</label>
			<Switch name="dark-mode-toggle" defaultChecked={darkMode} onChange={onDarkModeSwitched} />
		</>
	)
}

export default DarkModeSwitch
