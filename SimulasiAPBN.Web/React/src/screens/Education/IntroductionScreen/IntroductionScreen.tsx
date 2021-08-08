/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { useEffect, useState } from "react"
import { useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import ReactPlayer from "react-player"

import ColorVariant from "@common/enum/ColorVariant"
import useHTMLDocument from "@common/hooks/htmlDocument"
import ComponentProps from "@common/libraries/component"
import Button from "@components/Button"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import EducationRoutes from "@screens/Education/routes"

import "./IntroductionScreen.scoped.css"

type IntroductionScreenProps = ComponentProps<{}, React.AllHTMLAttributes<HTMLDivElement>>

const IntroductionScreen: React.FC<IntroductionScreenProps> = () => {
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const hashTag = useSelector<RootState, string>(state => state.dynamicMetadata.hashTag)
	const invitationText = useSelector<RootState, string[]>(state => state.dynamicMetadata.invitationText)
	const videoUrl = useSelector<RootState, string>(state => state.dynamicMetadata.videoUrl)
	const [videoPlaying, setVideoPlaying] = useState<boolean>(false)

	const onVideoPlayerReady = () => {
		setVideoPlaying(true)
	}

	const onNextButtonClicked: React.MouseEventHandler = () => {
		history.push(EducationRoutes.RulesScreen)
	}

	useEffect(() => {
		htmlDocument.setTitle("Pengenalan")
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Education}>
			<div className="content-wrapper">
				<div className="main-content">
					<div className="video-wrapper">
						<ReactPlayer
							controls={true}
							loop={true}
							pip={true}
							playing={videoPlaying}
							url={videoUrl}
							onReady={onVideoPlayerReady}
							height="100%"
							width="100%"
						/>
					</div>
				</div>
				<div className="side-content">
					<div className="header">Halo!</div>
					<div className="description">
						{invitationText.map((text, index) => (
							<React.Fragment key={index}>
								{text}
								<br />
							</React.Fragment>
						))}
					</div>
					<div className="hash-tag">{hashTag}</div>
					<div className="click-to-action">
						<Button color={ColorVariant.Secondary} onClick={onNextButtonClicked}>
							Lanjut
						</Button>
					</div>
				</div>
			</div>
		</DefaultLayout>
	)
}

export default IntroductionScreen
