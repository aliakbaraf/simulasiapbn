/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
function showSweetAlert(sweetAlert) {
    if (typeof sweetAlert === "undefined" 
        && typeof window.swal !== "undefined") return;
    
    if (sweetAlert.title === void 0) {
        sweetAlert.title = "Informasi:";
    }
    if (sweetAlert.icon === void 0) {
        sweetAlert.icon = "info";
    }
    if (sweetAlert.icon === "danger") {
        sweetAlert.icon = "error";
    }
    if (sweetAlert.icon === "primary") {
        sweetAlert.icon = "info";
    }
    if (!!sweetAlert.text) {
        swal(sweetAlert);
    }
}