/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"

import ComponentProps from "@common/libraries/component"

type ImageProps = ComponentProps<
	{
		source: string
		fallback: string
		alt: string
		type?: string
	},
	React.ImgHTMLAttributes<HTMLImageElement>
>

const Image: React.FC<ImageProps> = props => {
	const { source, alt, type, fallback, ...otherProps } = props

	return (
		<picture>
			<source srcSet={source} type={type ? type : "image/webp"} />
			<img src={fallback} alt={alt} {...otherProps} />
		</picture>
	)
}

export default Image
