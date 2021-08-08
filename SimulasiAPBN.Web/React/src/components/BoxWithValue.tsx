/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"

import ComponentProps from "@common/libraries/component"

import "./styles/BoxWithValue.scoped.css"
import mergeProps from "merge-props"

type BoxWithValueProps = ComponentProps<
	{
		kind?: "default" | "error" | "alternate"
		title: string
		value: string
	},
	React.AllHTMLAttributes<HTMLDivElement>
>

const BoxWithValue: React.FC<BoxWithValueProps> = props => {
	const { children, kind, title, value, ...otherProps } = mergeProps(
		{
			className: "box-with-value",
		},
		props
	)

	return (
		<div {...otherProps}>
			<div className={`upper-box ${kind === "error" ? "error-upper-box" : kind === "alternate" ? "alternative-upper-box" : "default-upper-box"}`}>
				<span>{value}</span>
			</div>
			<div className={`lower-box ${kind === "error" ? "error-lower-box" : kind === "alternate" ? "alternative-lower-box" : "default-lower-box"}`}>
				<div className="title">{title}</div>
				<div className="content">{children}</div>
			</div>
		</div>
	)
}

export default BoxWithValue
