/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useParams, useHistory } from "react-router-dom"
import { useSelector } from "react-redux"

import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import PolicyAllocationSlide from "@components/PolicyAllocationSlide"
import PolicyInformationSlide from "@components/PolicyInformationSlide"
import SimulationSpecialPolicyAllocation from "@core/models/SimulationSpecialPolicyAllocation"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import GamesRoutes from "@screens/Game/routes"

import "./SpecialPolicyScreen.scoped.css"
import { SimulationSpecialPolicy } from "@flow/types/simulation"

enum Slide {
	InformationSlide,
	AllocationSlide,
}
type SpecialPolicyScreenParams = { specialPolicyId: string }
type SpecialPolicyScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const SpecialPolicyScreen: React.FC<SpecialPolicyScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()
	const params = useParams<SpecialPolicyScreenParams>()

	const specialPolicies = useSelector<RootState, SimulationSpecialPolicy[]>(state => state.simulation.specialPolicies)
	const specialPolicy = specialPolicies.find(entity => entity.id === params.specialPolicyId)

	const [policyAllocations, setPolicyAllocations] = useState<SimulationSpecialPolicyAllocation[]>([])
	const [slide, setSlide] = useState<Slide>(Slide.InformationSlide)
	const [totalPolicyAllocation, setTotalPolicyAllocation] = useState<number>(0)

	/* Event Handlers */
	const onSwitchToAllocationSlide = () => {
		setSlide(Slide.AllocationSlide)
	}

	const onSwitchToInformationSlide = (
		policyAllocations: SimulationSpecialPolicyAllocation[],
		totalPolicyAllocation: number
	) => {
		setSlide(Slide.InformationSlide)
		setPolicyAllocations(policyAllocations)
		setTotalPolicyAllocation(totalPolicyAllocation)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadSpecialPolicyData = () => {
		if (typeof specialPolicy === "undefined" || specialPolicy === null) {
			history.push(GamesRoutes.AllocationScreen)
			return
		}

		const newPolicyAllocations = specialPolicy.specialPolicyAllocations
		const newTotalPolicyAllocation = specialPolicy.specialPolicyAllocations
			.map(policyAllocation => policyAllocation.total_allocation)
			.reduce((previousValue, currentValue) => previousValue + currentValue)
		setPolicyAllocations(newPolicyAllocations)
		setTotalPolicyAllocation(newTotalPolicyAllocation)
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		loadSpecialPolicyData()

		htmlDocument.setTitle(specialPolicy?.name || "Kebijakan Khusus")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])
	/* End of Effects */

	const Content: React.FC = () => {
		switch (slide) {
			case Slide.AllocationSlide:
				return (
					<PolicyAllocationSlide
						policyAllocations={policyAllocations}
						specialPolicy={specialPolicy}
						totalPolicyAllocation={totalPolicyAllocation}
						onSwitchToInformationSlide={onSwitchToInformationSlide}
					/>
				)
			case Slide.InformationSlide:
			default:
				return (
					<PolicyInformationSlide
						specialPolicy={specialPolicy}
						totalPolicyAllocation={totalPolicyAllocation}
						onSwitchToAllocationSlide={onSwitchToAllocationSlide}
					/>
				)
		}
	}

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<Content />
		</DefaultLayout>
	)
}

export default SpecialPolicyScreen
