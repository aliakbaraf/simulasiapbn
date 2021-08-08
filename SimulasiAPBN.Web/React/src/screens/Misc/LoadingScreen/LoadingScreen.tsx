/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useSelector } from "react-redux"

import ComponentProps from "@common/libraries/component"
import Image from "@components/Image"
import RootState from "@flow/types"
import BaseLayout from "@layouts/BaseLayout"

import CustomLoadingSpinnerVector from "@assets/svg/custom-loading-spinner.svg"
import "./LoadingScreen.scoped.css"

type LoadingScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const LoadingScreen: React.FC<LoadingScreenProps> = () => {
	const loading = useSelector<RootState, boolean | string>(state => state.common.loading)
	const [hasLoadingText, setHasLoadingText] = useState<boolean>(typeof loading === "string" && loading.length > 0)

	useEffect(() => {
		setHasLoadingText(typeof loading === "string" && loading.length > 0)
	}, [loading])

	return (
		<BaseLayout className="screen">
			<Image
				className="loading-spinner-image"
				source={CustomLoadingSpinnerVector}
				fallback={CustomLoadingSpinnerVector}
				type="image/svg+xml"
				alt={hasLoadingText ? (loading as string) : "Mohon tunggu..."}
			/>
			{hasLoadingText ? (
				<div className="loading-text-wrapper">
					<p className="loading-text">{loading as string}</p>
				</div>
			) : null}
		</BaseLayout>
	)
}

export default LoadingScreen
