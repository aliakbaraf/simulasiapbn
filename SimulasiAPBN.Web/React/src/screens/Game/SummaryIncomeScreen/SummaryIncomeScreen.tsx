/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
import React, { AllHTMLAttributes, useEffect, useState } from "react"
import { useDispatch, useSelector } from "react-redux"
import { useHistory } from "react-router-dom"
import { FiHelpCircle, FiPlusSquare, FiMinusSquare } from "react-icons/fi"

import ColorVariant from "@common/enum/ColorVariant"
import SizeVariant from "@common/enum/SizeVariant"
import useFormatter from "@common/hooks/formatter"
import useHTMLDocument from "@common/hooks/htmlDocument"
import { isIncomeProgressDone } from "@common/libraries/clientSimulationProgress"
import ComponentProps from "@common/libraries/component"
import { toLocale, toCurrency } from "@common/libraries/locale"
import BoxWithValue from "@components/BoxWithValue"
import Button from "@components/Button"
import Drawer from "@components/Drawer"
import SimulationSession from "@core/models/SimulationSession"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import { setLoading } from "@flow/slices/common"
import { SimulationProgress } from "@flow/types/simulation"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import GamesRoutes from "@screens/Game/routes"

import "./SummaryIncomeScreen.scoped.css"

type SummaryIncomeScreenProps = ComponentProps<{}, AllHTMLAttributes<HTMLDivElement>>

const SummaryIncomeScreen: React.FC<SummaryIncomeScreenProps> = () => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const history = useHistory()
	const htmlDocument = useHTMLDocument()

	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const economics = useSelector<RootState, SimulationEconomicMacro[]>(
		state => state.simulation.economicMacros.economicMacro
	)
	const progress = useSelector<RootState, SimulationProgress>(state => state.simulation.progress)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const totalIncome = useSelector<RootState, number>(
		state => state.simulation.session?.used_income ?? 0
	)
	const countryIncome = useSelector<RootState, number>(
		state => state.simulation.session?.state_budget.country_income ?? 0
	)

	const [economicExpenditures, setEconomicExpenditures] = useState<SimulationEconomicMacro[]>([])
	const [showIncomeDrawer, setShowIncomeDrawer] = useState<boolean>(false)

	/* Event Handlers */
	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.IncomeScreen)
	}

	const onNextButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.AllocationScreen)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadSummaryData = () => {
		if (typeof session === "undefined") {
			return
		}
	}

	/* End of Functions */

	/* Effects */
	useEffect(() => {
		if (!isIncomeProgressDone(progress)) {
			history.push(GamesRoutes.IncomeScreen)
		}
		htmlDocument.setTitle("Pratinjau Hasil")

		dispatch(setLoading("Memuat pratinjau hasil..."))
		loadSummaryData()
		dispatch(setLoading(false))
		return () => {
			htmlDocument.clearTitle()
		}
	}, [])

	useEffect(() => {
		loadSummaryData()
	}, [session])

	useEffect(() => {
		if (!isIncomeProgressDone(progress)) {
			history.push(GamesRoutes.IncomeScreen)
		}
	}, [progress])

	useEffect(() => {
		if (economics.length <= 0) {
			return
		}

		const newEconomicExpenditures = economics.filter(
			expenditure => expenditure.used_value != expenditure.economic_macro.default_value)

		setEconomicExpenditures(newEconomicExpenditures)
	}, [economics])
	/* End of Effects */

	/* Child Components */
	const IncomeDrawer: React.FC = () => (
		<Drawer title="Asumsi Ekonomi Makro" onCloseDrawer={() => setShowIncomeDrawer(false)}>
			<p>
				<strong>Asumsi Ekonomi Makro</strong> adalah
				komponen ekonomi yang mengatur pendapatan negara yang
				mengalami perbedaan dibandingkan dengan {formatter.getStateBudgetName(session?.state_budget)}.
			</p>
		</Drawer>
	)

	const renderEconomicExpenditure = (economic: SimulationEconomicMacro, index: number) => {
		const name = economic.economic_macro.name
		const naration = economic.economic_macro.naration || ""
		const narationDefisit = economic.economic_macro.naration_defisit || ""
		const usedValue = economic.used_value
		const threshold = economic.economic_macro.threshold
		const thresholdValue = economic.economic_macro.threshold_value
		const defaultValue = economic.economic_macro.default_value
		const unitDesc = economic.economic_macro.unit_desc

		const totalValue = usedValue * thresholdValue / threshold / 1000
		const change = usedValue - defaultValue

		const value = `${toCurrency(totalValue)} M`
		return (
			<BoxWithValue kind="alternate" key={index} title={name} value={value}>
				<p className="font-bold mb-2">
					Asumsi {name} versi kamu ({toLocale(usedValue)} {unitDesc}) {change > 0 ? "meningkat" : "berkurang"} {toLocale(change)} {unitDesc} dibandingkan
					alokasi pada {formatter.getStateBudgetName(session?.state_budget)} ({toLocale(defaultValue)} {unitDesc}).
				</p>
				{change > 0 ?
					!naration ? null : <p className="mb-3">{naration}</p>
					:
					!narationDefisit ? null : <p className="mb-3">{narationDefisit}</p>}
			</BoxWithValue>
		)
	}
	/* End of Child Components */

	const [showEconomics, setShowEconomics] = useState(false)
	const toggleEconomics = () => {
		setShowEconomics(!showEconomics)
	}

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<div className="main">
				<div className="header">
					<div className="title">
						<p>Pratinjau Hasil</p>
						<p>Rancangan Pendapatan Negara {formatter.getStateBudgetName(session?.state_budget)}</p>
					</div>
					<div className="abstract">
						<div>
							Berikut sekilas tentang Rancangan Pendapatan Negara versi kamu.
							<ol className="list-disc list-outside pl-6">
								<li>
									<strong>Pendapatan Negara</strong> sebesar {toCurrency(totalIncome)} T
									{toLocale(countryIncome) == toLocale(totalIncome) ? " (sesuai dengan data " + formatter.getStateBudgetName(session?.state_budget) + ")." : ""}
								</li>
							</ol>
						</div>
						<span className="font-normal italic mt-2">
							Ketuk tanda&nbsp;
							<FiHelpCircle className="inline-flex text-primary dark:text-secondary" />
							&nbsp;untuk informasi lebih lanjut.
						</span>
						<span />
					</div>
				</div>
				<div className="content">
					<div className="box .economic-box">
						<p className="box-header">
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={!showEconomics ? FiPlusSquare : FiMinusSquare}
								noOutline
								size={SizeVariant.Medium}
								onClick={() => toggleEconomics()}
							/>
							Asumsi Ekonomi Kamu&nbsp;
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={FiHelpCircle}
								noOutline
								size={SizeVariant.Small}
								onClick={() => setShowIncomeDrawer(true)}
							/>
						</p>
						<div className="box-content"
							style={{ display: (showEconomics ? 'block' : 'none') }}>
							{economicExpenditures.length <= 0
								? "Kamu tidak memiliki asumsi ekonomi makro yang berbeda."
								: economicExpenditures.map(renderEconomicExpenditure)}
							<p className="box-btn"
								style={{ display: (!showEconomics ? 'none' : economicExpenditures.length > 0 ? 'block' : 'none') }}
							>
								<Button
									color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
									icon={!showEconomics ? FiPlusSquare : FiMinusSquare}
									noOutline
									size={SizeVariant.Medium}
									onClick={() => toggleEconomics()}
								/>
							</p>
						</div>
					</div>
				</div>
				<hr />
			</div>
			<div className="click-to-action">
				<Button
					color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
					onClick={onBackButtonClicked}
				>
					Kembali
				</Button>
				<Button
					color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
					onClick={onNextButtonClicked}
				>
					Lanjut
				</Button>
			</div>
			{showIncomeDrawer ? <IncomeDrawer /> : null}
		</DefaultLayout>
	)
}

export default SummaryIncomeScreen
