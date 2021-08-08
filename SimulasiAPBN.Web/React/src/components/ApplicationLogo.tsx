/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { useSelector } from "react-redux"
import mergeProps from "merge-props"

import Image from "@components/Image"
import ComponentProps from "@common/libraries/component"
import RootState from "@flow/types"

import ApplicationLogoImage from "@assets/webp/application-logo.webp"
import ApplicationLogoFallbackImage from "@assets/png/application-logo.png"

type ApplicationLogoProps = ComponentProps<{}, React.ImgHTMLAttributes<HTMLImageElement>>

const ApplicationLogo: React.FC<ApplicationLogoProps> = props => {
	const applicationTitle = useSelector<RootState, string>(state => state.dynamicMetadata.applicationTitle)

	const { ...otherProps } = mergeProps(
		{
			alt: applicationTitle,
			className: "application-logo",
		},
		props
	)

	// noinspection HtmlRequiredAltAttribute
	return <Image source={ApplicationLogoImage} fallback={ApplicationLogoFallbackImage} {...otherProps} />
}

export default ApplicationLogo
