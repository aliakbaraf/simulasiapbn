﻿/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
if (typeof window.axios !== "undefined") {
	var engine = axios.create({
		withCredentials: true
	});
}