/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes } from "react"
import mergeProps from "merge-props"

import ComponentProps from "@common/libraries/component"
import MinistryLogo from "@components/MinistryLogo"
import "./styles/Navigation.css"

type NavigationProps = ComponentProps<
	{
		fixed: boolean
	},
	AllHTMLAttributes<HTMLDivElement>
>

const Navigation: React.FC<NavigationProps> = props => {
	const { fixed, ...pureProps } = props
	const { children, ...otherProps } = mergeProps(
		{
			className: "navigation-action",
		},
		pureProps
	)

	return (
		<>
			<nav
				className={`navigation ${
					fixed ? "bg-white dark:bg-gray-700 fixed shadow" : "bg-gray-50 dark:bg-gray-800 "
				}`}
			>
				<MinistryLogo withTitle />
				<div {...otherProps}>{children}</div>
			</nav>
			{fixed ? <div className="navigation-span" /> : null}
		</>
	)
}

export default Navigation
