mergeInto(LibraryManager.library, {
	TriggerDownloadTextFile: function (pData, pFilename) {
		var data = Pointer_stringify(pData)
		var filename = Pointer_stringify(pFilename)

		var file = new Blob([data], { type: 'text/plain' });
		var a = document.createElement("a"),
			url = URL.createObjectURL(file);
		a.href = url;
		a.download = filename;
		document.body.appendChild(a);
		a.click();
		setTimeout(function () {
			document.body.removeChild(a);
			window.URL.revokeObjectURL(url);
		}, 0);
	},
});
