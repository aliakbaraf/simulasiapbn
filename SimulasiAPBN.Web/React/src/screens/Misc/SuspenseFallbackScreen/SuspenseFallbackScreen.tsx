/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React  from "react"

import Image from "@components/Image"
import BaseLayout from "@layouts/BaseLayout"

import CustomLoadingSpinnerVector from "@assets/svg/custom-loading-spinner.svg"
import "./SuspenseFallbackScreen.scoped.css"

const SuspenseFallbackScreen: React.FC = () => (
	<BaseLayout className="screen">
		<Image
			className="loading-spinner-image"
			source={CustomLoadingSpinnerVector}
			fallback={CustomLoadingSpinnerVector}
			type="image/svg+xml"
			alt="Mohon tunggu..."
		/>
		<div className="loading-text-wrapper">
			<p className="loading-text">Mohon tunggu...</p>
		</div>
	</BaseLayout>
)

export default SuspenseFallbackScreen
