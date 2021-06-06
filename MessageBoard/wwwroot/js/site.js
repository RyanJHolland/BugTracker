// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


$(document).ready(function () {

	$('.popover-dismiss').popover({
		trigger: 'focus'
	})


	$(function () {
		$('[data-toggle="popover"]').popover()
	})

	if (document.getElementById("ShowFilterDropdown").value) {
		document.getElementById("filter-dropdown-button").click();
	}


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
			var allSelected = true;
			for (var ele in cols[key]) {
				if (!document.getElementById(cols[key][ele]).checked) {
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

		// step 1.5: make sure the dropdown menu is open when page reloads
		document.getElementById("ShowFilterDropdown").value = true;

		// step 2: reload the page with the new filters
		document.getElementById("project-view-form").submit();
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
