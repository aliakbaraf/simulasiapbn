/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes } from "react"
import { FiXSquare } from "react-icons/fi"

import ComponentProps from "@common/libraries/component"

import "./styles/Drawer.scoped.css"

type DrawerHeaderProps = ComponentProps<{ onCloseDrawer: () => void }>
const DrawerHeader: React.FC<DrawerHeaderProps> = props => (
	<>
		<div className="drawer-header">
			<span className="title">{props.children}</span>
			<button className="close-button" onClick={props.onCloseDrawer}>
				<FiXSquare />
			</button>
		</div>
		<hr className="border-primary dark:border-secondary" />
	</>
)

type DrawerProps = ComponentProps<
	{
		title: string
		onCloseDrawer: () => void
	},
	AllHTMLAttributes<HTMLDivElement>
>
const Drawer: React.FC<DrawerProps> = props => (
	<>
		<div className="drawer">
			<DrawerHeader onCloseDrawer={props.onCloseDrawer}>{props.title}</DrawerHeader>
			<div className="drawer-content">{props.children}</div>
		</div>
		<div className="drawer-background" />
	</>
)

export default Drawer
