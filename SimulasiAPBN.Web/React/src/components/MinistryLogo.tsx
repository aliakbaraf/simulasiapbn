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

import MinistryLogoImage from "@assets/webp/ministry-logo.webp"
import MinistryLogoFallbackImage from "@assets/png/ministry-logo.png"
import MinistryLogoWithTitleDarkImage from "@assets/webp/ministry-logo-with-title-dark.webp"
import MinistryLogoWithTitleDarkFallbackImage from "@assets/png/ministry-logo-with-title-dark.png"
import MinistryLogoWithTitleLightImage from "@assets/webp/ministry-logo-with-title-light.webp"
import MinistryLogoWithTitleLightFallbackImage from "@assets/png/ministry-logo-with-title-light.png"

type MinistryLogoProps = ComponentProps<
	{
		href?: string
		target?: "_blank" | "_parent" | "_self" | "_top"
		withTitle?: boolean
	},
	React.ImgHTMLAttributes<HTMLImageElement>
>

const MinistryLogo: React.FC<MinistryLogoProps> = props => {
	const { href, target, withTitle, ...otherProps } = mergeProps(
		{
			alt: "Kementerian Keuangan Republik Indonesia",
			className: "ministry-logo",
		},
		props
	)
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)

	const logoImage =
		withTitle === true
			? darkMode
				? MinistryLogoWithTitleDarkImage
				: MinistryLogoWithTitleLightImage
			: MinistryLogoImage
	const logoFallbackImage =
		withTitle === true
			? darkMode
				? MinistryLogoWithTitleDarkFallbackImage
				: MinistryLogoWithTitleLightFallbackImage
			: MinistryLogoFallbackImage
	const MinistryImage = () => {
		// noinspection HtmlRequiredAltAttribute
		return <Image source={logoImage} fallback={logoFallbackImage} {...otherProps} />
	}

	return typeof href === "string" ? (
		<a href={href} target={target}>
			<MinistryImage />
		</a>
	) : (
		<MinistryImage />
	)
}

export default MinistryLogo
