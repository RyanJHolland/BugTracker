// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


$(document).ready(function () {

	$('.popover-dismiss').popover({
		trigger: 'focus'
	})


	$(function () {
		$('[data-toggle="popover"]').popover()
	})

	const cols = {
		"Categories": ["Bug", "Feature", "Style", "Change"],
		"Statuses": ["Unassigned", "Open", "Resolved", "Cancelled"],
		"Priorities": ["Critical", "High", "Medium", "Low"]
	}

	// this is a helper function because of JS's closure trait 
	function allCheckboxCallback(key) {
		return function () {
			allCheckbox(key);
		}
	}

	// this is called when any SelectAll checkbox is clicked
	const allCheckbox = (key) => {
		const selectAll = document.getElementById("All" + key);
		for (var ele in cols[key]) {
			document.getElementById(cols[key][ele]).checked = selectAll.checked;
		}
		checkboxClicked();
	}

	const updateFilters = () => {
		// update the SelectAll checkbox to reflect current reality
		for (var key in cols) {
			console.log('checking ' + key);
			var allSelected = true;
			for (var ele in cols[key]) {
				console.log('... checking ' + cols[key][ele]);
				if (!document.getElementById(cols[key][ele]).checked) {
					console.log(cols[key][ele] + ' was false');
					allSelected = false;
					break;
				}
			}
			document.getElementById("All" + key).checked = allSelected ? true : false;
		}
	}

	// this is called when any non-SelectAll checkbox is clicked
	const checkboxClicked = () => {
		// step 1: apply the new filters
		updateFilters();
		// step 2: reload the page with the new filters
		document.getElementById("filterForm").submit();
	}

	// attach event listeners to checkboxes
	for (var key in cols) {
		// event listener for each SelectAll checkbox
		document.getElementById("All" + key).addEventListener("click", allCheckboxCallback(key));
		for (var ele in cols[key]) {
			// event listener for other checkboxes
			document.getElementById(cols[key][ele]).addEventListener("click", function () { checkboxClicked() });
		}
	}

	updateFilters();
});

// TRASH //
/////////
///////
/////

/*





const Categories = ["Bug", "Feature", "Style", "Change"];
const Priorities = ["Critical", "High", "Medium", "Low"];
const Statuses = ["Unassigned", "Open", "Resolved", "Cancelled"];
const columns = [Categories, Priorities, Statuses];

const linkCheckboxes = (column) => {
	var checkboxes;

	if (column == "Categories") {
		checkboxes = Categories;
	} else if (column == "Priorities") {
		checkboxes = Priorities;
	} else if (column == "Statuses") {
		checkboxes = Statuses;
	}

	return checkboxes;
}


const toggleAll = (toggle, column) => {
	var checkboxes = linkCheckboxes(column);

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


const updateToggleAll = (column) => {
	var checkboxes = linkCheckboxes(column);
	var allChecked = true;
	for (var x in checkboxes) {
		if (!document.getElementById(checkboxes[x]).checked) {
			allChecked = false;
			break;
		}
	}
	console.log("column: " + column);
	if (allChecked) {
		document.getElementById("All" + column).checked = true;
	} else {
		document.getElementById("All" + column).checked = false;
	}
}


const submitFilters = () => {
	for (var i in columns) {
		for (var j in columns[i])
			updateToggleAll(columns[i][j]);
	}

	this.form.submit();
}
*/
