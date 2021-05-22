// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$('.popover-dismiss').popover({
	trigger: 'focus'
})

$(function () {
	$('[data-toggle="popover"]').popover()
})

const Categories = ["Bug", "Feature", "Style", "Change"];
const Priorities = ["Critical", "High", "Medium", "Low"];
const Statuses = ["Open", "Resolved", "Cancelled"]

const toggleAll = function (toggle, column) {
	var checkboxes;
	if (column == "Categories") {
		checkboxes = Categories;
	} else if (column == "Priorities") {
		checkboxes = Priorities;
	} else if (column == "Statuses") {
		checkboxes = Statuses;
	}
	if (toggle.checked) {
		for (var x in checkboxes) {
			document.getElementById(checkboxes[x]).checked = true;
		}
	} else {
		for (var x in checkboxes) {
			document.getElementById(checkboxes[x]).checked = false;
		}
	}
}

const updateToggleAll = function (column) {
	var checkboxes;
	if (column == "Categories") {
		checkboxes = Categories;
	} else if (column == "Priorities") {
		checkboxes = Priorities;
	} else if (column == "Statuses") {
		checkboxes = Statuses;
	}
	var allChecked = true;
	for (var x in checkboxes) {
		if (!document.getElementById(checkboxes[x]).checked) {
			allChecked = false;
			break;
		}
	}
	if (allChecked) {
		document.getElementById("All" + column).checked = true;
	} else {
		document.getElementById("All" + column).checked = false;
	}
}
