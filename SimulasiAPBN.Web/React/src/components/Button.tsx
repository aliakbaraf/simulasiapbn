/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React from "react"
import { IconType } from "react-icons"
import mergeProps from "merge-props"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import ComponentProps from "@common/libraries/component"

type ButtonProps = ComponentProps<
	{
		color?: ColorVariant
		icon?: IconType
		noOutline?: boolean
		rightIcon?: boolean
		size?: SizeVariant
	},
	React.ButtonHTMLAttributes<HTMLButtonElement>
>

const Button = React.forwardRef<HTMLButtonElement, ButtonProps>((props, ref) => {
	const baseClassName: string[] = ["rounded"]
	const variantClassName: string[] = []

	switch (props.color) {
		case ColorVariant.Primary:
		default:
			variantClassName.push("text-white", "bg-primary", "disabled:bg-gray-200", "disabled:border-gray-200")
			if (!props.noOutline) {
				variantClassName.push("border", "border-primary")
			}
			if (!props.disabled) {
				variantClassName.push(
					"hover:bg-primary-dark",
					"hover:border-primary-dark",
					"active:border-primary-dark"
				)
			}
			break
		case ColorVariant.Secondary:
			variantClassName.push("text-black", "bg-secondary", "disabled:bg-gray-200", "disabled:border-gray-200")
			if (!props.noOutline) {
				variantClassName.push("border", "border-secondary")
			}
			if (!props.disabled) {
				variantClassName.push(
					"hover:bg-secondary-dark",
					"hover:border-secondary-dark",
					"active:border-secondary-dark"
				)
			}
			break
		case ColorVariant.PrimaryAlternate:
			variantClassName.push("text-primary", "bg-none", "disabled:text-gray-200", "disabled:border-gray-200")
			if (!props.noOutline) {
				variantClassName.push("border", "border-primary-light")
			}
			if (!props.disabled) {
				variantClassName.push(
					"hover:text-white",
					"hover:bg-primary-light",
					"hover:border-none",
					"active:border-primary-light"
				)
			}
			break
		case ColorVariant.SecondaryAlternate:
			variantClassName.push("text-secondary", "bg-none", "disabled:text-gray-200", "disabled:border-gray-200")
			if (!props.noOutline) {
				variantClassName.push("border", "border-secondary-light")
			}
			if (!props.disabled) {
				variantClassName.push(
					"hover:text-white",
					"hover:bg-secondary-light",
					"hover:border-none",
					"active:border-secondary-light"
				)
			}
			break
	}

	switch (props.size) {
		case SizeVariant.Small:
			variantClassName.push("h-7", "text-sm")
			if (!props.noOutline) {
				variantClassName.push("px-2")
			}
			break
		case SizeVariant.Medium:
		default:
			variantClassName.push("h-9", "text-md")
			if (!props.noOutline) {
				variantClassName.push("px-3")
			}
			break
		case SizeVariant.Large:
			variantClassName.push("h-11", "text-lg")
			if (!props.noOutline) {
				variantClassName.push("px-4")
			}
			break
	}

	if (props.noOutline) {
		variantClassName.push("px-2")
	} else {
		variantClassName.push("shadow", "hover:shadow-lg")
	}

	// eslint-disable-next-line @typescript-eslint/no-unused-vars,no-unused-vars
	const { children, color, size, icon, noOutline, rightIcon, ...otherProps } = mergeProps(
		{ className: `${baseClassName.join(" ")} ${variantClassName.join(" ")}` },
		props
	)

	const ButtonContent: React.FC = () => {
		if (!props.icon) {
			return <>{children}</>
		}

		if (!props.children) {
			return (
				<span className="flex justify-center items-center">
					{props.icon({
						className: "inline",
					})}
				</span>
			)
		}

		return props.rightIcon === true ? (
			<span className="flex justify-center items-center">
				{children}&nbsp;
				{props.icon({
					className: "inline",
				})}
			</span>
		) : (
			<span className="flex justify-center items-center">
				{props.icon({
					className: "inline",
				})}
				&nbsp;{children}
			</span>
		)
	}

	return (
		<button ref={ref} {...otherProps}>
			<ButtonContent />
		</button>
	)
})

export default Button
