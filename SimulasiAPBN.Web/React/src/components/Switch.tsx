/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import ComponentProps from "@common/libraries/component"

type SwitchProps = ComponentProps<{ name?: string }, React.InputHTMLAttributes<HTMLInputElement>>

const Switch: React.FC<SwitchProps> = ({ name, ...otherProps }) => {
	return (
		<div className="relative inline-block w-10 mr-2 align-middle select-none transition duration-200 ease-in">
			<input
				type="checkbox"
				name={name}
				className="toggle-checkbox absolute block w-6 h-6 rounded-full bg-white border-4 appearance-none cursor-pointer"
				{...otherProps}
			/>
			<label
				htmlFor={name}
				className="toggle-label block overflow-hidden h-6 rounded-full bg-gray-300 cursor-pointer"
			/>
		</div>
	)
}

export default Switch
