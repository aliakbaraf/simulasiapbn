/**
 * Simulasi APBN
 *
 * Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/)
 * untuk Kementerian Keuangan Republik Indonesia.
 */
export const showBanner = () => {
	const banner = `
.oPYo.  o                8                o        .d888888   888888ba   888888ba  888888ba  
8                        8                        d8'    88   88    \`8b  88    \`8b 88    \`8b 
\`Yooo. o8 ooYoYo. o    o 8 .oPYo. .oPYo. o8       88aaaaa88a a88aaaa8P' a88aaaa8P' 88     88 
    \`8  8 8' 8  8 8    8 8 .oooo8 Yb..    8       88     88   88         88   \`8b. 88     88 
     8  8 8  8  8 8    8 8 8    8   'Yb.  8       88     88   88         88    .88 88     88 
\`YooP'  8 8  8  8 \`YooP' 8 \`YooP8 \`YooP'  8       88     88   dP         88888888P dP     dP 


Program ditulis oleh Danang Galuh Tegar Prasetyo (https://danang.id/) untuk Kementerian Keuangan Republik Indonesia.
Hak Cipta © ${new Date().getFullYear()} Kementerian Keuangan Republik Indonesia. Seluruh hak dilindungi Undang-Undang.

`
	console.log(banner)
}

export default { showBanner }
