/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
Chart.platform.disableCSSInjection = true;
Chart.defaults.global.legend.display = true;

var canvasChartContext = document.getElementById("canvas-chart")
	.getContext("2d");

var chart = new Chart(canvasChartContext, {
	type: "horizontalBar",
	data: {
		labels: ["APBN"],
		datasets: [
			{
				label: "Pendapatan Negara",
				backgroundColor: "#005fac",
				borderColor: "rgb(255, 99, 132)",
				data: [window.__SERVER_DATA__.totalIncome]
			},
			{
				label: "Belanja Negara",
				backgroundColor: "#fcb813",
				borderColor: "rgb(255, 99, 132)",
				data: [window.__SERVER_DATA__.totalExpenditure]
			},
			{
				label: window.__SERVER_DATA__.isSurplus ? "Surplus" : "Defisit",
				backgroundColor: window.__SERVER_DATA__.isSurplus ? "green" : "red",
				borderColor: "rgb(255, 99, 132)",
				data: [window.__SERVER_DATA__.change]
			}
		]
	},
	options: {}
});

function onDocumentReady(callback) {
	if (document.readyState !== "loading") {
		callback();
	} else {
		document.addEventListener("DOMContentLoaded", callback);
	}
}

function onFacebookShareButtonClicked() {
	var facebookShareData = new FormData();
	facebookShareData.append("sessionId", window.__SERVER_DATA__.sessionId);
	facebookShareData.append("target", "facebook");
	axios.post("/engine/publication/share", facebookShareData, {
		baseURL: window.__SERVER_DATA__.baseUrl,
		headers: { "Content-Type": "multipart/form-data" }
	}).then(function (result) {
		var response = result.data;
		if (!response.success || !response.data) {
			window.open(window.__SERVER_DATA__.facebookShareUrl);
			return;
		}

		var shareId = response.data.id;
		var shareSearchParam = new URLSearchParams();
		shareSearchParam.append("shareId", shareId);
		var shareUrl = window.__SERVER_DATA__.baseUrl + "/publication?" + shareSearchParam.toString();
		var facebookShareUrl = window.__SERVER_DATA__.facebookUrl + "&u=" + encodeURI(shareUrl);

		window.open(facebookShareUrl);
	}).catch(console.error);
}

function onTwitterShareButtonClicked() {
	var twitterShareData = new FormData();
	twitterShareData.append("sessionId", window.__SERVER_DATA__.sessionId);
	twitterShareData.append("target", "twitter");
	axios.post("/engine/publication/share", twitterShareData, {
		baseURL: window.__SERVER_DATA__.baseUrl,
		headers: { "Content-Type": "multipart/form-data" }
	}).then(function (result) {
		var response = result.data;
		if (!response.success || !response.data) {
			window.open(window.__SERVER_DATA__.twitterShareUrl);
			return;
		} 

		var shareId = response.data.id;
		var shareSearchParam = new URLSearchParams();
		shareSearchParam.append("shareId", shareId);
		var shareUrl = window.__SERVER_DATA__.baseUrl + "/publication?" + shareSearchParam.toString();
		var twitterShareUrl = window.__SERVER_DATA__.twitterUrl + "&url=" + encodeURI(shareUrl);

		window.open(twitterShareUrl);
	}).catch(console.error);
}

function onNewSessionButtonClicked() {
	window.open(window.__SERVER_DATA__.baseUrl);
}

function toLocale(value) {
	return new Intl.NumberFormat("id-ID", {
		style: "decimal",
		maximumFractionDigits: 2,
	}).format(value);
}

function toCurrency(value) {
	return "Rp" + toLocale(value);
}

onDocumentReady(function (event) {
	document.getElementById("total-income")
		.innerText = toCurrency(window.__SERVER_DATA__.totalIncome) + " T";
	document.getElementById("total-expenditure")
		.innerText = toCurrency(window.__SERVER_DATA__.totalExpenditure) + " T";
	document.getElementById("change")
		.innerText = toCurrency(window.__SERVER_DATA__.positiveChange) + " T";

	document.getElementById("facebook-share-button")
		.addEventListener("click", onFacebookShareButtonClicked);
	document.getElementById("twitter-share-button")
		.addEventListener("click", onTwitterShareButtonClicked);
	document.getElementById("new-session-button")
		.addEventListener("click", onNewSessionButtonClicked);
});