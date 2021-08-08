/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes } from "react"
import { useHistory } from "react-router-dom"
import { useSelector } from "react-redux"

import ColorVariant from "@common/enum/ColorVariant"
import ComponentProps from "@common/libraries/component"
import { toCurrency } from "@common/libraries/locale"
import Button from "@components/Button"
import RootState from "@flow/types"
import { SimulationSpecialPolicy } from "@flow/types/simulation"
import GamesRoutes from "@screens/Game/routes"

import "react-rangeslider/lib/index.css"
import "./styles/InformationSlide.scoped.css"

type PolicyInformationSlideProps = ComponentProps<
	{
		specialPolicy?: SimulationSpecialPolicy
		totalPolicyAllocation: number
		onSwitchToAllocationSlide: () => void
	},
	AllHTMLAttributes<HTMLDivElement>
>

const PolicyInformationSlide: React.FC<PolicyInformationSlideProps> = props => {
	const history = useHistory()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)

	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.AllocationScreen)
	}

	return (
		<div className="content information-content">
			<p className="header">{props.specialPolicy?.name || ""}</p>
			<p className="description">{props.specialPolicy?.description || ""}</p>
			<div className="expenditure-information">
				<span className="value">{toCurrency(props.totalPolicyAllocation)} T</span>
				<span className="description font-bold">Alokasi {props.specialPolicy?.name || ""}</span>
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onBackButtonClicked}
				>
					Kembali
				</Button>
				&nbsp;&nbsp;&nbsp;&nbsp;
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					onClick={props.onSwitchToAllocationSlide}
				>
					Atur Alokasi
				</Button>
			</div>
		</div>
	)
}

export default PolicyInformationSlide
