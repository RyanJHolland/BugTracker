using BugTracker.Models;

namespace BugTracker.ViewModels
{
	public class ProjectBugViewModel
	{
		public Project Project { get; set; }
		public Bug Bug { get; set; }

		//		m => m.Bug.Priority, new SelectList(Enum.GetValues(typeof(Bug.PriorityEnum)))
	}
}