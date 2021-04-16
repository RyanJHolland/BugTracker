using BugTracker.Models;
using System.Collections.Generic;

namespace BugTracker.ViewModels
{
	public class ProjectEditViewModel
	{
		public Project Project { get; set; }
		public List<string> PossiblePMUserNames { get; set; }
		public string CurrentPMUserName { get; set; }

		//		m => m.Bug.Priority, new SelectList(Enum.GetValues(typeof(Bug.PriorityEnum)))
	}
}