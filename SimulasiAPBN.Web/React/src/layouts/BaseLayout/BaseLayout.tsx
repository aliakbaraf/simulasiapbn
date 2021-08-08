/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import mergeProps from "merge-props"

import ComponentProps from "@common/libraries/component"

type BaseLayoutProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const BaseLayout: React.FC<BaseLayoutProps> = props => {
	const { children, ...otherProps } = mergeProps(
		{
			className: "layout",
		},
		props
	)

	return <div {...otherProps}>{children}</div>
}

export default BaseLayout
