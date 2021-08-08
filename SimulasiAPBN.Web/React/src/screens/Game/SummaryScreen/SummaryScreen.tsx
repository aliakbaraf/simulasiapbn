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
import useErrorHandler from "@common/hooks/errorHandler"
import useFormatter from "@common/hooks/formatter"
import useHTMLDocument from "@common/hooks/htmlDocument"
import { useService } from "@common/hooks/services"
import { isAllClientProgressDone } from "@common/libraries/clientSimulationProgress"
import ComponentProps from "@common/libraries/component"
import { toLocale, toCurrency } from "@common/libraries/locale"
import { showPublication } from "@common/libraries/publication"
import BoxWithValue from "@components/BoxWithValue"
import Button from "@components/Button"
import Drawer from "@components/Drawer"
import Allocation from "@core/models/Allocation"
import SimulationSession from "@core/models/SimulationSession"
import SimulationStateExpenditure from "@core/models/SimulationStateExpenditure"
import SimulationEconomicMacro from "@core/models/SimulationEconomicMacro"
import { setClientSimulation } from "@flow/slices/simulation"
import { setLoading, setNetworkLoading } from "@flow/slices/common"
import { DynamicMetadataDeficit } from "@flow/types/dynamic-metadata"
import { SimulationProgress } from "@flow/types/simulation"
import RootState from "@flow/types"
import DefaultLayout, { ScreenType } from "@layouts/DefaultLayout"
import GamesRoutes from "@screens/Game/routes"

import "./SummaryScreen.scoped.css"

type AllocationValue = { allocation: Allocation; minValue: number; value: number }
type SummaryScreenProps = ComponentProps<{}, AllHTMLAttributes<HTMLDivElement>>

const SummaryScreen: React.FC<SummaryScreenProps> = () => {
	const dispatch = useDispatch()
	const formatter = useFormatter()
	const handleError = useErrorHandler(dispatch)
	const history = useHistory()
	const htmlDocument = useHTMLDocument()
	const service = useService()

	const allocations = useSelector<RootState, Allocation[]>(state => state.simulation.allocations)
	const darkMode = useSelector<RootState, boolean>(state => state.common.darkMode)
	const deficit = useSelector<RootState, DynamicMetadataDeficit>(state => state.dynamicMetadata.deficit)
	const expenditures = useSelector<RootState, SimulationStateExpenditure[]>(
		state => state.simulation.expenditures.all
	)
	const economics = useSelector<RootState, SimulationEconomicMacro[]>(
		state => state.simulation.economicMacros.economicMacro
	)
	const grossDomesticProduct = useSelector<RootState, number>(state => state.dynamicMetadata.grossDomesticProduct)
	const progress = useSelector<RootState, SimulationProgress>(state => state.simulation.progress)
	const rules = useSelector<RootState, string[]>(state => state.dynamicMetadata.rules)
	const session = useSelector<RootState, SimulationSession | undefined>(state => state.simulation.session)
	const totalExpenditures = useSelector<RootState, number>(state => state.simulation.totalExpenditure.all)
	const totalIncome = useSelector<RootState, number>(
		state => state.simulation.session?.used_income ?? 0
	)
	const countryIncome = useSelector<RootState, number>(
		state => state.simulation.session?.state_budget.country_income ?? 0
	)

	const [changePercentage, setChangePercentage] = useState<number>(0)
	const [isDeficit, setIsDeficit] = useState<boolean>(false)
	const [isDeficitWithinThreshold, setIsDeficitWithinThreshold] = useState<boolean>(false)
	const [mandatoryAllocations, setMandatoryAllocations] = useState<Allocation[]>([])
	const [mandatoryAllocationValues, setMandatoryAllocationValues] = useState<AllocationValue[]>([])
	const [positiveChange, setPositiveChange] = useState<number>(0)
	const [priorityExpenditures, setPriorityExpenditures] = useState<SimulationStateExpenditure[]>([])
	const [economicExpenditures, setEconomicExpenditures] = useState<SimulationEconomicMacro[]>([])
	const [showFinishPrompt, setShowFinishPrompt] = useState<boolean>(false)
	const [showPriorityDrawer, setShowPriorityDrawer] = useState<boolean>(false)
	const [showSummaryDrawer, setShowSummaryDrawer] = useState<boolean>(false)
	const [showIncomeDrawer, setShowIncomeDrawer] = useState<boolean>(false)
	const [finishWarningText, setFinishWarningText] = useState<string | undefined>()

	/* Event Handlers */
	const onBackButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		history.push(GamesRoutes.AllocationScreen)
	}

	const onFinishButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		endSimulation().then()
	}

	const onHideFinishPromptButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		setShowFinishPrompt(false)
		setFinishWarningText(void 0)
	}

	const onShowFinishPromptButtonClicked: React.MouseEventHandler<HTMLButtonElement> = () => {
		if (checkMandatoryAllocationValuesFulfillment() && (isDeficit && !isDeficitWithinThreshold)) {
			setFinishWarningText(`APBN versi kamu memiliki defisit sebesar ${toLocale(changePercentage)}% 
			dari Pendapatan Domestik Bruto (PDB). Angka defisit tersebut melebihi ambang batas aman sesuai 
			${deficit.law} sebesar ${toLocale(deficit.threshold)}% dari PDB.`)
		}

		setShowFinishPrompt(true)
	}
	/* End of Event Handlers */

	/* Functions */
	const loadSummaryData = () => {
		if (typeof session === "undefined") {
			return
		}
	}

	const checkMandatoryAllocationValuesFulfillment = () => {
		for (const mandatoryAllocationValue of mandatoryAllocationValues) {
			if (mandatoryAllocationValue.value < mandatoryAllocationValue.minValue) {
				const { name, mandatory_threshold } = mandatoryAllocationValue.allocation
				setFinishWarningText(
					`Alokasi ${name} minimal ${toCurrency(
						mandatoryAllocationValue.minValue
					)} T (${mandatory_threshold}% dari total Belanja Negara). 
					Kamu baru mengalokasikan sebesar ${toCurrency(mandatoryAllocationValue.value)} T.`
				)
				return false
			}
		}

		return true
	}

	const endSimulation = async () => {
		if (typeof session === "undefined" || session === null) {
			return
		}
		try {
			dispatch(setLoading(`Mengakhiri sesi simulasi...`))
			dispatch(setNetworkLoading())
			const finishedSimulation = await service.Simulation.finishSession(session.id)
			dispatch(setClientSimulation())

			showPublication(finishedSimulation?.session.id || "")
		} catch (error) {
			handleError(error)
		}
	}
	/* End of Functions */

	/* Effects */
	useEffect(() => {
		if (!isAllClientProgressDone(progress)) {
			history.push(GamesRoutes.AllocationScreen)
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
		if (!isAllClientProgressDone(progress)) {
			history.push(GamesRoutes.AllocationScreen)
		}
	}, [progress])

	useEffect(() => {
		if (expenditures.length <= 0) {
			return
		}

		const newMandatoryAllocations = allocations.filter(allocation => allocation.is_mandatory)

		const newPriorityExpenditures = expenditures.filter(expenditure => expenditure.is_priority)

		setMandatoryAllocations(newMandatoryAllocations)
		setPriorityExpenditures(newPriorityExpenditures)
	}, [expenditures])

	useEffect(() => {
		if (economics.length <= 0) {
			return
		}

		const newEconomicExpenditures = economics.filter(
			expenditure => expenditure.used_value != expenditure.economic_macro.default_value)

		setEconomicExpenditures(newEconomicExpenditures)
	}, [economics])

	useEffect(() => {
		if (mandatoryAllocations.length <= 0) {
			return
		}

		const newMandatoryAllocationValues = mandatoryAllocations.map(allocation => ({
			allocation,
			minValue: 0,
			value: 0,
		}))
		for (const expenditure of expenditures) {
			const stateExpenditureAllocations = expenditure.state_expenditure.state_expenditure_allocations
			const totalAllocation = expenditure.total_allocation
			for (const stateExpenditureAllocation of stateExpenditureAllocations) {
				if (stateExpenditureAllocation.allocation.is_mandatory) {
					const allocationValue = totalAllocation * (stateExpenditureAllocation.percentage / 100)
					const allocationValueIndex = newMandatoryAllocationValues.findIndex(
						value => value.allocation.id === stateExpenditureAllocation.allocation_id
					)
					newMandatoryAllocationValues[allocationValueIndex].value += allocationValue
				}
			}
		}
		for (const [index, allocationValue] of newMandatoryAllocationValues.entries()) {
			const mandatoryThreshold = (allocationValue.allocation.mandatory_threshold || 0) / 100
			newMandatoryAllocationValues[index].minValue = mandatoryThreshold * totalExpenditures
		}

		setMandatoryAllocationValues(newMandatoryAllocationValues)
	}, [mandatoryAllocations])

	useEffect(() => {
		const change = totalIncome - totalExpenditures
		setPositiveChange(change < 0 ? change * -1 : change)
	}, [totalExpenditures])

	useEffect(() => {
		setChangePercentage((positiveChange / grossDomesticProduct) * 100)
		if (totalExpenditures <= totalIncome) {
			/* Surplus */
			setIsDeficit(false)
			setIsDeficitWithinThreshold(true)
		} else {
			/* Deficit */
			const deficitThreshold = grossDomesticProduct * (deficit.threshold / 100)
			setIsDeficit(true)
			setIsDeficitWithinThreshold(totalExpenditures <= deficitThreshold + totalIncome)
		}
	}, [positiveChange])
	/* End of Effects */

	/* Child Components */
	const FinishPromptContainer: React.FC = () => (
		<>
			<div className="finish-prompt-container">
				<div className="card">
					<div className="header">
						<span className="title">Akhiri Penyusunan</span>
					</div>
					<div className="content">
						{typeof finishWarningText === "undefined" ? null : (
							<p className="warning">
								PERHATIAN!
								<br />
								<span className="warning-text">{finishWarningText}</span>
							</p>
						)}
						<p className="description">Apakah kamu yakin ingin mengakhiri penyusunan APBN versi kamu?</p>
					</div>
					<div className="click-to-action">
						<Button
							color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
							onClick={onHideFinishPromptButtonClicked}
						>
							Kembali
						</Button>
						<Button
							color={darkMode ? ColorVariant.Secondary : ColorVariant.Primary}
							onClick={onFinishButtonClicked}
						>
							AKHIRI
						</Button>
					</div>
				</div>
			</div>
			<div className="floating-background" />
		</>
	)

	const PriorityDrawer: React.FC = () => (
		<Drawer title="Program Prioritas" onCloseDrawer={() => setShowPriorityDrawer(false)}>
			<p>
				<strong>Program Prioritas</strong> adalah seluruh komponen belanja yang kamu anggap penting dan
				mengalami peningkatan yang signifikan dibandingkan {formatter.getStateBudgetName(session?.state_budget)}
				.
			</p>
		</Drawer>
	)

	const IncomeDrawer: React.FC = () => (
		<Drawer title="Asumsi Ekonomi Makro" onCloseDrawer={() => setShowIncomeDrawer(false)}>
			<p>
				<strong>Asumsi Ekonomi Makro</strong> adalah
				komponen ekonomi yang mengatur pendapatan negara yang
				mengalami perbedaan dibandingkan dengan {formatter.getStateBudgetName(session?.state_budget)}.
			</p>
		</Drawer>
	)

	const SummaryDrawer: React.FC = () => (
		<Drawer title="Ringkasan APBN" onCloseDrawer={() => setShowSummaryDrawer(false)}>
			<p className="mb-3">
				<strong>Anggaran Pendapatan dan Belanja Negara</strong> (APBN) dirancang agar dapat menciptakan
				pertumbuhan ekonomi untuk memberantas kemiskinan, mengurangi kesenjangan, serta meningkatkan
				produktivitas dan daya saing nasional.
			</p>
			<p className="mb-3">Penyusunan APBN harus memperhatikan amanat undang-undang, yaitu sebagai berikut.</p>
			<ol className="list-disc list-outside pl-6">
				{rules.map((rule, index) => (
					<li key={index}>{rule}.</li>
				))}
			</ol>
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
					Asumsi {name} versi kamu ({toLocale(usedValue)} {unitDesc}) {change>0?"meningkat":"berkurang" } {toLocale(change)} {unitDesc} dibandingkan
					alokasi pada {formatter.getStateBudgetName(session?.state_budget)} ({toLocale(defaultValue)} {unitDesc}).
				</p>
				{change > 0 ?
					!naration ? null : <p className="mb-3">{naration}</p>
					:
					!narationDefisit ? null : <p className="mb-3">{narationDefisit}</p>}
			</BoxWithValue>
		)
	}

	const renderPriorityExpenditure = (expenditure: SimulationStateExpenditure, index: number) => {
		const budgetDescription = expenditure.state_expenditure.budget.description || ""
		const budgetFunction = expenditure.state_expenditure.budget.function
		const budgetTargets = expenditure.state_expenditure.budget.budget_targets || []
		const change = expenditure.total_allocation - expenditure.state_expenditure.total_allocation
		const priorityPercentage = (change / expenditure.state_expenditure.total_allocation) * 100

		const value = `${toCurrency(expenditure.total_allocation)} T`
		return (
			<BoxWithValue key={index} title={budgetFunction} value={value}>
				<p className="font-bold mb-2">
					Alokasi anggaran {budgetFunction} versi kamu meningkat {toLocale(priorityPercentage)}% dibandingkan
					alokasi pada {formatter.getStateBudgetName(session?.state_budget)}.
				</p>
				{!budgetDescription ? null : <p className="mb-3">{budgetDescription}</p>}
				{budgetTargets.length <= 0 ? null : (
					<div>
						<span>Sasaran:</span>
						<ol className="list-decimal list-outside pl-6">
							{budgetTargets.map((target, index) => {
								let endString = ";"
								if (index === budgetTargets.length - 1) {
									endString = "."
								} else if (index === budgetTargets.length - 2) {
									endString = "; dan"
								}
								return (
									<li key={index}>
										{target.description}
										{endString}
									</li>
								)
							})}
						</ol>
					</div>
				)}
			</BoxWithValue>
		)
	}

	const renderMandatoryAllocationValue = (allocationValue: AllocationValue, index: number) => {
		const kind = allocationValue.value >= allocationValue.minValue ? "default" : "error"
		const title = `Alokasi ${allocationValue.allocation.name}`
		const minimalValue = `${toCurrency(allocationValue.minValue)} T`
		const values = `${toCurrency(allocationValue.value)} T`
		const { mandatory_explanation, mandatory_threshold } = allocationValue.allocation

		return (
			<BoxWithValue key={index} kind={kind} title={title} value={values}>
				<p className="font-bold mb-2">
					{title} minimal {minimalValue} ({toLocale(mandatory_threshold ?? 0)}% dari total Belanja Negara).
				</p>
				<p>{mandatory_explanation}</p>
			</BoxWithValue>
		)
	}
	/* End of Child Components */

	const [showEconomics, setShowEconomics] = useState(false)
	const [showPriority, setShowPriority] = useState(false)
	const [showSummary, setShowSummary] = useState(false)
	const toggleEconomics = () => {
		setShowEconomics(!showEconomics)
	}
	const togglePriority = () => {
		setShowPriority(!showPriority)
	}
	const toggleSummary = () => {
		setShowSummary(!showSummary)
	}

	return (
		<DefaultLayout className="screen" fixedNavigation screenType={ScreenType.Game}>
			<div className="main">
				<div className="header">
					<div className="title">
						<p>Pratinjau Hasil</p>
						<p>Rancangan {formatter.getStateBudgetName(session?.state_budget)}</p>
					</div>
					<div className="abstract">
						<div>
							Berikut sekilas tentang Rancangan APBN versi kamu.
							<ol className="list-disc list-outside pl-6">
								<li>
									<strong>Pendapatan Negara</strong> sebesar {toCurrency(totalIncome)} T
									{toLocale(countryIncome) == toLocale(totalIncome) ? " (sesuai dengan data "+formatter.getStateBudgetName(session?.state_budget)+")." : ""}
								</li>
								<li>
									<strong>Belanja Negara</strong> sebesar {toCurrency(totalExpenditures)} T.
								</li>
							</ol>
						</div>
						<span className="font-normal italic mt-2">
							Ketuk tanda&nbsp;
							<FiHelpCircle className="inline-flex text-primary dark:text-secondary" />
							&nbsp;untuk informasi lebih lanjut.
						</span>
					</div>
				</div>
				<div className="content">
					<div className="box economic-box">
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
					<div className="box priority-box">
						<p className="box-header">
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={!showPriority ? FiPlusSquare : FiMinusSquare}
								noOutline
								size={SizeVariant.Medium}
								onClick={() => togglePriority()}
							/>
							Program Prioritas Kamu&nbsp;
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={FiHelpCircle}
								noOutline
								size={SizeVariant.Small}
								onClick={() => setShowPriorityDrawer(true)}
							/>
						</p>
						<div className="box-content"
							style={{ display: (showPriority ? 'block' : 'none') }}>
							{priorityExpenditures.length <= 0
								? "Kamu tidak memiliki program prioritas."
								: priorityExpenditures.map(renderPriorityExpenditure)}
							<p className="box-btn"
								style={{ display: (!showPriority ? 'none' : priorityExpenditures.length > 0 ? 'block' : 'none') }}
							>
								<Button
									color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
									icon={!showPriority ? FiPlusSquare : FiMinusSquare}
									noOutline
									size={SizeVariant.Medium}
									onClick={() => togglePriority()}
								/>
							</p>
						</div>
						
					</div>
					<div className="box summary-box">
						<p className="box-header">
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={!showSummary ? FiPlusSquare : FiMinusSquare}
								noOutline
								size={SizeVariant.Medium}
								onClick={() => toggleSummary()}
							/>
							Ringkasan APBN versi Kamu&nbsp;
							<Button
								color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
								icon={FiHelpCircle}
								noOutline
								size={SizeVariant.Small}
								onClick={() => setShowSummaryDrawer(true)}
							/>
						</p>
						<div className="box-content"
							style={{ display: (showSummary ? 'block' : 'none') }}>
							<BoxWithValue
								kind={isDeficitWithinThreshold ? "default" : "error"}
								title={`${isDeficit ? "Defisit" : "Surplus"} Anggaran`}
								value={`${toCurrency(positiveChange)} T`}
							>
								<div className={isDeficit ? "hidden" : "block"}>
									<p className="font-bold mb-3">
										APBN versi kamu memiliki surplus sebesar {toLocale(changePercentage)}% dari
										Pendapatan Domestik Bruto (PDB).
									</p>
									<p>
										Sisa dana tetap berada dalam Kas Negara Tahun dan bisa dialokasikan untuk
										Belanja Negara tahun berikutnya setelah mendapatkan persetujuan Dewan Perwakilan
										Rakyat Republik Indonesia.
									</p>
								</div>
								<div className={isDeficit && isDeficitWithinThreshold ? "block" : "hidden"}>
									<p className="font-bold mb-2">
										APBN versi kamu memiliki defisit sebesar {toLocale(changePercentage)}% dari
										Pendapatan Domestik Bruto (PDB).
									</p>
									<p className="mb-3">
										Angka defisit tersebut masih dalam ambang batas aman sesuai {deficit.law}{" "}
										sebesar {toLocale(deficit.threshold)}% dari PDB.
									</p>
									<p>
										Dengan postur APBN ini, dibutuhkan pembiayaan anggaran melalui{" "}
										<strong>Utang Negara</strong>. Utang negara tersebut sebaiknya digunakan untuk
										belanja produktif seperti pendidikan, kesehatan dan infrastruktur. Utang Negara
										adalah investasi pembangunan manusia dan investasi pembangunan infrastruktur.
										Pengelolaan Utang Negara harus sesuai Undang-Undang dan dilakukan secara
										hati-hati, prefesional dan bijaksana, sesuai prinsip-prinsip pengelolaan utang
										global yang dianut oleh semua negara di dunia.
									</p>
								</div>
								<div className={isDeficit && !isDeficitWithinThreshold ? "block" : "hidden"}>
									<p className="font-bold mb-2">
										APBN versi kamu memiliki defisit sebesar {toLocale(changePercentage)}% dari
										Pendapatan Domestik Bruto (PDB).
									</p>
									<p className="font-bold mb-3">
										Angka defisit tersebut melebihi ambang batas aman sesuai {deficit.law} sebesar{" "}
										{toLocale(deficit.threshold)}% dari PDB.
									</p>
									<p>
										Dengan postur APBN ini, kamu melanggar {deficit.law}, sehingga perlu dilakukan
										penghematan Belanja Negara atau dilakukan peningkatan Pendapatan Negara.
									</p>
								</div>
							</BoxWithValue>
							{mandatoryAllocationValues.map(renderMandatoryAllocationValue)}
							<p className="box-btn"
								style={{ display: (!showSummary ? 'none' : 'block') }}
							>
								<Button
									color={darkMode ? ColorVariant.SecondaryAlternate : ColorVariant.PrimaryAlternate}
									icon={!showSummary ? FiPlusSquare : FiMinusSquare}
									noOutline
									size={SizeVariant.Medium}
									onClick={() => toggleSummary()}
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
					onClick={onShowFinishPromptButtonClicked}
				>
					SELESAI
				</Button>
			</div>
			{showFinishPrompt ? <FinishPromptContainer /> : null}
			{showIncomeDrawer ? <IncomeDrawer /> : null}
			{showPriorityDrawer ? <PriorityDrawer /> : null}
			{showSummaryDrawer ? <SummaryDrawer /> : null}
		</DefaultLayout>
	)
}

export default SummaryScreen
